using System;
using System.Collections.Generic;
using System.Data.SQLite; // NuGet paketini yüklediğinden emin ol
using System.IO;
using System.Linq; 
using System.Windows.Forms; 

namespace MekanRehberi
{
    public static class DataManagement
    {
        // -----------------------------------------------------------
        // 1. BÖLÜM: MEKAN VERİLERİ (Gezilecek Yerler DB'den Okuma)
        // -----------------------------------------------------------
        
        // Tüm şehirler ve içindeki mekanlar bu listede tutulacak
        public static List<Sehir> AllCities { get; set; } = new List<Sehir>();

        // Mekan veritabanı dosya adı
        private static string placesDbName = "gezilecek_yerler.db";
        private static string placesConnString = $"Data Source={placesDbName};Version=3;";

        public static void LoadPlacesFromDatabase()
        {
            AllCities.Clear(); // Listeyi temizle

            // Dosya kontrolü
            if (!File.Exists(placesDbName))
            {
                MessageBox.Show("Mekan veritabanı (gezilecek_yerler.db) bulunamadı! Lütfen dosyayı bin/Debug klasörüne attığından emin ol.");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(placesConnString))
            {
                try
                {
                    conn.Open();
                    
                    // !!! DİKKAT: Veritabanındaki tablo adı 'Places' ise burayı değiştir
                    string sql = "SELECT * FROM Mekanlar"; 

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // --- A) Şehir İşlemleri ---
                            // Veritabanından Şehir Adını alıyoruz (Sütun adı: CityName)
                            string dbCityName = reader["CityName"].ToString();

                            // Bu şehir listemizde zaten var mı?
                            Sehir mevcutSehir = AllCities.FirstOrDefault(x => x.Name == dbCityName);

                            if (mevcutSehir == null)
                            {
                                // Yoksa yeni şehir oluştur
                                mevcutSehir = new Sehir();
                                mevcutSehir.Name = dbCityName;
                                mevcutSehir.Plaka = 0; // DB'de plaka yoksa 0
                                mevcutSehir.Description = dbCityName + " şehri.";
                                mevcutSehir.ImageFileURL = ""; 
                                AllCities.Add(mevcutSehir);
                            }

                            // --- B) Mekan İşlemleri ---
                            Mekan yeniMekan = new Mekan();

                            // Senin yeni Mekan Class yapına göre eşleştirme:
                            yeniMekan.Id = Convert.ToInt32(reader["ID"]);
                            yeniMekan.Name = reader["PlaceName"].ToString();
                            
                            // Description ve Type veritabanında varsa çek, yoksa varsayılan ata
                            try { yeniMekan.Description = reader["Description"].ToString(); } catch { yeniMekan.Description = "Açıklama yok."; }
                            try { yeniMekan.Type = reader["Type"].ToString(); } catch { yeniMekan.Type = "Genel"; }
                            
                            yeniMekan.ImageUrl = reader["ImageUrl"].ToString();

                            // --- C) Puanlama Dönüşümü ---
                            // Veritabanında muhtemelen 'Rating' (örn: 4.5) diye bir sütun var.
                            // Senin Class'ın ise TotalScore ve VoteCount kullanıyor.
                            // Bunu dönüştürmemiz lazım:
                            try 
                            {
                                double dbRating = Convert.ToDouble(reader["Rating"]); 
                                // Ortalamanın veritabanındaki gibi görünmesi için hile yapıyoruz:
                                yeniMekan.VoteCount = 1; 
                                yeniMekan.TotalScore = (int)dbRating; 
                            }
                            catch 
                            {
                                // Eğer Rating sütunu yoksa puanı 0 yap
                                yeniMekan.VoteCount = 0;
                                yeniMekan.TotalScore = 0;
                            }

                            // Mekanı şehre ekle
                            mevcutSehir.Mekanlar.Add(yeniMekan);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Mekanlar yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        // -----------------------------------------------------------
        // 2. BÖLÜM: KULLANICI VERİLERİ (KullaniciVerileri.db - Yazma/Okuma)
        // -----------------------------------------------------------
        private static string userDbName = "KullaniciVerileri.db";
        private static string userConnString = $"Data Source={userDbName};Version=3;";

        // Program açılışında çağrılacak: Veritabanı yoksa oluşturur
        public static void InitializeUserDatabase()
        {
            if (!File.Exists(userDbName))
            {
                SQLiteConnection.CreateFile(userDbName);
            }

            using (SQLiteConnection conn = new SQLiteConnection(userConnString))
            {
                conn.Open();
                // Users tablosunu oluştur
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Nickname TEXT UNIQUE,
                        Password TEXT,
                        UserStatus TEXT
                    )";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Kullanıcı Kayıt (Register)
        public static string AddUserToDb(User user)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(userConnString))
                {
                    conn.Open();
                    
                    // Kullanıcı adı kontrolü
                    string checkSql = "SELECT COUNT(*) FROM Users WHERE Nickname = @nick";
                    using (SQLiteCommand cmd = new SQLiteCommand(checkSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nick", user.Nickname);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0) return "Bu kullanıcı adı zaten alınmış.";
                    }

                    // Ekleme
                    string insertSql = "INSERT INTO Users (Name, Nickname, Password, UserStatus) VALUES (@name, @nick, @pass, @status)";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", user.Name);
                        cmd.Parameters.AddWithValue("@nick", user.Nickname);
                        cmd.Parameters.AddWithValue("@pass", user.Password);
                        cmd.Parameters.AddWithValue("@status", user.UserStatus);
                        cmd.ExecuteNonQuery();
                    }
                }
                return "Kayıt başarılı";
            }
            catch (Exception ex)
            {
                return "Veritabanı Hatası: " + ex.Message;
            }
        }
        public static void UpdateMekanRating(Mekan mekan)
{
    using (var conn = new SQLiteConnection("Data Source=Mekanlar.db"))
    {
        conn.Open();
        using (var cmd = new SQLiteCommand(conn))
        {
            cmd.CommandText = @"
                UPDATE MekanTable
                SET Rating = @rating,
                    FavNumber = @fav,
                    TotalScore = @totalScore,
                    VoteCount = @voteCount
                WHERE ID = @id";

            cmd.Parameters.AddWithValue("@rating", mekan.AverageScore);
            cmd.Parameters.AddWithValue("@fav", mekan.FavoriteCount);
            cmd.Parameters.AddWithValue("@totalScore", mekan.TotalScore);
            cmd.Parameters.AddWithValue("@voteCount", mekan.VoteCount);
            cmd.Parameters.AddWithValue("@id", mekan.Id);
            cmd.ExecuteNonQuery();
        }
    }
}

       public static void InsertUserRating(string userNick, int mekanId, int score, string comment)
{
    using (var conn = new SQLiteConnection("Data Source=UserDB.db"))
    {
        conn.Open();
        string query = "INSERT INTO UserRatings (UserNickname, MekanId, Score, Comment) VALUES (@u, @m, @s, @c)";

        using (var cmd = new SQLiteCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@u", userNick);
            cmd.Parameters.AddWithValue("@m", mekanId);
            cmd.Parameters.AddWithValue("@s", score);
            cmd.Parameters.AddWithValue("@c", comment);
            cmd.ExecuteNonQuery();
        }
    }
}

        public static User GetUserFromDb(string nickname, string password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(userConnString))
            {
                conn.Open();
                string sql = "SELECT * FROM Users WHERE Nickname = @nick AND Password = @pass";
                
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@nick", nickname);
                    cmd.Parameters.AddWithValue("@pass", password);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User u = new User();
                            u.Name = reader["Name"].ToString();
                            u.Nickname = reader["Nickname"].ToString();
                            u.Password = reader["Password"].ToString();
                            u.UserStatus = reader["UserStatus"].ToString();
                            return u;
                        }
                    }
                }
            }
            return null;
        }
    }
}
