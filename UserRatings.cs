using System;

namespace MekanRehberi
{
    public class UserRatings
    {
        public Mekan RatedMekan { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public UserRatings() { }

        public UserRatings(Mekan ratedMekan, int score, string comment)
        {
            RatedMekan = ratedMekan;
            Score = score;
            Comment = comment;
            Date = DateTime.Now;
        }
    }
}