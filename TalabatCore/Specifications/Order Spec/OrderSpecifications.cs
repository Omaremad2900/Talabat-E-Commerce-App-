using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Spec
{
    public class OrderSpecifications :BaseSpecifications<Order>
    {
        public OrderSpecifications(string email) :base(O=>O.BuyerEmail==email)
        {
            Includes.Add(o=>o.DeliveryMethod);
            Includes.Add(o=>o.Items);
            AddOrderByDescending(o=>o.OrderDate);
        }
        public OrderSpecifications(string email,int OrderId):base(o=>o.BuyerEmail==email && o.Id == OrderId)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
    }
}
