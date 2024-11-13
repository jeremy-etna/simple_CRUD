using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quest_web.DAL;
using quest_web.DTO;
using quest_web.Models;
using quest_web.Utils;

namespace quest_web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AddressController : Controller
    {
        private readonly ILogger<AddressController> _logger;
        private readonly APIDbContext _context;
        private readonly JwtTokenUtil _jwt;

        public AddressController(ILogger<AddressController> logger, APIDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _logger = logger;
            _jwt = jwt;
        }

        [Authorize]
        [HttpPost("/address")]
        public async Task<ObjectResult> CreateAddress([FromBody] AddressCreationParams address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);

            var addedAddress = new Address
            {
                street = address.street,
                postalCode = address.postalCode,
                city = address.city,
                country = address.country,
                creationDate = DateTime.Now,
                updatedDate = DateTime.Now,
                User = currentUser.ID
            };

            _context.Address.Add((addedAddress));
            await _context.SaveChangesAsync();

            return Created("/address/" + addedAddress.id, addedAddress);
        }

        [Authorize]
        [HttpPost("/user/{id:int}/address")]
        public async Task<ObjectResult> CreateAddressForUser(int id, [FromBody] AddressCreationParams address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);

            if (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                               currentUser.ID != id)
                return Unauthorized(new { error = "You don't have permission to perform this request."});

            var addedAddress = new Address
            {
                street = address.street,
                postalCode = address.postalCode,
                city = address.city,
                country = address.country,
                creationDate = DateTime.Now,
                updatedDate = DateTime.Now,
                User = id
            };

            _context.Address.Add((addedAddress));
            await _context.SaveChangesAsync();

            return Ok(addedAddress);
        }

        [Authorize]
        [HttpGet("/address")]
        public async Task<ObjectResult> GetAddress()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);

            if (currentUser.Role == UserRole.ROLE_ADMIN)
                return Ok(_context.Address.ToList());

            var addressesList = _context.Address.ToList();
            var userAddresses = addressesList.Where(r => r.User == currentUser.ID);
            return Ok(userAddresses);
        }

        [Authorize]
        [HttpGet("/address/{id:int}")]
        public async Task<ObjectResult> GetAddressById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);

            var currentAddress = await _context.Address.FirstOrDefaultAsync(u => u.id == id);

            if(currentAddress == null)
                return BadRequest(new { error = "Address ID doesn't exist: " + id });

            if (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                currentUser.ID != currentAddress.User)
                return StatusCode(403, new { error = "You don't have permission to perform this request." });

            return Ok(currentAddress);
        }

        [Authorize]
        [HttpPut("/address/{id:int}")]
        public async Task<ObjectResult> ChangeAddress(int id, [FromBody] AddressUpdateParams address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);
            var currentAddress = await _context.Address.FirstOrDefaultAsync(a => a.id == id);

            if (currentAddress == null ||
                (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                 currentUser.ID != currentAddress.User))
                return StatusCode(403, new { error = "You don't have permission to perform this request."});

            currentAddress.street = address.street ?? currentAddress.street;
            currentAddress.city = address.city ?? currentAddress.city;
            currentAddress.country = address.country ?? currentAddress.country;
            currentAddress.postalCode = address.postalCode ?? currentAddress.postalCode;
            currentAddress.updatedDate = DateTime.Now;

            _context.Update(currentAddress);
            await _context.SaveChangesAsync();
            return Ok(currentAddress);
        }

        [Authorize]
        [HttpDelete("/address/{id:int}")]
        public async Task<ObjectResult> DeleteAddressById(int id)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = _context.User.First(u => u.Username == username);

            var currentAddress = await _context.Address.FirstOrDefaultAsync(a => a.id == id);

            if (currentAddress == null)
                return BadRequest(new { error = "Address doesn't exist"});

            if (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                currentUser.ID != currentAddress.User)
                return StatusCode(403, new { error = "You don't have permission to perform this request."});

            _context.Remove(currentAddress);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Success" });
        }

    }
}