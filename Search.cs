using System.Collections.Generic;
using System.Linq;

namespace MekanRehberi
{
    public static class SearchEngine
    {
        
        public static List<Sehir> SearchCities(List<Sehir> sourceList, string searchText)
        {
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return sourceList;
            }

           
            return sourceList
                .Where(city => city.Name.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }

        
        public static List<Mekan> SearchVenues(List<Mekan> sourceList, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return sourceList;
            }

            return sourceList
                .Where(mekan => mekan.Name.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }
    }
}