using System.Collections.Generic;

namespace MekanRehberi
{
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string UserStatus { get; set; }

        public List<Mekan> Favorites { get; set; } = new List<Mekan>();
        public List<UserRatings> MyRatings { get; set; } = new List<UserRatings>();

        public User() { }

        public User(string name, string password, string nickname, string userStatus)
        {
            Name = name;
            Password = password;
            Nickname = nickname;
            UserStatus = userStatus;
        }

        public void RateMekan(Mekan mekan, int score, string comment)
        {
            if (score < 1) score = 1;
            if (score > 5) score = 5;

            var existingRating = MyRatings.Find(r => r.Mekan.Id == mekan.Id);

            if (existingRating != null)
            {
                // -- GÜNCELLEME SENARYOSU --
                // 1. Mekan nesnesinin puanını güncelle (önce eskisini çıkar, yeniyi ekle)
                mekan.TotalScore -= existingRating.Score;
                mekan.TotalScore += score;

                // 2. Kullanıcının RAM'deki listesini güncelle
                existingRating.Score = score;
                existingRating.Comment = comment;
            }
            else
            {
                // -- YENİ EKLEME SENARYOSU --
                // 1. Mekan nesnesine puan ekle
                mekan.AddPoint(score);
                
                // 2. Kullanıcının RAM'deki listesine ekle
                var newRating = new UserRatings(mekan, score, comment);
                MyRatings.Add(newRating);
            }

            // 3. VERİTABANI GÜNCELLEMELERİ
            // UserRatings tablosuna yaz (INSERT OR REPLACE kullandığımız için tek metod yeterli)
            DataManagement.InsertUserRating(this.Nickname, mekan.Id, score, comment);

            // Mekanlar tablosunda ortalamayı güncelle
            DataManagement.UpdateMekanRating(mekan);
        }

        public bool ToggleFavorite(Mekan mekan)
        {
            Mekan foundMekan = Favorites.Find(m => m.Id == mekan.Id);

            if (foundMekan != null)
            {
                Favorites.Remove(foundMekan);
                mekan.ChangeFavorite(false);
                return false;
            }
            else
            {
                Favorites.Add(mekan);
                mekan.ChangeFavorite(true);
                return true;
            }
        }

        public bool IsFavorite(Mekan mekan)
        {
            return Favorites.Exists(m => m.Id == mekan.Id);
        }
    }
}
