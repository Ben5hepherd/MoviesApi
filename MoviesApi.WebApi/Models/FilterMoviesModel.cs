using System.Collections.Generic;

namespace MoviesApi.WebApi.Models
{
    public class FilterMoviesModel
    {
        public List<string> Genres { get; set; }
        public string Title { get; set; }
        public int? YearOfRelease { get; set; }
    }
}