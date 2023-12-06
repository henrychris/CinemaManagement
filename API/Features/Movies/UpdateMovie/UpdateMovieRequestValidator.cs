using API.Models.Enums;
using FluentValidation;
using Shared;

namespace API.Features.Movies.UpdateMovie;

public class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(x => x.MovieId)
            .NotEmpty();

        When(x => x.Title is not null, () =>
            RuleFor(x => x.Title)!
                .NotEmpty()
                .Length(DomainConstants.MinLength, DomainConstants.MaxLength));

        When(x => x.Description is not null, () =>
            RuleFor(x => x.Description)!
                .NotEmpty()
                .Length(DomainConstants.MinLength, DomainConstants.MaxLength));

        When(x => x.DurationInMinutes is not null, () =>
            RuleFor(x => x.DurationInMinutes)!
                .NotEmpty()
                .GreaterThan(0));

        When(x => x.ReleaseDate is not null, () =>
            RuleFor(x => x.ReleaseDate)!
                .NotEmpty());

        When(x => x.Genres is not null, () =>
            RuleFor(x => x.Genres)!
                .NotEmpty()
                .Must(AreAllGenresValid!)
                .WithMessage("These are the valid genres: " + string.Join(", ", Genres.AllGenres)));

        When(x => x.Rating is not null, () =>
            RuleFor(x => x.Rating)!
                .NotEmpty()
                .GreaterThanOrEqualTo(DomainConstants.MinimumRating)
                .LessThanOrEqualTo(DomainConstants.MaximumRating));

        When(x => x.Director is not null, () =>
            RuleFor(x => x.Director)!
                .NotEmpty()
                .Length(DomainConstants.MinLength, DomainConstants.MaxLength));
    }

    private static bool AreAllGenresValid(string[] selectedGenres)
    {
        // Check if all items in selectedGenres are present in allGenres
        var allValid = selectedGenres.All(genre => Genres.AllGenres.Contains(genre));

        return allValid;
    }
}