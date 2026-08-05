using Microsoft.Extensions.Logging;
﻿namespace Nop.Data.Mapping;

public partial class NopEntityDescriptor
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public NopEntityDescriptor()
    {
		_logger.LogDebug(">>> Entering NopEntityDescriptor()");
        Fields = new List<NopEntityFieldDescriptor>();
		_logger.LogDebug("<<< Exiting NopEntityDescriptor()");
		_logger.LogDebug("NopEntityDescriptor(): Fields → {Fields}", Fields);
    }

    public string EntityName { get; set; }
    public string SchemaName { get; set; }
    public ICollection<NopEntityFieldDescriptor> Fields { get; set; }
}