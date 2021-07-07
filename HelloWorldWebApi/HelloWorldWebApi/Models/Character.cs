using System.Collections.Generic;

namespace HelloWorldWebApi.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        public CharacterClass Class { get; set; }
        public User User { get; set; }
        public Weapon Weapon { get; set; }
        public List<CharacterSkill> CharacterSkills { get; set; }
    }
}
