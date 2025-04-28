using ApiMaratonRicardoNogales.Data;
using ApiMaratonRicardoNogales.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NugetMaraton;

namespace ApiMaratonRicardoNogales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private MaratonContext context;

        public PartidosController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<PartidoDTO>>> GetPartidos()
        {
            var grupos = await context.Grupos.ToListAsync();
            var equiposGrupo = await context.EquiposGrupo.ToListAsync();

            var partidos = await context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .ToListAsync();

            var listaPartidos = new List<PartidoDTO>();

            foreach (var partido in partidos)
            {
                var equipoGrupo = equiposGrupo.FirstOrDefault(eg => eg.IdEquipo == partido.IdEquipoLocal);
                var nombreGrupo = equipoGrupo != null
                    ? grupos.FirstOrDefault(g => g.IdGrupo == equipoGrupo.IdGrupo)?.Nombre
                    : "Sin Grupo";

                listaPartidos.Add(new PartidoDTO
                {
                    IdPartido = partido.IdPartido,
                    NombreEquipoLocal = partido.EquipoLocal.Nombre,
                    NombreEquipoVisitante = partido.EquipoVisitante.Nombre,
                    GolesLocal = partido.GolesLocal,
                    GolesVisitante = partido.GolesVisitante,
                    Fase = partido.Fase,
                    FechaHora = partido.FechaHora,
                    NombreGrupo = nombreGrupo
                });
            }

            return listaPartidos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartidoDetalleDTO>> GetPartido(int id)
        {
            var partido = await context.Partidos
                .Include(p => p.EquipoLocal)
                .Include(p => p.EquipoVisitante)
                .FirstOrDefaultAsync(p => p.IdPartido == id);

            if (partido == null)
            {
                return NotFound();
            }

            var goleadores = await context.Goles
                .Where(g => g.IdPartido == id)
                .Select(g => new GolDTO
                {
                    NombreJugador = context.Jugadores.FirstOrDefault(j => j.IdJugador == g.IdJugador).Nombre,
                    Minuto = g.Minuto ?? 0
                })
                .ToListAsync();

            var tarjetas = await context.Tarjetas
                .Where(t => t.IdPartido == id)
                .Select(t => new TarjetaPartidoDTO
                {
                    NombreJugador = t.IdJugador.HasValue
                        ? context.Jugadores.FirstOrDefault(j => j.IdJugador == t.IdJugador.Value).Nombre
                        : "Sin jugador asignado",
                    TipoTarjeta = t.TipoTarjeta
                })
                .ToListAsync();

            var partidoDetalle = new PartidoDetalleDTO
            {
                IdPartido = partido.IdPartido,
                NombreEquipoLocal = partido.EquipoLocal.Nombre,
                NombreEquipoVisitante = partido.EquipoVisitante.Nombre,
                GolesLocal = partido.GolesLocal,
                GolesVisitante = partido.GolesVisitante,
                FechaHora = partido.FechaHora,
                Goleadores = goleadores,
                Tarjetas = tarjetas
            };

            return partidoDetalle;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Partido>> CreatePartido(Partido partido)
        {
            context.Partidos.Add(partido);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePartido(int id)
        {
            var partido = await context.Partidos.FindAsync(id);
            context.Partidos.Remove(partido);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id}/resultado")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActualizarResultado(int id, [FromBody] ActualizarResultadoDTO dto)
        {
            var partido = await context.Partidos.FindAsync(id);

            partido.GolesLocal = dto.GolesLocal;
            partido.GolesVisitante = dto.GolesVisitante;

            foreach (var idJugador in dto.IdsGoleadoresLocal)
            {
                if (idJugador != 0)
                {
                    var gol = new Gol
                    {
                        IdPartido = partido.IdPartido,
                        IdJugador = idJugador,
                        Minuto = null
                    };
                    context.Goles.Add(gol);

                    var jugador = await context.Jugadores.FindAsync(idJugador);
                    if (jugador != null)
                    {
                        jugador.Goles += 1;
                    }
                }
            }

            foreach (var idJugador in dto.IdsGoleadoresVisitante)
            {
                if (idJugador != 0)
                {
                    var gol = new Gol
                    {
                        IdPartido = partido.IdPartido,
                        IdJugador = idJugador,
                        Minuto = null
                    };
                    context.Goles.Add(gol);

                    var jugador = await context.Jugadores.FindAsync(idJugador);
                    if (jugador != null)
                    {
                        jugador.Goles += 1;
                    }
                }
            }

            foreach (var tarjeta in dto.Tarjetas)
            {
                if (!(tarjeta == null || tarjeta.IdJugador == 0 || string.IsNullOrEmpty(tarjeta.TipoTarjeta)))
                {
                    var nuevaTarjeta = new Tarjeta
                    {
                        IdPartido = partido.IdPartido,
                        IdJugador = tarjeta.IdJugador,
                        TipoTarjeta = tarjeta.TipoTarjeta,
                        IdEquipo = tarjeta.IdEquipo
                    };
                    context.Tarjetas.Add(nuevaTarjeta);
                }
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
