using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAPI.Helpers;
using PaymentAPI.Infrastructure.Data;

namespace PaymentAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(ApplicationDbContext db, IConfiguration config)
        {            
            _config = config;
        }
        [HttpGet("GetToken")]
        public IActionResult GetToken()
        {            
            string  Token =Helper.GenerateJwtToken(_config);
            return Ok( Token);
        }
    }
}
