using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService order;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public OrdersController(IOrderService order,IMapper mapper,IUnitOfWork unitOfWork)
        {
            this.order = order;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        [ProducesResponseType(typeof(Order),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status400BadRequest)]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var userEmail=User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress=mapper.Map<AddressDto,Address>(orderDto.shipToAddress);
            var Order = await order.CreateOrderAsync(userEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
            if (Order is null) return BadRequest(new ApiResponse(400, "there is a problem with your Module"));
            return Ok(Order);
        }
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Order), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail=User.FindFirstValue(ClaimTypes.Email);
            var orders=await order.GetOrdersForSpecificUserAsync(BuyerEmail);
            if(orders is null) return NotFound(new ApiResponse(404,"there is no orders for this user"));
            var mapped = mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(mapped);
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Order=await order.GetOrderByIdForSpecificUserAsync(BuyerEmail, id);
            if (Order is null) return NotFound(new ApiResponse(404, $"There is no Order with Id={id} for this User"));
            var mapped=mapper.Map<Order,OrderToReturnDto>(Order);
            return Ok(mapped);
        }
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods() 
        {
            var delivery=await unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(delivery);
        
        }
    }
}
