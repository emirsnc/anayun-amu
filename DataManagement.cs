using System.Collections.Generic;

namespace MekanRehberi
{
    public static class DataManagement
    {
        // ÖRNEK VERİLER TEST İÇİN!
        public static List<Sehir> AllCities { get; set; } = new List<Sehir>();

       
        public static void LoadMockData()
        {
         
            User admin = new User("Yönetici", "1234", "admin", "admin");
            UserManager.AllUsers.Add(admin);
            
            Sehir izmir = new Sehir("İzmir", 35, "Ege'nin incisi", "izmir.jpg");
            
            Mekan kordon = new Mekan("Kordon Boyu", "Deniz kenarı.", "kordon.jpg", 0, "Gezi", "url", 1);
            kordon.AddPoint(5);
            kordon.ChangeFavorite(true);
            
            izmir.Mekanlar.Add(kordon);
            
            Sehir istanbul = new Sehir("İstanbul", 34, "Tarih başkenti", "istanbul.jpg");
            
            Mekan galata = new Mekan("Galata Kulesi", "Manzara.", "galata.jpg", 0, "Tarihi", "url", 2);
            galata.AddPoint(5);
            
            istanbul.Mekanlar.Add(galata);
            
            AllCities.Add(izmir);
            AllCities.Add(istanbul);
        }
    }
}