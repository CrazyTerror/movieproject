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
    [Authorize(Roles = "Admins, Users")]
    public class DashboardController : Controller
    {
        private readonly MovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(MovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("dashboard")]
        public ViewResult Index()
        {
            var episodesWatched = 0;
            int? episodeTimeWatched = 0;
            var moviesWatched = 0;
            int? moviesTimeWatched = 0;

            var user = _userManager.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
            var filmItems = _context.UserWatching.Include(f => f.FilmItem).Where(u => u.ApplicationUserId == user.Id).ToList();

            foreach (var filmItemWatched in filmItems)
            {
                if (filmItemWatched.FilmItem.Discriminator == "Movie") {
                    moviesWatched++;
                    moviesTimeWatched += filmItemWatched.FilmItem.Runtime;
                } else if (filmItemWatched.FilmItem.Discriminator == "Episode") {
                    episodesWatched++;
                    episodeTimeWatched += filmItemWatched.FilmItem.Runtime;
                } 
            }

            var seriesWatched = filmItems.Where(f => f.FilmItem.Discriminator == "Episode").GroupBy(x => x.FilmItem.Rel_SeriesId).Select(x => new { Count = x.Count() });

            DashboardIndexViewModel divm = new DashboardIndexViewModel
            {
                User = user,
                EpisodesWatched = episodesWatched,
                EpisodeTimeWatched = CalculateInDays(episodeTimeWatched),
                SeriesWatched = seriesWatched.Count(),
                MoviesWatched = moviesWatched,
                MovieTimeWatched = CalculateInDays(moviesTimeWatched),
                RecentlyWatchedFilmItems = RecentlyWatchedFilmItems(user)
            };
            
            return View(divm);
        }

        public string CalculateInDays(int? totalTime)
        {
            int? days = totalTime / 1440;
            int? hours = (totalTime % 1440)/60;
            int? minutes = totalTime % 60;

            if (days != 0)
            {
                return string.Format("{0} days, {1} hours, {2} mins watching", days, hours, minutes);
            } else if (days == 0 && hours != 0)
            {
                return string.Format("{0} hours, {1} mins watching", hours, minutes);
            } else 
            {
                return string.Format("{0} mins watching", minutes);
            }
        }

        public List<UserWatchedFilmItemOn> RecentlyWatchedFilmItems(ApplicationUser user)
        {
            var recentlyWatchedFilmItems = _context.UserWatching.Include(f => f.FilmItem)
                                                                .OrderByDescending(uw => uw.WatchedOn)
                                                                .Take(5)
                                                                .ToList();
            
            return recentlyWatchedFilmItems;
        }
    }
}
