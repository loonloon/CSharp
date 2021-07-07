using System.Threading.Tasks;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Models;

namespace HelloWorldWebApi.Services
{
    public interface ICharacterSkillService
    {
        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
    }
}
