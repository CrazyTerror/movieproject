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
            var seasons = _context.Seasons.Where(s => s.SeriesId == series.Id).ToList();

            ViewBag.Series = series;

            return View(seasons);
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


            ViewBag.Genres = series.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray();
            ViewBag.Series = series.Name;
            ViewBag.SeasonCount = series.Series_SeasonCount;
            ViewBag.TotalRuntime = FilmItemMethods.CalculateSeasonTotalRuntime(season);
            ViewBag.CommentCount = season.Reviews.Count;
            ViewBag.ListCount = season.ListItems.Select(l => l.List).ToList().Count;

            var firstEpisode = season.Episodes.OrderBy(e => e.ReleaseDate).First().ReleaseDate;
            if (firstEpisode.HasValue)
            {
                ViewBag.FirstEpisode = firstEpisode.Value.ToString("d MMMM yyyy");
            } else
            {
                ViewBag.FirstEpisode = "";
            }

            return View(season);
        }

        [HttpGet("series/{Slug}/seasons/create")]
        public ViewResult Create(string Slug)
        {
            ViewBag.Series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            return View();
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

            ViewBag.Series = series;

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
            ViewBag.FilmItem = season;
            ViewBag.ReleaseYear = season.ReleaseDate.Value.ToString("yyyy");

            return View(comments);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/media")]
        public ViewResult Media(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Where(m => m.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var media = _context.Media.Include(r => r.FilmItem).Where(r => r.FilmItem == season).FirstOrDefault();

            return View(media);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/lists")]
        public ViewResult Lists(string Slug, int SeasonNumber)
        {
            var season = _context.Seasons.Include(f => f.ListItems).ThenInclude(li => li.List).Where(s => s.Slug == Slug).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();

            ViewBag.FilmItem = season;
            ViewBag.ReleaseYear = season.ReleaseDate.Value.ToString("yyyy");

            return View(season);
        }
    }
}
