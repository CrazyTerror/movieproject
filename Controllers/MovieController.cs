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
    public class MovieController : Controller
    {
        private readonly MovieContext _context;

        public MovieController(MovieContext context)
        {
            _context = context;
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
            var movie = _context.Movies.Include(mg => mg.MovieGenre).ThenInclude(g => g.Genre)
                                       .Include(mc => mc.MovieCredits).ThenInclude(p => p.Person)
                                       .FirstOrDefault(m => m.Slug == Slug);

            var year = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "");
            var date = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("dd MMMM, yyyy") : "");
            ViewBag.Year = year;
            ViewBag.Date = date;
            
            if (movie == null)
            {
                return View("Index");
            }

            return View(movie);
        }

        [HttpGet("movies/create")]
        public ViewResult Create()
        {
            ViewBag.Genres = new SelectList((_context.Genres), "Id", "Name");

            return View("Create");
        }

        [HttpPost("movies/create")]
        public IActionResult Create(Movie movie)
        {
            var slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            movie.Slug = slug;

            _context.Movies.Add(movie);

            foreach (var item in Request.Form["Genre"])
            {
                var selectedGenre = Int32.Parse(item);
                Genre genre = _context.Genres.Find(selectedGenre);
                
                MovieGenre mvg = new MovieGenre()
                {
                    Movie = movie,
                    Genre = genre
                };

                _context.MovieGenre.Add(mvg);
            }
            _context.SaveChanges();

            TempData["message"] = $"{movie.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("movies/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.MovieGenre).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

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

                _context.SaveChanges();

                TempData["message"] = $"{movie.Name} has been changed";

                return RedirectToAction(nameof(Index));
            } 
            return View(movie);
        }

        [HttpPost("movies/{id}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            
            if (movie != null)
            {
                var deletedProduct = _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();

                TempData["message"] = $"{movie.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else {
                return View("Index");
            }
        }
    }
}
