using antflowcore.adaptor;
using antflowcore.constant.enus;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Http;

namespace antflowcore.factory;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class ThirdPartyCallbackFactory
{
    private static readonly Lazy<ThirdPartyCallbackFactory> _instance = new Lazy<ThirdPartyCallbackFactory>(() => new ThirdPartyCallbackFactory());

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ThirdPartyCallbackFactory> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;


    private ThirdPartyCallbackFactory()
    {
        // 需要由外部注入这三个服务：HttpClientFactory、Configuration、Logger
    }

    public static ThirdPartyCallbackFactory Instance => _instance.Value;

    public ThirdPartyCallbackFactory(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        OutSideBpmBusinessPartyService outSideBpmBusinessPartyService,
        BpmVerifyInfoBizService bpmVerifyInfoBizService,
        OutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService,
        ILogger<ThirdPartyCallbackFactory> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
         _httpContextAccessor= httpContextAccessor;
         _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
         _bpmVerifyInfoBizService = bpmVerifyInfoBizService;
         _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
    }

    public async Task<T?> DoCallbackAsync<T>(
        CallbackTypeEnum callbackTypeEnum,
        BpmnConfVo bpmnConfVo,
        string processNum,
        string businessId) where T : class
    {
        bool callBackSwitch = true;
        string? callbackSwitchStr = _configuration["outside.callback.switch"];
        if (!string.IsNullOrEmpty(callbackSwitchStr))
        {
            bool.TryParse(callbackSwitchStr, out callBackSwitch);
        }

        Dictionary<string,string> heads = new Dictionary<string, string>();
        string resultJson = string.Empty;
        CallbackReqVo? callbackReqVo = null;

        try
        {
            if (bpmnConfVo.BusinessPartyId == null || bpmnConfVo.BusinessPartyId == 0L)
            {
                _logger.LogError($"业务方缺失，操作失败!流程编号:{processNum},业务id:{businessId}");
                throw new AFBizException("业务方缺失，操作失败！");
            }

            var adaptor = ServiceProviderUtils.GetServices<ICallbackAdaptor<CallbackReqVo, CallbackRespVo>>()
                .First(a => a.IsSupportBusinessObject(callbackTypeEnum));
            if (adaptor == null)
                return null;

            if (!callBackSwitch)
            {
                CallbackRespVo newRespObj = adaptor.GetNewRespObj();
                if (!string.IsNullOrEmpty(businessId))
                    newRespObj.BusinessId = new Random().Next(1, int.MaxValue).ToString();
                return newRespObj as T;
            }

            String businessPartyMarkById = _outSideBpmBusinessPartyService.baseRepo
                .Where(a => a.Id == bpmnConfVo.BusinessPartyId.Value && a.IsDel == 0)
                .First(a => a.BusinessPartyMark);
            callbackReqVo = adaptor.FormatRequest(bpmnConfVo);
            callbackReqVo.BusinessPartyMark = businessPartyMarkById;
            callbackReqVo.EventType = callbackTypeEnum.GetMark();
            callbackReqVo.FormCode = FormatFormCode(callbackReqVo.BusinessPartyMark, bpmnConfVo.FormCode);
            callbackReqVo.ProcessNum = processNum;
            callbackReqVo.BusinessId = businessId;

            if (!string.IsNullOrEmpty(processNum))
            {
                bool finishFlag = callbackTypeEnum == CallbackTypeEnum.PROC_FINISH_CALL_BACK;
                List<BpmVerifyInfoVo> bpmVerifyInfoVos =
                    _bpmVerifyInfoBizService.GetBpmVerifyInfoVos(processNum, finishFlag);
                if (!bpmVerifyInfoVos.IsEmpty())
                {
                    callbackReqVo.ProcessRecord = bpmVerifyInfoVos.Select(a => new OutSideBpmAccessProcessRecordVo()
                        {
                            NodeName = a.TaskName,
                            ApprovalTime = a.VerifyDate.ToString(),
                            ApprovalStatusName = a.VerifyUserName,
                            ApprovalUserName = a.VerifyUserName,
                        })
                        .ToList();
                }
            }

            var loginUser = new BaseIdTranStruVo()
                { Id = SecurityUtils.GetLogInEmpId(), Name = SecurityUtils.GetLogInEmpName() };

            OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = _outSideBpmCallbackUrlConfService.baseRepo
                .Where(a => a.BusinessPartyId == bpmnConfVo.BusinessPartyId && a.Status == 1 &&
                            a.FormCode == bpmnConfVo.FormCode)
                .First();

            string url = string.Empty;

            if (outSideBpmCallbackUrlConf != null)
            {
                url = outSideBpmCallbackUrlConf.BpmFlowCallbackUrl;
                String apiClientId = outSideBpmCallbackUrlConf.ApiClientId;
                String apiClientSecret = outSideBpmCallbackUrlConf.ApiClientSecret;
                heads["api-workflow-sign"] = CreateMd5Sign(JsonSerializer.Serialize(callbackReqVo), apiClientSecret);
            }

            heads["central-service"] = GetCurrentSysDomain();



            heads["sso-uid"] = loginUser.Id;
            heads["sso-name"] = UrlEncoder.Default.Encode(loginUser.Name);

            _logger.LogInformation("执行外部回调: {Type} URL: {Url} 参数: {Body}", callbackTypeEnum.GetDesc(), url,
                JsonSerializer.Serialize(callbackReqVo));


            resultJson = await PostAsync(url, heads, callbackReqVo);

            var resultObj = JsonSerializer.Deserialize<Dictionary<string, object>>(resultJson);
            if (resultObj != null && resultObj.TryGetValue("status", out var status) && status?.ToString() == "000000")
            {
                var resp = adaptor.FormatResponse(resultJson);
                if (resp != null)
                {
                    resp.BusinessPartyMark = callbackReqVo.BusinessPartyMark;
                    return resp as T;
                }
            }

            string message = resultObj?.GetValueOrDefault("message")?.ToString() ?? "工作流对外服务回调失败";
            throw new AFBizException(message);
        }
        catch (AFBizException ex)
        {
            _logger.LogError(ex, "回调失败: {Type}, 请求参数: {Req}, 响应: {Resp}",
                callbackTypeEnum.GetMark(),
                JsonSerializer.Serialize(callbackReqVo),
                resultJson);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError("工作流对外服务回调失败，回调类型：{}，请求头信息{}，入参：{}，出参：{}",
                callbackTypeEnum.GetMark(),
                JsonSerializer.Serialize(heads),
                JsonSerializer.Serialize(callbackReqVo??new CallbackReqVo()),
                resultJson);
            return null;
        }
        return null;
    }

    private string FormatFormCode(string mark, string formCode)
    {
        return formCode.StartsWith(mark) ? formCode : $"{mark}_{formCode}";
    }

    private string CreateMd5Sign(string body, string secret)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(body + secret));
        return Convert.ToBase64String(hash);
    }

    private string GetCurrentSysDomain()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty; // 或者抛异常 / 返回默认域名
        }

        var scheme = request.Scheme;                         // http or https
        var host = request.Host.Value;                       // www.example.com:5001
        var pathBase = request.PathBase.HasValue ? request.PathBase.Value : string.Empty; // 项目上下文路径

        return $"{scheme}://{host}{pathBase}";
    }
    
    

    private async Task<string> PostAsync(string url, Dictionary<string, string> headers, object body)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        foreach (var (k, v) in headers)
        {
            request.Headers.TryAddWithoutValidation(k, v);
        }

        string json = JsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}
