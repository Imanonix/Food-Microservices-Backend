using Application.AdminDTO;
using Application.interfaces;
using AutoMapper;
using Domain.IRepository;
using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.implementation
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public Task<bool> AddAsync(AdminAddDTO addDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<AdminFoodItemDTO>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        //public OrderService(IOrderRepository catalogRepository, IMapper mapper)
        //{
        //    _repository = catalogRepository;
        //    _mapper = mapper;
        //}
        //public async Task<bool> AddAsync(AdminAddDTO addDTO)
        //{
        //    try
        //    {
        //        var foodItem = _mapper.Map<Order>(addDTO);
        //        var result = await _repository.CreateAsync(foodItem);
        //        return result;
        //    }
        //    catch {
        //        throw new Exception();
        //    }
        //}

        //public async Task<List<AdminFoodItemDTO>> GetAllAsync()
        //{
        //    try
        //    {
        //        var foodItems = await _repository.GetAllAsync();
        //        if (foodItems.Count() == 0 || !foodItems.Any()) { 
        //            throw new Exception("No FoodItem founded.");
        //        }

        //        return _mapper.Map<List<AdminFoodItemDTO>>(foodItems);
        //    }
        //    catch { 
        //        throw new Exception() ;
        //    }
        //}
    }
}
