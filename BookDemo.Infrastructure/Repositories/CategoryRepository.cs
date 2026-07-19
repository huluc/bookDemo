using BookDemo.Application.Contracts;
using BookDemo.Domain.Entities;
using BookDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookDemo.Infrastructure.Repositories
{
    public sealed class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await Set.AsNoTracking().AnyAsync(c => c.Id == id);
        }

        public async Task<Category?> GetByIdAsync(int id, bool trackChanges)
        {
            if (trackChanges)
                return await Set.FindAsync(id);
            else
                return await Set.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }
    }
}
