namespace PLANAZO_POO_CSharp.Servicios;

using System;
using System.Collections.Generic;
using System.Linq;
using PLANAZO_POO_CSharp.Modelos;

public class GestorReservas
{
    private readonly List<Evento> _eventos;
    private readonly List<Reserva> _reservas;
    private readonly List<Usuario> _usuarios;
    private int _siguienteIdReserva = 1;

    public GestorReservas()
    {
        _eventos = new List<Evento>();
        _reservas = new List<Reserva>();
        _usuarios = new List<Usuario>();
        InicializarDatosPrueba();
    }

    private void InicializarDatosPrueba()
    {
        _eventos.Add(new Cine(1, "Estreno Interestelar IMAX", DateTime.Now.AddDays(2), 25000m, 50));
        _eventos.Add(new Cine(2, "Ciclo Clasicos: El Padrino", DateTime.Now.AddDays(4), 18000m, 35));
        _eventos.Add(new Concierto(3, "Sinfonica Nacional en Vivo", DateTime.Now.AddDays(10), 85000m, 120));
        _eventos.Add(new Concierto(4, "Festival Rock de la Montana", DateTime.Now.AddDays(15), 110000m, 200));
        _eventos.Add(new Taller(5, "Taller de Pintura al Oleo", DateTime.Now.AddDays(7), 45000m, 15));
        _eventos.Add(new Taller(6, "Masterclass de Fotografia Digital", DateTime.Now.AddDays(9), 50000m, 20));
        _eventos.Add(new EventoDeportivo(7, "Final Torneo Regional de Futbol", DateTime.Now.AddDays(12), 35000m, 80));
    }

    public IReadOnlyList<Evento> ObtenerEventosDisponibles()
    {
        return _eventos.AsReadOnly();
    }

    public Evento? ObtenerEventoPorId(int id)
    {
        return _eventos.FirstOrDefault(e => e.Id == id);
    }

    public Reserva RegistrarReserva(Usuario usuario, int eventoId, int cantidadEntradas)
    {
        if (usuario == null)
        {
            throw new ArgumentNullException(nameof(usuario), "El usuario no puede ser nulo.");
        }

        if (cantidadEntradas <= 0)
        {
            throw new ArgumentException("La cantidad de entradas debe ser un numero entero positivo mayor a cero.");
        }

        Evento evento = ObtenerEventoPorId(eventoId) 
            ?? throw new InvalidOperationException($"No existe ningun evento registrado con el ID {eventoId}.");

        if (!evento.HayCupo(cantidadEntradas))
        {
            throw new InvalidOperationException($"No hay suficientes cupos disponibles para '{evento.Nombre}'. Cupos restantes: {evento.CuposDisponibles}, solicitados: {cantidadEntradas}.");
        }

        evento.DescontarCupos(cantidadEntradas);

        Reserva reserva = new Reserva(_siguienteIdReserva++, usuario, evento, cantidadEntradas);
        _reservas.Add(reserva);

        if (!_usuarios.Any(u => u.Identificacion.Equals(usuario.Identificacion, StringComparison.OrdinalIgnoreCase)))
        {
            _usuarios.Add(usuario);
        }

        return reserva;
    }

    public IReadOnlyList<Reserva> ObtenerReservas()
    {
        return _reservas.AsReadOnly();
    }

    public Reserva? ObtenerReservaPorId(int id)
    {
        return _reservas.FirstOrDefault(r => r.Id == id);
    }

    public bool CancelarReserva(int id)
    {
        Reserva? reserva = ObtenerReservaPorId(id);
        if (reserva == null)
        {
            return false;
        }

        reserva.Cancelar();
        return true;
    }

    public (int totalReservas, int activas, int canceladas, decimal totalRecaudado) ObtenerResumen()
    {
        int total = _reservas.Count;
        int activas = _reservas.Count(r => r.Estado == EstadoReserva.Activa);
        int canceladas = _reservas.Count(r => r.Estado == EstadoReserva.Cancelada);
        decimal totalRecaudado = _reservas.Where(r => r.Estado == EstadoReserva.Activa).Sum(r => r.ValorTotal);

        return (total, activas, canceladas, totalRecaudado);
    }

    public IReadOnlyList<Reserva> ObtenerReservasPorUsuario(string identificacion)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
        {
            return new List<Reserva>().AsReadOnly();
        }

        return _reservas
            .Where(r => r.Cliente.Identificacion.Equals(identificacion.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}
