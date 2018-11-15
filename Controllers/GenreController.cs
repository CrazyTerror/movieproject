using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class GenreController : Controller
    {
        private readonly MovieContext _context;

        public GenreController(MovieContext context)
        {
            _context = context;

        }

        [HttpGet("genres")]
        public ViewResult Index()
        {
            var genres = _context.Genres.OrderBy(item => item.Name).ToList();

            return View(genres);
        }

        [HttpGet("genres/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var movie = _context.Genres.FirstOrDefault(m => m.Slug == Slug);

            if (movie == null)
            {
                return View(nameof(Index));
            }

            return View(movie);
        }

        [HttpGet("genres/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost("genres/create")]
        public IActionResult Create(Genre genre)
        {
            genre.Slug = UrlEncoder.ToFriendlyUrl(Request.Form["Name"]);
            _context.Genres.Add(genre);
            _context.SaveChanges();

            TempData["message"] = $"{genre.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("genres/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var genre = _context.Genres.FirstOrDefault(m => m.Slug == Slug);

            return View(genre);
        }

        [HttpPost("genres/{Slug}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Genre tempGenre)
        {
            var genre = _context.Genres.FirstOrDefault(x => x.Id == tempGenre.Id);

            if (ModelState.IsValid)
            {
                _context.Genres.Attach(genre);

                genre.Name = tempGenre.Name;
                genre.Slug = UrlEncoder.ToFriendlyUrl(tempGenre.Name);
                genre.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                
                TempData["message"] = $"{genre.Name} has been changed";

                return RedirectToAction(nameof(Index));
            } 
            return View(genre);
        }

        [HttpPost("genres/{id}/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var genre = _context.Genres.Find(id);
            
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                _context.SaveChanges();

                TempData["message"] = $"{genre.Name} was deleted";

                return RedirectToAction(nameof(Index));
            } else {
                return View("Index");
            }
        }
    }
}
