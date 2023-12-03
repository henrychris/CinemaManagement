using System.ComponentModel.DataAnnotations;
using API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Shared;

namespace API.Models.Domain;

public class ApplicationUser : IdentityUser
{
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(DomainConstants.MaxNameLength)]
    public required string FirstName { get; set; }

    [MaxLength(DomainConstants.MaxNameLength)]
    public required string LastName { get; set; }
    
    /// <summary>
    /// Can be either Admin or User.
    /// </summary>
    [MaxLength(DomainConstants.MaxEnumLength)]
    public required string Role { get; set; } = UserRoles.User.ToString();
}