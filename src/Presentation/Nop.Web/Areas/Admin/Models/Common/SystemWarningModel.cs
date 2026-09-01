using Microsoft.Extensions.Logging;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Common;

public partial record SystemWarningModel : BaseNopModel
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public SystemWarningLevel Level { get; set; }

    public string Text { get; set; }

    public bool DontEncode { get; set; }

    public override string ToString()
    {
		_logger.LogDebug(">>> Entering ToString()");
        return Text;
    }
}