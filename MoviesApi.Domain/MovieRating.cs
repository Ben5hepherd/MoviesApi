namespace MoviesApi.Domain
{
    public class MovieRating
    {
        public virtual int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
        public virtual int Rating { get; set; }
    }
}