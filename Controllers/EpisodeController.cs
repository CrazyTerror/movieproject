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
    public class EpisodeController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public EpisodeController(MovieContext context, IHostingEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes")]
        [AllowAnonymous]
        public ViewResult Index(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.FirstOrDefault(s => s.Season_SeasonNumber == SeasonNumber);
            var episodes = _context.Episodes.Where(e => e.SeasonId == season.Id).ToList();

            ViewBag.Series = series.Name;
            ViewBag.Season = season.Name;

            return View(episodes);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}")]
        [AllowAnonymous]
        public ViewResult Details(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var series = _context.Series.Include(fig => fig.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.Where(srs => srs.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.Include(mr => mr.UserRatings)
                                           .Include(r => r.Reviews)
                                           .Include(m => m.Media)
                                           .Include(v => v.Videos)
                                           .Include(p => p.Photos)
                                           .Where(s => s.SeasonId == season.Id)
                                           .Where(ep => ep.Episode_EpisodeNumber == EpisodeNumber)
                                           .FirstOrDefault();

            if (episode.Episode_EpisodeNumber < 10)
            {
                ViewBag.Episode = "0" + episode.Episode_EpisodeNumber;
            } else
            {
                ViewBag.Episode = episode.Episode_EpisodeNumber;
            }
            ViewBag.Genres = series.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray();
            ViewBag.Series = series.Name;
            ViewBag.Season = season.Name;
            ViewBag.EpisodeCount = season.Season_EpisodeCount;
            ViewBag.CommentCount = episode.Reviews.Count;

            return View(episode);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/create")]
        public IActionResult Create(string Slug, int SeasonNumber, Episode episode)
        {
            var series = _context.Series.Include(s => s.Seasons).ThenInclude(s => s.Episodes).FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault(); 
            
            var updatedSeries = FilmItemMethods.SaveSeriesInfoAfterCreateEpisode(_context, series);
            var updatedSeason = FilmItemMethods.SaveSeasonInfoAfterCreateEpisode(_context, season);

            // Add new Episode
            episode.SeasonId = season.Id;
            episode.Episode_SeasonNumber = updatedSeason.Season_SeasonNumber;
            episode.Episode_EpisodeNumber = updatedSeason.Season_EpisodeCount;
            episode.Slug = Slug;
            episode.Rel_SeriesId = series.Id;
            episode.Rel_SeriesName = series.Name;
            _context.Episodes.Add(episode);
            FilmItemMethods.AddMediaEntry(_context, episode);

            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files; 
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }

            return RedirectToAction("Details", "Season", new { SeasonNumber = SeasonNumber});
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/edit")]
        public ViewResult Edit(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.Where(e => e.SeasonId == season.Id).Where(e => e.Episode_EpisodeNumber == EpisodeNumber).FirstOrDefault();

            if (episode.Episode_EpisodeNumber < 10)
            {
                ViewBag.Episode = "0" + episode.Episode_EpisodeNumber;
            } else
            {
                ViewBag.Episode = episode.Episode_EpisodeNumber;
            }

            ViewBag.Series = series.Name;
            ViewBag.Season = season.Name;
            ViewBag.EpisodeCount = season.Season_EpisodeCount;

            return View(episode);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/edit")]
        public IActionResult Edit(EditEpisodeInfoViewModel editEpisodeViewModel, int EpisodeNumber)
        {
            var episode = _context.Episodes.FirstOrDefault(s => s.Id == editEpisodeViewModel.Id);
            System.Console.WriteLine(editEpisodeViewModel.ReleaseDate);
            
            if (ModelState.IsValid)
            {
                _context.Episodes.Attach(episode);
                editEpisodeViewModel.MapToModel(episode);
                _context.SaveChanges();
                
                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", episode.Id);
                }

                TempData["message"] = $"{episode.Name} has been changed";

                return RedirectToAction("Details", "Episode", new { EpisodeNumber = EpisodeNumber });
            } 
            return View(episode);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/delete")]
        [Authorize(Roles = "Admins")]
        public IActionResult Delete(string Slug, int SeasonNumber, int Id)
        {
            var series = _context.Series.FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.FirstOrDefault(s => s.Id == Id);

            if (episode != null)
            {
                FilmItemMethods.EditSeriesAndSeasonAfterDeleteEpisode(_context, series, season);
                Images.DeleteAssetImage(_context, _env, "filmItem", episode.Id);

                _context.Episodes.Remove(episode);
                _context.SaveChanges();

                TempData["message"] = $"Episode {episode.Episode_EpisodeNumber} - {episode.Name} from {series.Name} - {season.Name} was deleted";
            } 
            
            return RedirectToAction("Details", "Season", new { Slug = Slug, SeasonNumber = SeasonNumber});
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/comments")]
        [AllowAnonymous]
        public ViewResult Comments(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var episode = _context.Episodes.Where(e => e.Slug == Slug).Where(e => e.Episode_SeasonNumber == SeasonNumber).Where(e => e.Episode_EpisodeNumber == EpisodeNumber).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(r => r.FilmItem == episode).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.FilmItem = episode;
            ViewBag.ReleaseYear = episode.ReleaseDate.Value.ToString("yyyy");

            return View(comments);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/media")]
        public ViewResult Media(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var episode = _context.Episodes.Where(m => m.Slug == Slug).Where(e => e.Episode_SeasonNumber == SeasonNumber).Where(e => e.Episode_EpisodeNumber == EpisodeNumber).FirstOrDefault();
            var media = _context.Media.Include(r => r.FilmItem).Where(r => r.FilmItem == episode).FirstOrDefault();

            return View(media);
        }
    }
}
