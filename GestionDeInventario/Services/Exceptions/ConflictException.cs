namespace GestionDeInventario.Services.Exceptions
{
    //ConflictException: Se usa típicamente para duplicados(ej.intentar registrar un usuario con un email que ya existe).
    public class ConflictException : Exception
    {
        // Se usa cuando se intenta crear algo que ya existe (ej: Producto con nombre duplicado)
        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
