using Microsoft.AspNetCore.Mvc;
using TesteJuntoSeguros.Security;
using TesteJuntoSeguros.Services;

namespace TesteJuntoSeguros.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UsuariosNegocio _userService;

        public AuthController(ITokenService tokenService, UsuariosNegocio userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (_userService.ValidarCredenciais(request.Email, request.Password))
            {
                var token = _tokenService.GenerateToken(request.Email);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (_userService.AlterarSenha(request.Email, request.CurrentPassword, request.NewPassword))
            {
                return Ok(new { Message = "Password changed successfully" });
            }

            return BadRequest(new { Message = "Error changing password" });
        }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
