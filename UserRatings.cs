namespace MekanRehberi
{
    public class UserRatings
    {
        public Mekan Mekan { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }

        public UserRatings() { }

        public UserRatings(Mekan mekan, int score, string comment)
        {
            Mekan = mekan;
            Score = score;
            Comment = comment;
        }
    }
}
