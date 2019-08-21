using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LTRegistrator.BLL.Services;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
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
        private readonly LTRegistratorDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        /// <param name="db"></param>
        /// <param name="userManager">Allows you to manage users</param>
        /// <param name="signInManager">Provides the APIs for user sign in</param>
        /// <param name="configuration">To use the file setting</param>
        public AccountController(
            LTRegistratorDbContext db,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = db;
        }

        /// <summary>
        /// The method tries to authorize the user and return the JWT-token.
        /// </summary>
        /// <param name="model"> LoginDto model containing username and password </param>
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
        /// <param name="model"> RegisterDto model containing data necessary for registration </param>
        /// <returns>JWT-token</returns>
            /* Passwords must be at least 6 characters.
             * Passwords must have at least one non alphanumeric character.
             * Passwords must have at least one digit ('0'-'9').
             * Passwords must have at least one lowercase ('a'-'z').
             * Passwords must have at least one uppercase ('A'-'Z').*/
        [HttpPost]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            using (var transaction = _context.Database.BeginTransaction())
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
                    _context.Employee.Add(employee);
                    User user = new User
                    {
                        UserName = model.FirstName + "_" + model.SecondName,
                        Email = model.Email,
                        EmployeeId = employee.Id
                    };
                    var res = await _userManager.CreateAsync(user, model.Password);
                    if (res.Succeeded)
                    {
                        //Retrieves the name of the constant in the specified enumeration that has the specified value.
                        var role = Enum.GetName(typeof(RoleType), employee.MaxRole);
                        var resAddRole = _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role)).Result;
                        if (resAddRole.Succeeded)
                        {
                            transaction.Commit();
                            await _signInManager.SignInAsync(user, false);
                            return GenerateJwtToken(user);
                        }
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
        private object GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //Generate almost unique identifier for token.
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("EmployeeID", user.EmployeeId.ToString()),
                _userManager.GetClaimsAsync(user).Result.Single(claim => claim.Type == ClaimTypes.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //Signing algorithm
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"])); //How many days is the token valid.

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return Content("{\"token\" : \"" + (new JwtSecurityTokenHandler().WriteToken(token)) + "\"}", "application/json");
        }
    }
}