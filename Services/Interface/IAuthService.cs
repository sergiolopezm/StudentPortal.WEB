using StudentPortal.WEB.Models.Auth;

namespace StudentPortal.WEB.Services.Interface
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, TokenDto? Data)> Login(UsuarioLoginDto loginDto);
        Task<(bool Success, string Message, object? Data)> Register(UsuarioRegistroDto registroDto);
        Task<(bool Success, string Message, object? Data)> RegisterComplete(UsuarioRegistroCompletoDto registroDto);
        Task<(bool Success, string Message, UsuarioPerfilDto? Data)> GetProfile();
        Task<(bool Success, string Message)> Logout();
        Task<bool> IsAuthenticated();
        UsuarioPerfilDto? GetCurrentUser();
    }
}
