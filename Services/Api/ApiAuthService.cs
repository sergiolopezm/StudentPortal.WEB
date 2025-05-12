using Microsoft.JSInterop;
using StudentPortal.WEB.Helpers;
using StudentPortal.WEB.Models.Auth;
using StudentPortal.WEB.Services.Interface;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentPortal.WEB.Services.Api
{
    public class ApiAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;
        private readonly JwtAuthenticationStateProvider _authStateProvider;
        private UsuarioPerfilDto? _currentUser;

        public ApiAuthService(
            HttpClient httpClient,
            LocalStorageService localStorage,
            IJSRuntime jsRuntime,
            JwtAuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        public async Task<(bool Success, string Message, TokenDto? Data)> Login(UsuarioLoginDto loginDto)
        {
            try
            {
                // Obtener IP del cliente (simulada para ejemplo)
                loginDto.Ip = await _jsRuntime.InvokeAsync<string>("clientInfo.getIpAddress");

                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Login exitoso";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<TokenDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (resultado != null)
                        {
                            await _localStorage.SetItemAsync("authToken", resultado.Token);
                            await _localStorage.SetItemAsync("usuarioId", resultado.Usuario.Id.ToString());
                            _currentUser = resultado.Usuario;

                            // Notificar al framework Blazor sobre el cambio en el estado de autenticación
                            _authStateProvider.NotifyUserAuthentication(resultado.Token);

                            return (true, mensaje, resultado);
                        }
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error en la autenticación";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "Revise sus credenciales";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, object? Data)> Register(UsuarioRegistroDto registroDto)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("api/Auth/registro", registroDto);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Registro exitoso";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        return (true, mensaje, resultadoJson);
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error en el registro";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "Revise los datos ingresados";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, UsuarioPerfilDto? Data)> GetProfile()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    return (false, "No ha iniciado sesión", null);
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/Auth/perfil");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Perfil obtenido";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<UsuarioPerfilDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        _currentUser = resultado;
                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo perfil";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> Logout()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    return (false, "No ha iniciado sesión");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsync("api/Auth/logout", null);

                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("usuarioId");
                _currentUser = null;

                // Notificar al framework Blazor sobre el cierre de sesión
                _authStateProvider.NotifyUserLogout();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Sesión cerrada exitosamente";
                    return (true, mensaje);
                }
                else
                {
                    return (true, "Sesión cerrada localmente");
                }
            }
            catch (Exception)
            {
                // Si hay error al comunicarse con el servidor, cerramos la sesión localmente
                await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.RemoveItemAsync("usuarioId");
                _currentUser = null;

                // Notificar al framework Blazor sobre el cierre de sesión
                _authStateProvider.NotifyUserLogout();

                return (true, "Sesión cerrada localmente");
            }
        }

        public async Task<bool> IsAuthenticated()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            return !string.IsNullOrEmpty(token);
        }

        public UsuarioPerfilDto? GetCurrentUser()
        {
            return _currentUser;
        }

        public async Task<(bool Success, string Message, object? Data)> RegisterComplete(UsuarioRegistroCompletoDto registroDto)
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.PostAsJsonAsync("api/Auth/registroCompleto", registroDto);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Registro exitoso";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        return (true, mensaje, resultadoJson);
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error en el registro";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "Revise los datos ingresados";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }
    }
}