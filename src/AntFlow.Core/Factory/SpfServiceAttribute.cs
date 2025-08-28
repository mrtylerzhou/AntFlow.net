namespace AntFlow.Core.Factory;

/// <summary>
///     This is used by the AntFlow framework to build up adaptor factories.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class SpfServiceAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SpfServiceAttribute" /> class.
    /// </summary>
    /// <param name="tagParser">The type of the TagParser associated with this service.</param>
    public SpfServiceAttribute(Type tagParser)
    {
        TagParser = tagParser ?? throw new ArgumentNullException(nameof(tagParser));
    }

    /// <summary>
    ///     Gets or sets the type of the TagParser associated with this service.
    /// </summary>
    public Type TagParser { get; }
}