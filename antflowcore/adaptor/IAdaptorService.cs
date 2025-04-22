using System.Collections.Concurrent;

namespace antflowcore.adaptor;

/// <summary>
/// This interface is used to adapt to different business objects.
/// </summary>
public interface IAdaptorService
{
    // Used to store supported business objects
    static readonly ConcurrentDictionary<string, List<Enum>> SupportedBusiness = new();

    /// <summary>
    /// Set supported business objects.
    /// </summary>
    void SetSupportBusinessObjects();

    /// <summary>
    /// Add supported business objects without a marker.
    /// </summary>
    /// <param name="businessObjects">Business objects to add.</param>
    void AddSupportBusinessObjects(params Enum[] businessObjects)
    {
        AddSupportBusinessObjects(string.Empty, businessObjects);
    }

    /// <summary>
    /// Add supported business objects with a marker.
    /// </summary>
    /// <param name="marker">Marker for categorizing business objects.</param>
    /// <param name="businessObjects">Business objects to add.</param>
    void AddSupportBusinessObjects(string marker, params Enum[] businessObjects)
    {
        string key = GetType().FullName + marker;

        if (businessObjects != null)
        {
            foreach (var businessObject in businessObjects)
            {
                if (SupportedBusiness.ContainsKey(key))
                {
                    bool alreadyExists = false;
                    foreach (Enum eEnum in SupportedBusiness[key])
                    {
                        if (eEnum.Equals(businessObject))
                        {
                            alreadyExists = true;
                        }
                    }

                    if (!alreadyExists)
                    {
                        SupportedBusiness[key].Add(businessObject);
                    }
                }
                else
                {
                    SupportedBusiness[key] = new List<Enum> { businessObject };
                }
            }
        }
    }

    /// <summary>
    /// Check whether a given business object is supported without a marker.
    /// </summary>
    /// <param name="businessObject">The business object to check.</param>
    /// <returns>True if the business object is supported; otherwise, false.</returns>
    public bool IsSupportBusinessObject(Enum businessObject)
    {
        return IsSupportBusinessObject(string.Empty, businessObject);
    }

    /// <summary>
    /// Check whether a given business object is supported with a marker.
    /// </summary>
    /// <param name="marker">Marker for categorizing business objects.</param>
    /// <param name="businessObject">The business object to check.</param>
    /// <returns>True if the business object is supported; otherwise, false.</returns>
    bool IsSupportBusinessObject(string marker, Enum businessObject)
    {
        string key = GetType().FullName + marker;

        if (SupportedBusiness.TryGetValue(key, out var enums))
        {
            if (enums.Contains(businessObject))
            {
                return true;
            }
        }

        SetSupportBusinessObjects();

        if (SupportedBusiness.TryGetValue(key, out enums))
        {
            return enums.Contains(businessObject);
        }

        return false;
    }
}