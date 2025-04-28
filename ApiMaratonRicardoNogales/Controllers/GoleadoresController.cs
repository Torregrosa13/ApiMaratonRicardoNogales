using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiMaratonRicardoNogales.Data;
using ApiMaratonRicardoNogales.DTOs;

namespace ApiMaratonRicardoNogales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoleadoresController : ControllerBase
    {
        private readonly MaratonContext _context;

        public GoleadoresController(MaratonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<GoleadorDTO>>> GetGoleadores()
        {
            var jugadores = await _context.Jugadores.ToListAsync();
            var equipos = await _context.Equipos.ToListAsync();

            var goleadores = await _context.Goles
                .GroupBy(g => g.IdJugador)
                .Select(g => new
                {
                    IdJugador = g.Key,
                    Goles = g.Count()
                })
                .OrderByDescending(g => g.Goles)
                .ToListAsync();

            var listaGoleadores = goleadores.Select(g => new GoleadorDTO
            {
                IdJugador = g.IdJugador,
                NombreJugador = jugadores.FirstOrDefault(j => j.IdJugador == g.IdJugador)?.Nombre + " " +
                                 jugadores.FirstOrDefault(j => j.IdJugador == g.IdJugador)?.Apellidos,
                Dorsal = jugadores.FirstOrDefault(j => j.IdJugador == g.IdJugador)?.Dorsal ?? 0,
                Goles = g.Goles,
                NombreEquipo = equipos.FirstOrDefault(e => e.IdEquipo ==
                                 jugadores.FirstOrDefault(j => j.IdJugador == g.IdJugador)?.IdEquipo)?.Nombre
            }).ToList();

            return listaGoleadores;
        }
    }
}
