using Application.DTO;
using Application.IBasketService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IShoppingBasketService _shoppingBasketService;
        public BasketController(IShoppingBasketService shoppingBasketService)
        {
            _shoppingBasketService = shoppingBasketService;
        }
        [Route("/api/basket")]
        [HttpGet]
        public async Task<IActionResult> GetShoppingBasketAsync(string id)
        {
            try
            {
                var basket = await _shoppingBasketService.GetShoppingBasketAsync(id);
                return Ok(new ApiResponse { Data = basket, Message = "", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = true, Message = ex.Message, Status = 500 });   //"An error occurred while processing your request."
            }
        }
        [Route("/api/basket")]
        [HttpPost]
        public async Task<IActionResult> UpdateShoppingBasketAsync(string id,[FromBody] AddItemDTO item)
        {
            try
            {
                var upDatedBasket = await _shoppingBasketService.UpdateShoppingBasketAsync(id, item);
                return Ok(new ApiResponse { Data = upDatedBasket, Message = "Basket has updated successfilly", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = true, Message = ex.Message, Status = 500 });   //"An error occurred while processing your request."
            }
        }

        [Route("/api/basket")]
        [HttpDelete]
        public async Task<IActionResult> DeleteBasketAsync(string id)
        {
            try
            {
                var isDeleted = await _shoppingBasketService.DeleteBasketAsync(id);
                return Ok(new ApiResponse { Data = isDeleted, Message = "Basket has deleted successfully", Status = 200 });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = true, Message = ex.Message, Status = 500 });   //"An error occurred while processing your request."
            }
        }

        //[Route("/api/basket/checkout")]
        //[HttpGet]
        //public async Task<IActionResult> Checkout(string id)
        //{
        //    try
        //    {
        //        var basket = await _shoppingBasketService.SendBasketToOrderAsync(id);
        //        return Ok(new ApiResponse { Data = basket, Message = "", Status = 200 });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse { Data = true, Message = ex.Message, Status = 500 });   //"An error occurred while processing your request."
        //    }
        //}
    }
}
