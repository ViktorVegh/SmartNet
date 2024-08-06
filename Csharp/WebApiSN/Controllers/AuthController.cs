namespace WebApiSN.Controllers;
using IServices;
using Models.UserManagement;
using Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUser user)
        {
            var token = await _authService.RegisterUserAsync(user);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser user)
        {
            var token = await _authService.LoginUserAsync(user);
            if (token != null)
            {
                return Ok(token);
            }
            return Unauthorized("Invalid username or password");
        }
    }

