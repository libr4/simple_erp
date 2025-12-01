using System;
using AutoMapper;
using Backend.DTOs;
using Backend.Models;

namespace Backend.Mapping;

/// <summary>
/// Perfil de mapeamento AutoMapper para DTOs e entidades.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeamento Produto -> ProdutoDto
        CreateMap<Produto, ProdutoDto>()
            .ReverseMap();

        // Mapeamento MovimentacaoEstoque -> MovimentacaoDto
        CreateMap<MovimentacaoEstoque, MovimentacaoDto>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString().ToUpperInvariant()));

        // Mapeamento explícito DTO -> Entidade (conversão de string para enum, case-insensitive)
        CreateMap<MovimentacaoDto, MovimentacaoEstoque>()
            .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => Enum.Parse<TipoMovimentacao>(src.Tipo, true)));
    }
}
