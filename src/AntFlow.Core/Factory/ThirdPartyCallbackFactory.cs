using AntFlow.Core.Adaptor;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using AntFlowException = AntFlow.Core.Exception;

namespace AntFlow.Core.Factory;

public class ThirdPartyCallbackFactory
{
    private static readonly Lazy<ThirdPartyCallbackFactory> _instance = new(() => new ThirdPartyCallbackFactory());
    private readonly BpmVerifyInfoBizService _bpmVerifyInfoBizService;
    private readonly IConfiguration _configuration;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ThirdPartyCallbackFactory> _logger;
    private readonly OutSideBpmBusinessPartyService _outSideBpmBusinessPartyService;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;


    private ThirdPartyCallbackFactory()
    {
        // 需要通过外部注入来获取HttpClientFactory、Configuration、Logger
    }

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
        _httpContextAccessor = httpContextAccessor;
        _outSideBpmBusinessPartyService = outSideBpmBusinessPartyService;
        _bpmVerifyInfoBizService = bpmVerifyInfoBizService;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
    }

    public static ThirdPartyCallbackFactory Instance => _instance.Value;

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

        Dictionary<string, string> heads = new();
        string resultJson = string.Empty;
        CallbackReqVo? callbackReqVo = null;

        try
        {
            if (bpmnConfVo.BusinessPartyId == null || bpmnConfVo.BusinessPartyId == 0L)
            {
                _logger.LogError($"业务方缺失，回调失败！流程编号:{processNum},业务id:{businessId}");
                throw new AntFlowException.AFBizException("业务方缺失，回调失败！");
            }

            ICallbackAdaptor<CallbackReqVo, CallbackRespVo>? adaptor = ServiceProviderUtils
                .GetServices<ICallbackAdaptor<CallbackReqVo, CallbackRespVo>>()
                .First(a => a.IsSupportBusinessObject(callbackTypeEnum));
            if (adaptor == null)
            {
                return null;
            }

            if (!callBackSwitch)
            {
                CallbackRespVo newRespObj = adaptor.GetNewRespObj();
                if (!string.IsNullOrEmpty(businessId))
                {
                    newRespObj.BusinessId = new Random().Next(1, int.MaxValue).ToString();
                }

                return newRespObj as T;
            }

            string businessPartyMarkById = _outSideBpmBusinessPartyService.baseRepo
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
                    callbackReqVo.ProcessRecord = bpmVerifyInfoVos.Select(a => new OutSideBpmAccessProcessRecordVo
                        {
                            NodeName = a.TaskName,
                            ApprovalTime = a.VerifyDate.ToString(),
                            ApprovalStatusName = a.VerifyUserName,
                            ApprovalUserName = a.VerifyUserName
                        })
                        .ToList();
                }
            }

            BaseIdTranStruVo? loginUser =
                new() { Id = SecurityUtils.GetLogInEmpId(), Name = SecurityUtils.GetLogInEmpName() };

            OutSideBpmCallbackUrlConf outSideBpmCallbackUrlConf = _outSideBpmCallbackUrlConfService.baseRepo
                .Where(a => a.BusinessPartyId == bpmnConfVo.BusinessPartyId && a.Status == 1 &&
                            a.FormCode == bpmnConfVo.FormCode)
                .First();

            string url = string.Empty;

            if (outSideBpmCallbackUrlConf != null)
            {
                url = outSideBpmCallbackUrlConf.BpmFlowCallbackUrl;
                string apiClientId = outSideBpmCallbackUrlConf.ApiClientId;
                string apiClientSecret = outSideBpmCallbackUrlConf.ApiClientSecret;
                heads["api-workflow-sign"] = CreateMd5Sign(JsonSerializer.Serialize(callbackReqVo), apiClientSecret);
            }

            heads["central-service"] = GetCurrentSysDomain();


            heads["sso-uid"] = loginUser.Id;
            heads["sso-name"] = UrlEncoder.Default.Encode(loginUser.Name);

            _logger.LogInformation("执行外部回调: {Type} URL: {Url} 参数: {Body}", callbackTypeEnum.GetDesc(), url,
                JsonSerializer.Serialize(callbackReqVo));


            resultJson = await PostAsync(url, heads, callbackReqVo);

            Dictionary<string, object>? resultObj = JsonSerializer.Deserialize<Dictionary<string, object>>(resultJson);
            if (resultObj != null && resultObj.TryGetValue("status", out object? status) &&
                status?.ToString() == "000000")
            {
                CallbackRespVo? resp = adaptor.FormatResponse(resultJson);
                if (resp != null)
                {
                    resp.BusinessPartyMark = callbackReqVo.BusinessPartyMark;
                    return resp as T;
                }
            }

            string message = resultObj?.GetValueOrDefault("message")?.ToString() ?? "未知错误，外部回调失败";
            throw new AntFlowException.AFBizException(message);
        }
        catch (AntFlowException.AFBizException ex)
        {
            _logger.LogError(ex, "回调失败: {Type}, 请求参数: {Req}, 响应: {Resp}",
                callbackTypeEnum.GetMark(),
                JsonSerializer.Serialize(callbackReqVo),
                resultJson);
            throw;
        }
        catch (System.Exception ex)
        {
            _logger.LogError("未知错误，外部回调失败，回调类型：{}，请求头信息{}，参数：{}，响应：{}",
                callbackTypeEnum.GetMark(),
                JsonSerializer.Serialize(heads),
                JsonSerializer.Serialize(callbackReqVo ?? new CallbackReqVo()),
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
        using MD5? md5 = MD5.Create();
        byte[]? hash = md5.ComputeHash(Encoding.UTF8.GetBytes(body + secret));
        return Convert.ToBase64String(hash);
    }

    private string GetCurrentSysDomain()
    {
        HttpRequest? request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
        {
            return string.Empty; // 处理异常情况 / 返回默认值
        }

        string? scheme = request.Scheme; // http or https
        string? host = request.Host.Value; // www.example.com:5001
        string? pathBase = request.PathBase.HasValue ? request.PathBase.Value : string.Empty; // 项目基础路径

        return $"{scheme}://{host}{pathBase}";
    }


    private async Task<string> PostAsync(string url, Dictionary<string, string> headers, object body)
    {
        HttpClient? client = _httpClientFactory.CreateClient();
        HttpRequestMessage? request = new(HttpMethod.Post, url);
        foreach ((string? k, string? v) in headers)
        {
            request.Headers.TryAddWithoutValidation(k, v);
        }

        string json = JsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage? response = await client.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }
}