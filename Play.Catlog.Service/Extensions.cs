using Catalog.Service.Api.DTOs;
using Catalog.Service.Api.Entities;

namespace Catalog.Service.Api
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreateDate);
        }
    }
}
 