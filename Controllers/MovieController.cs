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

            var year = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "");
            var date = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("dd MMMM, yyyy") : "");
            ViewBag.Year = year;
            ViewBag.Date = date;
            
            if (movie == null)
            {
                return View(nameof(Index));
            }

            return View(movie);
        }

        [HttpGet("movies/create")]
        public ViewResult Create()
        {
            ViewBag.Genres = new SelectList((_context.Genres), "Id", "Name");

            return View();
        }

        [HttpPost("movies/create")]
        public IActionResult Create(Movie movie)
        {
            var slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            movie.Slug = slug;
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
    }
}
