using DotNETModernAPI.Domain.Entities;
using FluentValidation;

namespace DotNETModernAPI.Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Id)
            .NotEqual(Guid.Empty).WithMessage("User.Id can't be equal Guid.Empty");

        RuleFor(u => u.UserName)
            .NotEmpty().WithMessage("User.UserName can't be empty")
            .MinimumLength(4).WithMessage("User.UserName can't have less than 4 characters")
            .MaximumLength(16).WithMessage("User.UserName can't be greater than 16 characters");

        RuleFor(u => u.NormalizedUserName)
            .NotNull().WithMessage("User.NormalizedUserName can't be null")
            .NotEmpty().WithMessage("User.NormalizedUserName can't be empty")
            .Equal(u => u.UserName.ToUpper()).WithMessage("User.NormalizedUserName must be equal User.UserName in UpperCase")
            .MinimumLength(4).WithMessage("User.NormalizedUserName can't have less than 4 characters")
            .MaximumLength(16).WithMessage("User.NormalizedUserName can't be greater than 16 characters");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("User.Email can't be empty")
            .EmailAddress().WithMessage("User.Email must be a valid email")
            .MaximumLength(320).WithMessage("User.Email can't be greater than 320 characters");

        RuleFor(u => u.NormalizedEmail)
            .NotEmpty().WithMessage("User.NormalizedEmail can't be empty")
            .Equal(u => u.Email.ToUpper()).WithMessage("User.NormalizedEmail must be equal User.NormalizedEmail in UpperCase")
            .EmailAddress().WithMessage("User.NormalizedEmail must be a valid email")
            .MaximumLength(320).WithMessage("User.NormalizedEmail can't be greater than 320 characters");

        RuleFor(u => u.ConcurrencyStamp)
            .NotEmpty().WithMessage("User.ConcurrencyStamp can't be empty")
            .NotEqual(Guid.Empty.ToString()).WithMessage("User.ConcurrencyStamp can't be equal Guid.Empty");
    }
}
