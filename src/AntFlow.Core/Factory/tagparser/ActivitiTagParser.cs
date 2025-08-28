using AntFlow.Core.Adaptor;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Collections;
using System.Reflection;

namespace AntFlow.Core.Factory.TagParser;

public class ActivitiTagParser<T> : TagParser<IFormOperationAdaptor<T>, BusinessDataVo> where T : BusinessDataVo
{
    public IFormOperationAdaptor<T> ParseTag(BusinessDataVo data)
    {
        IEnumerable services = ServiceProviderUtils.GetServicesByOpenGenericType(typeof(IFormOperationAdaptor<>));
        foreach (object service in services)
        {
            object[]? customAttributes = service.GetType().GetCustomAttributes(true);

            IEnumerable<AfFormServiceAnnoAttribute> afFormServiceAnnoAttributes =
                customAttributes.OfType<AfFormServiceAnnoAttribute>();
            if (!afFormServiceAnnoAttributes.Any())
            {
                return null;
            }

            foreach (AfFormServiceAnnoAttribute afFormServiceAnnoAttribute in afFormServiceAnnoAttributes)
            {
                if (afFormServiceAnnoAttribute.SvcName.Equals(data.FormCode) ||
                    (data.IsLowCodeFlow is 1 && afFormServiceAnnoAttribute.SvcName.Equals("LF")))
                {
                    //获取IFormOperationAdaptor接口的实现类，如果IFormOperationAdaptor为空，则表示没有对应的适配器，返回空值0
                    Type superInterfaceArgType = service.GetType().GetInterfaces()
                        .First(a => a.GetGenericTypeDefinition() == typeof(IFormOperationAdaptor<>))
                        .GetGenericArguments()[0];
                    Type[] genericArgs = [superInterfaceArgType];
                    Type iFormOperationAdaptorWithGenericType =
                        typeof(IFormOperationAdaptor<>).MakeGenericType(genericArgs);
                    // 获取服务实例
                    MethodInfo? method = typeof(ActivitiTagParser<T>)
                        .GetMethod(nameof(AdaptServiceToRequiredType), BindingFlags.NonPublic | BindingFlags.Instance)
                        .MakeGenericMethod(superInterfaceArgType);

                    // 检查是否为泛型接口
                    return (IFormOperationAdaptor<T>)method.Invoke(this, new[] { service });
                }
            }
        }

        return null;
    }

    private IFormOperationAdaptor<T> AdaptServiceToRequiredType<TSubClass>(object service) where TSubClass : T
    {
        // 获取泛型参数类型
        return new AdaptedServiceWrapper<TSubClass>(service as IFormOperationAdaptor<TSubClass>);
    }

    public class AdaptedServiceWrapper<TSubClass> : IFormOperationAdaptor<T> where TSubClass : BusinessDataVo
    {
        private readonly IFormOperationAdaptor<TSubClass> _inner;

        public AdaptedServiceWrapper(IFormOperationAdaptor<TSubClass> inner)
        {
            _inner = inner;
        }


        // 查找匹配的 IFormOperationAdaptor<T> 服务实现
        public BpmnStartConditionsVo PreviewSetCondition(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                return _inner.PreviewSetCondition(subClassVo);
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}");
        }

        public BpmnStartConditionsVo LaunchParameters(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                return _inner.LaunchParameters(subClassVo);
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}");
        }

        public void OnInitData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnInitData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnQueryData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnQueryData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnSubmitData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnSubmitData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnConsentData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnConsentData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnBackToModifyData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnBackToModifyData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnCancellationData(T vo)
        {
            if (vo is TSubClass subClassVo)
            {
                _inner.OnCancellationData(subClassVo);
                return;
            }

            throw new ArgumentException($"The provided argument is not of the expected type {typeof(TSubClass).Name}.");
        }

        public void OnFinishData(BusinessDataVo vo)
        {
            _inner.OnFinishData(vo);
        }
    }
}