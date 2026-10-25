using System;
using System.Collections.Generic;
using PLANAZO_POO_CSharp.Modelos;
using PLANAZO_POO_CSharp.Servicios;

namespace PLANAZO_POO_CSharp;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        GestorReservas gestor = new GestorReservas();
        bool continuar = true;

        while (continuar)
        {
            Console.WriteLine();
            Console.WriteLine("SISTEMA PLANAZO · GESTION DE EVENTOS Y RESERVAS");
            Console.WriteLine("1. Ver eventos disponibles");
            Console.WriteLine("2. Realizar una reserva");
            Console.WriteLine("3. Cancelar una reserva");
            Console.WriteLine("4. Listar todas las reservas");
            Console.WriteLine("5. Consultar reservas por identificacion de usuario");
            Console.WriteLine("6. Ver resumen financiero y operativo");
            Console.WriteLine("0. Salir");
            Console.Write("Seleccione una opcion: ");

            string? entradaOpcion = Console.ReadLine();
            Console.WriteLine();

            switch (entradaOpcion)
            {
                case "1":
                    MostrarEventos(gestor);
                    break;
                case "2":
                    RealizarReserva(gestor);
                    break;
                case "3":
                    CancelarReserva(gestor);
                    break;
                case "4":
                    ListarReservas(gestor);
                    break;
                case "5":
                    ConsultarPorUsuario(gestor);
                    break;
                case "6":
                    MostrarResumen(gestor);
                    break;
                case "0":
                    continuar = false;
                    Console.WriteLine("Gracias por utilizar el sistema Planazo. Hasta pronto.");
                    break;
                default:
                    Console.WriteLine("Opcion invalida. Por favor seleccione un numero entre 0 y 6.");
                    break;
            }
        }
    }

    private static void MostrarEventos(GestorReservas gestor)
    {
        Console.WriteLine("CATALOGO DE EVENTOS REGISTRADOS");
        var eventos = gestor.ObtenerEventosDisponibles();

        if (eventos.Count == 0)
        {
            Console.WriteLine("No hay eventos disponibles en este momento.");
            return;
        }

        foreach (var ev in eventos)
        {
            Console.WriteLine($"ID: {ev.Id} | {ev.Nombre}");
            Console.WriteLine($"   Tipo: {ev.ObtenerTipo()}");
            Console.WriteLine($"   Fecha: {ev.Fecha:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"   Precio Base: ${ev.PrecioBase:N0} COP | Precio Final: ${ev.CalcularPrecioFinal():N0} COP");
            Console.WriteLine($"   Cupos Disponibles: {ev.CuposDisponibles} / {ev.CapacidadMaxima}");
            Console.WriteLine();
        }
    }

    private static void RealizarReserva(GestorReservas gestor)
    {
        Console.WriteLine("PROCESO DE NUEVA RESERVA");
        try
        {
            Console.Write("Ingrese la identificacion o cedula del cliente: ");
            string? idCliente = Console.ReadLine();

            Console.Write("Ingrese el nombre completo del cliente: ");
            string? nombreCliente = Console.ReadLine();

            Console.Write("Ingrese el correo electronico del cliente: ");
            string? correoCliente = Console.ReadLine();

            Usuario usuario = new Usuario(idCliente ?? string.Empty, nombreCliente ?? string.Empty, correoCliente ?? string.Empty);

            MostrarEventos(gestor);

            Console.Write("Ingrese el ID del evento que desea reservar: ");
            if (!int.TryParse(Console.ReadLine(), out int idEvento))
            {
                Console.WriteLine("Error: El ID del evento debe ser un numero entero valido.");
                return;
            }

            Console.Write("Ingrese la cantidad de entradas a reservar: ");
            if (!int.TryParse(Console.ReadLine(), out int cantidadEntradas))
            {
                Console.WriteLine("Error: La cantidad de entradas debe ser un numero entero.");
                return;
            }

            Reserva reserva = gestor.RegistrarReserva(usuario, idEvento, cantidadEntradas);

            Console.WriteLine();
            Console.WriteLine("RESERVA EXITOSA");
            Console.WriteLine($"Numero de Reserva: #{reserva.Id}");
            Console.WriteLine($"Cliente: {reserva.Cliente.NombreCompleto} ({reserva.Cliente.Identificacion})");
            Console.WriteLine($"Evento: {reserva.EventoReservado.Nombre} ({reserva.EventoReservado.ObtenerTipo()})");
            Console.WriteLine($"Cantidad de entradas: {reserva.CantidadEntradas}");
            Console.WriteLine($"Precio por entrada: ${reserva.EventoReservado.CalcularPrecioFinal():N0} COP");
            Console.WriteLine($"Total a pagar: ${reserva.ValorTotal:N0} COP");
            Console.WriteLine($"Estado: {reserva.Estado}");
            Console.WriteLine($"Cupos restantes en el evento: {reserva.EventoReservado.CuposDisponibles}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Dato invalido: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Operacion rechazada: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrio un error inesperado: {ex.Message}");
        }
    }

    private static void CancelarReserva(GestorReservas gestor)
    {
        Console.WriteLine("CANCELACION DE RESERVA");
        Console.Write("Ingrese el numero de identificacion de la reserva a cancelar: ");

        if (!int.TryParse(Console.ReadLine(), out int idReserva))
        {
            Console.WriteLine("Error: El ID de la reserva debe ser un numero entero.");
            return;
        }

        try
        {
            Reserva? reserva = gestor.ObtenerReservaPorId(idReserva);
            if (reserva == null)
            {
                Console.WriteLine($"No se encontro ninguna reserva con el identificador #{idReserva}.");
                return;
            }

            gestor.CancelarReserva(idReserva);
            Console.WriteLine();
            Console.WriteLine($"La reserva #{idReserva} ha sido cancelada satisfactoriamente.");
            Console.WriteLine($"Se han devuelto {reserva.CantidadEntradas} cupo(s) al evento '{reserva.EventoReservado.Nombre}'.");
            Console.WriteLine($"Cupos disponibles actuales del evento: {reserva.EventoReservado.CuposDisponibles}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"No fue posible cancelar la reserva: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrio un error inesperado: {ex.Message}");
        }
    }

    private static void ListarReservas(GestorReservas gestor)
    {
        Console.WriteLine("HISTORIAL DE RESERVAS REGISTRADAS");
        var reservas = gestor.ObtenerReservas();

        if (reservas.Count == 0)
        {
            Console.WriteLine("No se han registrado reservas hasta el momento.");
            return;
        }

        foreach (var r in reservas)
        {
            Console.WriteLine($"Reserva #{r.Id} | Estado: {r.Estado} | Fecha: {r.FechaRegistro:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"   Cliente: {r.Cliente.NombreCompleto} - Cedula: {r.Cliente.Identificacion} - Correo: {r.Cliente.Correo}");
            Console.WriteLine($"   Evento: {r.EventoReservado.Nombre} ({r.EventoReservado.ObtenerTipo()})");
            Console.WriteLine($"   Entradas: {r.CantidadEntradas} | Total: ${r.ValorTotal:N0} COP");
            Console.WriteLine();
        }
    }

    private static void ConsultarPorUsuario(GestorReservas gestor)
    {
        Console.WriteLine("CONSULTA DE RESERVAS POR CLIENTE");
        Console.Write("Ingrese la cedula o identificacion a consultar: ");
        string? cedula = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(cedula))
        {
            Console.WriteLine("Debe ingresar un documento valido.");
            return;
        }

        var reservas = gestor.ObtenerReservasPorUsuario(cedula);

        if (reservas.Count == 0)
        {
            Console.WriteLine($"No se encontraron reservas asociadas a la identificacion '{cedula}'.");
            return;
        }

        Console.WriteLine($"Se encontraron {reservas.Count} reserva(s) para el documento '{cedula}':");
        Console.WriteLine();

        foreach (var r in reservas)
        {
            Console.WriteLine($"Reserva #{r.Id} | Estado: {r.Estado}");
            Console.WriteLine($"   Evento: {r.EventoReservado.Nombre} ({r.EventoReservado.ObtenerTipo()})");
            Console.WriteLine($"   Entradas: {r.CantidadEntradas} | Total: ${r.ValorTotal:N0} COP");
            Console.WriteLine($"   Registrada el: {r.FechaRegistro:yyyy-MM-dd HH:mm}");
            Console.WriteLine();
        }
    }

    private static void MostrarResumen(GestorReservas gestor)
    {
        Console.WriteLine("RESUMEN OPERATIVO Y FINANCIERO");
        var resumen = gestor.ObtenerResumen();

        Console.WriteLine($"Total de reservas generadas: {resumen.totalReservas}");
        Console.WriteLine($"Reservas en estado Activa: {resumen.activas}");
        Console.WriteLine($"Reservas en estado Cancelada: {resumen.canceladas}");
        Console.WriteLine($"Recaudo total efectivo (Reservas activas): ${resumen.totalRecaudado:N0} COP");
    }
}
