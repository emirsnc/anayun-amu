namespace MekanRehberi
{
    public class Mekan
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public double Rating { get; set; } = 0;
        public int RatingNumber { get; set; } = 0;
        public int FavoriteNumber { get; set; } = 0;
        public string Type { get; set; }
        public string MapsURL { get; set; }
        public int ID { get; set; }

        public Mekan() { }

        public Mekan(string name, string description, string imageURL, double rating, string type, string mapsURL, int id)
        {
            Name = name;
            Description = description;
            ImageURL = imageURL;
            Rating = rating;
            Type = type;
            MapsURL = mapsURL;
            ID = id;
        }

        public void AddPoint(int newScore)
        {
            double totalScore = this.Rating * this.RatingNumber;
            totalScore += newScore;
            this.RatingNumber++;
            this.Rating = totalScore / this.RatingNumber;
        }

        public void ChangeFavorite(bool isAdd)
        {
            if (isAdd)
            {
                this.FavoriteNumber++;
            }
            else
            {
                this.FavoriteNumber--;
                if (this.FavoriteNumber < 0)
                {
                    this.FavoriteNumber = 0;
                }
            }
        }
    }
}