﻿namespace ApiMaratonRicardoNogales.DTOs
{
    public class ActualizarResultadoDTO
    {
        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }
        public List<int> IdsGoleadoresLocal { get; set; } = new();
        public List<int> IdsGoleadoresVisitante { get; set; } = new();
        public List<TarjetasDTO> Tarjetas { get; set; } = new();
    }

    public class TarjetasDTO
    {
        public int IdJugador { get; set; }
        public string TipoTarjeta { get; set; }  
        public int IdEquipo { get; set; }
    }
}
