using Microsoft.AspNetCore.Mvc.Filters;
using BoxOptimizerMicroservice.EntityFramework;
using BoxOptimizerMicroservice.Security;
using Microsoft.EntityFrameworkCore;
using BoxOptimizerMicroservice.Exceptions;

namespace BoxOptimizerMicroservice.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)] 
    public class AuthApiKeyAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private const string _apiKeyHeaderName = "X-API-KEY";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_apiKeyHeaderName, out var potentialApiKey) ||
                string.IsNullOrWhiteSpace(potentialApiKey))
                throw new ApiKeyMissingException("API Key não fornecida no header 'X-API-KEY' ou está vazia.");

            var dbContext = context.HttpContext.RequestServices.GetService<BoxOptimizerDbContext>();

            if (dbContext == null)
                throw new InvalidOperationException("Erro interno do servidor: DbContext não configurado.");

            var hashedApiKeyToValidate = ApiKeyManager.HashApiKey(potentialApiKey.ToString());
            var clientApp = await dbContext.AplicacaoCliente
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(app => EF.Property<string>(app, "_hashedKey") == hashedApiKeyToValidate);

            if (clientApp == null)
                throw new InvalidApiKeyException("API Key inválida.");
        }
    }
}