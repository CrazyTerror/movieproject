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
    public class SeasonController : Controller
    {
        private readonly MovieContext _context;

        public SeasonController(MovieContext context)
        {
            _context = context;
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
                                         .Where(x => x.SeasonNumber == SeasonNumber)
                                         .Where(y => y.SeriesId == series.Id)
                                         .FirstOrDefault();

            ViewBag.Series = series.Name;
            ViewBag.SeasonCount = series.NumberOfSeasons;

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
            season.SeasonNumber = series.NumberOfSeasons + 1;
            _context.Seasons.Add(season);

            // Change amount of seasons in series
            _context.Series.Attach(series);
            series.NumberOfSeasons = series.Seasons.Count;

            _context.SaveChanges();

            return RedirectToAction("Details", "Series", new { Slug = Slug});
        }

        [HttpGet("series/{Slug}/seasons/{id}/edit")]
        public ViewResult Edit()
        {
            return View();
        }

        [HttpPost("series/{Slug}/seasons/{id}/edit")]
        public IActionResult Edit(Season season)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("series/{Slug}/seasons/{id}/delete")]
        public IActionResult Delete(Season season)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
