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
                                        .Include(mc => mc.FilmItemCredits).ThenInclude(p => p.Person)
                                        .Include(m => m.Media)
                                        .Include(v => v.Videos)
                                        .Include(p => p.Photos)
                                        .FirstOrDefault(s => s.Slug == Slug);

            ViewBag.People = from f in _context.FilmItem
                         join fc in _context.FilmItemCredits on f.Id equals fc.FilmItemId
                         join p in _context.Persons on fc.PersonId equals p.Id
                         where f.Slug == Slug
                         orderby p.Surname
                         select new PeopleOnSeries {
                            Id = p.Id,
                            FirstName = p.FirstName,
                            Surname = p.Surname,
                            CharacterName = fc.Character
                         };

            ViewBag.Year = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("dd MMMM yyyy") : "");
            ViewBag.TotalRuntime = FilmItemMethods.CalculateTotalRuntime(series);

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
                series.ReleaseDate = seriesViewModel.FirstAirDate;
                series.UpdatedAt = DateTime.Now;
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
        
        [HttpPost("series/{Slug}/genres/{Id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Genre(string Slug, int Id = 0)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var filmItemGenre = _context.FilmItemGenres.Include(g => g.Genre).Where(f => f.FilmItemId == series.Id).Where(g => g.GenreId == Id).FirstOrDefault();

            if (filmItemGenre != null)
            {   
                TempData["message"] = $"Deleted genre '{filmItemGenre.Genre.Name}' from {series.Name}";  
                _context.FilmItemGenres.Remove(filmItemGenre);
                _context.SaveChanges();

                return RedirectToAction("Details", "Series", new { Slug = Slug });
            } else
            {
                return View(nameof(Index));
            }
        }

        [HttpGet("series/{Slug}/genres/add")]
        public ViewResult AddGenre(string Slug)
        {
            ViewBag.FilmItem = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            ViewBag.Genres = new SelectList((_context.Genres.OrderBy(x => x.Name)), "Id", "Name");

            return View();
        }

        [HttpPost("series/{Slug}/genres/add")]
        [ValidateAntiForgeryToken]
        public IActionResult AddGenre(string Slug, int i = 0)
        {
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var currentGenres = _context.FilmItemGenres.Where(m => m.FilmItemId == series.Id).Select(g => g.GenreId).ToList();
            
            foreach (var newGenre in Request.Form["Genre"])
            {
                if (currentGenres.Contains(int.Parse(newGenre)))
                {
                    continue;
                } else
                {
                    FilmItemMethods.SaveFilmItemGenres(_context, series, newGenre);
                }
                TempData["message"] = $"Added new genres to {series.Name}";
            }   
            return RedirectToAction("Details", "Series", new { Slug = Slug });
        }

        [HttpGet("series/{Slug}/credits")]
        public ViewResult Credits(string Slug)
        {
            var series = _context.Series.Include(fic => fic.FilmItemCredits).ThenInclude(p => p.Person).FirstOrDefault(m => m.Slug == Slug);

            return View(series);
        }

        [HttpPost("series/{Slug}/credits/{Id}")]
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
            var currentCredits = _context.FilmItemCredits.Where(m => m.FilmItemId == series.Id).Select(p => p.PersonId).ToList();
            
            var person = _context.Persons.Where(fn => fn.FirstName == Request.Form["Firstname"]).Where(sn => sn.Surname == Request.Form["Surname"]).FirstOrDefault();
            var character = Request.Form["Character"].ToString();

            if (person != null && character != null)
            {
                if (!currentCredits.Contains(person.Id))
                {
                    FilmItemMethods.SaveFilmItemCredits(_context, series, person, character);
                    TempData["message"] = $"Added {person.FirstName} {person.Surname} as '{character}' to {series.Name}";  
                } else
                {
                    TempData["message"] = $"{person.FirstName} {person.Surname} already belongs to {series.Name}";
                }
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

            if (ModelState.IsValid)
            {
                FilmItemMethods.EditFilmItemCredit(_context, filmItemCredit, character);
                
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
