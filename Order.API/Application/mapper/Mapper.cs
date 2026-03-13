
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
            CreateMap<AdminAddDTO, Order>();
            CreateMap<Order, AdminFoodItemDTO>();

        }
    }
}
