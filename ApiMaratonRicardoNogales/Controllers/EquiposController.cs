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
        public async Task<ActionResult<Equipo>> GetEquipo(int id)
        {
            var equipo = await context.Equipos.FindAsync(id);
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
