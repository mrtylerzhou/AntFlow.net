using System.Data;
using Rougamo.Context;
using SqlSugar;

namespace AntFlowCore.Abstraction.aop;

[AttributeUsage(AttributeTargets.Method)]
public class TransactionalAttribute : Rougamo.MoAttribute
{
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.ReadCommitted;

    static AsyncLocal<IServiceProvider> m_ServiceProvider = new AsyncLocal<IServiceProvider>();
    public static void SetServiceProvider(IServiceProvider serviceProvider) => m_ServiceProvider.Value = serviceProvider;

    ISqlSugarClient _db;
    public override void OnEntry(MethodContext context)
    {
        ISqlSugarClient db = (ISqlSugarClient)m_ServiceProvider.Value.GetService(typeof(ISqlSugarClient));
        _db = db;
        db.Ado.BeginTran(IsolationLevel);
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
                if (context.Exception == null) _db.Ado.CommitTran();
                else _db.Ado.RollbackTran();
            }
            finally
            {
                _db.Ado.Close();
            }
        }
    }
}
