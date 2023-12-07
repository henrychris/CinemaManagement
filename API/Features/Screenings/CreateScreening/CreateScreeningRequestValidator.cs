using FluentValidation;

namespace API.Features.Screenings.CreateScreening;

public class CreateScreeningRequestValidator : AbstractValidator<CreateScreeningRequest>
{
    private readonly TimeProvider _timeProvider;

    public CreateScreeningRequestValidator(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;

        RuleFor(x => x.ScreeningDate)
            .NotEmpty()
            .Must(BeInFuture)
            .WithMessage("Date must be in the future.");

        RuleFor(x => x.MovieId)
            .NotEmpty();
        RuleFor(x => x.TheaterId)
            .NotEmpty();
    }

    private bool BeInFuture(DateTime screeningDate)
    {
        return screeningDate > _timeProvider.GetUtcNow().DateTime;
    }
}