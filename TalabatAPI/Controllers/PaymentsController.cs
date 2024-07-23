using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    
    public class PaymentsController : APIBaseController
    {
        private readonly IPaymentService paymentService;
        private readonly IMapper mapper;
        const string endpointSecret = "whsec_b88788fa95134d216a01544271a6189bd9bf1f6d894613dcce73dcc56987a3b9";

        public PaymentsController(IPaymentService paymentService,IMapper mapper)
        {
            this.paymentService = paymentService;
            this.mapper = mapper;
        }
        [HttpPost("{basketId}")]
        [ProducesResponseType(typeof(CustomerBasketDTo),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerBasketDTo), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult <CustomerBasketDTo>> CreateOrUpdatePaymentIntent(string basketId)
        { 
            var CustomerBasket=await paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (CustomerBasket == null) return BadRequest(new ApiResponse(400, "there is a Problem With Your Basket"));
            var MappedBasket=mapper.Map<CustomerBasket,CustomerBasketDTo>(CustomerBasket);
            return Ok(MappedBasket);
        
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook() 
        { 
             var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var payintent=stripeEvent.Data.Object as PaymentIntent;
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await paymentService.UpdatePaymentIntent(payintent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await paymentService.UpdatePaymentIntent(payintent.Id, true);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
             catch (StripeException e)
             {
                return BadRequest();
             }
        }
    }
    
}
