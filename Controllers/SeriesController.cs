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
    public class SeriesController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;

        public SeriesController(MovieContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
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
            var series = _context.Series.Include(sg => sg.FilmItemGenres).ThenInclude(g => g.Genre)
                                        .Include(s => s.Seasons).ThenInclude(e => e.Episodes)
                                        .FirstOrDefault(s => s.Slug == Slug);

            var people = from f in _context.FilmItem
                         join fc in _context.FilmItemCredits on f.Id equals fc.FilmItemId
                         join p in _context.Persons on fc.PersonId equals p.Id
                         where f.Slug == Slug
                         orderby p.Surname
                         select new PeopleOnSeries {
                            FirstName = p.FirstName,
                            Surname = p.Surname,
                            CharacterName = fc.Character
                         };

            ViewBag.People = people;
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
            var firstAirDate = DateTime.Parse(Request.Form["ReleaseDate"]);

            series.Slug = slug;
            series.FirstAirDate = firstAirDate;
            _context.Series.Add(series);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files; 
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }

            foreach (var item in Request.Form["Genre"])
            {
                var selectedGenre = Int32.Parse(item);
                Genre genre = _context.Genres.Find(selectedGenre);
                
                FilmItemGenre sg = new FilmItemGenre()
                {
                    FilmItem = series,
                    Genre = genre
                };

                _context.FilmItemGenres.Add(sg);
            }
            _context.SaveChanges();

            TempData["message"] = $"{series.Name} has been created";

            return RedirectToAction("Details", "Series", new { Slug = slug});
        }

        [HttpGet("series/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var series = _context.Series.Include(sg => sg.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

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
                series.UpdatedAt = DateTime.Now;

                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", series.Id);
                }

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
            var series = _context.Series.Include(s => s.Seasons).ThenInclude(e => e.Episodes).FirstOrDefault(s => s.Id == id);

            if (series != null)
            {
                DeleteImagesBelongingToSeries(series);
                _context.Series.Remove(series);
                _context.SaveChanges();

                TempData["message"] = $"{series.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else 
            {
                return View(nameof(Index));
            }
        }

        public void DeleteImagesBelongingToSeries(Series series)
        {
            var filmItemIds = new List<int>();
            filmItemIds.Add(series.Id);

            foreach (var season in series.Seasons)
            {
                filmItemIds.Add(season.Id);
                foreach (var episode in season.Episodes)
                {
                    filmItemIds.Add(episode.Id);
                }
            }

            foreach (var filmItem in filmItemIds)
            {
                Images.DeleteAssetImage(_context, _env, "filmitem", filmItem);
            }
        }
    }
}
