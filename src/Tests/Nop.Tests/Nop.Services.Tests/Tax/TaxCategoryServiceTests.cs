using Nop.Core.Domain.Tax;
using Nop.Services.Tax;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace Nop.Tests.Nop.Services.Tests.Tax;

[TestFixture]
public class TaxCategoryServiceTests : ServiceTest<TaxCategory>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    private ITaxCategoryService _taxCategoryService;

    public TaxCategoryServiceTests()
    {
		_logger.LogDebug(">>> Entering TaxCategoryServiceTests()");
        _taxCategoryService = GetService<ITaxCategoryService>();
		_logger.LogDebug("<<< Exiting TaxCategoryServiceTests()");
		_logger.LogDebug("TaxCategoryServiceTests(): _taxCategoryService → {_taxCategoryService}", _taxCategoryService);
    }

    protected override CrudData<TaxCategory> CrudData
    {
        get
        {
            var baseEntity = new TaxCategory
            {
                Name = "Test tax category",
                DisplayOrder = 1
            };

            var updatedEntity = new TaxCategory
            {
                Name = "Updated test tax category",
                DisplayOrder = 2
            };

            return new CrudData<TaxCategory>
            {
                BaseEntity = baseEntity,
                UpdatedEntity = updatedEntity,
                Insert = _taxCategoryService.InsertTaxCategoryAsync,
                Update = _taxCategoryService.UpdateTaxCategoryAsync,
                Delete = _taxCategoryService.DeleteTaxCategoryAsync,
                GetById = _taxCategoryService.GetTaxCategoryByIdAsync,
                IsEqual = (first, second) => first.Name.Equals(second.Name) && first.DisplayOrder == second.DisplayOrder
            };
        }
    }
}
