using Application.DTO;
using Application.IBasketService;
using AutoMapper;
using Domain.IRepository;
using Domain.models;
using FoodOrder.Shared.Contracts.Class;
using FoodOrder.Shared.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.BasketService
{
    public class ShoppingBasketService : IShoppingBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly HttpClient _client;
        public ShoppingBasketService(IBasketRepository basketRepository, IMapper mapper, HttpClient client)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _client = client;
        }

        public async Task<BasketDTO?> GetShoppingBasketAsync(string userId)
        {
            var basket = await _basketRepository.GetShoppingBasketAsync(userId);
            return _mapper.Map<BasketDTO>(basket);
        }

        public async Task<BasketDTO?> UpdateShoppingBasketAsync(string id, AddItemDTO item)
        {
            // get item from Catalog Service 
            var response = await _client.GetFromJsonAsync<ApiResponse<CatalogItemResponse>>($"http://localhost:5102/api/private/catalog/{item.ProductId}");
            var product = response.Data;

            // get existed basket from redis
            var existedBasket = await _basketRepository.GetShoppingBasketAsync(id);

            if (existedBasket != null)
            {
                var existedItem = existedBasket.BasketItems.SingleOrDefault(bi =>
                                 bi.ProductId == item.ProductId &&
                                 (bi.SelectedOptions?.Count?? 0) == (item.SelectedOptions?.Count?? 0) &&
                                 (bi.SelectedOptions == null || bi.SelectedOptions.All(so => item.SelectedOptions != null && item.SelectedOptions.Any(iso => iso.OptionId == so.OptionId))));
                if (existedItem != null)
                {
                    existedItem.Quantity += item.Quantity;
                    await _basketRepository.UpdateShoppingBasketAsync(id, existedBasket);
                }
                else
                {
                    existedBasket.BasketItems.Add(new BasketItem { ProductId = product.Id, Name = product.Name, Price = product.Price, Quantity = item.Quantity });
                    await _basketRepository.UpdateShoppingBasketAsync(id, existedBasket);
                }
            }
            else
            {
                // Create Shopping Basket and store in redis
                var newBasket = new Basket(Guid.Parse(id))
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = Guid.Parse(id),
                    BasketItems = new List<BasketItem>{new BasketItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = item.Quantity,
                        ImageUrl = "test/url",
                        SelectedOptions = product.Customizations
                            .SelectMany(c => c.Options)
                            .Where(opt => item.SelectedOptions.Any(so => so.OptionId == opt.OptionId)) // فقط آن‌هایی که کاربر فرستاده
                            .Select(opt => new SelectedOption
                            {
                                OptionId = opt.OptionId,
                                Name = opt.Name,
                                ExtraPrice = opt.ExtraPrice
                            })
                            .ToList(),
                    }
                    }
                };
                await _basketRepository.UpdateShoppingBasketAsync(id, newBasket);

            }

            var updatedBasket = await GetShoppingBasketAsync(id);
            return _mapper.Map<BasketDTO>(updatedBasket);
        }

        public async Task<bool> DeleteBasketAsync(string userId)
        {
            var result = await _basketRepository.DeleteBasketAsync(userId);
            return result;
        }

        public Task SendBasketToOrder(string id)
        {
            throw new NotImplementedException();
        }
    }
}
