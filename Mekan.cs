namespace MekanRehberi
{
    public class Mekan
    {
        public int Id { get; set; } // ID yerine Id yaptım
        public string Name { get; set; }
        public string Description { get; set; } // VERİTABANI İÇİN GERİ EKLENDİ
        public string Type { get; set; }        // VERİTABANI İÇİN GERİ EKLENDİ
        public string ImageUrl { get; set; }    // ImageURL yerine ImageUrl
        
        // Puanlama Sistemi
        public int TotalScore { get; set; } = 0;
        public int VoteCount { get; set; } = 0;
        public int FavoriteCount { get; set; } = 0;

        // Otomatik hesaplanan ortalama (Veritabanına yazılmaz, hesaplanır)
        public double AverageScore
        {
            get
            {
                if (VoteCount == 0) return 0;
                return (double)TotalScore / VoteCount;
            }
        }

        public Mekan() { }

        // Puan Ekleme Metodu
        public void AddPoint(int score)
        {
            TotalScore += score;
            VoteCount++;
        }

        // Favori Güncelleme
        public void ChangeFavorite(bool isAdding)
        {
            if (isAdding) FavoriteCount++;
            else if (FavoriteCount > 0) FavoriteCount--;
        }
    }
}
