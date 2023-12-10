using FluentValidation;

namespace API.Features.Screenings.UpdateScreening;

public class UpdateScreeningRequestValidator : AbstractValidator<UpdateScreeningRequest>
{
    public UpdateScreeningRequestValidator()
    {
    }
}