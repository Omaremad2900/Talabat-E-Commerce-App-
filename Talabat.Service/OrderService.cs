using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IPaymentService paymentService;

        public OrderService(IBasketRepository basketRepository, IUnitOfWork unitOfWork,IPaymentService paymentService)
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address ShippingAddress)
        {
            var Basket =await basketRepository.GetBasketAsync(basketId);
            var OrderItems=new List<OrderItem>();
            if (Basket?.Items.Count > 0) 
            {
                foreach (var item in Basket.Items) 
                {
                    var Product =await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductItemOrdered=new ProductItemOrdered(Product.Id,Product.Name,Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrdered, item.Quantity, Product.Price);
                    OrderItems.Add(OrderItem);
                
                }
            }
            var SubTotal= OrderItems.Sum(item=>item.Price*item.Quantity);
            var DeliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);
            var spec = new orderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder=await unitOfWork.Repository<Order>().GetByIDWithSpecAsync(spec);
            if (ExOrder != null) 
            {
                unitOfWork.Repository<Order>().Delete(ExOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basketId);
            
            }

            var Order=new Order(buyerEmail,ShippingAddress,DeliveryMethod,OrderItems,SubTotal,Basket.PaymentIntentId);
            await unitOfWork.Repository<Order>().AddAsync(Order);
            var res=await unitOfWork.CompleteAsync();
            if(res<=0)return null;

            return Order;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var spec=new OrderSpecifications(buyerEmail, orderId);
            var Order=await unitOfWork.Repository<Order>().GetByIDWithSpecAsync(spec);
            return Order;
            
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var spec = new OrderSpecifications(buyerEmail);
            var Orders =await unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return Orders;
        }
    }
}
