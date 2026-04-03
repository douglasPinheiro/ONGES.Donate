using FluentValidation;
using ONGES.Donate.Application.DTOs.Requests;

namespace ONGES.Donate.Infrastructure.Validators;

public sealed class CreateDonationRequestValidator : AbstractValidator<CreateDonationRequest>
{
    public CreateDonationRequestValidator()
    {
        RuleFor(request => request.IdCampanha)
            .NotEmpty()
            .WithMessage("O id da campanha e obrigatorio.");

        RuleFor(request => request.ValorDoado)
            .GreaterThan(0)
            .WithMessage("O valor da doacao deve ser maior que zero.");
    }
}
