using ArquivosLibrary.Repository;
using ArquivosLibrary.Service;
using ArquivosLibrary.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gerenciamento da API...",
        Version = "v1",
        Description = $@"<h3>Título <b>da API</b></h3>
                                      <p>
                                          Alguma descrição....
                                      </p>",
        Contact = new OpenApiContact
        {
            Name = "Suporte Unoeste",
            Email = string.Empty,
            Url = new Uri("https://www.unoeste.br"),
        },
    });

    // Set the comments path for the Swagger JSON and UI.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// Add services to the container.

builder.Services.AddControllers();





// Acessando o valor diretamente pelo builder.Configuration
Environment.SetEnvironmentVariable("STRING_CONEXAO", builder.Configuration["StringConexao"]);


builder.Services.AddScoped<AlunosRepository>();
builder.Services.AddScoped<AlunosService>();

builder.Services.AddScoped<CidadesRepository>();
builder.Services.AddScoped<CidadesService>();

DbContext dbContext = new DbContext();
builder.Services.AddSingleton(dbContext);


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = ""; //habilitar a página inicial da API ser a doc.
    c.DocumentTitle = "Gerenciamento de Produtos - API V1";
});


// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
