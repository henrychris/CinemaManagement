using ErrorOr;

namespace Shared.ServiceErrors;

public static partial class Errors
{
    public static class Screening
    {
        public static Error NotFound => Error.NotFound(
            code: "Screening.NotFound",
            description: "Screening not found.");

        public static Error TimeConflict => Error.Conflict(
            code: "Screening.TimeConflict",
            description:
            "This screening is conflicting with another scheduled for this time slot. Change the time slot and try again.");
    }
}