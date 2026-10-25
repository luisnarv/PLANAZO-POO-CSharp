namespace PLANAZO_POO_CSharp.Modelos
{
    public enum EstadoReserva
    {
        Activa,
        Cancelada
    }

    public class Reserva
    {
        public int Id { get; }
        public Usuario Cliente { get; }
        public Evento EventoReservado { get; }
        public int CantidadEntradas { get; }
        public decimal ValorTotal { get; }
        public EstadoReserva Estado { get; private set; }
        public DateTime FechaRegistro { get; }

        public string NombrePersona => Cliente.NombreCompleto;

        public Reserva(int id, Usuario cliente, Evento eventoReservado, int cantidadEntradas)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "El cliente no puede ser nulo.");
            }

            if (eventoReservado == null)
            {
                throw new ArgumentNullException(nameof(eventoReservado), "El evento no puede ser nulo.");
            }

            if (cantidadEntradas <= 0)
            {
                throw new ArgumentException("La cantidad de entradas debe ser un numero entero positivo.");
            }

            Id = id;
            Cliente = cliente;
            EventoReservado = eventoReservado;
            CantidadEntradas = cantidadEntradas;
            ValorTotal = eventoReservado.CalcularPrecioFinal() * cantidadEntradas;
            Estado = EstadoReserva.Activa;
            FechaRegistro = DateTime.Now;

            eventoReservado.DescontarCupos(cantidadEntradas);
        }

        public void Cancelar()
        {
            if (Estado == EstadoReserva.Cancelada)
            {
                throw new InvalidOperationException($"La reserva con ID {Id} ya se encuentra cancelada.");
            }

            Estado = EstadoReserva.Cancelada;
            EventoReservado.DevolverCupos(CantidadEntradas);
        }
    }
}
