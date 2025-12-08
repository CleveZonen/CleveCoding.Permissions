namespace CleveCoding.Kernel;

public interface IImageData
{
    /// <summary>
    ///     Photo in bytes array.
    /// </summary>
    public byte[]? PhotoData { get; set; }

    /// <summary>
    ///     Photo as Url on the web.
    /// </summary>
    public string? PhotoUrl { get; set; }
}