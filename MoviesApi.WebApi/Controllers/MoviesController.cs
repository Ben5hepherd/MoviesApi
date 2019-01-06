using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Domain;
using MoviesApi.EntityFramework;
using MoviesApi.WebApi.Models;

namespace MoviesApi.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        DataContext dataContext;

        public MoviesController(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        [HttpGet("FilterMovies")]
        public IActionResult FilterMovies([FromBody] FilterMoviesModel model)
        {
            if((model.Genres == null || !model.Genres.Any()) && string.IsNullOrEmpty(model.Title) && !model.YearOfRelease.HasValue)
                return BadRequest();

            var moviesToReturn = dataContext.Movies.Include(m => m.MovieRatings).ToList();
            if (model.Genres != null && model.Genres.Any())
            {
                moviesToReturn = moviesToReturn.Where(movie => model.Genres.Any(genre => genre.ToLower() == movie.Genre.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(model.Title))
            {
                moviesToReturn = moviesToReturn.Where(movie => movie.Title.ToLower().Contains(model.Title.ToLower())).ToList();
            }
            if (model.YearOfRelease.HasValue)
            {
                moviesToReturn = moviesToReturn.Where(movie => movie.YearOfRelease == model.YearOfRelease).ToList();
            }

            moviesToReturn = moviesToReturn.OrderByDescending(m => m.AverageMovieRating).ThenBy(m => m.Title).ToList();

            if (!moviesToReturn.Any())
                return NotFound();

            return Ok(moviesToReturn.Select(m => new MovieModel
            {
                Id = m.Id,
                Title = m.Title,
                YearOfRelease = m.YearOfRelease,
                RunningTime = m.RunningTime,
                AverageRating = RoundAverageRating(dataContext.MovieRatings.Where(mr => mr.Movie.Id == m.Id).Average(mr => mr.Rating))
            }).ToList());
        }

        [HttpGet("GetTopFiveMoviesByAverageRating")]
        public IActionResult GetTopFiveMoviesByAverageRating()
        {
            var allMovies = dataContext.Movies.Include(m => m.MovieRatings).ToList();
            var topFiveMoviesByAverageRating = allMovies.OrderByDescending(m => m.AverageMovieRating).ThenBy(m => m.Title).Take(5).ToList();

            var moviesToReturn = new List<MovieModel>();

            topFiveMoviesByAverageRating.ForEach(m =>
            {
                moviesToReturn.Add(new MovieModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    YearOfRelease = m.YearOfRelease,
                    RunningTime = m.RunningTime,
                    AverageRating = RoundAverageRating(m.AverageMovieRating)
                });
            });

            if (!moviesToReturn.Any())
                return NotFound();

            return Ok(moviesToReturn.OrderByDescending(m => m.AverageRating).ThenBy(m => m.Title).ToList());
        }

        [HttpGet("GetTopFiveMoviesByUserId/{userId}")]
        public IActionResult GetTopFiveMoviesByUserId(int userId)
        {
            if (userId < 1)
                return BadRequest();

            var moviesToReturn = dataContext.MovieRatings
                .Where(mr => mr.User.Id == userId)
                .OrderByDescending(mr => mr.Rating).ThenBy(mr => mr.Movie.Title)
                .Take(5)
                .Select(mr => new MovieModel
                {
                    Id = mr.Movie.Id,
                    Title = mr.Movie.Title,
                    YearOfRelease = mr.Movie.YearOfRelease,
                    RunningTime = mr.Movie.RunningTime,
                    AverageRating = RoundAverageRating(dataContext.MovieRatings.Where(x => x.Movie.Id == mr.Movie.Id).Average(x => x.Rating))
                }).ToList();

            if (!moviesToReturn.Any())
                return NotFound();

            return Ok(moviesToReturn);
        }

        [HttpPost("AddMovieRating")]
        public IActionResult AddMovieRating([FromBody] AddMovieRatingModel model)
        {
            var isRatingWithinProperRange = model.Rating > 0 && model.Rating < 6;
            if (!isRatingWithinProperRange)
                return BadRequest();

            var existingRating = dataContext.MovieRatings.SingleOrDefault(mr => mr.User.Id == model.UserId && mr.Movie.Id == model.MovieId);
            if (existingRating != null)
            {
                existingRating.Rating = model.Rating;
                dataContext.SaveChanges();
            }
            else
            {
                var user = dataContext.Users.SingleOrDefault(u => u.Id == model.UserId);
                var movie = dataContext.Movies.SingleOrDefault(m => m.Id == model.MovieId);

                if (user == null || movie == null)
                    return NotFound();

                var newRating = new MovieRating { Movie = movie, User = user, Rating = model.Rating };
                dataContext.MovieRatings.Add(newRating);
                dataContext.SaveChanges();
            }
            return Ok();
        }

        double RoundAverageRating(double averageRating)
        {
            return Math.Round(averageRating * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}