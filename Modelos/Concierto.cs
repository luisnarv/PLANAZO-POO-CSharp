namespace PLANAZO_POO_CSharp.Modelos
{
    public class Concierto : Evento
    {
        public Concierto(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
        }

        public override decimal CalcularPrecioFinal()
        {
            return PrecioBase * 1.15m;
        }

        public override string ObtenerTipo()
        {
            return "Concierto";
        }
    }
}
