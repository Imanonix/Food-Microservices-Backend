using Application.DTO;
using AutoMapper;
using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Mapper
{
    public class Mapper: Profile
    {
        public Mapper()
        {
            CreateMap<Basket, BasketDTO>();
            CreateMap<BasketItem, BasketItemDTO>()
                .ForMember(des => des.SelectedOptionDTOs , opt => opt.MapFrom(src => src.SelectedOptions));

            CreateMap<SelectedOption, SelectedOptionDTO>();
        }

    }
}
