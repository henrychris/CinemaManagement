using API.Extensions;
using API.Features.Movies.Requests;
using API.Models.Enums;
using FluentValidation;
using Shared;

namespace API.Features.Movies.Validators;

public class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieRequestValidator()
    {
        RuleFor(x => x).NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(DomainConstants.MinLength, DomainConstants.MaxLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(DomainConstants.MinLength, DomainConstants.MaxLength);

        RuleFor(x => x.DurationInMinutes)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.ReleaseDate)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty()
            .Must(AreAllGenresValid)
            .WithMessage("These are the valid genres: " + string.Join(", ", Genres.AllGenres));

        RuleFor(x => x.Rating)
            .NotEmpty()
            .GreaterThanOrEqualTo(DomainConstants.MinimumRating)
            .LessThanOrEqualTo(DomainConstants.MaximumRating);

        RuleFor(x => x.Director)
            .NotEmpty()
            .Length(DomainConstants.MinLength, DomainConstants.MaxLength);
    }

    private static bool AreAllGenresValid(string[] selectedGenres)
    {
        // Check if all items in selectedGenres are present in allGenres
        var allValid = selectedGenres.All(genre => Genres.AllGenres.Contains(genre.FirstCharToUpper()));

        return allValid;
    }
}