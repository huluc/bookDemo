using AutoMapper;
using BookDemo.Application.Contracts;
using BookDemo.Application.DTOs;
using BookDemo.Domain.Entities;
using BookDemo.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMapper _mapper;

        public CategoryService(IRepositoryManager manager, ILogger<CategoryService> logger, IMapper mapper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _manager.Categories.GetAllAsync(trackChanges: false);
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await GetCategoryOrThrowAsync(id, trackChanges: false);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto categoryDto)
        {
            if (categoryDto is null)
            {
                _logger.LogWarning("CreateCategory was called with null payload");
                throw new ArgumentNullException(nameof(categoryDto));
            }

            _logger.LogInformation("Creating new category. Name={Name}", categoryDto.Name);

            var category = _mapper.Map<Category>(categoryDto);
            _manager.Categories.Add(category);
            await _manager.SaveAsync();

            _logger.LogInformation("Category created successfully. Id={CategoryId}", category.Id);
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task UpdateCategoryAsync(int id, CategoryForUpdateDto categoryDto)
        {
            if (categoryDto is null)
            {
                _logger.LogWarning("Category update failed because payload is null. RouteId={RouteId}", id);
                throw new BadRequestException("Category update payload is null.");
            }

            var existingCategory = await GetCategoryOrThrowAsync(id, trackChanges: true);
            _mapper.Map(categoryDto, existingCategory);
            await _manager.SaveAsync();

            _logger.LogInformation("Category updated successfully. Id={CategoryId}", id);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            _logger.LogDebug("Attempting to delete category. Id={CategoryId}", id);
            var category = await GetCategoryOrThrowAsync(id, trackChanges: true);
            _manager.Categories.Delete(category);
            await _manager.SaveAsync();
            _logger.LogInformation("Category deleted successfully. Id={CategoryId}", id);
        }

        private async Task<Category> GetCategoryOrThrowAsync(int id, bool trackChanges)
        {
            _logger.LogDebug("Fetching category by Id={CategoryId}", id);
            var category = await _manager.Categories.GetByIdAsync(id, trackChanges);
            if (category is null)
            {
                _logger.LogWarning("Category not found. Id={CategoryId}", id);
                throw new CategoryNotFoundException(id);
            }
            return category;
        }
    }
}
