namespace ApiMaratonRicardoNogales.DTOs
{
    public class PartidoDTO
    {
        public int IdPartido { get; set; }
        public string NombreEquipoLocal { get; set; }
        public string NombreEquipoVisitante { get; set; }
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public string Fase { get; set; }
        public DateTime FechaHora { get; set; }
        public string NombreGrupo { get; set; }
    }
}
