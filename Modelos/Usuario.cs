namespace PLANAZO_POO_CSharp.Modelos
{
    public class Usuario
    {
        public string Identificacion { get; }
        public string NombreCompleto { get; }
        public string Correo { get; }

        public Usuario(string identificacion, string nombreCompleto, string correo)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
            {
                throw new ArgumentException("La identificacion del usuario es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                throw new ArgumentException("El nombre del usuario es obligatorio.");
            }

            Identificacion = identificacion.Trim();
            NombreCompleto = nombreCompleto.Trim();
            Correo = correo?.Trim() ?? string.Empty;
        }

        public override string ToString()
        {
            return $"{NombreCompleto} (CC: {Identificacion})";
        }
    }
}
