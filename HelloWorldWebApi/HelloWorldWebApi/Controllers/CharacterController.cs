using System.Threading.Tasks;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldWebApi.Controllers
{
    [Authorize(Roles = "Player,Admin")]
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _characterService.GetAllCharacters());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            return Ok(await _characterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto character)
        {
            return Ok(await _characterService.AddCharacter(character));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto character)
        {
            var serviceResponse = await _characterService.UpdateCharacter(character);
            return serviceResponse.Success ? Ok(serviceResponse) : NotFound(serviceResponse);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var serviceResponse = await _characterService.DeleteCharacter(id);
            return serviceResponse.Success ? Ok(serviceResponse) : NotFound(serviceResponse);
        }
    }
}
