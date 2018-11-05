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
        private readonly IHostingEnvironment _environment;

        public MovieController(MovieContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _environment = hostingEnvironment;

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
            System.Console.WriteLine(movie.Id);
            var slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            movie.Slug = slug;
            _context.Movies.Add(movie);
            _context.SaveChanges();
       
            var lastMovie = _context.Movies.Last(); 

            var poster = HttpContext.Request.Form.Files["Poster"];

            if (poster != null && poster.Length > 0)
            {
                var fileName = Path.GetFileName(poster.FileName);
                
                var extension = System.IO.Path.GetExtension(fileName);
                var newMovieId = lastMovie.Id;
                var newFileName = newMovieId + "" + extension;
                var path = Path.Combine(_environment.WebRootPath, "images\\movie\\poster\\") + newFileName;
                
                using (FileStream fs = System.IO.File.Create(path))
                {
                    poster.CopyTo(fs);
                    fs.Flush();
                }
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
