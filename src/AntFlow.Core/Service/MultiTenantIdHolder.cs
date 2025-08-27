using AntFlow.Core.Constant;
using AntFlow.Core.Service.Interface;
using AntFlow.Core.Util;

namespace AntFlow.Core.Service;

public class MultiTenantIdHolder : ITenantIdHolder
{
    public void SetCurrentTenantId(string tenantId)
    {
        ThreadLocalContainer.Set(StringConstants.TENANT_ID, tenantId);
    }

    public string GetCurrentTenantId()
    {
        object value = ThreadLocalContainer.Get(StringConstants.TENANT_ID);
        return value == null ||
               StringConstants.DEFAULT_TENANT.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase)
            ? ""
            : value.ToString();
    }

    public void ClearCurrentTenantId()
    {
        ThreadLocalContainer.Remove(StringConstants.TENANT_ID);
    }
}