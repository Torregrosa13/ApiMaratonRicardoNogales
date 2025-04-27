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
    public class TarjetasController : ControllerBase
    {
        private MaratonContext context;

        public TarjetasController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tarjeta>>> GetTarjetas()
        {
            return await context.Tarjetas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarjeta>> GetTarjeta(int id)
        {
            var tarjeta = await context.Tarjetas.FindAsync(id);
            return tarjeta;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Tarjeta>> CreateTarjeta(Tarjeta tarjeta)
        {
            context.Tarjetas.Add(tarjeta);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTarjeta(int id)
        {
            var tarjeta = await context.Tarjetas.FindAsync(id);
            context.Tarjetas.Remove(tarjeta);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
