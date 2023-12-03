using ErrorOr;

namespace Shared.ServiceErrors;

public static class GenericErrors
{
    public static Error SystemError => Error.Unexpected(
        code: "API.SystemError",
        description: "Sorry, something went wrong. Please reach out to an admin.");
}