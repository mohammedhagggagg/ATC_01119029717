using EventBooking.ApI.DTOs;
using EventBooking.ApI.Helper;
using EventBooking.BLL.Repositories.Contract;
using EventBooking.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBooking.ApI.Controllers
{
    
    public class CategoryController : BaseAPIControllercs
    {
        private readonly IGenericRepository<Category> categoryRepository;
        public CategoryController(IGenericRepository<Category> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await categoryRepository.GetAllAsync();
            var categoriesDtos = categories.Select(c => new CategoryDto
            {
                Name = c.Name,
                Description = c.Description,
                PhotoName = c.Photo != null ? $"{Request.Scheme}://{Request.Host}{c.Photo}" : null

            });

            return Ok(categoriesDtos);
        }
        [HttpGet("GetCategoryById/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name =category.Name,
                Description = category.Description,
                PhotoName = category.Photo != null ? $"{Request.Scheme}://{Request.Host}{category.Photo}" : null
            };
            return Ok(categoryDto);
        }
        [HttpPost("CreateCategory")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromForm] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageName = "";
            string photoUrl = null;
            if (categoryDto.Photo != null)
            {
                var result = HandlerPhotos.UploadPhoto(categoryDto.Photo, "categories");
                if (result == "Invalid image format.")
                {
                    return BadRequest("Invalid image format. Only .jpg, .jpeg, and .png are allowed.");
                }
                photoUrl = $"/images/categories/{result}";
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description,
                Photo = photoUrl,
            };

            await categoryRepository.AddAsync(category);
            await categoryRepository.SaveChangesAsync();
            categoryDto.Id = category.Id;
            categoryDto.PhotoName = photoUrl;
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, categoryDto);
        }
        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest("Category data is null");
            }
            if (id != categoryDto.Id)
            {
                return BadRequest("Category ID mismatch");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var categoryToUpdate = await categoryRepository.GetByIdAsync(id);
            if (categoryToUpdate == null) return NotFound("Category not found");

            string photoUrl = categoryToUpdate.Photo;
            if (categoryDto.Photo != null)
            {

                // Delete the old photo if it exists
                if (!string.IsNullOrEmpty(categoryToUpdate.Photo))
                {
                    var oldFileName = Path.GetFileName(categoryToUpdate.Photo);
                    HandlerPhotos.DeletePhoto("Categories", oldFileName);
                }
                try
                {
                    // Upload the new photo
                    var imageName = HandlerPhotos.UploadPhoto(categoryDto.Photo, "Categories");
                    if (imageName == "Invalid image format.")
                    {
                        return BadRequest("Invalid image format. Only .jpg, .jpeg, and .png are allowed.");
                    }
                    photoUrl = $"/images/Categories/{imageName}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error uploading photo: {ex.Message}");
                }
            }
            categoryToUpdate.Name = string.IsNullOrEmpty(categoryDto.Name) ? categoryToUpdate.Name : categoryDto.Name;
            categoryToUpdate.Description = string.IsNullOrEmpty(categoryDto.Description) ? categoryToUpdate.Description : categoryDto.Description;
            categoryToUpdate.Photo = photoUrl;
            categoryToUpdate.ModifiedDate = DateTime.Now;

            await categoryRepository.UpdateAsync(categoryToUpdate);
            await categoryRepository.SaveChangesAsync();
            categoryDto.Name = categoryToUpdate.Name;
            categoryDto.Description = categoryToUpdate.Description;

            categoryDto.PhotoName = photoUrl;
            return Ok(categoryDto);
           
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return BadRequest("Category not found");
            }
            if (!string.IsNullOrEmpty(category.Photo))
            {
                var fileName = Path.GetFileName(category.Photo);
                HandlerPhotos.DeletePhoto("categories", fileName);
            }
            await categoryRepository.DeleteAsync(category);
            await categoryRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
