using FluentValidation;
using FCG_Games.Application.Games.Requests;

namespace FCG_Games.Application.Games.Validators
{
    public class GameRequestValidator : AbstractValidator<GameRequest>
    {
        public GameRequestValidator() 
        { 
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(200).WithMessage("O Título não pode exceder 200 caracteres.");

            RuleFor(x => x.Genre)
                .IsInEnum().WithMessage("Gênero inválido.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("O preço não pode ser negativo.");

            RuleFor(x => x.LaunchYear)
                .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("O ano de lançamento não pode ser no futuro.");

            RuleFor(x => x.Developer)
                .NotEmpty().WithMessage("A desenvolvedora é obrigatória.")
                .MaximumLength(100).WithMessage("A desenvolvedora não pode exceder 100 caracteres.");
        }
    }
}
