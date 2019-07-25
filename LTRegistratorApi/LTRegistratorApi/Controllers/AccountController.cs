using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LTRegistratorApi.Model;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// The controller that is responsible for basic user operations.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ApplicationContext context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        /// <param name="userManager">Allows you to manage users</param>
        /// <param name="signInManager">Provides the APIs for user sign in</param>
        /// <param name="configuration">To use the file setting</param>
        public AccountController(
            ApplicationContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            context = db;

        }

        /// <summary>
        /// The method tries to authorize the user and return the JWT-token.
        /// </summary>
        /// <param name="model">LoginDto (user)</param>
        /// <returns>JWT-token</returns>
        [HttpPost]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
                return GenerateJwtToken(user);
            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        /// <summary>
        /// The method attempts to register a user and return the JWT-token.
        /// </summary>
        /// <param name="model">User</param>
        /// <returns>JWT-token</returns>
            /* Passwords must be at least 6 characters.
             * Passwords must have at least one non alphanumeric character.
             * Passwords must have at least one digit ('0'-'9').
             * Passwords must have at least one lowercase ('a'-'z').
             * Passwords must have at least one uppercase ('A'-'Z').*/
        [HttpPost]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Employee employee = new Employee
                    {
                        FirstName = model.FirstName,
                        SecondName = model.SecondName,
                        Mail = model.Email,
                        MaxRole = model.Role
                    };
                    context.Employee.Add(employee);
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = model.FirstName + "_" + model.SecondName,
                        Email = model.Email,
                        EmployeeId = employee.EmployeeId
                    };
                    transaction.Commit();
                    var res = _userManager.CreateAsync(user, model.Password).Result;
                    var resAddRole = _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, model.Role)).Result;
                    if (res.Succeeded && resAddRole.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return GenerateJwtToken(user);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                throw new ApplicationException("ERROR_REGISTER");
            }
        }
        /// <summary>
        /// Method generates JWT-token for user.
        /// </summary>
        /// <param name="user">The ApplicationUser who has mail</param>
        /// <returns>JWT-token</returns>
        private object GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //generate almost unique identifier for token
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                _userManager.GetClaimsAsync(user).Result.Single(claim => claim.Type == ClaimTypes.Role) //role
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //signing algorithm
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"])); //how many days is the token valid

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return Content("{'token' : '" + (new JwtSecurityTokenHandler().WriteToken(token)) + "'}", "application/json");
        }
    }
}