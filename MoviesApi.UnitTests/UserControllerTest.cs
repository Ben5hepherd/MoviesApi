using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoviesApi.Domain;
using MoviesApi.EntityFramework;
using MoviesApi.WebApi.Controllers;
using MoviesApi.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace MoviesApi.UnitTests
{
    public class MoviesControllerTest
    {
        MoviesController controller;
        readonly DataContext dataContext;
        Seeder seeder;

        public MoviesControllerTest()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var options = new DbContextOptionsBuilder<DataContext>().UseSqlServer(configuration.GetConnectionString("MoviesConnection")).Options;
            dataContext = new DataContext(options);
            seeder = new Seeder();
            seeder.SeedData(dataContext);
            controller = new MoviesController(dataContext);
        }

        #region Filtering movies
        [Fact]
        public void FilterByYearOfRelease_FilterMovies()
        {
            const int yearOfRelease = 2000;
            var model = new FilterMoviesModel { YearOfRelease = yearOfRelease, Title = "", Genres = new List<string>() };

            var okObjectResult = controller.FilterMovies(model) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Equal(2, results.Count);
            CheckOrderOfResults(results, seeder.Movie1, seeder.Movie2);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void FilterByGenres_FilterMovies()
        {
            var genres = new List<string> { "genre1", "genre2" };
            var model = new FilterMoviesModel { YearOfRelease = null, Title = "", Genres = genres };

            var okObjectResult = controller.FilterMovies(model) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Equal(5, results.Count);
            CheckOrderOfResults(results, seeder.Movie1, seeder.Movie3, seeder.Movie2, seeder.Movie6, seeder.Movie7);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void FilterByTitle_FilterMovies()
        {
            const string title = "movie1";
            var model = new FilterMoviesModel { YearOfRelease = null, Title = title, Genres = new List<string>() };

            var okObjectResult = controller.FilterMovies(model) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Single(results);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void FilterByPartialTitle_FilterMovies()
        {
            const string partialTitle = "ie1";
            var model = new FilterMoviesModel { YearOfRelease = null, Title = partialTitle, Genres = new List<string>() };

            var okObjectResult = controller.FilterMovies(model) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Single(results);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void FilterEverything_FilterMovies()
        {
            const string partialTitle = "mov";
            var genres = new List<string> { "genre1" };
            const int yearOfRelease = 2000;
            var model = new FilterMoviesModel { YearOfRelease = yearOfRelease, Title = partialTitle, Genres = genres };

            var okObjectResult = controller.FilterMovies(model) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Single(results);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void FilterError_FilterMovies()
        {
            var model = new FilterMoviesModel { YearOfRelease = null, Title = "", Genres = new List<string>() };

            var badRequestObject = controller.FilterMovies(model) as BadRequestResult;
            Assert.NotNull(badRequestObject);
            Assert.Equal(400, badRequestObject.StatusCode);
        }

        [Fact]
        public void NoMovieFound_FilterMovies()
        {
            var model = new FilterMoviesModel { YearOfRelease = null, Title = "NotAnExistingMovie", Genres = new List<string>() };

            var notFoundObject = controller.FilterMovies(model) as NotFoundResult;
            Assert.NotNull(notFoundObject);
            Assert.Equal(404, notFoundObject.StatusCode);
        }
        #endregion

        #region Getting top five movies by average rating
        [Fact]
        public void GetTopFiveMoviesByAverageRating()
        {
            var okObjectResult = controller.GetTopFiveMoviesByAverageRating() as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Equal(5, results.Count);
            CheckOrderOfResults(results, seeder.Movie1, seeder.Movie3, seeder.Movie4, seeder.Movie2, seeder.Movie5);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void NoMovieFound_GetTopFiveMoviesByAverageRating()
        {
            var movies = dataContext.Movies;
            foreach (var movie in movies)
            {
                dataContext.Movies.Remove(movie);
            }
            dataContext.SaveChanges();

            var notFoundObject = controller.GetTopFiveMoviesByAverageRating() as NotFoundResult;
            Assert.NotNull(notFoundObject);
            Assert.Equal(404, notFoundObject.StatusCode);
        }
        #endregion

        #region Getting top five movies by user id
        [Fact]
        public void GetTopFiveMoviesByUserId()
        {
            var okObjectResult = controller.GetTopFiveMoviesByUserId(seeder.User1.Id) as OkObjectResult;
            Assert.Equal(200, okObjectResult.StatusCode);
            var results = okObjectResult.Value as List<MovieModel>;

            Assert.Equal(5, results.Count);
            CheckOrderOfResults(results, seeder.Movie1, seeder.Movie2, seeder.Movie3, seeder.Movie4, seeder.Movie5);
            CheckMovieModelProperties(results);
        }

        [Fact]
        public void UserIdInvalid_GetTopFiveMoviesByUserId()
        {
            var badRequestObject = controller.GetTopFiveMoviesByUserId(-1) as BadRequestResult;
            Assert.NotNull(badRequestObject);
            Assert.Equal(400, badRequestObject.StatusCode);
        }

        [Fact]
        public void NoMovieFound_GetTopFiveMoviesByUserId()
        {
            var notFoundObject = controller.GetTopFiveMoviesByUserId(99) as NotFoundResult;
            Assert.NotNull(notFoundObject);
            Assert.Equal(404, notFoundObject.StatusCode);
        }
        #endregion

        #region Adding a movie rating
        [Fact]
        public void UpdatingAnExistingMovieRating_AddMovieRating()
        {
            var user = seeder.User1;
            var movie = seeder.Movie1;
            var updatedRating = 4;
            var model = new AddMovieRatingModel { MovieId = movie.Id, UserId = user.Id, Rating = updatedRating };

            var okResult = controller.AddMovieRating(model) as OkResult;
            Assert.Equal(200, okResult.StatusCode);

            var updatedMovieRating = GetMovieRatingByUserAndMovie(user.Id, movie.Id);
            Assert.Equal(updatedRating, updatedMovieRating?.Rating);
        }

        [Fact]
        public void AddingANewMovieRating_AddMovieRating()
        {
            var user = seeder.User3;
            var movie = seeder.Movie8;
            var newRating = 4;

            Assert.Null(GetMovieRatingByUserAndMovie(user.Id, movie.Id));

            var model = new AddMovieRatingModel { MovieId = movie.Id, UserId = user.Id, Rating = newRating };

            var okResult = controller.AddMovieRating(model) as OkResult;
            Assert.Equal(200, okResult.StatusCode);

            var newMovingRating = GetMovieRatingByUserAndMovie(user.Id, movie.Id);
            Assert.Equal(newRating, newMovingRating?.Rating);
        }

        [Fact]
        public void InvalidRating_AddMovieRating()
        {
            var user = seeder.User3;
            var movie = seeder.Movie8;
            var invalidRating = 6;

            var model = new AddMovieRatingModel { MovieId = movie.Id, UserId = user.Id, Rating = invalidRating };

            var badRequest = controller.AddMovieRating(model) as BadRequestResult;
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public void UserNotFound_AddMovieRating()
        {
            var movie = seeder.Movie8;
            var rating = 5;

            var model = new AddMovieRatingModel { MovieId = movie.Id, UserId = -1, Rating = rating };

            var notFoundResult = controller.AddMovieRating(model) as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void MovieNotFound_AddMovieRating()
        {
            var user = seeder.User3;
            var rating = 5;

            var model = new AddMovieRatingModel { MovieId = -1, UserId = user.Id, Rating = rating };

            var notFoundResult = controller.AddMovieRating(model) as NotFoundResult;
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        #endregion

        MovieRating GetMovieRatingByUserAndMovie(int userId, int movieId)
        {
            return dataContext.MovieRatings.SingleOrDefault(mr => mr.Movie.Id == movieId && mr.User.Id == userId);
        }

        void CheckOrderOfResults(List<MovieModel> results, Movie firstMovie, Movie secondMovie, Movie thirdMovie = null, Movie fourthMovie = null, Movie fifthMovie = null)
        {
            Assert.Equal(firstMovie.Id, results[0].Id);
            Assert.Equal(secondMovie.Id, results[1].Id);
            if(thirdMovie != null)
                Assert.Equal(thirdMovie.Id, results[2].Id);
            if (fourthMovie != null)
                Assert.Equal(fourthMovie.Id, results[3].Id);
            if (fifthMovie != null)
                Assert.Equal(fifthMovie.Id, results[4].Id);
        }

        void CheckMovieModelProperties(List<MovieModel> results)
        {
            foreach (var result in results)
            {
                var expectedMovie = seeder.StoredMovies.Single(m => m.Id == result.Id);
                Assert.Equal(expectedMovie.Id, result.Id);
                Assert.Equal(expectedMovie.Title, result.Title);
                Assert.Equal(expectedMovie.YearOfRelease, result.YearOfRelease);
                Assert.Equal(expectedMovie.RunningTime, result.RunningTime);
                Assert.Equal(RoundAverageRating(expectedMovie.AverageMovieRating), result.AverageRating);
            }
        }

        double RoundAverageRating(double averageRating)
        {
            return Math.Round(averageRating * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}