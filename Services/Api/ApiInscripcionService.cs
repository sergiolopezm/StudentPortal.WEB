﻿using StudentPortal.WEB.Models.Academico;
using StudentPortal.WEB.Models;
using StudentPortal.WEB.Services.Interface;
using System.Net.Http.Json;
using System.Text.Json;
using StudentPortal.WEB.Helpers;

namespace StudentPortal.WEB.Services.Api
{
    public class ApiInscripcionService : IInscripcionService
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStorageService _localStorage;

        public ApiInscripcionService(HttpClient httpClient, LocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<(bool Success, string Message, List<InscripcionDto>? Data)> GetByEstudiante(int estudianteId)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"api/InscripcionEstudiante/estudiante/{estudianteId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripciones obtenidas";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<List<InscripcionDto>>(resultadoJson, new JsonSerializerOptions
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
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo inscripciones";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, List<InscripcionDto>? Data)> GetByMateria(int materiaId)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"api/InscripcionEstudiante/materia/{materiaId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripciones obtenidas";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<List<InscripcionDto>>(resultadoJson, new JsonSerializerOptions
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
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo inscripciones";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, InscripcionDto? Data)> GetById(int id)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.GetAsync($"api/InscripcionEstudiante/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripción obtenida";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<InscripcionDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "Inscripción no encontrada", null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo inscripción";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, PaginacionDto<InscripcionDto>? Data)> GetPaginated(int page, int itemsPerPage, int? estudianteId = null, int? materiaId = null)
        {
            try
            {
                await SetAuthorizationHeader();

                var url = $"api/InscripcionEstudiante/paginado?pagina={page}&elementosPorPagina={itemsPerPage}";
                if (estudianteId.HasValue)
                {
                    url += $"&estudianteId={estudianteId.Value}";
                }
                if (materiaId.HasValue)
                {
                    url += $"&materiaId={materiaId.Value}";
                }

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripciones obtenidas";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<PaginacionDto<InscripcionDto>>(resultadoJson, new JsonSerializerOptions
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
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error obteniendo inscripciones";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, InscripcionDto? Data)> Inscribir(InscripcionDto inscripcion)
        {
            try
            {
                await SetAuthorizationHeader();

                // Registrar en consola lo que estamos enviando para depuración
                Console.WriteLine($"Inscribiendo: EstudianteId={inscripcion.EstudianteId}, MateriaId={inscripcion.MateriaId}");

                // Asegurarse que la URL es correcta (sin sufijos extraños)
                const string endpoint = "api/InscripcionEstudiante";
                Console.WriteLine($"Enviando petición a: {endpoint}");
                
                var response = await _httpClient.PostAsJsonAsync(endpoint, inscripcion);

                // Siempre leer el contenido completo de la respuesta, independientemente del estado
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta del servidor: {response.StatusCode} - {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var exito = jsonResponse.TryGetProperty("exito", out var exitoProperty) ? exitoProperty.GetBoolean() : false;
                    var mensaje = jsonResponse.TryGetProperty("mensaje", out var mensajeProperty) ? 
                        mensajeProperty.GetString() : "Inscripción creada";

                    if (exito && jsonResponse.TryGetProperty("resultado", out var resultadoProperty))
                    {
                        var resultadoJson = resultadoProperty.ToString();
                        var resultado = JsonSerializer.Deserialize<InscripcionDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje ?? "Error desconocido", null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    try
                    {
                        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                        string mensaje = "Error al inscribir materia";
                        string detalle = "";

                        if (jsonResponse.TryGetProperty("mensaje", out var mensajeProperty))
                        {
                            mensaje = mensajeProperty.GetString() ?? mensaje;
                            if (jsonResponse.TryGetProperty("detalle", out var detalleProperty))
                            {
                                detalle = detalleProperty.GetString() ?? "";
                            }
                        }
                        // Verificar si es formato de validación de ASP.NET Core
                        else if (jsonResponse.TryGetProperty("title", out var titleProperty))
                        {
                            mensaje = titleProperty.GetString() ?? mensaje;
                            if (jsonResponse.TryGetProperty("errors", out var errorsProperty))
                            {
                                var errorsArray = new List<string>();
                                foreach (var error in errorsProperty.EnumerateObject())
                                {
                                    foreach (var item in error.Value.EnumerateArray())
                                    {
                                        errorsArray.Add($"{error.Name}: {item.GetString()}");
                                    }
                                }
                                detalle = string.Join(", ", errorsArray);
                            }
                        }

                        return (false, detalle.Length > 0 ? $"{mensaje}: {detalle}" : mensaje, null);
                    }
                    catch
                    {
                        return (false, $"Error de formato en la solicitud. Respuesta: {responseContent}", null);
                    }
                }
                else
                {
                    return (false, $"Error del servidor ({response.StatusCode}): {responseContent}", null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en Inscribir: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, InscripcionDto? Data)> Calificar(int id, decimal calificacion)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.PutAsync($"api/InscripcionEstudiante/{id}/calificar?calificacion={calificacion}", null);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripción calificada";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<InscripcionDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "Inscripción no encontrada", null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error calificando inscripción";
                    var detalle = jsonResponse.GetProperty("detalle").GetString() ?? "";
                    return (false, $"{mensaje}: {detalle}", null);
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message, InscripcionDto? Data)> Cancelar(int id)
        {
            try
            {
                await SetAuthorizationHeader();

                var response = await _httpClient.PutAsync($"api/InscripcionEstudiante/{id}/cancelar", null);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var exito = jsonResponse.GetProperty("exito").GetBoolean();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Inscripción cancelada";

                    if (exito)
                    {
                        var resultadoJson = jsonResponse.GetProperty("resultado").ToString();
                        var resultado = JsonSerializer.Deserialize<InscripcionDto>(resultadoJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return (true, mensaje, resultado);
                    }

                    return (false, mensaje, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, "Inscripción no encontrada", null);
                }
                else
                {
                    var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
                    var mensaje = jsonResponse.GetProperty("mensaje").GetString() ?? "Error cancelando inscripción";
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
