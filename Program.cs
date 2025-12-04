using System;
using System.Collections.Generic;
using MekanRehberi; 

namespace MekanRehberi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- MEKAN REHBERİ TEST BAŞLANGICI ---\n");

            
            Mekan testMekan = new Mekan("Mavi Cafe", "Deniz manzaralı", "img1.jpg", 0, "Cafe", "maps_link", 101);
            Sehir testSehir = new Sehir("İzmir", 35, "Ege'nin incisi", "izmir.jpg");
            testSehir.Mekanlar.Add(testMekan);

            Console.WriteLine($"[Sistem] Şehir: {testSehir.Name}, Mekan: {testMekan.Name} (Başlangıç Puanı: {testMekan.Rating}) oluşturuldu.");

          
            User yeniKullanici = new User("Ahmet Yılmaz", "12345", "ahmet_y", "Öğrenci");
            string kayitSonuc = UserManager.Register(yeniKullanici);
            
            Console.WriteLine($"\n[Kayıt Testi]: {kayitSonuc}");

           
            Console.WriteLine("\n[Login Testi - Hatalı Şifre]");
            User hataliGiris = UserManager.Login("ahmet_y", "yanlis_sifre");
            if (hataliGiris == null)
            {
                Console.WriteLine("-> Başarılı: Sistem yanlış şifreyi kabul etmedi.");
            }
            else
            {
                Console.WriteLine("-> HATA: Sistem yanlış şifreye izin verdi!");
            }

          
            Console.WriteLine("\n[Login Testi - Doğru Bilgiler]");
            User girisYapan = UserManager.Login("ahmet_y", "12345");
            
            if (girisYapan != null && UserManager.CurrentUser == girisYapan)
            {
                Console.WriteLine($"-> Başarılı: {girisYapan.Name} giriş yaptı.");
            }
            else
            {
                Console.WriteLine("-> HATA: Giriş yapılamadı.");
                return; 
            }

            
            Console.WriteLine("\n[Puanlama Testi]");
            
            UserManager.CurrentUser.RateMekan(testMekan, 5, "Harika bir yer, kahveleri çok taze.");
            
            Console.WriteLine($"Verilen Puan: 5");
            Console.WriteLine($"Mekanın Yeni Ortalaması: {testMekan.Rating}");
            Console.WriteLine($"Mekanın Oy Sayısı: {testMekan.RatingNumber}");
            Console.WriteLine($"Kullanıcının Yorumu: {UserManager.CurrentUser.MyRatings[0].Comment}");

            if (testMekan.Rating == 5)
                Console.WriteLine("-> Sonuç: Puanlama sistemi DOĞRU çalışıyor.");
            else
                Console.WriteLine("-> Sonuç: Puanlama sistemi HATALI.");
            
            Console.WriteLine("\n[Favori Testi]");
            UserManager.CurrentUser.Favorites.Add(testMekan);
            testMekan.ChangeFavorite(true);

            Console.WriteLine($"Kullanıcının Favori Sayısı: {UserManager.CurrentUser.Favorites.Count}");
            Console.WriteLine($"Mekanın Favorilenme Sayısı: {testMekan.FavoriteNumber}");

            Console.WriteLine("\n--- TEST BİTTİ ---");
            Console.ReadLine();
        }
    }
}