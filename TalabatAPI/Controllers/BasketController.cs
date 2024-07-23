using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;

namespace Talabat.APIs.Controllers
{
   
    public class BasketController : APIBaseController
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetCustomer (string BasketId)
        {
            var Basket=await basketRepository.GetBasketAsync(BasketId);
            if(Basket == null) return new CustomerBasket(BasketId);
            return Ok(Basket);
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTo Basket)
        {
            var mappedBasket = mapper.Map<CustomerBasketDTo, CustomerBasket>(Basket);
            var CreatedOrUpdated=await basketRepository.UpdateBasketAsync(mappedBasket);
            if (CreatedOrUpdated == null) return BadRequest(new ApiResponse(400));
            return Ok(CreatedOrUpdated);
        }
        [HttpDelete]
        public async Task<bool> DeleteBasket(string BasketId) { return await basketRepository.DeleteBasketAsync(BasketId); }
    }
}
