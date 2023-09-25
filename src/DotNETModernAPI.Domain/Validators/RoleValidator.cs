using DotNETModernAPI.Domain.Entities;
using FluentValidation;

namespace DotNETModernAPI.Domain.Validators;

public class RoleValidator : AbstractValidator<Role>
{
    public RoleValidator()
    {
        RuleFor(r => r.Id)
            .NotEqual(Guid.Empty).WithMessage("Role.Id can't be equal Guid.Empty");

        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Role.Name can't be empty")
            .MinimumLength(4).WithMessage("Role.Name can't have less than 4 characters")
            .MaximumLength(10).WithMessage("Role.Name can't be greater than 10 characters");

        RuleFor(r => r.NormalizedName)
            .NotNull().WithMessage("Role.NormalizedName can't be null")
            .NotEmpty().WithMessage("Role.NormalizedName can't be empty")
            .Equal(r => r.Name.ToUpper()).WithMessage("Role.NormalizedName must be equal Role.Name in UpperCase")
            .MinimumLength(4).WithMessage("Role.NormalizedName can't have less than 4 characters")
            .MaximumLength(10).WithMessage("Role.NormalizedName can't be greater than 10 characters");

        RuleFor(r => r.ConcurrencyStamp)
            .NotEmpty().WithMessage("Role.ConcurrencyStamp can't be empty")
            .NotEqual(Guid.Empty.ToString()).WithMessage("Role.ConcurrencyStamp can't be equal Guid.Empty");
    }
}
