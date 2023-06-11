using Catalog.Service.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Service.Repository
{
    public interface IItemsRepository
    {
        Task CreateAsync(Item entity);
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item> GetByIdAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Item entity);
    }
}