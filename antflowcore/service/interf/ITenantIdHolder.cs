namespace antflowcore.service.interf;

public interface ITenantIdHolder
{
    /**
    * Sets the current tenant identifier.
    */
    void SetCurrentTenantId(String tenantId);

    /**
     * Returns the current tenant identifier.
     */
    String GetCurrentTenantId();

    /**
     * Clears the current tenant identifier settings.
     */
    void ClearCurrentTenantId();
}