
using Application.AdminDTO;
using AutoMapper;
using Domain.models;
using Microsoft.AspNetCore.Identity;
using System.Net;


namespace Application.mapper
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<AdminAddDTO, FoodItem>();
            CreateMap<FoodItem, AdminFoodItemDTO>();
            CreateMap<AdminAddFoodCustomization, FoodCustomization>().ReverseMap();
            CreateMap<AdminAddCustomizationOption, CustomizationOption>().ReverseMap();

        }
    }
}
