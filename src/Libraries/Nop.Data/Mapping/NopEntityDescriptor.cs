using Microsoft.Extensions.Logging;
namespace Nop.Data.Mapping;

public partial class NopEntityDescriptor
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NopEntityDescriptor()
    {
		_logger.LogError(">>> Entering NopEntityDescriptor()");
        Fields = new List<NopEntityFieldDescriptor>();
		_logger.LogDebug("NopEntityDescriptor(): Fields → {Fields}", Fields);
    _logger.LogError("NopEntityDescriptor(): Fields → {Fields}", Fields);
    _logger.LogWarning("<<< Exiting NopEntityDescriptor()");
    }

    public string EntityName { get; set; }
    public string SchemaName { get; set; }
    public ICollection<NopEntityFieldDescriptor> Fields { get; set; }
}