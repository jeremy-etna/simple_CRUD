using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using quest_web.DAL;
using quest_web.Models;
using quest_web.Utils;

namespace quest_web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly APIDbContext _context;
        private readonly JwtTokenUtil _jwt;

        public AuthenticationController(ILogger<AuthenticationController> logger, APIDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _logger = logger;
            _jwt = jwt;
        }

        [AllowAnonymous]
        [HttpPost("/register")]
        public async Task<ObjectResult> Register([FromBody]User user)
        {
            var usr = new User
            {
                Username = user.Username, 
                Password = user.Password, 
                Role = user.Role,
                Creation_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };

            if (string.IsNullOrEmpty(usr.Username) ||
                string.IsNullOrEmpty(usr.Password))
            {
                return BadRequest("Invalid Username or Password fields!");
            }

            var usernameTaken = _context.Users.Count(u => u.Username == user.Username);
            if(usernameTaken > 0)
                return Conflict("Username is already taken!");
            
            usr.Password = BCrypt.Net.BCrypt.HashPassword(usr.Password);

            await _context.Set<User>().AddAsync(usr);
            await _context.SaveChangesAsync();

            usr.Password = "";

            return Created("/user/" + usr.ID, usr);
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public async Task<ObjectResult> Authenticate([FromBody]User user)
        {
            if (string.IsNullOrEmpty(user.Username) ||
                string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Invalid Username or Password fields!" + JsonConvert.SerializeObject(user));
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

            if(currentUser == null)
                return BadRequest("User doesn't exist");

            if (BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password) == false)
                return Unauthorized("Password is not valid");


            return Ok(_jwt.GenerateToken(user));
        }

        [Authorize]
        [HttpGet("/me")]
        public async Task<ObjectResult> Me()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);

            if(string.IsNullOrEmpty(username))
                return BadRequest("Please authenticate first.");

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (currentUser == null)
                return BadRequest("User doesn't exist");

            var userDetails =  new UserDetails
            {
                ID = currentUser.ID,
                Username = currentUser.Username, 
                Role = currentUser.Role
            };

            return Ok(userDetails);
        }

    }
}