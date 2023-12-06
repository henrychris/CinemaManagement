using API.Extensions;
using API.Models.Enums;
using FluentValidation;

namespace API.Features.Theaters.CreateTheater;

public class CreateTheaterRequestValidator : AbstractValidator<CreateTheaterRequest>
{
    private readonly string[] _validScreenTypes = Enum.GetNames(typeof(ScreenTypes));

    public CreateTheaterRequestValidator()
    {
        RuleFor(x => x.AvailableSeats)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.ScreenType)
            .Must(IsScreenTypeValid)
            .WithMessage("These are the valid screen types: " + string.Join(", ", _validScreenTypes));

        // todo: screen types should be in appSettings.
    }

    // todo: write tests for this shit dawg.
    private bool IsScreenTypeValid(string screenType)
    {
        return _validScreenTypes.Contains(screenType.FirstCharToUpper());
    }
}