using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MoviesApi.Domain
{
    public class Movie
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual int YearOfRelease { get; set; }
        public virtual int RunningTime { get; set; }
        public virtual string Genre { get; set; }
        public virtual ICollection<MovieRating> MovieRatings { get; set; }

        [NotMapped]
        public double AverageMovieRating => MovieRatings.Average(mr => mr.Rating);
    }
}