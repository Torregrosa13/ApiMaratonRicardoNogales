namespace ApiMaratonRicardoNogales.DTOs
{
    public class EquipoDetalleDTO
    {
        public int IdEquipo { get; set; }
        public string Nombre { get; set; }
        public string Escudo { get; set; }
        public List<JugadorDTO> Jugadores { get; set; }
    }
}
