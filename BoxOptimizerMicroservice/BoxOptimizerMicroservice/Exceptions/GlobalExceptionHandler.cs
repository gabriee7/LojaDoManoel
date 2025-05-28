using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace BoxOptimizerMicroservice.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Ocorreu uma exceção não tratada. TraceId: {TraceId}, Path: {Path}, Mensagem: {ErrorMessage}",
                httpContext.TraceIdentifier,
                httpContext.Request.Path,
                exception.Message);

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            object responsePayload;
            string mensagemClientePadrao = "Ocorreu um erro inesperado ao processar sua requisição.";

            if (_env.IsDevelopment())
            {
                responsePayload = new
                {
                    status = httpContext.Response.StatusCode,
                    titulo = "Erro no Servidor (Desenvolvimento)",
                    mensagem = exception.Message,
                    tipo = exception.GetType().Name
                };
            }
            else
            {
                responsePayload = new
                {
                    status = httpContext.Response.StatusCode,
                    titulo = "Erro no Servidor",
                    mensagem = mensagemClientePadrao
                };
            }

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(
                    responsePayload, 
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                cancellationToken);

            return true;
        }
    }
}