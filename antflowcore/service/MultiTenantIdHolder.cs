using AntFlowCore.Constants;
using antflowcore.service.interf;
using AntFlowCore.Util;

namespace antflowcore.service;

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