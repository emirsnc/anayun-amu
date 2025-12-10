using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MekanRehberi
{
    public static class DataManagement
    {
        // -----------------------------------------------------------
        // 1. BÖLÜM: MEKAN VERİLERİ (gezilecek_yerler.db - Sadece Okuma)
        // -----------------------------------------------------------
        public static List<Sehir> AllCities { get; set; } = new List<Sehir>();

        private static string placesDbName = "gezilecek_yerler.db";
        private static string placesConnString = $"Data Source={placesDbName};Version=3;";

        public static void LoadPlacesFromDatabase()
        {
            AllCities.Clear();

            if (!File.Exists(placesDbName))
            {
                MessageBox.Show("Mekan veritabanı (gezilecek_yerler.db) bulunamadı!");
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(placesConnString))
            {
                conn.Open();
                string sql = "SELECT * FROM Mekanlar";

                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string dbCityName = reader["CityName"].ToString();
                        Sehir mevcutSehir = AllCities.FirstOrDefault(x => x.Name == dbCityName);

                        if (mevcutSehir == null)
                        {
                            mevcutSehir = new Sehir()
                            {
                                Name = dbCityName,
                                Plaka = 0,
                                Description = dbCityName + " şehri.",
                                ImageFileURL = ""
                            };
                            AllCities.Add(mevcutSehir);
                        }

                        Mekan yeniMekan = new Mekan()
                        {
                            Id = Convert.ToInt32(reader["ID"]),
                            Name = reader["PlaceName"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString()
                        };

                        try
                        {
                            double dbRating = Convert.ToDouble(reader["Rating"]);
                            yeniMekan.VoteCount = 1;
                            yeniMekan.TotalScore = (int)dbRating;
                        }
                        catch
                        {
                            yeniMekan.VoteCount = 0;
                            yeniMekan.TotalScore = 0;
                        }

                        mevcutSehir.Mekanlar.Add(yeniMekan);
                    }
                }
            }
        }

        // -----------------------------------------------------------
        // 2. BÖLÜM: KULLANICI VERİLERİ (KullaniciVerileri.db - Yazma/Okuma)
        // -----------------------------------------------------------
        private static string userDbName = "KullaniciVerileri.db";
        private static string userConnString = $"Data Source={userDbName};Version=3;";

        public static void InitializeUserDatabase()
        {
            if (!File.Exists(userDbName))
                SQLiteConnection.CreateFile(userDbName);

            using (SQLiteConnection conn = new SQLiteConnection(userConnString))
            {
                conn.Open();

                // USERS TABLOSU
                string sqlUsers = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT,
                        Nickname TEXT UNIQUE,
                        Password TEXT,
                        UserStatus TEXT
                    );";

                new SQLiteCommand(sqlUsers, conn).ExecuteNonQuery();

                // USERRATINGS TABLOSU
                string sqlRatings = @"
                    CREATE TABLE IF NOT EXISTS UserRatings (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserNickname TEXT NOT NULL,
                        MekanId INTEGER NOT NULL,
                        Score INTEGER NOT NULL,
                        Comment TEXT,
                        UNIQUE(UserNickname, MekanId)
                    );";

                new SQLiteCommand(sqlRatings, conn).ExecuteNonQuery();
            }
        }

        public static string AddUserToDb(User user)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(userConnString))
                {
                    conn.Open();
                    string checkSql = "SELECT COUNT(*) FROM Users WHERE Nickname = @nick";

                    using (SQLiteCommand cmd = new SQLiteCommand(checkSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@nick", user.Nickname);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        if (count > 0) return "Bu kullanıcı adı zaten alınmış.";
                    }

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

        public static void InsertUserRating(string userNick, int mekanId, int score, string comment)
        {
            using (var conn = new SQLiteConnection(userConnString))
            {
                conn.Open();
                string query = "INSERT OR REPLACE INTO UserRatings (UserNickname, MekanId, Score, Comment) VALUES (@u, @m, @s, @c)";
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

        public static void UpdateMekanRating(Mekan mekan)
        {
            using (var conn = new SQLiteConnection(placesConnString))  // ✔ Düzeltildi
            {
                conn.Open();
                string sql = @"
                    UPDATE Mekanlar
                    SET Rating = @rating,
                        TotalScore = @totalScore,
                        VoteCount = @voteCount
                    WHERE ID = @id";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@rating", mekan.AverageScore);
                    cmd.Parameters.AddWithValue("@totalScore", mekan.TotalScore);
                    cmd.Parameters.AddWithValue("@voteCount", mekan.VoteCount);
                    cmd.Parameters.AddWithValue("@id", mekan.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 🔥 KULLANICI GİRİNCE KENDİ OYLARINI GERİ YÜKLER
        public static void LoadUserRatings(User user)
        {
            user.MyRatings.Clear();

            using (var conn = new SQLiteConnection(userConnString))
            {
                conn.Open();
                string query = "SELECT MekanId, Score, Comment FROM UserRatings WHERE UserNickname = @nick";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nick", user.Nickname);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int mekanId = reader.GetInt32(0);
                            int score = reader.GetInt32(1);
                            string comment = reader.IsDBNull(2) ? "" : reader.GetString(2);

                            Mekan mekan = AllCities.SelectMany(s => s.Mekanlar).FirstOrDefault(m => m.Id == mekanId);
                            if (mekan != null)
                                user.MyRatings.Add(new UserRatings(mekan, score, comment));
                        }
                    }
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
