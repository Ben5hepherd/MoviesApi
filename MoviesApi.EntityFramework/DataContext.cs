using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain;

namespace MoviesApi.EntityFramework
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options){}

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieRating> MovieRatings { get; set; }
    }
}