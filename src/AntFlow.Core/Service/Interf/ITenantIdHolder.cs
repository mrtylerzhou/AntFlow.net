namespace AntFlow.Core.Service.Interface;

public interface ITenantIdHolder
{
    /**
    * Sets the current tenant identifier.
    */
    void SetCurrentTenantId(string tenantId);

    /**
     * Returns the current tenant identifier.
     */
    string GetCurrentTenantId();

    /**
     * Clears the current tenant identifier settings.
     */
    void ClearCurrentTenantId();
}