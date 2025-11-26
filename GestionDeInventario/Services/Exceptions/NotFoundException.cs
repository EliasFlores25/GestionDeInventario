namespace GestionDeInventario.Services.Exceptions
{
    public class NotFoundException : DomainException
    {
        // Constructor que acepta el mensaje específico (ej: "El Producto con ID 5 no existe")
        public NotFoundException(string message) : base(message) {   }
    }
}
