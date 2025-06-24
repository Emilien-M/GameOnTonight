namespace GameOnTonight.Api.Config.Attributes;

/// <summary>
/// Indicates whether the UnitOfWork should be applied for a given action or controller.
/// Can be used to override the default behavior based on HTTP methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class UnitOfWorkAttribute : Attribute
{
    /// <summary>
    /// Indicates whether changes should be persisted via UnitOfWork.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkAttribute"/> class.
    /// </summary>
    /// <param name="enabled">If true, the UnitOfWork will be applied. Otherwise, it will be ignored.</param>
    public UnitOfWorkAttribute(bool enabled = true)
    {
        Enabled = enabled;
    }
}
