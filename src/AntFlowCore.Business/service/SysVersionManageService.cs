using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Configuration;

namespace AntFlowCore.Business.service
{
    /// <summary>
    /// App版本管理 业务服务. 对应 Java SysVersionServiceImpl(管理面+App升级检查).
    /// </summary>
    public class SysVersionManageService : ISysVersionManageService
    {
        private readonly ISysVersionRepository _sysVersionRepository;
        private readonly IBpmProcessAppDataRepository _appDataRepository;
        private readonly IBpmProcessAppApplicationRepository _appApplicationRepository;
        private readonly IQuickEntryRepository _quickEntryRepository;
        private readonly IBpmnConfService _bpmnConfService;

        private const int TYPE_APP = 1;
        private const int TYPE_APP_DATA = 2;
        private const int TYPE_APP_QUICK_ENTRY = 3;

        public SysVersionManageService(
            ISysVersionRepository sysVersionRepository,
            IBpmProcessAppDataRepository appDataRepository,
            IBpmProcessAppApplicationRepository appApplicationRepository,
            IQuickEntryRepository quickEntryRepository,
            IBpmnConfService bpmnConfService)
        {
            _sysVersionRepository = sysVersionRepository;
            _appDataRepository = appDataRepository;
            _appApplicationRepository = appApplicationRepository;
            _quickEntryRepository = quickEntryRepository;
            _bpmnConfService = bpmnConfService;
        }

        // ==================== App端升级检查 ====================

        public AppVersionVo GetAppVersion(string application, string appVersion)
        {
            if (string.IsNullOrEmpty(application) || string.IsNullOrEmpty(appVersion))
            {
                return null;
            }
            List<SysVersion> curList = _sysVersionRepository.Find(v => v.IsDel == 0 && v.Version == appVersion);
            if (curList.Count == 0)
            {
                return null;
            }
            SysVersion cur = curList[0];
            AppVersionVo vo = new AppVersionVo();
            if (cur.IsHide == SysVersion.HIDE_STATUS_1)
            {
                vo.IsLatest = 1;
                return vo;
            }
            int curIndex = cur.Index;
            int? maxIndex = MaxPublishedIndex();
            if (maxIndex == null)
            {
                return null;
            }
            if (curIndex == maxIndex.Value)
            {
                vo.IsLatest = 1;
                return vo;
            }
            SysVersion maxVersion = _sysVersionRepository
                .FirstOrDefault(v => v.IsDel == 0 && v.IsHide == 0 && v.Index == maxIndex.Value);
            if (maxVersion == null)
            {
                return null;
            }
            //强更豁免配置(与Java @Value默认空一致)
            IConfiguration configuration = ServiceProviderUtils.GetService<IConfiguration>();
            string iosSkip = configuration.GetValue<string>("app:ios:skip_force_version") ?? "";
            string androidSkip = configuration.GetValue<string>("app:android:skip_force_version") ?? "";
            string harmonySkip = configuration.GetValue<string>("app:harmony:skip_force_version") ?? "";
            bool skipForce =
                ("android".Equals(application, StringComparison.OrdinalIgnoreCase) && androidSkip == maxVersion.Version)
                || ("ios".Equals(application, StringComparison.OrdinalIgnoreCase) && iosSkip == maxVersion.Version)
                || ("harmony_os".Equals(application, StringComparison.OrdinalIgnoreCase) && harmonySkip == maxVersion.Version);
            if (skipForce)
            {
                vo.IsLatest = 1;
                return vo;
            }
            //curIndex与maxIndex之间的全部版本任一强更则强更
            List<SysVersion> between = _sysVersionRepository.Find(v => v.Index > curIndex && v.Index <= maxIndex.Value);
            bool force = between.Any(s => s.IsForce == 1);
            if ("android".Equals(application, StringComparison.OrdinalIgnoreCase))
            {
                vo.DownloadUrl = maxVersion.AndroidUrl;
            }
            else if ("ios".Equals(application, StringComparison.OrdinalIgnoreCase))
            {
                vo.DownloadUrl = maxVersion.IosUrl;
            }
            vo.Description = maxVersion.Description;
            vo.Id = maxVersion.Id;
            vo.Version = maxVersion.Version;
            vo.CurVersion = appVersion;
            vo.IsLatest = 0;
            vo.IsForce = force ? 1 : 0;
            return vo;
        }

        // ==================== 二维码 ====================

        public SysVersionVo GetDownloadQrCode()
        {
            SysVersionVo vo = new SysVersionVo();
            int? maxIndex = MaxPublishedIndex();
            if (maxIndex == null)
            {
                return vo;
            }
            SysVersion version = _sysVersionRepository
                .FirstOrDefault(v => v.IsDel == 0 && v.IsHide == 0 && v.Index == maxIndex.Value);
            if (version == null)
            {
                return vo;
            }
            vo.DownloadCode = version.DownloadCode;
            return vo;
        }

        // ==================== 版本列表 ====================

        public ResultAndPage<SysVersionVo> ListSysVersion(int page, int pageSize, string version)
        {
            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 10; }
            if (pageSize > 200) { pageSize = 200; }
            List<SysVersion> all = _sysVersionRepository.Find(v => v.IsDel == 0)
                .OrderByDescending(v => v.Id)
                .ToList();
            if (!string.IsNullOrEmpty(version))
            {
                all = all.Where(v => !string.IsNullOrEmpty(v.Version) && v.Version.Contains(version)).ToList();
            }
            int total = all.Count;
            List<SysVersion> records = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            List<SysVersionVo> vos = records.Select(ToVo).ToList();
            return new ResultAndPage<SysVersionVo>(vos,
                PageDto.BuildCountedPage(new PageDto { Page = page, PageSize = pageSize }, total));
        }

        private SysVersionVo ToVo(SysVersion v)
        {
            return new SysVersionVo
            {
                Id = v.Id,
                Version = v.Version,
                Description = v.Description,
                Index = v.Index,
                IsForce = v.IsForce,
                IsHide = v.IsHide,
                AndroidUrl = v.AndroidUrl,
                IosUrl = v.IosUrl,
                DownloadCode = v.DownloadCode,
                EffectiveTime = v.EffectiveTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateTime = v.CreateTime,
                UpdateTime = v.UpdateTime,
            };
        }

        // ==================== 保存(新增草稿/编辑) ====================

        public bool Edit(SysVersionVo vo)
        {
            if (vo == null)
            {
                throw new AFBizException("400110", "参数错误");
            }
            CheckVersionUnique(vo);
            if (vo.Id != 0)
            {
                SysVersion current = _sysVersionRepository.GetById(vo.Id);
                if (current == null || current.IsDel == 1)
                {
                    throw new AFBizException("404001", "版本不存在");
                }
                if (current.IsHide == SysVersion.HIDE_STATUS_1)
                {
                    //草稿: 全量可编辑
                    current.Version = vo.Version;
                    current.Description = vo.Description;
                    current.IsForce = vo.IsForce ?? 0;
                    current.AndroidUrl = vo.AndroidUrl;
                    current.IosUrl = vo.IosUrl;
                    current.DownloadCode = vo.DownloadCode;
                }
                else
                {
                    //已发布: 仅运营参数白名单(is_force/android_url/ios_url/download_code)
                    current.IsForce = vo.IsForce ?? current.IsForce;
                    current.AndroidUrl = vo.AndroidUrl ?? current.AndroidUrl;
                    current.IosUrl = vo.IosUrl ?? current.IosUrl;
                    current.DownloadCode = vo.DownloadCode ?? current.DownloadCode;
                }
                current.UpdateUser = vo.UpdateUser;
                current.UpdateTime = DateTime.Now;
                _sysVersionRepository.Update(current);
                return true;
            }
            //新增: index = 已发布最大index + 1, 草稿态
            SysVersion entity = new SysVersion
            {
                Version = vo.Version,
                Description = vo.Description,
                IsForce = vo.IsForce ?? 0,
                AndroidUrl = vo.AndroidUrl,
                IosUrl = vo.IosUrl,
                DownloadCode = vo.DownloadCode,
                CreateUser = vo.CreateUser,
                UpdateUser = vo.UpdateUser,
                IsDel = 0,
                IsHide = SysVersion.HIDE_STATUS_1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                EffectiveTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            int? maxIndex = MaxPublishedIndex();
            entity.Index = (maxIndex ?? 0) + 1;
            _sysVersionRepository.Add(entity);
            if (vo.InheritFromLast == true)
            {
                CopyAppDataFromPreviousVersion(entity.Id);
            }
            return true;
        }

        /// <summary>
        /// 版本号在有效记录中唯一
        /// </summary>
        private void CheckVersionUnique(SysVersionVo vo)
        {
            if (string.IsNullOrEmpty(vo.Version))
            {
                throw new AFBizException("400100", "版本号不能为空");
            }
            bool duplicate = _sysVersionRepository.Any(v => v.Version == vo.Version && v.IsDel == 0
                && (vo.Id == 0 || v.Id != vo.Id));
            if (duplicate)
            {
                throw new AFBizException("400101", "版本号已存在: " + vo.Version);
            }
        }

        /// <summary>
        /// 从上一最大index版本复制三类关联数据(含sort)到新版本
        /// </summary>
        private void CopyAppDataFromPreviousVersion(long newVersionId)
        {
            SysVersion source = _sysVersionRepository.Find(v => v.IsDel == 0 && v.Id != newVersionId)
                .OrderByDescending(v => v.Index)
                .ThenByDescending(v => v.Id)
                .FirstOrDefault();
            if (source == null)
            {
                return;
            }
            List<BpmProcessAppData> rows = new List<BpmProcessAppData>();
            foreach (int type in new[] { TYPE_APP, TYPE_APP_DATA, TYPE_APP_QUICK_ENTRY })
            {
                rows.AddRange(_appDataRepository
                    .Find(d => d.VersionId == source.Id && d.Type == type && d.State == 0)
                    .Select(d => new BpmProcessAppData
                    {
                        VersionId = newVersionId,
                        Type = type,
                        ApplicationId = d.ApplicationId,
                        ProcessKey = d.ProcessKey,
                        ProcessName = d.ProcessName,
                        State = 0,
                        Sort = d.Sort,
                        IsAll = 0,
                    }));
            }
            if (rows.Count > 0)
            {
                _appDataRepository.AddRange(rows);
            }
        }

        // ==================== 发布/删除 ====================

        public bool Publish(long id)
        {
            SysVersion current = _sysVersionRepository.GetById(id);
            if (current == null || current.IsDel == 1)
            {
                throw new AFBizException("404001", "版本不存在");
            }
            if (current.IsHide != SysVersion.HIDE_STATUS_1)
            {
                throw new AFBizException("400102", "仅草稿版本可发布");
            }
            current.IsHide = SysVersion.HIDE_STATUS_0;
            current.EffectiveTime = DateTime.Now;
            current.UpdateTime = DateTime.Now;
            _sysVersionRepository.Update(current);
            return true;
        }

        public bool DeleteDraft(long id)
        {
            SysVersion current = _sysVersionRepository.GetById(id);
            if (current == null || current.IsDel == 1)
            {
                throw new AFBizException("404001", "版本不存在");
            }
            if (current.IsHide != SysVersion.HIDE_STATUS_1)
            {
                throw new AFBizException("400103", "仅草稿版本可删除");
            }
            current.IsDel = 1;
            current.UpdateTime = DateTime.Now;
            _sysVersionRepository.Update(current);
            //级联清理关联数据(物理删除, 与Java deleteAppVersionData一致)
            List<BpmProcessAppData> related = _appDataRepository.Find(d => d.VersionId == id);
            if (related.Count > 0)
            {
                _appDataRepository.RemoveRange(related);
            }
            return true;
        }

        // ==================== 关联数据(全量替换) ====================

        public bool SaveAppDatas(AppDataSaveVo vo)
        {
            if (vo == null || vo.VersionId <= 0 || vo.Type <= 0)
            {
                throw new AFBizException("400110", "参数错误");
            }
            if (vo.Type != TYPE_APP && vo.Type != TYPE_APP_DATA && vo.Type != TYPE_APP_QUICK_ENTRY)
            {
                throw new AFBizException("400104", "关联数据类型错误");
            }
            SysVersion current = _sysVersionRepository.GetById(vo.VersionId);
            if (current == null || current.IsDel == 1)
            {
                throw new AFBizException("404001", "版本不存在");
            }
            if (current.IsHide != SysVersion.HIDE_STATUS_1)
            {
                throw new AFBizException("400105", "已发布版本的关联数据只读");
            }
            //全量替换: 先删旧数据
            List<BpmProcessAppData> oldRows = _appDataRepository.Find(d => d.VersionId == vo.VersionId && d.Type == vo.Type);
            if (oldRows.Count > 0)
            {
                _appDataRepository.RemoveRange(oldRows);
            }
            if (vo.Items == null || vo.Items.Count == 0)
            {
                return true;
            }
            List<BpmProcessAppData> rows = new List<BpmProcessAppData>();
            int order = 1;
            foreach (AppDataSaveVo.AppDataItem item in vo.Items)
            {
                if (item == null || string.IsNullOrEmpty(item.Id))
                {
                    continue;
                }
                int sort = item.Sort ?? order;
                BpmProcessAppData row = new BpmProcessAppData
                {
                    VersionId = vo.VersionId,
                    Type = vo.Type,
                    ApplicationId = item.Id,
                    State = 0,
                    Sort = sort,
                    IsAll = 0,
                };
                if (vo.Type == TYPE_APP_DATA)
                {
                    //上线流程: 候选来自 bpmn_conf(effective_status=1), application_id/process_key 均存 formCode
                    BpmnConf conf = _bpmnConfService._repository
                        .FirstOrDefault(c => c.FormCode == item.Id && c.EffectiveStatus == 1);
                    if (conf == null)
                    {
                        continue;
                    }
                    row.ProcessName = conf.BpmnName;
                    row.ProcessKey = conf.FormCode;
                }
                else
                {
                    if (!long.TryParse(item.Id, out long objectId))
                    {
                        continue;
                    }
                    if (vo.Type == TYPE_APP_QUICK_ENTRY)
                    {
                        QuickEntry quickEntry = _quickEntryRepository.GetById(objectId);
                        if (quickEntry == null || quickEntry.IsDel == 1)
                        {
                            continue;
                        }
                        row.ProcessName = quickEntry.Title;
                    }
                    else
                    {
                        BpmProcessAppApplication application = _appApplicationRepository.GetById(objectId);
                        if (application == null || application.IsDel == 1)
                        {
                            continue;
                        }
                        row.ProcessName = application.Title;
                        row.ProcessKey = string.IsNullOrEmpty(application.BusinessCode)
                            ? application.ProcessKey
                            : application.BusinessCode + "_" + application.ProcessKey;
                    }
                }
                rows.Add(row);
                order++;
            }
            if (rows.Count > 0)
            {
                _appDataRepository.AddRange(rows);
            }
            return true;
        }

        // ==================== 候选/已关联数据 ====================

        public List<BaseIdTranStruVo> GetCandidates(int type, string search, int limitSize)
        {
            if (limitSize <= 0) { limitSize = 50; }
            if (type == TYPE_APP_QUICK_ENTRY)
            {
                IEnumerable<QuickEntry> entries = _quickEntryRepository.Find(q => q.IsDel == 0);
                if (!string.IsNullOrEmpty(search))
                {
                    entries = entries.Where(q => !string.IsNullOrEmpty(q.Title) && q.Title.Contains(search));
                }
                return entries.OrderBy(q => q.Sort).ThenBy(q => q.Id).Take(limitSize)
                    .Select(q => new BaseIdTranStruVo { Id = q.Id.ToString(), Name = q.Title })
                    .ToList();
            }
            if (type == TYPE_APP_DATA)
            {
                //上线流程候选: 全部有效流程配置(bpmn_conf effective_status=1), id=formCode
                List<BpmnConf> confs = _bpmnConfService._repository.Find(c => c.EffectiveStatus == 1);
                IEnumerable<BpmnConf> filtered = confs;
                if (!string.IsNullOrEmpty(search))
                {
                    filtered = confs.Where(c =>
                        (!string.IsNullOrEmpty(c.BpmnName) && c.BpmnName.Contains(search))
                        || (!string.IsNullOrEmpty(c.FormCode) && c.FormCode.Contains(search)));
                }
                return filtered.OrderBy(c => c.Id).Take(limitSize)
                    .Select(c => new BaseIdTranStruVo { Id = c.FormCode, Name = c.BpmnName })
                    .ToList();
            }
            IEnumerable<BpmProcessAppApplication> applications = _appApplicationRepository.Find(a => a.IsDel == 0);
            if (!string.IsNullOrEmpty(search))
            {
                applications = applications.Where(a => !string.IsNullOrEmpty(a.Title) && a.Title.Contains(search));
            }
            return applications.OrderBy(a => a.Id).Take(limitSize)
                .Select(a => new BaseIdTranStruVo { Id = a.Id.ToString(), Name = a.Title })
                .ToList();
        }

        public List<AppDataSaveVo.AppDataItem> GetAppDatas(long versionId, int type)
        {
            return _appDataRepository.Find(d => d.VersionId == versionId && d.Type == type && d.State == 0)
                .OrderBy(d => d.Sort)
                .Select(d => new AppDataSaveVo.AppDataItem
                {
                    Id = d.ApplicationId,
                    Name = d.ProcessName,
                    Sort = d.Sort,
                })
                .ToList();
        }

        // ==================== 工具 ====================

        /// <summary>
        /// 已发布(is_hide=0)且未删除的最大index, 与Java SysVersionMapper.maxIndex一致
        /// </summary>
        private int? MaxPublishedIndex()
        {
            List<SysVersion> published = _sysVersionRepository.Find(v => v.IsDel == 0 && v.IsHide == 0);
            if (published.Count == 0)
            {
                return null;
            }
            return published.Max(v => v.Index);
        }
    }
}