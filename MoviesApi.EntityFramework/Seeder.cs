
using MoviesApi.Domain;
using System.Collections.Generic;

namespace MoviesApi.EntityFramework
{
    public class Seeder
    {
        public User User1;
        public User User2;
        public User User3;
        public Movie Movie1;
        public Movie Movie2;
        public Movie Movie3;
        public Movie Movie4;
        public Movie Movie5;
        public Movie Movie6;
        public Movie Movie7;
        public Movie Movie8;
        public List<Movie> StoredMovies;

        public Seeder()
        {
            StoredMovies = new List<Movie>();
        }

        public void SeedData(DataContext dataContext)
        {
            foreach (var movieRating in dataContext.MovieRatings)
            {
                dataContext.MovieRatings.Remove(movieRating);
            }

            foreach (var user in dataContext.Users)
            {
                dataContext.Users.Remove(user);
            }

            foreach (var movie in dataContext.Movies)
            {
                dataContext.Movies.Remove(movie);
            }

            User1 = new User { Name = "User1" };
            User2 = new User { Name = "User2" };
            User3 = new User { Name = "User3" };

            dataContext.Users.AddRange(new List<User> { User1, User2, User3 });
            dataContext.SaveChanges();

            Movie1 = new Movie { Title = "Movie1", YearOfRelease = 2000, RunningTime = 100, Genre = "Genre1" };
            Movie2 = new Movie { Title = "Movie2", YearOfRelease = 2000, RunningTime = 100, Genre = "Genre2" };
            Movie3 = new Movie { Title = "Movie3", YearOfRelease = 2001, RunningTime = 101, Genre = "Genre2" };
            Movie4 = new Movie { Title = "Movie4", YearOfRelease = 2001, RunningTime = 101, Genre = "Genre3" };
            Movie5 = new Movie { Title = "Movie5", YearOfRelease = 2002, RunningTime = 120, Genre = "Genre3" };
            Movie6 = new Movie { Title = "Movie6", YearOfRelease = 2002, RunningTime = 121, Genre = "Genre2" };
            Movie7 = new Movie { Title = "Movie7", YearOfRelease = 2002, RunningTime = 103, Genre = "Genre1" };
            Movie8 = new Movie { Title = "Movie8", YearOfRelease = 2003, RunningTime = 107, Genre = "Genre4" };

            dataContext.Movies.AddRange(new List<Movie> { Movie1, Movie2, Movie3, Movie4, Movie5, Movie6, Movie7, Movie8 });
            dataContext.SaveChanges();

            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie1, Rating = 5 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie2, Rating = 4 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie3, Rating = 4 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie4, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie5, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie6, Rating = 1 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie7, Rating = 2 });
            dataContext.MovieRatings.Add(new MovieRating { User = User1, Movie = Movie8, Rating = 2 });

            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie1, Rating = 4 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie2, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie3, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie4, Rating = 2 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie5, Rating = 2 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie6, Rating = 5 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie7, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User2, Movie = Movie8, Rating = 3 });
                                                                      
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie1, Rating = 2 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie2, Rating = 1 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie3, Rating = 4 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie4, Rating = 5 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie5, Rating = 3 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie6, Rating = 2 });
            dataContext.MovieRatings.Add(new MovieRating { User = User3, Movie = Movie7, Rating = 1 });

            dataContext.SaveChanges();

            StoredMovies.AddRange(new List<Movie> { Movie1, Movie2, Movie3, Movie4, Movie5, Movie6, Movie7, Movie8 });
        }
    }
}
