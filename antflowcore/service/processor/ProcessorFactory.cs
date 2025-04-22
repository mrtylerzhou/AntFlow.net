using antflowcore.util;

namespace antflowcore.service.processor;

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
        var orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (var processor in orderedProcessors)
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
        var orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (var processor in orderedProcessors)
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
        var orderedProcessors = GetOrderedProcessors<TProcessor, TEntity>();
        foreach (var processor in orderedProcessors)
        {
            processor.PreWriteProcess(entity);
        }
    }

    // Gets ordered processors matching the given entity type
    private static List<TProcessor> GetOrderedProcessors<TProcessor, TEntity>() where TProcessor : IOrderedService
    {
        var services = ServiceProviderUtils.GetOrderedServices<TProcessor>();

        List<TProcessor> processors = services.ToList();
        if (processors.Count() == 1)
        {
            return processors;
        }

        var matchedProcessors = new List<TProcessor>();
        foreach (var processor in processors)
        {
            var interfaces = processor.GetType().GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(TProcessor))
                {
                    var typeArgument = @interface.GetGenericArguments().FirstOrDefault();
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