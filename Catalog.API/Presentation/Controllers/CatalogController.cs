using Application.AdminDTO;
using Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }
        //[Authorize(Roles = "owner,admin")]
        [Route("/api/private/catalog")]
        [HttpPost]
        public async Task<IActionResult> CreateGallery([FromBody] AdminAddDTO foodItemDTO)
        {
            try
            {
                var addedGallery = await _catalogService.AddAsync(foodItemDTO);
                return Ok(new ApiResponse { Data = addedGallery, Message = "FoodItem added successfully.", Status = 200 });
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

        //[Authorize(Roles = "owner,admin")]
        //[Route("/api/private/gallery")]
        //[HttpPut]        //PUT
        //public async Task<IActionResult> UpdateSlide([FromForm] AdminEditGallery galleryDTO)
        //{
        //    try
        //    {
        //        var updatedSlide = await _galleryService.UpdateAsync(galleryDTO);
        //        return Ok(new ApiResponse { Data = true, Message = "Slide updated successfully", Status = 200 });
        //    }

        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new ApiResponse { Data = false, Message = ex.Message, Status = 404 });
        //    }

        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new ApiResponse { Data = false, Message = ex.Message, Status = 400 });
        //    }

        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });     //"An error occurred while processing your request."
        //    }

        //}

        //[Authorize(Roles = "owner,admin")]
        [Route("/api/private/catalog")]
        [HttpGet]
        public async Task<IActionResult> AdminGetAll()
        {
            try
            {
                var galleries = await _catalogService.GetAllAsync();
                return Ok(new ApiResponse { Data = galleries, Message = " ", Status = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }
        }

        //[Authorize(Roles = "owner,admin")]
        [Route("/api/private/catalog/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var product = await _catalogService.GetByIdAsync(id);
                return Ok(new ApiResponse { Data = product, Message = "", Status = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            }

            //}
            //[Authorize(Roles = "owner,admin")]
            //[Route("/api/private/gallery")]
            //[HttpDelete]   // DELETE
            //public async Task<IActionResult> DeleteAsync([FromForm] AdminDeleteSlideDTO adminDeleteSlideDTO)
            //{
            //    try
            //    {
            //        var result = await _galleryService.DeleteAsync(adminDeleteSlideDTO.Id);
            //        return Ok(new ApiResponse { Data = true, Message = "Gallery deleted successfully.", Status = 200 });
            //    }
            //    catch (NotFoundException ex)
            //    {
            //        return NotFound(new ApiResponse { Data = false, Message = ex.Message, Status = 404 });
            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            //    }
            //}

            //[Route("/api/galleries")]
            //[HttpGet]
            //public async Task<IActionResult> GetAll([FromQuery] string lang)
            //{
            //    try
            //    {
            //        var galleries = await _galleryService.GetAllAsync();
            //        return Ok(new ApiResponse { Data = galleries, Message = " ", Status = 200 });
            //    }
            //    catch (NotFoundException ex)
            //    {
            //        return NotFound(new ApiResponse { Data = false, Message = ex.Message, Status = 404 });
            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            //    }
            //}

            //[Route("/api/public/gallery")]
            //[HttpGet]
            //public async Task<IActionResult> GetGalleryById([FromQuery] string lang, int id)
            //{
            //    try
            //    {
            //        var galleryDTO = await _galleryService.GetByIdAsync(id);
            //        return Ok(new ApiResponse { Data = galleryDTO, Message = "", Status = 200 });
            //    }
            //    catch (NotFoundException ex)
            //    {
            //        return NotFound(new ApiResponse { Data = false, Message = ex.Message, Status = 404 });
            //    }
            //    catch (Exception ex)
            //    {
            //        return StatusCode(500, new ApiResponse { Data = false, Message = ex.Message, Status = 500 });
            //    }

            //}
        }
    }
}
