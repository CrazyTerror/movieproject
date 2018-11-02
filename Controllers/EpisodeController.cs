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
            var seasons = _context.Seasons.FirstOrDefault(s => s.SeasonNumber == SeasonNumber);
            var episodes = _context.Episodes.Where(e => e.SeasonId == seasons.Id).ToList();

            return View(episodes);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}/episodes/{EpisodeNumber}")]
        public ViewResult Details(string Slug, int SeasonNumber, int EpisodeNumber)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.Where(srs => srs.SeriesId == series.Id).Where(s => s.SeasonNumber == SeasonNumber).FirstOrDefault();
            var episode = _context.Episodes.Where(s => s.SeasonId == season.Id).Where(ep => ep.EpisodeNumber == EpisodeNumber).FirstOrDefault();

            if (episode.EpisodeNumber < 10)
            {
                ViewBag.Episode = "0" + episode.EpisodeNumber;
            } else
            {
                ViewBag.Episode = episode.EpisodeNumber;
            }
            ViewBag.Series = series.Name;
            ViewBag.EpisodeCount = season.EpisodeCount;

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
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.SeasonNumber == SeasonNumber).FirstOrDefault(); 
            
            // Add new Episode
            episode.SeasonId = season.Id;
            episode.SeasonNumber = season.SeasonNumber;
            episode.EpisodeNumber = season.EpisodeCount + 1;
            _context.Episodes.Add(episode);

            // Change episode count in series
            var allSeasons = series.Seasons.ToList();
            var seriesEpisodeCount = 0;
            foreach (var perSeason in allSeasons)
            {
                seriesEpisodeCount += perSeason.Episodes.Count;
            }
            _context.Series.Attach(series);
            series.NumberOfEpisodes = seriesEpisodeCount;

            // Change episode count in season
            _context.Seasons.Attach(season);
            season.EpisodeCount = season.Episodes.Count;

            _context.SaveChanges();

            return RedirectToAction("Details", "Season", new { SeasonNumber = SeasonNumber});
        }

        [HttpGet("series/{Slug}/seasons/{seasonId}/episodes/{episodeId}/edit")]
        public ViewResult Edit()
        {
            return View();
        }

        [HttpPost("series/{Slug}/seasons/{seasonId}/episodes/{episodeId}/edit")]
        public IActionResult Edit(Season season)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("series/{Slug}/seasons/{seasonId}/episodes/{episodeId}/delete")]
        public IActionResult Delete(Season season)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
