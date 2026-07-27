namespace Nop.Data.Mapping;

public partial class NopEntityDescriptor
{
    public NopEntityDescriptor()
    {
		logger.LogDebug(">>> Entering NopEntityDescriptor()");
        Fields = new List<NopEntityFieldDescriptor>();
		logger.LogDebug("<<< Exiting NopEntityDescriptor()");
		logger.LogDebug("NopEntityDescriptor(): Fields → {Fields}", Fields);
    }

    public string EntityName { get; set; }
    public string SchemaName { get; set; }
    public ICollection<NopEntityFieldDescriptor> Fields { get; set; }
}