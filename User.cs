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
        // Mekan puanını güncelle
        mekan.TotalScore -= existingRating.Score;
        mekan.TotalScore += score;

        existingRating.Score = score;
        existingRating.Comment = comment;

        DataManagement.UpdateUserRating(this.Nickname, mekan.Id, score, comment);
    }
    else
    {
        mekan.AddPoint(score);
        var newRating = new UserRatings(mekan, score, comment);
        MyRatings.Add(newRating);

        DataManagement.InsertUserRating(this.Nickname, mekan.Id, score, comment);
    }

    // Mekan tablosunu da güncelle
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
