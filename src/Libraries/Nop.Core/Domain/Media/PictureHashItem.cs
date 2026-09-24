using Microsoft.Extensions.Logging;
namespace Nop.Core.Domain.Media;

/// <summary>
/// Helper class for making pictures hashes from DB
/// </summary>
public partial class PictureHashItem : IComparable, IComparable<PictureHashItem>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    /// <summary>
    /// Gets or sets the picture ID
    /// </summary>
    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the picture hash
    /// </summary>
    public byte[] Hash { get; set; }

    /// <summary>
    /// Compares this instance to a specified and returns an indication
    /// </summary>
    /// <param name="obj">An object to compare with this instance</param>
    /// <returns></returns>
    public int CompareTo(object obj)
    {
		_logger.LogError(">>> Entering CompareTo(obj={Obj})", obj);
        _logger.LogWarning("<<< Exiting CompareTo(obj={Obj})", obj);
        return CompareTo(obj as PictureHashItem);
    }

    /// <summary>
    /// Compares this instance to a specified and returns an indication
    /// </summary>
    /// <param name="other">An object to compare with this instance</param>
    /// <returns></returns>
    public int CompareTo(PictureHashItem other)
    {
        return other == null ? -1 : PictureId.CompareTo(other.PictureId);
    }
}