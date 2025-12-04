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

            mekan.AddPoint(score);

            UserRatings newRating = new UserRatings(mekan, score, comment);
            this.MyRatings.Add(newRating);
        }
    }
}