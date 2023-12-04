using ErrorOr;

namespace Shared.ServiceErrors;

public static partial class Errors
{
    public static class Movie
    {
        public static Error NotFound => Error.NotFound(
            code: "Movie.NotFound",
            description: "Movie not found.");
        
        public static Error MovieAlreadyExists => Error.Conflict(
            code: "Movie.MovieAlreadyExists",
            description: "This movie might already exist. Please check to be sure.");
    }
}