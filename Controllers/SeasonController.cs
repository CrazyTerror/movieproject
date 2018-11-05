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

namespace MovieProject.Controllers
{
    public class SeasonController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;

        public SeasonController(MovieContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("series/{Slug}/seasons")]
        public ViewResult Index(string Slug)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var seasons = _context.Seasons.Where(s => s.SeriesId == series.Id).ToList();

            return View(seasons);
        }

        [HttpGet("series/{Slug}/seasons/{SeasonNumber}")]
        public ViewResult Details(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var season = _context.Seasons.Include(ep => ep.Episodes)
                                         .Where(x => x.Season_SeasonNumber == SeasonNumber)
                                         .Where(y => y.SeriesId == series.Id)
                                         .FirstOrDefault();

            ViewBag.Series = series.Name;
            ViewBag.SeasonCount = series.Series_SeasonCount;

            return View(season);
        }

        [HttpGet("series/{Slug}/seasons/create")]
        public ViewResult Create(string Slug)
        {
            return View();
        }

        [HttpPost("series/{Slug}/seasons/create")]
        public IActionResult Create(string Slug, Season season)
        {
            var series = _context.Series.Include(s => s.Seasons).FirstOrDefault(x => x.Slug == Slug);

            // Add New Season
            season.SeriesId = series.Id;
            if (series.Series_EpisodeCount == null)
            {
                season.Season_SeasonNumber = 1;
            } else 
            {
                season.Season_SeasonNumber++;
            }
            _context.Seasons.Add(season);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files; 
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }

            // Change amount of seasons in series
            _context.Series.Attach(series);
            series.Series_SeasonCount = series.Seasons.Count;
            series.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("Details", "Season", new { Slug = Slug, SeasonNumber = season.Season_SeasonNumber});
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

                season.Name = editSeasonViewModel.Name;
                season.Description = editSeasonViewModel.Description;
                season.ReleaseDate = editSeasonViewModel.AirDate;
                season.UpdatedAt = DateTime.Now;

                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", season.Id);
                }

                _context.SaveChanges();

                TempData["message"] = $"{season.Name} has been changed";

                return RedirectToAction("Details", "Season", new { SeasonNumber = SeasonNumber });
            } 
            return View(season);
        }

        [HttpPost("series/{Slug}/seasons/{SeasonNumber}/delete")]
        public async Task<IActionResult> Delete(string Slug, int SeasonNumber)
        {
            var series = _context.Series.FirstOrDefault(srs => srs.Slug == Slug);
            var season = _context.Seasons.Where(s => s.SeriesId == series.Id).Where(s => s.Season_SeasonNumber == SeasonNumber).FirstOrDefault();
            
            if (season != null)
            {
                _context.Attach(series);
                series.Series_SeasonCount--;
                series.Series_EpisodeCount = series.Series_EpisodeCount - season.Season_EpisodeCount;
                series.UpdatedAt = DateTime.Now;

                _context.Seasons.Remove(season);
                await _context.SaveChangesAsync();
                
                TempData["message"] = $"{series.Name} - {season.Name} was deleted";

                return RedirectToAction("Details", "Series", new { Slug = Slug});
            } else 
            {
                
                return View("Error");
            }
        }
    }
}
