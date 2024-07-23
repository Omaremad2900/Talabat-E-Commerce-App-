using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
   
    public class AccountsController : APIBaseController
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountsController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ITokenService tokenService,IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value) { return BadRequest(new ApiResponse(400, "Email Already Exists")); }
            
            var User=new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName=model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Res=await userManager.CreateAsync(User, model.Password);
            if (!Res.Succeeded) return BadRequest(new ApiResponse(400));
            var ReturnedUser = new UserDto()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                Token =await tokenService.CreateTokenAsync(User, userManager)
            };
            return Ok(ReturnedUser);

        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User=await userManager.FindByEmailAsync(model.Email);
            if(User is null)return Unauthorized(new ApiResponse(401));
            var res=await signInManager.CheckPasswordSignInAsync(User, model.Password,false);
            if (!res.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await tokenService.CreateTokenAsync(User, userManager)
            });
        }
        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser() 
        {
           var Email= User.FindFirstValue(ClaimTypes.Email);
           var user= await userManager.FindByEmailAsync(Email);
            var ReturnedObject = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await tokenService.CreateTokenAsync(user, userManager)
            };
            return Ok(ReturnedObject); 
        
        }
        [Authorize]
        [HttpGet("Address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            var user=await userManager.FindUserithAddressAsync(User);
            var mappedAddress=mapper.Map<Address,AddressDto>(user.Address);
            return Ok(mappedAddress);
        }
        [HttpPost("Address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto address)
        {
            var user = await userManager.FindUserithAddressAsync(User);
            var mappedAddress = mapper.Map<AddressDto, Address>(address);
            mappedAddress.Id=user.Address.Id;
            user.Address= mappedAddress;
            var res=await userManager.UpdateAsync(user);
            if (!res.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(address);
        }
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await userManager.FindByEmailAsync(email) is not null;
        }
    }
}
