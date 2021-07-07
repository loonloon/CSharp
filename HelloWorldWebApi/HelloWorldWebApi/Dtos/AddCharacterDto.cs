using HelloWorldWebApi.Models;

namespace HelloWorldWebApi.Dtos
{
    public class AddCharacterDto
    {
        public string Name { get; set; }
        public double HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        public CharacterClass Class { get; set; }
    }
}
