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
    public class EquiposController : ControllerBase
    {
        private MaratonContext context;

        public EquiposController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Equipo>>> GetEquipos()
        {
            return await context.Equipos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDetalleDTO>> GetEquipo(int id)
        {
            var equipo = await context.Equipos
                .Where(e => e.IdEquipo == id)
                .Select(e => new EquipoDetalleDTO
                {
                    IdEquipo = e.IdEquipo,
                    Nombre = e.Nombre,
                    Escudo = e.Escudo,
                    Jugadores = context.Jugadores
                                .Where(j => j.IdEquipo == e.IdEquipo)
                                .Select(j => new JugadorDTO
                                {
                                    IdJugador = j.IdJugador,
                                    Nombre = j.Nombre,
                                    Apellidos = j.Apellidos,
                                    Dorsal = j.Dorsal,
                                    Goles = j.Goles
                                }).ToList()
                })
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return NotFound();
            }

            return equipo;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Equipo>> CreateEquipo(Equipo equipo)
        {
            context.Equipos.Add(equipo);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await context.Equipos.FindAsync(id);
            context.Equipos.Remove(equipo);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
