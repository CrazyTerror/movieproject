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

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
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
        [AllowAnonymous]
        public ViewResult Index()
        {
            var series = _context.Series.OrderByDescending(item => item.VoteAverage).ToList();

            return View(series);
        }

        [HttpGet("series/{slug}")]
        [AllowAnonymous]
        public ViewResult Details(string Slug)
        {
            var series = _context.Series.Include(sg => sg.FilmItemGenres).ThenInclude(g => g.Genre)
                                        .Include(s => s.Seasons).ThenInclude(e => e.Episodes)
                                        .Include(mc => mc.FilmItemCredits).ThenInclude(p => p.Person)
                                        .Include(m => m.Media)
                                        .Include(v => v.Videos)
                                        .Include(p => p.Photos)
                                        .Include(mr => mr.UserRatings)
                                        .Include(r => r.Reviews)
                                        .FirstOrDefault(s => s.Slug == Slug);

            ViewBag.Genres = series.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray();
            ViewBag.Year = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("yyyy") : "");
            ViewBag.Premiere = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("dd MMMM yyyy") : "");
            ViewBag.TotalRuntime = FilmItemMethods.CalculateSeriesTotalRuntime(series);

            return View(series);
        }

        [HttpGet("series/create")]
        public ViewResult Create()
        {
            ViewBag.Genres = new SelectList((_context.Genres.OrderBy(g => g.Name)), "Id", "Name");
            ViewBag.Languages = new SelectList((_context.Languages.OrderBy(l => l.Name)), "Id", "Name");

            return View();
        }

        [HttpPost("series/create")]
        public IActionResult Create(Series series)
        {
            //check if slug is available, else with release year
            if (string.IsNullOrWhiteSpace(Request.Form["ReleaseDate"]))
            {
                series.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"]);
            } else
            {
                series.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"], DateTime.Parse(Request.Form["ReleaseDate"]).Year);
                series.FirstAirDate = DateTime.Parse(Request.Form["ReleaseDate"]);
            }
            _context.Series.Add(series);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files; 
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }
            
            foreach (var genre in Request.Form["Genre"])
            {
                FilmItemMethods.SaveFilmItemGenres(_context, series, genre);
            }

            TempData["message"] = $"{series.Name} has been created";

            return RedirectToAction("Details", "Series", new { Slug = series.Slug});
        }

        [HttpGet("series/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var series = _context.Series.Include(sg => sg.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);
            ViewBag.Year = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("yyyy") : "");

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
                seriesViewModel.MapToModel(series);
                _context.SaveChanges();

                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", series.Id);
                }

                TempData["message"] = $"{series.Name} has been changed";

                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } 
            return View(series);
        }

        [HttpPost("series/{id}/Delete")]
        [Authorize(Roles = "Admins")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var series = _context.Series.Include(s => s.Seasons).ThenInclude(e => e.Episodes).FirstOrDefault(s => s.Id == id);

            if (series != null)
            {
                Images.DeleteImagesBelongingToSeries(_context, _env, series);
                _context.Series.Remove(series);
                _context.SaveChanges();

                TempData["message"] = $"{series.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else 
            {
                return View(nameof(Index));
            }
        }

        [HttpGet("series/{Slug}/genres")]
        public ViewResult Genre(string Slug)
        {
            var series = _context.Series.Include(fig => fig.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

            return View(series);
        }

        [HttpGet("series/{Slug}/genres/add")]
        public ViewResult AddGenre(string Slug)
        {
            ViewBag.FilmItem = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            ViewBag.Genres = new SelectList((_context.Genres.OrderBy(x => x.Name)), "Id", "Name");

            return View();
        }

        [HttpGet("series/{Slug}/credits")]
        public ViewResult Credits(string Slug)
        {
            var series = _context.Series.Include(fic => fic.FilmItemCredits).ThenInclude(p => p.Person).FirstOrDefault(m => m.Slug == Slug);

            return View(series);
        }

        [HttpPost("series/{Slug}/credits")]
        [ValidateAntiForgeryToken]
        public IActionResult Credits(string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            if (filmItemCredit != null)
            {
                _context.FilmItemCredits.Remove(filmItemCredit);
                _context.SaveChanges();
                TempData["message"] = $"Removed {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} from '{filmItemCredit.FilmItem.Name}'"; 

                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } else
            {
                return View(nameof(Index));
            }
        }
        
        [HttpGet("series/{Slug}/credits/add")]
        public ViewResult AddCredit(string Slug)
        {
            ViewBag.FilmItem  = _context.Series.FirstOrDefault(m => m.Slug == Slug);

            return View();
        }

        [HttpPost("series/{Slug}/credits/add")]
        public IActionResult AddCredit(string Slug, int i = 0)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var person = _context.Persons.Where(fn => fn.FirstName == Request.Form["Firstname"]).Where(sn => sn.Surname == Request.Form["Surname"]).FirstOrDefault();
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (series != null && person != null && character != null && (partType >= 1 && partType <= 7 ))
            {
                FilmItemMethods.SaveFilmItemCredits(_context, series, person, partType, character);
                TempData["message"] = $"You added {person.FirstName} {person.Surname} to {series.Name} as {(PartType) partType}"; 
                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } else
            {
                TempData["message"] = $"You made an error filling in the Person or Character"; 
                return RedirectToAction("AddCredit", "Series", new { Slug = Slug});
            }
        }

        [HttpGet("series/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            return View(filmItemCredit);
        }

        [HttpPost("series/{Slug}/credits/{Id}/edit")]
        public IActionResult EditCredit(EditMovieCreditViewModel edc, string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (ModelState.IsValid)
            {
                FilmItemMethods.EditFilmItemCredit(_context, filmItemCredit, partType, character);
                
                TempData["message"] = $"Edited {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} as '{character}'";  
                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } else 
            {
                TempData["message"] = $"Something went wrong";
                return View(filmItemCredit);
            }
        }
    }
}
