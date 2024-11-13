using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using quest_web.DAL;
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
        public async Task<ObjectResult> CreateAddress([FromBody]Address address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = await _context.User.FirstOrDefaultAsync(u => u.Username == username);

            var addedAddress = new Address
            {
                road = address.road ?? "",
                postalCode = address.postalCode ?? "",
                city = address.city ?? "",
                country = address.country ?? "",
                creationDate = DateTime.Now,
                updatedDate = DateTime.Now,
                User = currentUser.ID
            };

            if (!addedAddress.IsValid())
                return BadRequest("Invalid address");

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
            var currentUser = await _context.User.FirstOrDefaultAsync(u => u.Username == username);

            var addressesList = _context.Address.ToList();
            var userAddresses = addressesList.Where(r => r.User == currentUser.ID);
            return Ok(userAddresses);
        }

        [Authorize]
        [HttpGet("/address/{id:int}")]
        public async Task<ObjectResult> GetAddressById(int id)
        {
            var currentAddress = await _context.Address.FirstOrDefaultAsync(u => u.id == id);
            return Ok(currentAddress);
        }

        [Authorize]
        [HttpPut("/address/{id:int}")]
        public async Task<ObjectResult> ChangeAddress(int id, [FromBody] Address address)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var username = _jwt.GetUsernameFromToken(accessToken);
            var currentUser = await _context.User.FirstOrDefaultAsync(u => u.Username == username);
            var currentAddress = await _context.Address.FirstOrDefaultAsync(a => a.id == id);

            if (currentAddress == null ||
                (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                 currentUser.ID != currentAddress.User))
                return Unauthorized("You don't have permission to perform this request.");

            currentAddress.road = address.road ?? currentAddress.road;
            currentAddress.city = address.city ?? currentAddress.city;
            currentAddress.country = address.country ?? currentAddress.country;
            currentAddress.postalCode = address.postalCode ?? currentAddress.postalCode;

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
            var currentUser = await _context.User.FirstOrDefaultAsync(u => u.Username == username);

            var currentAddress = await _context.Address.FirstOrDefaultAsync(u => u.id == id);

            if (currentAddress == null)
                return BadRequest("Address doesn't exist");

            if (!currentUser.Role.Equals(UserRole.ROLE_ADMIN) &&
                currentUser.ID != currentAddress.User)
                return Unauthorized("You don't have permission to perform this request.");

            _context.Remove(currentAddress);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

    }
}