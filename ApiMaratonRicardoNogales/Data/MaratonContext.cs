using Microsoft.EntityFrameworkCore;
using NugetMaraton;

namespace ApiMaratonRicardoNogales.Data
{
    public class MaratonContext:DbContext
    {
        public MaratonContext(DbContextOptions<MaratonContext> options) : base(options) { } 

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<EquipoGrupo> EquiposGrupo { get; set; }
        public DbSet<Partido> Partidos { get; set; }
        public DbSet<Gol> Goles { get; set; }
        public DbSet<Tarjeta> Tarjetas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
