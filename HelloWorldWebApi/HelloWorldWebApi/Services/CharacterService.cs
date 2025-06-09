using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var characters = GetUserRole() == "Admin"
                ? await _dataContext.Characters.ToListAsync()
                : await _dataContext.Characters.Where(x => x.User.Id == GetUserId()).ToListAsync();

            return new()
            {
                Data = _mapper.Map<List<GetCharacterDto>>(characters)
            };
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            return new()
            {
                Data = _mapper.Map<GetCharacterDto>(await _dataContext.Characters.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == GetUserId()))
            };
        }

        public async Task<ServiceResponse<List<AddCharacterDto>>> AddCharacter(AddCharacterDto character)
        {
            var serviceResponse = new ServiceResponse<List<AddCharacterDto>>();

            try
            {
                var newCharacter = _mapper.Map<Character>(character);
                newCharacter.User = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == GetUserId());

                await _dataContext.Characters.AddAsync(newCharacter);
                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<List<AddCharacterDto>>(_dataContext.Characters
                    .Where(x => x.User.Id == GetUserId())
                    .Select(x => _mapper.Map<AddCharacterDto>(x)));
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto character)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                var found = await _dataContext.Characters
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == character.Id);

                if (found == null || found.User.Id != GetUserId())
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                    return serviceResponse;
                }

                found.Class = character.Class;
                found.Defense = character.Defense;
                found.HitPoints = character.HitPoints;
                found.Intelligence = character.Intelligence;
                found.Name = character.Name;
                found.Strength = character.Strength;

                _dataContext.Characters.Update(found);
                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(found);
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var found = await _dataContext.Characters.FirstOrDefaultAsync(x => x.Id == id && x.User.Id == GetUserId());

                if (found == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                    return serviceResponse;
                }

                _dataContext.Characters.Remove(found);
                await _dataContext.SaveChangesAsync();

                serviceResponse.Data = _dataContext.Characters.Where(x => x.User.Id == GetUserId())
                    .Select(x => _mapper.Map<GetCharacterDto>(x)).ToList();
            }
            catch (Exception e)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        private int GetUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out var userId) ? userId : 0;
        }
        private string GetUserRole() => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}
