namespace GestionDeInventario.Services.Exceptions
{
    // Usamos BusinessRuleException porque es una violación de validación
    public class BusinessRuleException : Exception
    {
        // Constructor para el mensaje que explica la regla violada (ej: "El stock no puede ser negativo")
        public BusinessRuleException(string message)
            : base(message)
        {
        }

        // Opcional: Para encadenar la excepción interna (útil para depuración)
        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
