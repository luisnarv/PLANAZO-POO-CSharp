namespace PLANAZO_POO_CSharp.Modelos
{
    public class Taller : Evento
    {
        public Taller(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
            : base(id, nombre, fecha, precioBase, capacidadMaxima)
        {
        }

        public override decimal CalcularPrecioFinal()
        {
            return PrecioBase * 0.90m;
        }

        public override string ObtenerTipo()
        {
            return "Taller";
        }
    }
}
