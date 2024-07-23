using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.Repository
{
    public class BasketRepository : IBasketRepository 
    {
        private readonly IDatabase database;
        public BasketRepository(IConnectionMultiplexer redis)
        {
            database=redis.GetDatabase();
        
        }

        public async Task<bool> DeleteBasketAsync(string BasketId)
        {
            return await database.KeyDeleteAsync(BasketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string Basketid)
        {
            
            var Basket=await database.StringGetAsync(Basketid);
            return Basket.IsNull? null:JsonSerializer.Deserialize<CustomerBasket>(Basket);
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket)
        {
            var jsonBasket=JsonSerializer.Serialize(Basket);
            var CreatedOrUpdated=await database.StringSetAsync(Basket.Id, jsonBasket,TimeSpan.FromDays(1));
            if (!CreatedOrUpdated) return null;
            return await GetBasketAsync(Basket.Id);
        }
    }
}
