using BookDemo.Application.Constants;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Presentation.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Presentation.Controllers
{
    [AllowAnonymous]
    [ServiceFilter(typeof(LogActionAttribute))]
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IServiceManager _services;

        public CategoriesController(IServiceManager services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        [HttpGet(Name = CategoryRoutes.GetAll)]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _services.CategoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}", Name = CategoryRoutes.GetById)]
        public async Task<IActionResult> GetCategoryById([FromRoute(Name = "id")] int id)
        {
            var category = await _services.CategoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        [HttpGet("{id:int}/books", Name = CategoryRoutes.GetBooksByCategory)]
        public async Task<IActionResult> GetBooksByCategory([FromRoute] int id)
        {
            var books = await _services.CategoryService.GetBooksByCategoryAsync(id);
            return Ok(books);
        }

        [HttpPost(Name = CategoryRoutes.Create)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryForCreationDto categoryDto)
        {
            if (categoryDto is null)
                return BadRequest("Category cannot be null");

            var created = await _services.CategoryService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}", Name = CategoryRoutes.Update)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryForUpdateDto categoryDto)
        {
            if (categoryDto is null)
                return BadRequest("Category cannot be null");

            await _services.CategoryService.UpdateCategoryAsync(id, categoryDto);
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = CategoryRoutes.Delete)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            await _services.CategoryService.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
