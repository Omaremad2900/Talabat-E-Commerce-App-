using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : APIBaseController
    {
        
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork)
        {
           
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
       
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(Params);
            var Products = await unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var MappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
            var CountSpec = new ProductWithFilterationForCountAsync(Params);
            var Count= await unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(Params.PageIndex,Params.PageSize,MappedProducts,Count));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), 200)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var Product = await unitOfWork.Repository<Product>().GetByIDWithSpecAsync(spec);
            if (Product == null) return NotFound(new ApiResponse(404));
            var MappedProduct = mapper.Map<Product, ProductToReturnDto>(Product);
            return Ok(MappedProduct);
        }
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var Types = await unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(brands);
        }
    }
}
