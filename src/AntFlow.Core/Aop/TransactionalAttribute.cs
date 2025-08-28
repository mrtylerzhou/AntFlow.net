using FreeSql;
using Rougamo;
using Rougamo.Context;
using System.Data;

namespace AntFlow.Core.Aop;

[AttributeUsage(AttributeTargets.Method)]
public class TransactionalAttribute : MoAttribute
{
    private static readonly AsyncLocal<IServiceProvider> m_ServiceProvider = new();

    private IUnitOfWork _uow;
    private IsolationLevel? m_IsolationLevel;
    public Propagation Propagation { get; set; } = Propagation.Required;
    public IsolationLevel IsolationLevel { get => m_IsolationLevel.Value; set => m_IsolationLevel = value; }

    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        m_ServiceProvider.Value = serviceProvider;
    }

    public override void OnEntry(MethodContext context)
    {
        UnitOfWorkManager uowManager = (UnitOfWorkManager)m_ServiceProvider.Value.GetService(typeof(UnitOfWorkManager));
        _uow = uowManager.Begin(Propagation, m_IsolationLevel);
    }

    public override void OnExit(MethodContext context)
    {
        if (typeof(Task).IsAssignableFrom(context.ReturnType))
        {
            ((Task)context.ReturnValue).ContinueWith(t => _OnExit());
        }
        else
        {
            _OnExit();
        }

        void _OnExit()
        {
            try
            {
                if (context.Exception == null)
                {
                    _uow.Commit();
                }
                else
                {
                    _uow.Rollback();
                }
            }
            finally
            {
                _uow.Dispose();
            }
        }
    }
}