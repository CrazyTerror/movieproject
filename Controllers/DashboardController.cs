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
            var user = _userManager.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
            var filmItems = _context.UserWatching.Include(f => f.FilmItem).Where(u => u.ApplicationUserId == user.Id).ToList();

            var seriesWatched = filmItems.Where(f => f.FilmItem.Discriminator == "Episode").GroupBy(x => x.FilmItem.Rel_SeriesId).Select(x => new { Count = x.Count() });
            var userViewings = GetUserWatched(filmItems);

            DashboardIndexViewModel dashboardIndexViewModel = new DashboardIndexViewModel
            {
                User = user,
                EpisodesWatched = userViewings.EpisodesWatched,
                EpisodeTimeWatched = CalculateInDays(userViewings.EpisodeTimeWatched),
                SeriesWatched = seriesWatched.Count(),
                MoviesWatched = userViewings.MoviesWatched,
                MovieTimeWatched = CalculateInDays(userViewings.MovieTimeWatched),
                RecentlyWatchedFilmItems = RecentlyWatchedFilmItems(user),
                UserViewingByDate = GetLast30Days(filmItems)
            };
            
            return View(dashboardIndexViewModel);
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
            } else if (minutes != 0)
            {
                return string.Format("{0} mins watching", minutes);
            } else {
                return string.Format("");
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

        public UserViewings GetUserWatched(List<UserWatchedFilmItemOn> filmItems)
        {
            int? movieTimeWatched = 0;
            int? episodeTimeWatched = 0;
            UserViewings userViewings = new UserViewings();

            foreach (var filmItemWatched in filmItems)
            {
                if (filmItemWatched.FilmItem.Discriminator == "Movie") {
                    userViewings.MoviesWatched++;
                    movieTimeWatched += filmItemWatched.FilmItem.Runtime;
                } else if (filmItemWatched.FilmItem.Discriminator == "Episode") {
                    userViewings.EpisodesWatched++;
                    episodeTimeWatched += filmItemWatched.FilmItem.Runtime;
                } 
            }
            userViewings.MovieTimeWatched = movieTimeWatched;
            userViewings.EpisodeTimeWatched = episodeTimeWatched;

            return userViewings;
        }

        public List<UserViewingByDate> GetLast30Days(List<UserWatchedFilmItemOn> filmItems)
        {
            List<DateTime> thirtyDaysDateList = new List<DateTime>();
            for (DateTime d = DateTime.Now.AddDays(-29); d <= DateTime.Now; d = d.AddDays(1))
            {
                thirtyDaysDateList.Add(d.Date);
            }

            List<UserViewingByDate> userViewingsList = new List<UserViewingByDate>();
            foreach (var date in thirtyDaysDateList)
            {
                UserViewingByDate userViewingByDate = new UserViewingByDate();
                userViewingByDate.Date = date;

                var userViewing = GetUserWatchedLast30Days(filmItems, userViewingByDate);
                userViewingsList.Add(userViewing);
            }

            return userViewingsList;
        }

        public UserViewingByDate GetUserWatchedLast30Days(List<UserWatchedFilmItemOn> filmItems, UserViewingByDate userViewingByDate)
        {
            int? timeWatched = 0;
            var orderedFilmItems = filmItems.Where(f => f.FilmItem.Discriminator == "Movie" || f.FilmItem.Discriminator == "Episode").OrderByDescending(f => f.WatchedOn);

            foreach (var filmItemWatched in orderedFilmItems)
            {
                if (userViewingByDate.Date == filmItemWatched.WatchedOn.Date)
                {
                    timeWatched += filmItemWatched.FilmItem.Runtime;
                    if (filmItemWatched.FilmItem.Discriminator == "Episode")
                    {
                        userViewingByDate.AmountOfEpisodes++;
                    } else if (filmItemWatched.FilmItem.Discriminator == "Movie") {
                        userViewingByDate.AmountOfMovies++;
                    }
                }
            }
            userViewingByDate.TimeWatched = timeWatched;

            return userViewingByDate;
        }
    }
}
