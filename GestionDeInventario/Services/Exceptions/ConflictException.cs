namespace GestionDeInventario.Services.Exceptions
{
    public class ConflictException : Exception
    {
        // Se usa cuando se intenta crear algo que ya existe (ej: Producto con nombre duplicado)
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
