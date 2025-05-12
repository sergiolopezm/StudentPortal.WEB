namespace StudentPortal.WEB.Models.Auth
{
    public class TokenDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expiracion { get; set; }
        public UsuarioPerfilDto Usuario { get; set; } = null!;
    }
}
