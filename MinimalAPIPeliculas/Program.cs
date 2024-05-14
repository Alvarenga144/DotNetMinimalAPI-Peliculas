using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var origenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!;

// Inicio de area de los servicios

builder.Services.AddDbContext<ApplicationDBContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuracion =>
    {
        configuracion.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuracion =>
    {
        configuracion.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

// Fin de area de los servicios entre builder y var app

var app = builder.Build();

// Inicio de area de los middlewares

//if (builder.Environment.IsDevelopment())
// {
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")] () => "¡Hola, mundo!");

app.MapGet("/generos", async (IRepositorioGeneros repositorio) =>
{
    return await repositorio.ObtenerTodos();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("generos-get")); // Para cachear con tiempo

app.MapGet("/generos/{id:int}", async(IRepositorioGeneros repositorio, int id) =>
{
    var genero = await repositorio.ObtenerPorId(id);

    if (genero is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genero);
});

app.MapPost("/generos", async (Genero genero, IRepositorioGeneros repositorio, 
    IOutputCacheStore outputCacheStore) =>
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.Created($"/generos/{id}", genero);
});

// Fin de area de los middlewares

app.Run();
