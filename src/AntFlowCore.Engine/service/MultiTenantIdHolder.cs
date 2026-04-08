using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;

namespace AntFlowCore.Engine.service;

public class MultiTenantIdHolder: ITenantIdHolder
{
    public void SetCurrentTenantId(string tenantId)
    {
        ThreadLocalContainer.Set(StringConstants.TENANT_ID,tenantId);
    }

    public string GetCurrentTenantId()
    {
        Object value = ThreadLocalContainer.Get(StringConstants.TENANT_ID);
        return value==null||StringConstants.DEFAULT_TENANT.Equals(value.ToString(),StringComparison.InvariantCultureIgnoreCase)?"":value.ToString();
    }

    public void ClearCurrentTenantId()
    {
        ThreadLocalContainer.Remove(StringConstants.TENANT_ID);
    }
}