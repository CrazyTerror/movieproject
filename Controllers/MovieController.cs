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
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;

namespace MovieProject.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;

        public MovieController(MovieContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("movies")]
        public ViewResult Index()
        {
            var movies = _context.Movies.OrderBy(item => item.VoteAverage).ToList();

            return View(movies);
        }

        [HttpGet("movies/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.FilmItemGenres).ThenInclude(g => g.Genre)
                                       .Include(mc => mc.FilmItemCredits).ThenInclude(p => p.Person)
                                       .FirstOrDefault(m => m.Slug == Slug);

            ViewBag.Year = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "Unknown");
            ViewBag.Date = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("dd MMMM, yyyy") : "Unknown");
            
            if (movie == null)
            {
                return View(nameof(Index));
            }

            return View(movie);
        }

        [HttpGet("movies/create")]
        public ViewResult Create()
        {
            ViewBag.Genres = new SelectList((_context.Genres.OrderBy(g => g.Name)), "Id", "Name");

            return View();
        }

        [HttpPost("movies/create")]
        public IActionResult Create(Movie movie)
        {
            //check if slug is available, else with releaseyear else random
            //if (string.IsNullOrWhiteSpace(Request.Form["ReleaseDate"]))
            //{
            //    movie.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"]);
            //} else
            //{
            //    movie.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"], DateTime.Parse(Request.Form["ReleaseDate"]).Year);
            //}
            
            movie.Slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            _context.Movies.Add(movie);
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
                
                FilmItemGenre mvg = new FilmItemGenre()
                {
                    FilmItem = movie,
                    Genre = genre
                };

                _context.FilmItemGenres.Add(mvg);
            }
            _context.SaveChanges();

            TempData["message"] = $"{movie.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("movies/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

            return View(movie);
        }

        [HttpPost("movies/{Slug}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditMovieInfoViewModel movieViewModel)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == movieViewModel.Id);
            
            if (ModelState.IsValid)
            {
                _context.Movies.Attach(movie);

                movie.Name = movieViewModel.Name;
                movie.Description = movieViewModel.Description;
                movie.ReleaseDate = movieViewModel.ReleaseDate;
                movie.Slug = UrlEncoder.ToFriendlyUrl(movieViewModel.Name + " " + movieViewModel.ReleaseDate.Year);
                movie.UpdatedAt = DateTime.Now;
                
                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", movie.Id);
                }

                _context.SaveChanges();

                TempData["message"] = $"{movie.Name} has been changed";

                return RedirectToAction(nameof(Index));
            } 
            return View(movie);
        }

        [HttpPost("movies/{id}/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            
            if (movie != null)
            {
                Images.DeleteAssetImage(_context, _env, "filmitem", movie.Id);

                _context.Movies.Remove(movie);
                _context.SaveChanges();

                TempData["message"] = $"{movie.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else {
                return View("Index");
            }
        }

        [HttpGet("movies/{Slug}/genres")]
        public ViewResult Genre(string Slug)
        {
            var movie = _context.Movies.Include(fig => fig.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

            return View(movie);
        }

        [HttpGet("movies/{Slug}/genres/add")]
        public ViewResult AddGenre(string Slug)
        {
            ViewBag.FilmItem = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            ViewBag.Genres = new SelectList((_context.Genres.OrderBy(x => x.Name)), "Id", "Name");

            return View();
        }

        [HttpPost("movies/{Slug}/genres/add")]
        [ValidateAntiForgeryToken]
        public IActionResult AddGenre(string Slug, int i = 0)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            var currentGenres = _context.FilmItemGenres.Where(m => m.FilmItemId == movie.Id).Select(g => g.GenreId).ToList();
            
            var newGenres = Request.Form["Genre"];
            foreach (var newGenre in newGenres)
            {
                if (currentGenres.Contains(int.Parse(newGenre)))
                {
                    continue;
                } else
                {
                    FilmItemGenre fig = new FilmItemGenre
                    {
                        FilmItem = movie,
                        GenreId = int.Parse(newGenre)
                    };
                    _context.FilmItemGenres.Add(fig);  
                }
                TempData["message"] = $"Added {newGenres.Count} new genres to {movie.Name}";  
                _context.SaveChanges();
            }   
            return RedirectToAction("Details", "Movie", new { Slug = Slug });
        }

        [HttpPost("movies/{Slug}/genres/{Id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Genre(string Slug, int Id = 0)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            var filmItemGenre = _context.FilmItemGenres.Include(g => g.Genre).Where(f => f.FilmItemId == movie.Id).Where(g => g.GenreId == Id).FirstOrDefault();

            if (filmItemGenre != null)
            {   
                TempData["message"] = $"Deleted genre '{filmItemGenre.Genre.Name}' from {movie.Name}";  
                _context.FilmItemGenres.Remove(filmItemGenre);
                _context.SaveChanges();

                return RedirectToAction("Details", "Movie", new { Slug = Slug });
            } else
            {
                return View(nameof(Index));
            }
        }

        [HttpGet("movies/{Slug}/credits/add")]
        public ViewResult AddCredit(string Slug)
        {
            ViewBag.FilmItem = _context.Movies.FirstOrDefault(m => m.Slug == Slug);

            return View();
        }

        [HttpPost("movies/{Slug}/credits/add")]
        public IActionResult AddCredit(string Slug, int i = 0)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            var currentCredits = _context.FilmItemCredits.Where(m => m.FilmItemId == movie.Id).Select(p => p.PersonId).ToList();
            
            var person = _context.Persons.Where(fn => fn.FirstName == Request.Form["Firstname"]).Where(sn => sn.Surname == Request.Form["Surname"]).FirstOrDefault();
            var character = Request.Form["Character"].ToString();

            if (person != null && character != null)
            {
                if (!currentCredits.Contains(person.Id))
                {
                    FilmItemCredits fic = new FilmItemCredits
                    {
                        FilmItem = movie,
                        Person = person,
                        Character = character
                    };

                    _context.FilmItemCredits.Add(fic);
                }
                _context.SaveChanges();
            
                TempData["message"] = $"Added {person.FirstName} {person.Surname} as '{character}' to {movie.Name}";  
                return RedirectToAction("Details", "Movie", new { Slug = Slug });
            } else 
            {
                TempData["message"] = $"You made an error filling in the Person or Character"; 
                return RedirectToAction("AddCredit", "Movie", new { Slug = Slug});
            }
        }

        [HttpGet("movies/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            return View(filmItemCredit);
        }

        [HttpPost("movies/{Slug}/credits/{Id}/edit")]
        public IActionResult EditCredit(EditMovieCreditViewModel emc, string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);
            var character = Request.Form["Character"].ToString();

            if (ModelState.IsValid)
            {
                _context.FilmItemCredits.Attach(filmItemCredit);

                if (character != null)
                {
                    filmItemCredit.Character = emc.Character;
                }

                _context.SaveChanges();
                
                TempData["message"] = $"Edited {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} as '{character}'";  
                return RedirectToAction("Details", "Movie", new { Slug = Slug });
            } else 
            {
                TempData["message"] = $"Something went wrong";
                return View(filmItemCredit);
            }
        }

        [HttpGet("movies/{Slug}/credits")]
        public ViewResult Credits(string Slug)
        {
            var movie = _context.Movies.Include(fic => fic.FilmItemCredits).ThenInclude(p => p.Person).FirstOrDefault(m => m.Slug == Slug);

            return View(movie);
        }

        [HttpPost("movies/{Slug}/credits/{Id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Credits(string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            if (filmItemCredit != null)
            {
                _context.FilmItemCredits.Remove(filmItemCredit);
                _context.SaveChanges();
                TempData["message"] = $"Removed {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} from '{filmItemCredit.FilmItem.Name}'"; 

                return RedirectToAction("Details", "Movie", new { Slug = Slug });
            } else
            {
                return View(nameof(Index));
            }
        }
    }
}
