using BoxOptimizerMicroservice.EntityFramework;
using BoxOptimizerMicroservice.Exceptions;
using BoxOptimizerMicroservice.Services;
using BoxOptimizerMicroservice.Services.EstrategiaOtimizacaoEmbalagem;
using BoxOptimizerMicroservice.Services.OtimizadorEmbalagem;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BoxOptimizerDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IOtimizadorEmbalagemService, OtimizadorEmbalagemService>();
builder.Services.AddTransient<IEstrategiaDeEmpacotamento, EstrategiaEmpacotamentoEco>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1", 
        Title = "BoxOptimizer Microservice API",
        Description = "Microsserviço para calcular a otimização o empacotamento de produtos em caixas.",
    });
    options.AddSecurityDefinition("ApiKeyAuth", new OpenApiSecurityScheme
    {
        Description = "Autenticação por API Key. Insira 'X-API-KEY: {sua_chave_aqui}' no header da requisição ou cole sua chave aqui.",
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKeyAuth"
                },
                Scheme = "ApiKey", 
                Name = "X-API-KEY",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BoxOptimizerDbContext>();
    try
    {
        dbContext.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao aplicar as migrações do banco de dados.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BoxOptimizer API V1");
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();