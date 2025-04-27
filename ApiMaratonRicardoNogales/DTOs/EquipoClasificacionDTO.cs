namespace ApiMaratonRicardoNogales.DTOs
{
    public class EquipoClasificacionDTO
    {
        public int IdEquipo { get; set; }
        public string NombreEquipo { get; set; }
        public int Puntos { get; set; }
        public int GolesFavor { get; set; }
        public int GolesContra { get; set; }
        public int DiferenciaGoles => GolesFavor - GolesContra;
        public int Tarjetas { get; set; } 
    }
}
