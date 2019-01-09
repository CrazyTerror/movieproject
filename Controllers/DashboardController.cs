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
                UserViewingByDate = GetLast30Days(filmItems),
                UpcomingFilmItems = UpcomingFilmItemsFromWatchlist(user)
            };

            UpcomingFilmItemsFromWatchlist(user);
            
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
            var recentlyWatchedFilmItems = _context.UserWatching.Where(uw => uw.ApplicationUserId == user.Id)
                                                                .Include(f => f.FilmItem)
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

        public List<UpcomingFilmItems> UpcomingFilmItemsFromWatchlist(ApplicationUser user)
        {
            var watchList = _context.Lists.Include(l => l.ListItems).ThenInclude(li => li.FilmItem).Where(l => l.ApplicationUserId == user.Id).Where(l => l.Name == "Watchlist").FirstOrDefault();
            
            var upcomingWatchListItems = watchList.ListItems.Where(li => li.FilmItem.Discriminator == "Movie" || li.FilmItem.Discriminator == "Episode")
                                                            .Where(l => l.FilmItem.ReleaseDate.Value.Date >= DateTime.Now.Date)
                                                            .Where(l => l.FilmItem.ReleaseDate.Value.Date < DateTime.Now.AddDays(7));

            List<FilmItem> upcomingFilmItems = new List<FilmItem>();
            foreach (var listItem in upcomingWatchListItems)
            {
                upcomingFilmItems.Add(listItem.FilmItem);
            }

            var seasonSeriesInWishlist = watchList.ListItems.Where(li => li.FilmItem.Discriminator == "Season" || li.FilmItem.Discriminator == "Series").ToList();
            if (seasonSeriesInWishlist.Count > 0)
            {
                foreach (var listItem in seasonSeriesInWishlist)
                {
                    List<Episode> episodes = new List<Episode>(); 
                    if (listItem.FilmItem.Discriminator == "Season")
                    {
                        episodes = _context.Episodes.Where(e => e.Episode_SeasonNumber == listItem.FilmItem.Season_SeasonNumber).Where(e => e.Rel_SeriesId == listItem.FilmItem.Rel_SeriesId).ToList();
                    } else if (listItem.FilmItem.Discriminator == "Series")
                    {
                        episodes = _context.Episodes.Where(e => e.Rel_SeriesId == listItem.FilmItemId).ToList();
                    }

                    foreach (var episode in episodes)
                    {
                        if ((episode.ReleaseDate.Value.Date >= DateTime.Now && episode.ReleaseDate.Value.Date < DateTime.Now.AddDays(7)) && !upcomingFilmItems.Contains(episode))
                        {
                            upcomingFilmItems.Add(episode);
                        }
                    }
                }
            }

            var upcomingFilmItemsByDate = UpcomingFilmItemsByDate(upcomingFilmItems);

            return upcomingFilmItemsByDate;
        }

        public List<UpcomingFilmItems> UpcomingFilmItemsByDate(List<FilmItem> upcomingWatchListItems)
        {
            List<UpcomingFilmItems> upcomingFilmItemList = new List<UpcomingFilmItems>();
            for (DateTime d = DateTime.Now; d < DateTime.Now.AddDays(7); d = d.AddDays(1))
            {
                UpcomingFilmItems upcomingFilmItems = new UpcomingFilmItems();
                upcomingFilmItems.Date = d.Date;
                upcomingFilmItems.FilmItems = new List<FilmItem>();

                foreach (var item in upcomingWatchListItems)
                {
                    if (item.ReleaseDate.Value.Date == d.Date)
                    {
                        upcomingFilmItems.FilmItems.Add(item);
                    }
                }

                upcomingFilmItemList.Add(upcomingFilmItems);
            }

            return upcomingFilmItemList;
        }
    }
}
