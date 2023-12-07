using API.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class CinemaDbContext : IdentityDbContext<ApplicationUser>
{
    public CinemaDbContext()
    {
        
    }

    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("CinemaDb");
    }

    public virtual DbSet<Movie> Movies { get; set; } = null!;
    public virtual DbSet<Seat> Seats { get; set; } = null!;
    public virtual DbSet<Screening> Screenings { get; set; } = null!;
    public virtual DbSet<Theater> Theaters { get; set; } = null!;
}