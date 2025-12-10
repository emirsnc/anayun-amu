namespace MekanRehberi
{
    public class Mekan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        
        public int TotalScore { get; set; }
        public int VoteCount { get; set; }
        public int FavoriteCount { get; set; }

        public double AverageScore
        {
            get
            {
                if (VoteCount == 0) return 0;
                return (double)TotalScore / VoteCount;
            }
        }

        public Mekan() { }

        public void AddPoint(int score)
        {
            TotalScore += score;
            VoteCount++;
        }

        public void UpdatePoint(int oldScore, int newScore)
        {
            TotalScore = TotalScore - oldScore + newScore;
        }

        public void ChangeFavorite(bool isAdding)
        {
            if (isAdding)
                FavoriteCount++;
            else
            {
                if(FavoriteCount > 0) FavoriteCount--;
            }
        }
    }
}
