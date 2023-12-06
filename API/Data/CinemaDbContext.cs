using API.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class CinemaDbContext(DbContextOptions<CinemaDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("CinemaDb");
    }

    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Seat> Seats { get; set; } = null!;
    public DbSet<Screening> Screenings { get; set; } = null!;
    public DbSet<Theater> Theaters { get; set; } = null!;
}