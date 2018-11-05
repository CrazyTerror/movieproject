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

namespace MovieProject.Controllers
{
    public class EpisodeController : Controller
    {
        private readonly MovieContext _context;

        public EpisodeController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes")]
        public ViewResult Index(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var seasons = _context.Seasons.FirstOrDefault(s => s.Season_SeasonNumber == SeasonNumber);
            var episodes = _context.Episodes.Where(e => e.SeasonId == seasons.Id).ToList();

            return View(episodes);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}")]
        public ViewResult Details(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.Where(srs => srs.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.Where(s => s.SeasonId == season.Id).Where(ep => ep.Episode_EpisodeNumber == EpisodeNumber).FirstOrDefault();

            if (episode.Episode_EpisodeNumber < 10)
            {
                ViewBag.Episode = "0" + episode.Episode_EpisodeNumber;
            } else
            {
                ViewBag.Episode = episode.Episode_EpisodeNumber;
            }
            ViewBag.Series = series.Name;
            ViewBag.EpisodeCount = season.Season_EpisodeCount;

            return View(episode);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/create")]
        public ViewResult Create(string Slug, int SeasonNumber)
        {
            return View();
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/create")]
        public IActionResult Create(string Slug, int SeasonNumber, Episode episode)
        {
            var series = _context.Series.Include(s => s.Seasons).ThenInclude(s => s.Episodes).FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault(); 
            
            // Change episode count in series
            _context.Series.Attach(series);
            if (series.Series_EpisodeCount == null)
            {
                series.Series_EpisodeCount = 1;
            } else
            {
                series.Series_EpisodeCount++;
            }
            series.UpdatedAt = DateTime.Now;

            // Change episode count in season
            _context.Seasons.Attach(season);
            if (season.Season_EpisodeCount == null)
            {
                season.Season_EpisodeCount = 1;
            } else 
            {
                season.Season_EpisodeCount++;
            }
            season.UpdatedAt = DateTime.Now;
       
            // Add new Episode
            episode.SeasonId = season.Id;
            episode.Episode_SeasonNumber = season.Season_SeasonNumber;
            episode.Episode_EpisodeNumber = season.Season_EpisodeCount;
            episode.UpdatedAt = DateTime.Now;
            _context.Episodes.Add(episode);

            _context.SaveChanges();

            return RedirectToAction("Details", "Season", new { SeasonNumber = SeasonNumber});
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/edit")]
        public ViewResult Edit(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.Where(e => e.SeasonId == season.Id).Where(e => e.Episode_EpisodeNumber == EpisodeNumber).FirstOrDefault();

            return View(episode);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/edit")]
        public IActionResult Edit(EditEpisodeInfoViewModel editEpisodeViewModel, int EpisodeNumber)
        {
            var episode = _context.Episodes.FirstOrDefault(s => s.Id == editEpisodeViewModel.Id);
            
            if (ModelState.IsValid)
            {
                _context.Episodes.Attach(episode);

                episode.Name = editEpisodeViewModel.Name;
                episode.Description = editEpisodeViewModel.Description;
                episode.ReleaseDate = editEpisodeViewModel.AirDate;
                episode.Runtime = editEpisodeViewModel.Runtime;
                episode.UpdatedAt = DateTime.Now;

                _context.SaveChanges();

                TempData["message"] = $"{episode.Name} has been changed";

                return RedirectToAction("Details", "Episode", new { EpisodeNumber = EpisodeNumber });
            } 
            return View(episode);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}/delete")]
        public IActionResult Delete(string Slug, int SeasonNumber, int Id)
        {
            System.Console.WriteLine("-------------" + Id);
            var series = _context.Series.FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.FirstOrDefault(s => s.Id == Id);

            if (episode != null)
            {
                _context.Attach(series);
                series.Series_EpisodeCount--;
                series.UpdatedAt = DateTime.Now;

                _context.Attach(season);
                season.Season_EpisodeCount--;
                season.UpdatedAt = DateTime.Now;

                _context.Episodes.Remove(episode);
                _context.SaveChanges();

                TempData["message"] = $"Episode {episode.Episode_EpisodeNumber} - {episode.Name} from {series.Name} - {season.Name} was deleted";

                return RedirectToAction("Details", "Season", new { Slug = Slug, SeasonNumber = SeasonNumber});
            } else 
            {
                return RedirectToAction("Details", "Season", new { Slug = Slug, SeasonNumber = SeasonNumber});
            }
        }
    }
}
