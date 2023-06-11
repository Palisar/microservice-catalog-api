using Catalog.Service.Entities;
using Play.Catlog.Service.DTOs;

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
