using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HelloWorldWebApi.Data;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldWebApi.Services
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public CharacterSkillService(DataContext dataContext, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                var character = await _dataContext.Characters
                    .Include(x => x.Weapon)
                    .Include(x => x.CharacterSkills)
                    .ThenInclude(x => x.Skill)
                    .FirstOrDefaultAsync(x => x.Id == newCharacterSkill.CharacterId && x.User.Id == GetUserId());

                if (character == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                    return serviceResponse;
                }

                var skill = await _dataContext.Skills.FirstOrDefaultAsync(x => x.Id == newCharacterSkill.SkillId);

                if (skill == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Skill not found";
                    return serviceResponse;
                }

                var characterSkill = new CharacterSkill
                {
                    Character = character,
                    Skill = skill
                };

                await _dataContext.CharacterSkills.AddAsync(characterSkill);
                await _dataContext.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
                return serviceResponse;
            }

            return serviceResponse;
        }

        private int GetUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }
    }
}
