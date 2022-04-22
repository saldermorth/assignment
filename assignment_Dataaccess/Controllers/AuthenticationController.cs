﻿using assignment_Dataaccess.Models;
using assignment_Dataaccess.Models.Enities;
using assignment_Dataaccess.Models.Forms;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace assignment_Dataaccess.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly SqlContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(SqlContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(CustomerForm form)
        {

            try
            {
                if (await _context.Customers.AnyAsync(x => x.Email == form.Email))
                {
                    return new ConflictObjectResult("USer already exits");
                }
                var address = new AddressEntity
                {
                    City = form.City,
                    Street = form.Street,
                    ZipCode = form.ZipCode
                };

                var CustomerEntity = new CustomerEntity()
                {
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    Email = form.Email,
                    Address = address
                };
                CustomerEntity.CreateSecurePassword(form.Password);
                _context.Customers.Add(CustomerEntity);
                await _context.SaveChangesAsync();


                return new OkObjectResult("User created");
            }
            catch (Exception)
            {
                return new BadRequestResult();
            }
           
        }
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInForm form)  
        {
            if (string.IsNullOrEmpty(form.Email) || string.IsNullOrEmpty(form.Password))
            {
                return new BadRequestObjectResult("Email and password must be filled in.");
            }
            var userEntity = await _context.Customers.FirstOrDefaultAsync(x => x.Email == form.Email);
            if (userEntity == null)
            {
                return new BadRequestObjectResult("Incorrect email or password..");
            }
            if (!userEntity.CompareSecurePassword(form.Password))
            {
                return new BadRequestObjectResult("Incorrect email or password..");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userEntity.Email),
                    new Claim(ClaimTypes.Email, userEntity.Email),
                    new Claim("UserID", userEntity.Id.ToString()),
                    new Claim("ApiKey", _configuration.GetValue<string>("AdminApiKey"))

            }),
                Expires = DateTime.Now.AddDays(1),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("SecretKey"))),
                    SecurityAlgorithms.HmacSha512Signature
                    )
            };
            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            Console.WriteLine(accessToken);
            return new OkObjectResult(accessToken);

        }
    }
}
