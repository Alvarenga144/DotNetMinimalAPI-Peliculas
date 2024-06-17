using AutoMapper;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entidades;
using MinimalAPIPeliculas.Migrations;

namespace MinimalAPIPeliculas.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, CrearGeneroDTO>();
        }
    }
}
