using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;

namespace StudentPortal.WEB.Helpers
{
    public class HttpInterceptor : DelegatingHandler
    {
        private readonly NavigationManager _nav;
        private readonly LocalStorageService _local;
        private readonly IJSRuntime _js;

        public HttpInterceptor(NavigationManager nav,
                               LocalStorageService local,
                               IJSRuntime js)
        {
            _nav = nav;
            _local = local;
            _js = js;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                     CancellationToken cancellationToken)
        {
            // ► Encabezados fijos para la API
            request.Headers.Add("Sitio", "StudentPortal");
            request.Headers.Add("Clave", "Portal123");

            // ► Token JWT (si existe)
            var token = await _local.GetItemAsync<string>("authToken");
            var usuarioId = await _local.GetItemAsync<string>("usuarioId");

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (!string.IsNullOrEmpty(usuarioId))
                request.Headers.Add("UsuarioId", usuarioId);

            // ► Enviar
            var response = await base.SendAsync(request, cancellationToken);

            // ► 401 → forzar logout
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _local.RemoveItemAsync("authToken");
                await _local.RemoveItemAsync("usuarioId");
                _nav.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }
    }
}