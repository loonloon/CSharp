using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HelloWorldWebApi;
using HelloWorldWebApi.Data;
using HelloWorldWebApi.Dtos;
using HelloWorldWebApi.Models;
using HelloWorldWebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HelloWorldWebApi.Tests
{
    public class CharacterServiceTests
    {
        private static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg => { cfg.AddProfile<AutoMapperProfile>(); });
            return config.CreateMapper();
        }

        private static IHttpContextAccessor CreateContextAccessor(int userId)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Player")
            }));

            return new HttpContextAccessor { HttpContext = httpContext };
        }

        [Fact]
        public async Task AddCharacter_AddsCharacterForCurrentUser()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("AddCharacterTest")
                .Options;

            using var context = new DataContext(options);
            var user = new User { Id = 1, Username = "test", Role = "Player" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new CharacterService(CreateMapper(), context, CreateContextAccessor(user.Id));

            var newChar = new AddCharacterDto
            {
                Name = "Hero",
                HitPoints = 100,
                Strength = 10,
                Defense = 10,
                Intelligence = 10,
                Class = CharacterClass.Knight
            };

            var response = await service.AddCharacter(newChar);

            Assert.True(response.Success);
            Assert.Single(context.Characters);
            Assert.Equal("Hero", context.Characters.First().Name);
        }
    }
}
