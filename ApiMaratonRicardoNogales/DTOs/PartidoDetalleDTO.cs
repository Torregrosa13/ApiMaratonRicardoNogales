namespace ApiMaratonRicardoNogales.DTOs
{
    public class PartidoDetalleDTO
    {
        public int IdPartido { get; set; }
        public string NombreEquipoLocal { get; set; }
        public string NombreEquipoVisitante { get; set; }
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public DateTime FechaHora { get; set; }
        public List<GolDTO> Goleadores { get; set; }
        public List<TarjetaPartidoDTO> Tarjetas { get; set; }
    }

    public class GolDTO
    {
        public string NombreJugador { get; set; }
        public int Minuto { get; set; }
    }

    public class TarjetaPartidoDTO
    {
        public string NombreJugador { get; set; }
        public string TipoTarjeta { get; set; }
    }
}
