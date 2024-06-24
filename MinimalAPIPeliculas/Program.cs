using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Repositorios;
using MinimalAPIPeliculas.Servicios;

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
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

// Fin de area de los servicios entre builder y var app

var app = builder.Build();

//if (builder.Environment.IsDevelopment())
// {
// }

// Inicio de area de los middlewares
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")] () => "¡Hola, mundo!");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();

// Fin de area de los middlewares

app.Run();