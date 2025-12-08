using System;
using System.Collections.Generic;

namespace MekanRehberi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- MEKAN REHBERİ MANTIK TESTİ BAŞLIYOR ---\n");

            try
            {
                // 1. ADIM: Veri Yükleme (Mock Data)
                // DataManagement sınıfındaki verileri yüklüyoruz.
                DataManagement.LoadMockData();
                Console.WriteLine($"[OK] Veriler Yüklendi.");
                Console.WriteLine($" -> Toplam Şehir: {DataManagement.AllCities.Count}");
                Console.WriteLine($" -> Admin Kullanıcısı: {UserManager.AllUsers[0].Nickname}\n");


                // 2. ADIM: Kayıt Olma (Register)
                Console.WriteLine("--- Kayıt Testi ---");
                User yeniUye = new User("Ahmet Yılmaz", "12345", "ahmet35", "standart");
                string kayitSonuc = UserManager.Register(yeniUye);
                Console.WriteLine($"[TEST] Yeni üye kaydı: {kayitSonuc}");

                // Aynı kullanıcı adıyla tekrar kayıt denemesi
                User kopyaUye = new User("Mehmet", "0000", "ahmet35", "standart");
                string kopyaSonuc = UserManager.Register(kopyaUye);
                Console.WriteLine($"[TEST] Aynı isimle kayıt denemesi: {kopyaSonuc}\n");


                // 3. ADIM: Giriş Yapma (Login)
                Console.WriteLine("--- Giriş Testi ---");
                User girisYapan = UserManager.Login("ahmet35", "12345");
                
                if (girisYapan != null && UserManager.CurrentUser == girisYapan)
                {
                    Console.WriteLine($"[OK] Giriş Başarılı! Hoşgeldin: {girisYapan.Name}");
                }
                else
                {
                    Console.WriteLine("[HATA] Giriş Yapılamadı!");
                }
                Console.WriteLine();


                // 4. ADIM: Mekan İşlemleri (Puanlama & Favori)
                Console.WriteLine("--- Mekan İşlemleri Testi ---");
                
                // İzmir şehrini ve içindeki ilk mekanı alalım (MockData'dan geliyor)
                Sehir izmir = DataManagement.AllCities.Find(x => x.Name == "İzmir");
                
                if (izmir != null && izmir.Mekanlar.Count > 0)
                {
                    Mekan kordon = izmir.Mekanlar[0];
                    Console.WriteLine($"Seçilen Mekan: {kordon.Name} (Eski Puan: {kordon.Rating})");

                    // Kullanıcı mekana puan versin (Eğer giriş yapıldıysa)
                    if (UserManager.CurrentUser != null)
                    {
                        UserManager.CurrentUser.RateMekan(kordon, 4, "Güzel manzara.");
                        Console.WriteLine($"[OK] Puan Verildi (4). Yeni Puan Ortalaması: {kordon.Rating}");
                        Console.WriteLine($"[OK] Yorum Eklendi: {UserManager.CurrentUser.MyRatings[0].Comment}");

                        // Favoriye ekleme
                        bool favDurumu = UserManager.CurrentUser.ToggleFavorite(kordon);
                        Console.WriteLine($"[OK] Favori Durumu: {(favDurumu ? "Eklendi" : "Çıkarıldı")}");
                    }
                }
                else
                {
                    Console.WriteLine("[HATA] İzmir şehri veya mekanı bulunamadı.");
                }
                Console.WriteLine();


                // 5. ADIM: Arama Motoru (SearchEngine)
                Console.WriteLine("--- Arama Testi ---");
                var aramaSonuc = SearchEngine.SearchCities(DataManagement.AllCities, "izm");
                Console.WriteLine($"[TEST] 'izm' araması sonucu: {aramaSonuc.Count} şehir bulundu.");
                if(aramaSonuc.Count > 0)
                    Console.WriteLine($" -> Bulunan: {aramaSonuc[0].Name}");
                Console.WriteLine();


                // 6. ADIM: Filtreleme (FilterManager)
                Console.WriteLine("--- Filtreleme Testi ---");
                if (izmir != null)
                {
                    // 'Gezi' tipindeki mekanları getir
                    var filtrelenmisMekanlar = FilterManager.FilterByCategory(izmir.Mekanlar, "Gezi");
                    Console.WriteLine($"[TEST] 'Gezi' kategorisi filtreleme: {filtrelenmisMekanlar.Count} mekan bulundu.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[KRİTİK HATA] Test sırasında bir hata oluştu: {ex.Message}");
            }

            Console.WriteLine("\n--- TEST SONU ---");
            Console.ReadLine(); // Konsol hemen kapanmasın diye bekletiyoruz
        }
    }
}
