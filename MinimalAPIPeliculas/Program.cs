using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
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

var endpointsGeneros = app.MapGroup("/generos");

endpointsGeneros.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("generos-get")); // Para cachear con tiempo

endpointsGeneros.MapGet("/{id:int}", ObtenerGeneroPorId);

endpointsGeneros.MapPost("/", CrearGenero);

endpointsGeneros.MapPut("/{id:int}", );

endpointsGeneros.MapDelete("/{id:int}", async (int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);

    if (!existe)
    {
        return Results.NotFound();
    }

    await repositorio.Borrar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

// Fin de area de los middlewares

app.Run();

static async Task<Ok<List<Genero>>> ObtenerGeneros(IRepositorioGeneros repositorio)
{
    var generos = await repositorio.ObtenerTodos();
    return TypedResults.Ok(generos);
}

static async Task<Results<Ok<Genero>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id)
{
    var genero = await repositorio.ObtenerPorId(id);

    if (genero is null)
    {
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(genero);
}

static async Task<Created<Genero>> CrearGenero(Genero genero, IRepositorioGeneros repositorio,
    IOutputCacheStore outputCacheStore)
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.Created($"/generos/{id}", genero);
}

async (int id, Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);

    if (!existe)
    {
        return Results.NotFound();
    }

    await repositorio.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
}