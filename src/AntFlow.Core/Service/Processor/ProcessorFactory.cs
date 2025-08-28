using AntFlow.Core.Util;

namespace AntFlow.Core.Service.Processor;

public class ProcessorFactory
{
    // Executes post-processors
    public static void ExecutePostProcessors<TEntity>(TEntity entity)
    {
        ExecutePostProcessors<IAntFlowOrderPostProcessor<TEntity>, TEntity>(entity);
    }

    public static void ExecutePostProcessors<TProcessor, TEntity>(TEntity entity)
        where TProcessor : IAntFlowOrderPostProcessor<TEntity>
    {
        List<TProcessor>? orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (TProcessor? processor in orderedProcessors)
        {
            processor.PostProcess(entity);
        }
    }

    // Executes pre-read processors
    public static void ExecutePreReadProcessors<TEntity>(TEntity entity)
    {
        ExecutePreReadProcessors<IAntFlowOrderPreProcessor<TEntity>, TEntity>(entity);
    }

    public static void ExecutePreReadProcessors<TProcessor, TEntity>(TEntity entity)
        where TProcessor : IAntFlowOrderPreProcessor<TEntity>
    {
        List<TProcessor>? orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (TProcessor? processor in orderedProcessors)
        {
            processor.PreReadProcess(entity);
        }
    }

    // Executes pre-write processors
    public static void ExecutePreWriteProcessors<TEntity>(TEntity entity)
    {
        ExecutePreWriteProcessors<IAntFlowOrderPreProcessor<TEntity>, TEntity>(entity);
    }

    public static void ExecutePreWriteProcessors<TProcessor, TEntity>(TEntity entity)
        where TProcessor : IAntFlowOrderPreProcessor<TEntity>
    {
        List<TProcessor>? orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (TProcessor? processor in orderedProcessors)
        {
            processor.PreWriteProcess(entity);
        }
    }

    // Gets ordered processors matching the given entity type
    private static List<TProcessor> GetOrderedProcessors<TProcessor, TEntity>() where TProcessor : IOrderedService
    {
        IEnumerable<TProcessor>? services = ServiceProviderUtils.GetOrderedServices<TProcessor>();


        List<TProcessor> processors = services.ToList();
        if (processors.Count() == 1)
        {
            return processors;
        }

        List<TProcessor>? matchedProcessors = new();
        foreach (TProcessor? processor in processors)
        {
            Type[]? interfaces = processor.GetType().GetInterfaces();
            foreach (Type? @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(TProcessor))
                {
                    Type? typeArgument = @interface.GetGenericArguments().FirstOrDefault();
                    if (typeArgument != null && typeof(TEntity).IsAssignableFrom(typeArgument))
                    {
                        matchedProcessors.Add(processor);
                    }
                }
            }
        }

        return matchedProcessors;
    }
}