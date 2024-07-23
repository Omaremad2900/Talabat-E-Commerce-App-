using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket(string basketId)
        {
            Id = basketId;
        }
        [JsonConstructor]
        public CustomerBasket(string id, List<BasketItem> items)
        {
            Id = id;
            Items = items ?? new List<BasketItem>();
        }

        public string Id { get; set; }
        
        public List<BasketItem> Items { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public int? DeliveryMethodId { get; set; }

    }
}
