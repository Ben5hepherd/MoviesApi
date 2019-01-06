namespace MoviesApi.WebApi.Models
{
    public class AddMovieRatingModel
    {
        public int MovieId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
    }
}