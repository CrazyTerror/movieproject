using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Data;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class UserController : Controller
    {
        private readonly MovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(MovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admins")]
        public ViewResult Index()
        {
            var users = _userManager.Users.ToList();

            return View(users);
        }

        [HttpGet("users/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var ratingDistribution = _context.UserRatings.Where(u => u.ApplicationUserId == user.Id).GroupBy(ur => ur.Rating).Select(grp => new RatingDistribution { Value = grp.Key, Count = grp.Count()}).ToList();
            var recentlyWatchedMovies = _context.UserWatching.Include(f => f.FilmItem).Where(f => f.FilmItem.Discriminator == "Movie").OrderByDescending(uw => uw.WatchedOn).Take(5).ToList();
            var recentlyWatchedEpisodes = _context.UserWatching.Include(f => f.FilmItem).Where(f => f.FilmItem.Discriminator == "Episode").OrderByDescending(uw => uw.WatchedOn).Take(5).ToList();
            var userFavoriteMovies = _context.UserRatings.Include(f => f.FilmItem).Where(u => u.ApplicationUserId == user.Id).Where(f => f.FilmItem.Discriminator == "Movie").OrderByDescending(r => r.Rating).Take(5).ToList();
            var userFavoriteSeries = _context.UserRatings.Include(f => f.FilmItem).Where(u => u.ApplicationUserId == user.Id).Where(f => f.FilmItem.Discriminator == "Series").OrderByDescending(r => r.Rating).Take(5).ToList();
            var lastWatchedItem = _context.UserWatching.Include(f => f.FilmItem).OrderByDescending(uw => uw.WatchedOn).First();
            //var mostWatchedMovie = _context.UserWatching.Include(f => f.FilmItem).Where(u => u.ApplicationUserId == user.Id).Where(f => f.FilmItem.Discriminator == "Movie").GroupBy(uw => uw.FilmItemId).Select(grp => new MostWatchedMovie { Movie = grp.}).First();

            UserInfoViewModel ivm = new UserInfoViewModel
            {
                User = user,
                RatingDistribution = LoadFullGraph(ratingDistribution),
                UserLatestSeenMovies = recentlyWatchedMovies,
                UserLatestSeenEpisodes = recentlyWatchedEpisodes,
                UserFavoriteMovies = userFavoriteMovies,
                UserFavoriteSeries = userFavoriteSeries,
                LastWatchedFilmItem = lastWatchedItem
            };

            if (user == null)
            {
                return View("Index", "Home");
            }

            return View(ivm);
        }

        [HttpGet("users/{slug}/history")]
        public ViewResult History(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var filmItemsWatched = _context.UserWatching.Include(f => f.FilmItem).OrderByDescending(x => x.WatchedOn).ToList();

            ViewBag.User = user.UserName;

            return View(filmItemsWatched);
        }

        [HttpGet("users/{slug}/ratings")]
        public ViewResult Ratings(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var ratings = _context.UserRatings.Include(u => u.FilmItem).Where(u => u.ApplicationUserId == user.Id).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.User = user.UserName;

            return View(ratings);
        }

        [HttpGet("users/{slug}/comments")]
        public ViewResult Comments(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(u => u.ApplicationUserId == user.Id).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.User = user.UserName;

            return View(comments);
        }

        [HttpPost("addRating")]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating()
        {
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            var rating = int.Parse(Request.Form["Rating"]);
            var user = _userManager.GetUserId(User);
            
            UserRating userRating = new UserRating
            {
                ApplicationUserId = user,
                FilmItem = filmItem,
                Rating = rating
            };
            _context.UserRatings.Add(userRating);
            
            AlterFilmItemAverage(filmItem, rating);
            _context.SaveChanges();

            return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug});
        }

        [HttpPost("deleteRating")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRating()
        {
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            var userRating = _context.UserRatings.Include(f => f.FilmItem).Where(m => m.FilmItem == filmItem).Where(u => u.ApplicationUserId == _userManager.GetUserId(User)).FirstOrDefault();

            if (userRating != null)
            {
                _context.UserRatings.Remove(userRating);
                CalculateFilmItemAverageAfterDeletion(filmItem, userRating.Rating);
                _context.SaveChanges();
                TempData["message"] = $"Your rating on {userRating.FilmItem.Name} was deleted";
            }

            return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug});
        }

        public void AlterFilmItemAverage(FilmItem filmItem, int rating)
        {
            _context.Attach(filmItem);
            if (filmItem.VoteCount == null && filmItem.VoteAverage == null)
            {
                filmItem.VoteCount = 1;
                filmItem.VoteAverage = rating;
            } else {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount++;
                filmItem.VoteAverage = (totalRating + rating) / filmItem.VoteCount;
            }
            
            filmItem.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }

        public void CalculateFilmItemAverageAfterDeletion(FilmItem filmItem, int rating)
        {
            _context.Attach(filmItem);

            if (filmItem.VoteCount == 1)
            {
                filmItem.VoteCount = null;
                filmItem.VoteAverage = null;
            } else 
            {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount--;
                filmItem.VoteAverage = (totalRating - rating) / filmItem.VoteCount;
            }
            filmItem.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }

        [HttpPost("historyModal")]
        public IActionResult HistoryModal()
        {
            var applicationUser = _userManager.GetUserId(User);
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));

            UserWatchedFilmItemOn userWatchedOn = new UserWatchedFilmItemOn
            {
                ApplicationUserId = applicationUser,
                FilmItem = filmItem,
                WatchedOn = DateTime.Parse(Request.Form["WatchedOn"])
            };
            _context.UserWatching.Add(userWatchedOn);
            _context.SaveChanges();
            
            if (filmItem.Discriminator == "Season") {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug, SeasonNumber = filmItem.Season_SeasonNumber });
            } else if (filmItem.Discriminator == "Episode") {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug, SeasonNumber = filmItem.Episode_SeasonNumber, EpisodeNumber = filmItem.Episode_EpisodeNumber });
            } else {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug });
            }
        }

        public List<RatingDistribution> LoadFullGraph(List<RatingDistribution> distribution)
        {
            List<int> hasRatings = new List<int>();
            foreach (var rating in distribution)
            {
                hasRatings.Add(rating.Value);
            }
            for (int i = 1; i <= 10; i++)
            {
                if (!hasRatings.Contains(i))
                {
                    distribution.Add(new RatingDistribution {Value = i, Count = 0});
                }
            }

            return distribution.OrderBy(x => x.Value).ToList();
        }
    }
}
