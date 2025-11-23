namespace GestionDeInventario.Models
{
    public class Usuario
    {
        public int idUsuario { get; set; }
        public string nombre { get; set; }
        public int tipoRol { get; set; }
        public string email { get; set; }
        public string passwordHash { get; set; }
       
    }
}
