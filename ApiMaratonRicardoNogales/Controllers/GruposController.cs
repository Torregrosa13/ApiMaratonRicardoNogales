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
    public class GruposController : ControllerBase
    {
        private MaratonContext context;

        public GruposController(MaratonContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Grupo>>> GetGrupos()
        {
            return await context.Grupos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Grupo>> GetGrupo(int id)
        {
            var grupo = await context.Grupos.FindAsync(id);
            return grupo;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Grupo>> CreateGrupo(Grupo grupo)
        {
            context.Grupos.Add(grupo);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGrupo(int id)
        {
            var grupo = await context.Grupos.FindAsync(id);
            context.Grupos.Remove(grupo);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("clasificacion/{idGrupo}")]
        public async Task<ActionResult<List<EquipoClasificacionDTO>>> GetClasificacionGrupo(int idGrupo)
        {
            var equiposIds = await context.EquiposGrupo
                .Where(eg => eg.IdGrupo == idGrupo)
                .Select(eg => eg.IdEquipo)
                .ToListAsync();
    
            var equiposEnGrupo = await context.Equipos
                .Where(e => equiposIds.Contains(e.IdEquipo))
                .ToListAsync();


            var clasificacion = new List<EquipoClasificacionDTO>();

            foreach (var equipo in equiposEnGrupo)
            {
                var partidos = await context.Partidos
                    .Where(p => p.Fase == "Triangular" &&
                               (p.IdEquipoLocal == equipo.IdEquipo || p.IdEquipoVisitante == equipo.IdEquipo))
                    .ToListAsync();

                int puntos = 0;
                int golesFavor = 0;
                int golesContra = 0;

                foreach (var partido in partidos)
                {
                    if (partido.IdEquipoLocal == equipo.IdEquipo)
                    {
                        golesFavor += partido.GolesLocal ?? 0;
                        golesContra += partido.GolesVisitante ?? 0;

                        if (partido.GolesLocal > partido.GolesVisitante) puntos += 3;
                        else if (partido.GolesLocal == partido.GolesVisitante) puntos += 1;
                    }
                    else if (partido.IdEquipoVisitante == equipo.IdEquipo)
                    {
                        golesFavor += partido.GolesVisitante ?? 0;
                        golesContra += partido.GolesLocal ?? 0;

                        if (partido.GolesVisitante > partido.GolesLocal) puntos += 3;
                        else if (partido.GolesVisitante == partido.GolesLocal) puntos += 1;
                    }
                }

                var tarjetas = await context.Tarjetas
                    .Where(t => t.IdEquipo == equipo.IdEquipo)
                    .ToListAsync();

                int puntosTarjetas = 0;
                foreach (var tarjeta in tarjetas)
                {
                    if (tarjeta.TipoTarjeta == "Amarilla")
                        puntosTarjetas += 1;
                    else if (tarjeta.TipoTarjeta == "Roja")
                        puntosTarjetas += 2;
                }

                clasificacion.Add(new EquipoClasificacionDTO
                {
                    IdEquipo = equipo.IdEquipo,
                    NombreEquipo = equipo.Nombre,
                    Puntos = puntos,
                    GolesFavor = golesFavor,
                    GolesContra = golesContra,
                    Tarjetas = puntosTarjetas
                });
            }

            clasificacion = clasificacion
                .OrderByDescending(c => c.Puntos)
                .ThenByDescending(c => c.DiferenciaGoles)
                .ThenBy(c => c.Tarjetas)
                .ToList();

            return clasificacion;
        }
    }
}
