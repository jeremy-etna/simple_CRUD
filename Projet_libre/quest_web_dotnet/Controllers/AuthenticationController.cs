using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quest_web.DAL;
using quest_web.Models;
using quest_web.Utils;
using quest_web.DTO;

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
        public async Task<ObjectResult> Register([FromBody]UserRegistrationParams user)
        {
            var usr = new User
            {
                Username = user.Username.ToLower(), 
                Password = user.Password, 
                Role = (UserRole) user.Role,
                Creation_Date = DateTime.Now,
                Updated_Date = DateTime.Now,
            };

            var usernameTaken = _context.User.Where(u => u.Username == usr.Username);
            if (usernameTaken.Any())
                return Conflict(new { error = "Username is already taken!" });
            
            usr.Password = BCrypt.Net.BCrypt.HashPassword(usr.Password);

            await _context.User.AddAsync(usr);
            await _context.SaveChangesAsync();

            usr.Password = "";

            return Created("/user/" + usr.ID, usr.GetUserDetails());
        }

        [AllowAnonymous]
        [HttpPost("/authenticate")]
        public async Task<ObjectResult> Authenticate([FromBody]UserAnthenticationParams user)
        {
            var currentUser = await _context.User.FirstOrDefaultAsync(u => u.Username == user.Username);

            if(currentUser == null)
                return BadRequest(new {error = "User doesn't exist"});

            if (BCrypt.Net.BCrypt.Verify(user.Password, currentUser.Password) == false)
                return Unauthorized(new {error = "Password is not valid"});

            var tokenResponse = new UserAuthenticationResponse()
            {
                token = _jwt.GenerateToken(currentUser)
            };

            return Ok(tokenResponse);
        }

        [Authorize]
        [HttpGet("/me")]
        public async Task<ObjectResult> GetMe()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);

            if(string.IsNullOrEmpty(username))
                return BadRequest(new {error = "Please authenticate first."});

            var currentUser = _context.User.First(u => u.Username == username);

            var userDetails = new UserDetailsResponse
            {
                username = currentUser.Username,
                role = (int)currentUser.Role
            };

            return Ok(userDetails);
        }

        [Authorize]
        [HttpDelete("/me")]
        public async Task<ObjectResult> DeleteMe()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);

            if (string.IsNullOrEmpty(username))
                return BadRequest(new { error = "Please authenticate first." });

            var currentUser = _context.User.First(u => u.Username == username);

            _context.Remove(currentUser);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }

    }
}