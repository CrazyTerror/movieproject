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
    public class SeriesController : Controller
    {
        private readonly MovieContext _context;

        public SeriesController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("series")]
        public ViewResult Index()
        {
            var series = _context.Series.OrderByDescending(item => item.VoteAverage).ToList();

            return View(series);
        }

        [HttpGet("series/{slug}")]
        public ViewResult Details(string Slug)
        {
            var series = _context.Series.Include(sg => sg.SeriesGenre).ThenInclude(g => g.Genre)
                                        .Include(s => s.Seasons).ThenInclude(e => e.Episodes)
                                        .FirstOrDefault(s => s.Slug == Slug);

            var year = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("yyyy") : "");
            ViewBag.Year = year;

            return View(series);
        }

        [HttpGet("series/create")]
        public ViewResult Create()
        {
            ViewBag.Genres = new SelectList((_context.Genres), "Id", "Name");

            return View();
        }

        [HttpPost("series/create")]
        public IActionResult Create(Series series)
        {
            var slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            series.Slug = slug;

            _context.Series.Add(series);

            foreach (var item in Request.Form["Genre"])
            {
                var selectedGenre = Int32.Parse(item);
                Genre genre = _context.Genres.Find(selectedGenre);
                
                SeriesGenre sg = new SeriesGenre()
                {
                    Series = series,
                    Genre = genre
                };

                _context.SeriesGenre.Add(sg);
            }
            _context.SaveChanges();

            TempData["message"] = $"{series.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("series/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var series = _context.Series.Include(sg => sg.SeriesGenre).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

            return View(series);
        }

        [HttpPost("series/{Slug}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string Slug, EditSeriesInfoViewModel seriesViewModel)
        {
            var series = _context.Series.FirstOrDefault(m => m.Id == seriesViewModel.Id);
            
            if (ModelState.IsValid)
            {
                _context.Series.Attach(series);

                series.Name = seriesViewModel.Name;
                series.Description = seriesViewModel.Description;
                series.FirstAirDate = seriesViewModel.FirstAirDate;

                _context.SaveChanges();

                TempData["message"] = $"{series.Name} has been changed";

                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } 
            return View(series);
        }

        [HttpPost("series/{id}/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var series = _context.Series.FirstOrDefault(s => s.Id == id);

            if (series != null)
            {
                _context.Series.Remove(series);
                _context.SaveChanges();

                TempData["message"] = $"{series.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else 
            {
                return View(nameof(Index));
            }
        }
    }
}
