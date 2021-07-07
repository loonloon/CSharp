using System.Threading.Tasks;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Models;

namespace HelloWorldWebApi.Services
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}
