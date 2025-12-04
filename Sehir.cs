using System.Collections.Generic;

namespace MekanRehberi
{
    public class Sehir
    {
        public string Name { get; set; }
        public int Plaka { get; set; }
        public string Description { get; set; }
        public string ImageFileURL { get; set; }
        
        public List<Mekan> Mekanlar { get; set; } = new List<Mekan>();  
        
        public Sehir() { }

        public Sehir(string name, int plaka, string description, string imageFileURL)
        {
            Name = name;
            Plaka = plaka;
            Description = description;
            ImageFileURL = imageFileURL;
        }
    }
}