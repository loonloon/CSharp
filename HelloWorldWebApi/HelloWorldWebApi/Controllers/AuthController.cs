using System.Threading.Tasks;
using HelloWorldWebApi.Data;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var response = await _authRepository.Register(new User
            {
                Username = request.Username,
            }, request.Password);

            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var response = await _authRepository.Login(request.Username, request.Password);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
