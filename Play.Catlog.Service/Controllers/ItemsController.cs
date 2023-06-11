using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Service.Api;
using Catalog.Service.Entities;
using Catalog.Service.Repository;
using Microsoft.AspNetCore.Mvc;
using Play.Catlog.Service.DTOs;

namespace Play.Catlog.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        //private static readonly List<ItemDto> items = new()
        //{
        //    new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Antidote", "Cures Poison", 7, DateTimeOffset.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Bronze Sword", "Atk +3", 20, DateTimeOffset.UtcNow)
        //};
        private readonly IItemsRepository _itemsRepository;

        public ItemsController(IItemsRepository itemsRepository)
        {
            this._itemsRepository = itemsRepository;
        }


        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = await _itemsRepository.GetAllAsync();
            return items.Select(item => item.AsDto());

        }

        [HttpGet("{id}")]
        
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemsRepository.GetByIdAsync(id);
            if (item is null) 
                return NotFound();

            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync([FromBody] CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreateDate = DateTimeOffset.UtcNow
            };
            await _itemsRepository.CreateAsync(item);

            return CreatedAtAction( nameof(GetByIdAsync) ,new {id = item.Id }, item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {

            var existingItem = await _itemsRepository.GetByIdAsync(id);

            if (existingItem is null)
                return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _itemsRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _itemsRepository.GetByIdAsync(id);

            if (item is null)
                return NotFound();

            await _itemsRepository.RemoveAsync(item.Id);

            return NoContent();
        }

    }
}
