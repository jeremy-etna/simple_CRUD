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
    public class PostsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly APIDbContext _context;
        private readonly JwtTokenUtil _jwt;

        public PostsController(ILogger<PostsController> logger, APIDbContext context, JwtTokenUtil jwt)
        {
            _context = context;
            _logger = logger;
            _jwt = jwt;
        }
    }
}