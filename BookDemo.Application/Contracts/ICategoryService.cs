using BookDemo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Application.Contracts
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CategoryForCreationDto categoryDto);
        Task UpdateCategoryAsync(int id, CategoryForUpdateDto categoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
