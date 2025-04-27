using ApiMaratonRicardoNogales.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NugetMaraton;

namespace ApiMaratonRicardoNogales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JugadoresController : ControllerBase
    {
        private MaratonContext context;

        public JugadoresController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Jugador>>> GetJugadores()
        {
            return await context.Jugadores.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Jugador>> GetJugador(int id)
        {
            var jugador = await context.Jugadores.FindAsync(id);
            return jugador;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Jugador>> CreateJugador(Jugador jugador)
        {
            context.Jugadores.Add(jugador);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteJugador(int id)
        {
            var jugador = await context.Jugadores.FindAsync(id);
            context.Jugadores.Remove(jugador);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
