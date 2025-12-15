namespace GestionDeInventario.Services.Exceptions
{

    // Excepción personalizada para errores específicos de la lógica de negocio (dominio).
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message){ }

        public DomainException(string message, Exception innerException) : base(message, innerException){ }
    }
}
