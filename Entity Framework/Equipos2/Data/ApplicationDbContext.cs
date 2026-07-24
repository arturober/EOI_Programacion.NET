using Equipos.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipos.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Jugador> Jugadores => Set<Jugador>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Juego).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);

            entity.HasMany(e => e.Jugadores)
                  .WithOne(j => j.Equipo)
                  .HasForeignKey(j => j.EquipoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Jugador>(entity =>
        {
            entity.HasKey(j => j.Id);
            entity.Property(j => j.Nickname).IsRequired().HasMaxLength(50);
            entity.Property(j => j.NombreCompleto).IsRequired().HasMaxLength(100);
            entity.Property(j => j.Rol).IsRequired().HasMaxLength(50);
        });

        // Seed Data para demostración eSports
        var equipo1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var equipo2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

        modelBuilder.Entity<Equipo>().HasData(
            new Equipo
            {
                Id = equipo1Id,
                Nombre = "Fnatic eSports",
                Juego = "Valorant",
                LogoUrl = "https://images.unsplash.com/photo-1542751371-adc38448a05e?w=200&q=80",
                FechaCreacion = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Equipo
            {
                Id = equipo2Id,
                Nombre = "T1 Gaming",
                Juego = "League of Legends",
                LogoUrl = "https://images.unsplash.com/photo-1538481199705-c710c4e965fc?w=200&q=80",
                FechaCreacion = new DateTime(2023, 11, 20, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Jugador>().HasData(
            new Jugador { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), Nickname = "Chronicle", NombreCompleto = "Timofey Khromov", Rol = "Initiator", EquipoId = equipo1Id },
            new Jugador { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), Nickname = "Boaster", NombreCompleto = "Jake Howlett", Rol = "IGL / Controller", EquipoId = equipo1Id },
            new Jugador { Id = Guid.Parse("b1111111-1111-1111-1111-111111111111"), Nickname = "Faker", NombreCompleto = "Lee Sang-hyeok", Rol = "Midlaner", EquipoId = equipo2Id },
            new Jugador { Id = Guid.Parse("b2222222-2222-2222-2222-222222222222"), Nickname = "Gumayusi", NombreCompleto = "Lee Min-hyeong", Rol = "ADC", EquipoId = equipo2Id }
        );
    }
}
