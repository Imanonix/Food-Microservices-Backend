using Application.AdminDTO;
using Application.interfaces;
using AutoMapper;
using Domain.IRepository;
using Domain.models;
using FoodOrder.Shared.Contracts.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.implementation
{
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoints;
        public CatalogService(ICatalogRepository catalogRepository, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repository = catalogRepository;
            _mapper = mapper;
            _publishEndpoints = publishEndpoint;
        }
        public async Task<bool> AddAsync(AdminAddDTO addDTO)
        {
            try
            {
                var foodItem = _mapper.Map<FoodItem>(addDTO);
                //foodItem.ImageUrl = "test url";
                var result = await _repository.CreateAsync(foodItem);
                var customizations = foodItem.Customizations;
                await _publishEndpoints.Publish<IFoodItemCreated>(new {Id = foodItem.Id, Name = foodItem.Name, Price= foodItem.Price, Customizations = customizations});
                return result;
            }
            catch {
                throw new Exception();
            }
        }

        public async Task<List<AdminFoodItemDTO>> GetAllAsync()
        {
            try
            {
                var foodItems = await _repository.GetAllAsync();
                if (foodItems.Count == 0 || !foodItems.Any()) { 
                    throw new Exception("No FoodItem founded.");
                }
                
                return _mapper.Map<List<AdminFoodItemDTO>>(foodItems);
            }
            catch { 
                throw new Exception() ;
            }
        }

        public async Task<AdminFoodItemDTO> GetByIdAsync(string id)
        {
            var foodItem = await _repository.GetByIdAsync(id);
            return _mapper.Map<AdminFoodItemDTO>(foodItem);
        }
    }
}
