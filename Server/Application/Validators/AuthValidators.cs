using FluentValidation;
using Server.Application.DTOs;

namespace Server.Application.Validators {
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest> {
        public RegisterRequestValidator() {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username es requerido")
                .MinimumLength(3).WithMessage("Username debe tener al menos 3 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email es requerido")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password es requerido")
                .MinimumLength(6).WithMessage("Password debe tener al menos 6 caracteres");
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequest> {
        public LoginRequestValidator() {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email es requerido")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password es requerido");
        }
    }

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest> {
        public RefreshTokenRequestValidator() {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshToken es requerido");
        }
    }
}