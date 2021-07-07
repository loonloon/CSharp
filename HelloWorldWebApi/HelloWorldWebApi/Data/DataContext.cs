using HelloWorldWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloWorldWebApi.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Character> Characters { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }

        public DataContext(DbContextOptions<DataContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterSkill>().HasKey(x => new { x.CharacterId, x.SkillId });
            modelBuilder.Entity<User>().Property(x => x.Role).HasDefaultValue("Player");
        }
    }
}
