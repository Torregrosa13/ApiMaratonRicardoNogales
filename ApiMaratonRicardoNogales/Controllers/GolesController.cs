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
    public class GolesController : ControllerBase
    {
        private MaratonContext context;

        public GolesController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Gol>>> GetGoles()
        {
            return await context.Goles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Gol>> GetGol(int id)
        {
            var gol = await context.Goles.FindAsync(id);
            return gol;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Gol>> CreateGol(Gol gol)
        {
            context.Goles.Add(gol);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGol(int id)
        {
            var gol = await context.Goles.FindAsync(id);
            context.Goles.Remove(gol);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("goleadores")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GoleadorDTO>>> GetGoleadores()
        {
            var goleadores = await context.Goles
                .GroupBy(g => g.IdJugador)
                .Select(group => new
                {
                    IdJugador = group.Key,
                    Goles = group.Count()
                })
                .OrderByDescending(x => x.Goles)
                .ToListAsync();

            var lista = new List<GoleadorDTO>();

            foreach (var goleador in goleadores)
            {
                var jugador = await context.Jugadores
                    .FirstOrDefaultAsync(j => j.IdJugador == goleador.IdJugador);

                var equipo = await context.Equipos
                    .FirstOrDefaultAsync(e => e.IdEquipo == jugador.IdEquipo);

                if (jugador != null)
                {
                    lista.Add(new GoleadorDTO
                    {
                        IdJugador = jugador.IdJugador,
                        NombreJugador = jugador.Nombre + " " + jugador.Apellidos,
                        Dorsal = jugador.Dorsal,
                        Goles = goleador.Goles,
                        NombreEquipo = equipo?.Nombre
                    });
                }
            }

            return lista;
        }
    }
}
