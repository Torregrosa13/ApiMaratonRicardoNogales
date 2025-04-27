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
        public async Task<ActionResult<List<Partido>>> GetPartidos()
        {
            return await context.Partidos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Partido>> GetPartido(int id)
        {
            var partido = await context.Partidos.FindAsync(id);
            return partido;
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
                if (tarjeta != null && tarjeta.IdJugador != 0 && !string.IsNullOrEmpty(tarjeta.TipoTarjeta))
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
