using System.Collections.Generic;
using System.Linq;

namespace MekanRehberi
{
    public static class FilterManager
    {
        public static List<Mekan> FilterByCategory(List<Mekan> venueList, string category)
        {

            if (category == "Tümü" || string.IsNullOrEmpty(category))
            {
                return venueList;
            }
            
            return venueList
                .Where(mekan => mekan.Type == category)
                .ToList();
        }

        
        public static List<string> GetUniqueCategories(List<Mekan> venueList)
        {
            List<string> categories = new List<string>();
            categories.Add("Tümü");

            foreach (var mekan in venueList)
            {
                if (!categories.Contains(mekan.Type))
                {
                    categories.Add(mekan.Type);
                }
            }
            return categories;
        }
    }
}