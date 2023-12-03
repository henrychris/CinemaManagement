namespace API.Models.Enums;

public static class UserRoles
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);
    public static readonly string[] AllRoles = [Admin, User];
}