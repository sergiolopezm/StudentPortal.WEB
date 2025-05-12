using StudentPortal.WEB.Helpers;
using StudentPortal.WEB.Models.Academico;
using StudentPortal.WEB.Models;
using StudentPortal.WEB.Services.Interface;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentPortal.WEB.Services.Api
{
    public class ApiProgramaService : IProgramaService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStorageService _localStorage;

        public ApiProgramaService(HttpClient httpClient, LocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<(bool Success, string Message, List<ProgramaDto>? Data)> GetAll()
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.GetAsync("api/Programa");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Programas obtenidos";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<List<ProgramaDto>>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo programas";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, ProgramaDto? Data)> GetById(int id)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"api/Programa/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Programa obtenido";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<ProgramaDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "Programa no encontrado", null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo programa";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, PaginacionDto<ProgramaDto>? Data)> GetPaginated(int page, int itemsPerPage, string? search = null)
        {
            try
            {
                await SetAuthorizationHeader();

                var url = $"api/Programa/paginado?pagina={page}&elementosPorPagina={itemsPerPage}";
                if (!string.IsNullOrEmpty(search))
                {
                    url += $"&busqueda={Uri.EscapeDataString(search)}";
                }

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Programas obtenidos";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<PaginacionDto<ProgramaDto>>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo programas";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        private async Task SetAuthorizationHeader()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
