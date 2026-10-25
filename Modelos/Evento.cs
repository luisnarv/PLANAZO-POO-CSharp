namespace PLANAZO_POO_CSharp.Modelos
{
    public abstract class Evento
    {
        public int Id { get; }
        public string Nombre { get; }
        public DateTime Fecha { get; }
        public decimal PrecioBase { get; }
        public int CapacidadMaxima { get; }
        public int CuposDisponibles { get; private set; }

        protected Evento(int id, string nombre, DateTime fecha, decimal precioBase, int capacidadMaxima)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre del evento no puede estar vacio.");
            }

            if (precioBase < 0)
            {
                throw new ArgumentException("El precio base no puede ser negativo.");
            }

            if (capacidadMaxima <= 0)
            {
                throw new ArgumentException("La capacidad maxima debe ser mayor a cero.");
            }

            Id = id;
            Nombre = nombre;
            Fecha = fecha;
            PrecioBase = precioBase;
            CapacidadMaxima = capacidadMaxima;
            CuposDisponibles = capacidadMaxima;
        }

        public abstract decimal CalcularPrecioFinal();

        public abstract string ObtenerTipo();

        public bool HayCupo(int cantidad)
        {
            return cantidad > 0 && CuposDisponibles >= cantidad;
        }

        public void DescontarCupos(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a descontar debe ser mayor a cero.");
            }

            if (cantidad > CuposDisponibles)
            {
                throw new InvalidOperationException($"No hay suficientes cupos disponibles. Cupos actuales: {CuposDisponibles}.");
            }

            CuposDisponibles -= cantidad;
        }

        public void DevolverCupos(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ArgumentException("La cantidad a devolver debe ser mayor a cero.");
            }

            if (CuposDisponibles + cantidad > CapacidadMaxima)
            {
                CuposDisponibles = CapacidadMaxima;
            }
            else
            {
                CuposDisponibles += cantidad;
            }
        }
    }
}
