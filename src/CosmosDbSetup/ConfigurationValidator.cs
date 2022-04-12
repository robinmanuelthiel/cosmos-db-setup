using FluentValidation;

namespace CosmosDbSetup
{
    public class ConfigurationValidator : AbstractValidator<Configuration>
    {
        public ConfigurationValidator()
        {
            RuleFor(x => x).NotNull().WithMessage("Could not parse configuration.");
            RuleFor(x => x.DatabaseName).NotEmpty().WithMessage("Database name is required.");
        }
    }
}
