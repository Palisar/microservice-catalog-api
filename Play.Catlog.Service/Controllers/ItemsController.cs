using Catalog.Service.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Service.Api;
using Catalog.Service.Api.DTOs;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;

namespace Catalog.Service.Controllers
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
        private readonly IRepository<Item> _repository;
        private readonly IPublishEndpoint _publishEndpoint;

   
        public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            this._repository = repository;
            _publishEndpoint = publishEndpoint;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
           
            var items = await _repository.GetAllAsync();

            return Ok(items.Select(item => item.AsDto()));
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _repository.GetAsync(id);
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
            await _repository.CreateAsync(item);

            await _publishEndpoint.Publish(new CatalogIItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {

            var existingItem = await _repository.GetAsync(id);

            if (existingItem is null)
                return NotFound();

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await _repository.UpdateAsync(existingItem);

            await _publishEndpoint.Publish(new CatalogIItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _repository.GetAsync(id);

            if (item is null)
                return NotFound();

            await _repository.RemoveAsync(item.Id);

            await _publishEndpoint.Publish(new CatalogIItemDeleted(item.Id));

            return NoContent();
        }

    }
}
