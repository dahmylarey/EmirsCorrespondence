using DreyCorrespondence.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmirsCorrespondence.Areas.Admin.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {

        private readonly JwtTokenService _tokenService;

        public AuthController(JwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Validate user credentials (replace with your DB logic)
            if (model.Username == "admin" && model.Password == "password")
            {
                var token = _tokenService.GenerateToken(model.Username, "Admin");
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}
