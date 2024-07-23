﻿using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped(typeof(IPaymentService),typeof(PaymentService));
            Services.AddScoped(typeof(IOrderService), typeof(OrderService));   
            Services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
           /* Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));*/
            Services.AddAutoMapper(typeof(MappingProfiles));
            Services.Configure<ApiBehaviorOptions>(
                options =>
                {
                    options.InvalidModelStateResponseFactory = (actionContext) =>
                    {
                        var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                        .SelectMany(P => P.Value.Errors)
                        .Select(E => E.ErrorMessage)
                        .ToArray();
                        var ValidationErrorResponse = new ApiValidationErrorResponse() { Errors = errors };
                        return new BadRequestObjectResult(ValidationErrorResponse);
                    };
                });
            
            return Services;
        }
    }
}
