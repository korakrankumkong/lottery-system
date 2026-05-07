using AutoMapper;
using LotterySystem.Application.DTOs;
using LotterySystem.Domain.Entities;

namespace LotterySystem.Application.Common;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TicketItem, TicketItemDto>();
        CreateMap<Ticket, TicketDto>();
    }
}
