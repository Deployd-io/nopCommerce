using FluentValidation;
using Nop.Data;
using Nop.Web.Framework.Validators;
using Nop.Web.Infrastructure.Installation;
using Nop.Web.Models.Install;
using Microsoft.Extensions.Logging;

namespace Nop.Web.Validators.Install;

public partial class InstallValidator : BaseNopValidator<InstallModel>
{
	private static readonly Microsoft.Extensions.Logging.ILogger _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
    public InstallValidator(IInstallationLocalizationService locService)
    {
		_logger.LogDebug(">>> Entering InstallValidator(locService={LocService})", locService);
        RuleFor(x => x.AdminEmail).NotEmpty().WithMessage(locService.GetResource("AdminEmailRequired"));
        RuleFor(x => x.AdminEmail)
            .IsEmailAddress();
        RuleFor(x => x.AdminPassword).NotEmpty().WithMessage(locService.GetResource("AdminPasswordRequired"));
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(locService.GetResource("ConfirmPasswordRequired"));
        RuleFor(x => x.AdminPassword).Equal(x => x.ConfirmPassword).WithMessage(locService.GetResource("PasswordsDoNotMatch"));

        RuleFor(x => x.DataProvider).NotEqual(DataProviderType.Unknown).WithMessage(locService.GetResource("DataProviderRequired"));
        RuleFor(x => x.ConnectionString)
            .NotEmpty()
            .When(x => x.ConnectionStringRaw)
            .WithMessage(locService.GetResource("ConnectionStringRequired"));

        When(x => !x.ConnectionStringRaw, () =>
        {
            RuleFor(x => x.ServerName).NotEmpty().WithMessage(locService.GetResource("ServerNameRequired"));
            RuleFor(x => x.DatabaseName).NotEmpty().WithMessage(locService.GetResource("ConnectionStringRequired"));

            When(x => !x.IntegratedSecurity, () =>
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage(locService.GetResource("SqlUsernameRequired"));
                RuleFor(x => x.Password).NotEmpty().WithMessage(locService.GetResource("SqlPasswordRequired"));
            });
        });

        RuleFor(x => x.Collation)
            .NotEmpty()
            .When(x => x.UseCustomCollation)
            .WithMessage(locService.GetResource("CollationRequired"));
			_logger.LogDebug("<<< Exiting InstallValidator(locService={LocService})", locService);
    }
}