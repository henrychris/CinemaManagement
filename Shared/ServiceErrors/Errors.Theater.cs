using ErrorOr;

namespace Shared.ServiceErrors;

public static partial class Errors
{
    public static class Theater
    {
        public static Error NotFound => Error.NotFound(
            code: "Theater.NotFound",
            description: "Theater not found.");
        
        public static Error TheaterAlreadyExists => Error.Conflict(
            code: "Theater.TheaterAlreadyExists",
            description: "This theater might already exist. Please check to be sure.");
    }
}