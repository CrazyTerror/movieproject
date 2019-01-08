using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieProject.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class SeasonController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeasonController(MovieContext context, IHostingEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet("series/{Slug}/seasons")]
        [AllowAnonymous]
        public ViewResult Index(string Slug)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);

            SeasonIndexViewModel seasonIndexViewModel = new SeasonIndexViewModel
            {
                Series = series,
                Seasons = _context.Seasons.Where(s => s.SeriesId == series.Id).ToList()
            };

            return View(seasonIndexViewModel);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}")]
        [AllowAnonymous]
        public ViewResult Details(string Slug, int SeasonNumber)
        {
            var series = _context.Series.Include(fig => fig.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.Include(ep => ep.Episodes)
                                         .Include(mr => mr.UserRatings)
                                         .Include(r => r.Reviews)
                                         .Include(m => m.Media)
                                         .Include(v => v.Videos)
                                         .Include(p => p.Photos)
                                         .Include(l => l.ListItems).ThenInclude(l => l.List)
                                         .Where(x => x.Season_SeasonNumber == SeasonNumber)
                                         .Where(y => y.SeriesId == series.Id)
                                         .FirstOrDefault();

            SeasonDetailsViewModel seasonDetailsViewModel = new SeasonDetailsViewModel
            {
                Season = season,
                Genres = series.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray(),
                SeasonCount = series.Series_SeasonCount,
                TotalRuntime = FilmItemMethods.CalculateSeasonTotalRuntime(season),
                CommentCount = season.Reviews.Count,
                ListCount = season.ListItems.Select(l => l.List).ToList().Count,
                FirstEpisodeDate = GetDateTimeFirstEpisode(season)
            };

            return View(seasonDetailsViewModel);
        }

        [HttpGet("series/{Slug}/seasons/create")]
        public ViewResult Create(string Slug)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            return View(series);
        }

        [HttpPost("series/{Slug}/seasons/create")]
        public IActionResult Create(string Slug, Season season)
        {
            var series = _context.Series.Include(s => s.Seasons).FirstOrDefault(x => x.Slug == Slug);

            var updatedSeries = FilmItemMethods.SaveSeriesInfoAfterCreateSeason(_context, series);

            // Add New Season
            season.SeriesId = series.Id;
            season.Season_SeasonNumber = updatedSeries.Series_SeasonCount;
            season.Slug = Slug;
            season.Rel_SeriesId = series.Id;
            season.Rel_SeriesName = series.Name;
            _context.Seasons.Add(season);
            FilmItemMethods.AddMediaEntry(_context, season);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files; 
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }

            return RedirectToAction("Details", "Series", new { Slug = Slug});
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/edit")]
        public ViewResult Edit(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();

            return View(season);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/edit")]
        public IActionResult Edit(EditSeasonInfoViewModel editSeasonViewModel, int SeasonNumber)
        {
            var season = _context.Seasons.FirstOrDefault(s => s.Id == editSeasonViewModel.Id);
            
            if (ModelState.IsValid)
            {
                _context.Seasons.Attach(season);
                editSeasonViewModel.MapToModel(season);
                _context.SaveChanges();

                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", season.Id);
                }

                TempData["message"] = $"{season.Name} has been changed";

                return RedirectToAction("Details", "Season", new { SeasonNumber = SeasonNumber });
            } 
            return View(season);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/delete")]
        [Authorize(Roles = "Admins")]
        public IActionResult Delete(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Include(e => e.Episodes).Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            
            if (season != null)
            {
                FilmItemMethods.EditSeriesInfoAfterDeleteSeason(_context, series, season);
                Images.DeleteImagesBelongingToSeason(_context, _env, season);

                _context.Seasons.Remove(season);
                _context.SaveChanges();
                
                TempData["message"] = $"{series.Name} - {season.Name} was deleted";
            } 
            
            return RedirectToAction("Details", "Series", new { Slug = Slug});
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/comments")]
        [AllowAnonymous]
        public ViewResult Comments(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Where(s => s.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(r => r.FilmItem == season).OrderByDescending(x => x.CreatedAt).ToList();

            FilmItemCommentsViewModel filmItemCommentsViewModel = new FilmItemCommentsViewModel
            {
                Comments = comments,
                FilmItem = season,
                ReleaseYear = season.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemCommentsViewModel);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/media")]
        public ViewResult Media(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Where(m => m.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var media = _context.Media.Include(r => r.FilmItem).Where(r => r.FilmItem == season).FirstOrDefault();

            return View(media);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/lists")]
        [AllowAnonymous]
        public ViewResult Lists(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Include(f => f.ListItems).ThenInclude(li => li.List).Where(s => s.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();

            FilmItemListsViewModel filmItemListsViewModel = new FilmItemListsViewModel
            {
                FilmItem = season,
                ReleaseYear = season.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemListsViewModel);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/listsModal")]
        public IActionResult ListsModal(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Where(s => s.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var lists = _context.Lists.Include(li => li.ListItems).ThenInclude(m => m.FilmItem).Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).ToList();

            FilmItemListsModalViewModel filmItemListsViewModel = new FilmItemListsModalViewModel
            {
                Lists = lists,
                FilmItem = season.Rel_SeriesName + " - " + season.Name,
                FilmItemId = season.Id,
                ListsWithFilmItem = FilmItemMethods.ListHavingFilmItem(lists, season)
            };

            return PartialView("_FilmItemListsModalPartial", filmItemListsViewModel);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/listsModal")]
        public IActionResult ListsModal(string Slug, int SeasonNumber, bool check = false)
        {
            var listsChecked = Request.Form["Lists"].ToList();
            var season = _context.Seasons.Where(s => s.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var lists = _context.Lists.Include(li => li.ListItems).Where(u => u.ApplicationUserId == _userManager.GetUserId(User)).ToList();

            foreach (var list in lists)
            {
                var itemInList = list.ListItems.Where(m => m.FilmItem == season).FirstOrDefault();
                if (itemInList != null && !listsChecked.Contains(list.Id.ToString())) //unchecked -> checked
                {
                    FilmItemMethods.RemoveListItem(_context, itemInList);
                } else if (itemInList == null && listsChecked.Contains(list.Id.ToString())) //checked -> unchecked
                {
                    FilmItemMethods.SaveListItem(_context, list, season);
                }
            }

            return RedirectToAction("Details", new { Slug = Slug });
        }

        public string GetDateTimeFirstEpisode(Season season)
        {
            var firstEpisode = season.Episodes.OrderBy(e => e.ReleaseDate).First().ReleaseDate;
            var episodeDateString = "";
            if (firstEpisode.HasValue)
            {
                episodeDateString = firstEpisode.Value.ToString("d MMMM yyyy");
            } else
            {
                episodeDateString = "";
            }

            return episodeDateString;
        } 
    }
}
