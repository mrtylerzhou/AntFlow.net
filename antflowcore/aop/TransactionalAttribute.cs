using FreeSql;
using Rougamo.Context;
using System.Data;

namespace antflowcore.aop;

[AttributeUsage(AttributeTargets.Method)]
public class TransactionalAttribute : Rougamo.MoAttribute
{
    public Propagation Propagation { get; set; } = Propagation.Required;
    public IsolationLevel IsolationLevel { get => m_IsolationLevel.Value; set => m_IsolationLevel = value; }
    private IsolationLevel? m_IsolationLevel;

    private static AsyncLocal<IServiceProvider> m_ServiceProvider = new AsyncLocal<IServiceProvider>();

    public static void SetServiceProvider(IServiceProvider serviceProvider) => m_ServiceProvider.Value = serviceProvider;

    private IUnitOfWork _uow;

    public override void OnEntry(MethodContext context)
    {
        UnitOfWorkManager uowManager = (UnitOfWorkManager)m_ServiceProvider.Value.GetService(typeof(UnitOfWorkManager));
        _uow = uowManager.Begin(this.Propagation, this.m_IsolationLevel);
    }

    public override void OnExit(MethodContext context)
    {
        if (typeof(Task).IsAssignableFrom(context.ReturnType))
            ((Task)context.ReturnValue).ContinueWith(t => _OnExit());
        else _OnExit();

        void _OnExit()
        {
            try
            {
                if (context.Exception == null) _uow.Commit();
                else _uow.Rollback();
            }
            finally
            {
                _uow.Dispose();
            }
        }
    }
}