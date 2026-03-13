using Application.AdminDTOs;
using Application.AdminDTOs.user;
using Application.DTOs;
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
            CreateMap<RegisterDTO, User>();
            CreateMap<User, AdminUserDTO>();
            CreateMap<AdminAddUserDTO, User>();
            CreateMap<User, UserDTO>();
        }
    }
}
