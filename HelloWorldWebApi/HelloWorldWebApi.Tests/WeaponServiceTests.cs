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
    public class WeaponServiceTests
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
        public async Task AddWeapon_AddsWeaponToCharacter()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("AddWeaponTest")
                .Options;

            using var context = new DataContext(options);
            var user = new User { Id = 1, Username = "test", Role = "Player" };
            var character = new Character { Id = 1, Name = "Hero", User = user };
            context.Users.Add(user);
            context.Characters.Add(character);
            await context.SaveChangesAsync();

            var service = new WeaponService(context, CreateContextAccessor(user.Id), CreateMapper());

            var dto = new AddWeaponDto { CharacterId = character.Id, Name = "Sword", Damage = 20 };
            var response = await service.AddWeapon(dto);

            Assert.True(response.Success);
            Assert.Single(context.Weapons);
            Assert.Equal("Sword", context.Weapons.First().Name);
            Assert.Equal("Sword", response.Data.Weapon.Name);
        }
    }
}
