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
using MovieProject.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class MovieController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public MovieController(MovieContext context, IHostingEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet("movies")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var movies = _context.Movies.OrderByDescending(item => item.VoteAverage).ToList();

            return View(movies);
        }

        [HttpGet("movies/{Slug}")]
        [AllowAnonymous]
        public ViewResult Details(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.FilmItemGenres).ThenInclude(g => g.Genre)
                                       .Include(mc => mc.FilmItemCredits).ThenInclude(p => p.Person)
                                       .Include(v => v.Videos)
                                       .Include(p => p.Photos)
                                       .Include(t => t.Trivia)
                                       .Include(m => m.Media)
                                       .Include(mr => mr.UserRatings)
                                       .Include(r => r.Reviews)
                                       .FirstOrDefault(m => m.Slug == Slug);

            ViewBag.Genres = movie.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray();
            ViewBag.Year = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "Unknown");
            ViewBag.Date = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("dd MMMM, yyyy") : "Unknown");

            ViewBag.Directors = movie.FilmItemCredits.Where(p => p.PartType == PartType.Director).OrderBy(x => x.Person.Surname).ToList();
            ViewBag.Producers = movie.FilmItemCredits.Where(p => p.PartType == PartType.Producer).ToList();
            ViewBag.Writers = movie.FilmItemCredits.Where(p => p.PartType == PartType.Writer).ToList();
            ViewBag.CommentCount = movie.Reviews.Count;

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
            if (string.IsNullOrWhiteSpace(Request.Form["ReleaseDate"]))
            {
                movie.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"]);
            } else
            {
                movie.Slug = UrlEncoder.IsSlugAvailable(_context, "filmitem", Request.Form["Name"], DateTime.Parse(Request.Form["ReleaseDate"]).Year);
            }
            
            _context.Movies.Add(movie);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files;
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "filmitem");
            }

            foreach (var genre in Request.Form["Genre"])
            {
                FilmItemMethods.SaveFilmItemGenres(_context, movie, genre);
            }

            TempData["message"] = $"{movie.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("movies/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);
            ViewBag.Year = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "Unknown");

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
                movieViewModel.MapToModel(movie);
                _context.SaveChanges();
                
                var images = HttpContext.Request.Form.Files; 
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "filmitem", movie.Id);
                }

                TempData["message"] = $"{movie.Name} has been changed";

                return RedirectToAction(nameof(Index));
            } 
            return View(movie);
        }

        [HttpPost("movies/{id}/Delete")]
        [Authorize(Roles = "Admins")]
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
            var person = _context.Persons.Where(fn => fn.FirstName == Request.Form["Firstname"]).Where(sn => sn.Surname == Request.Form["Surname"]).FirstOrDefault();
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (movie != null && person != null && character != null && (partType >= 1 && partType <= 7 ))
            {
                FilmItemMethods.SaveFilmItemCredits(_context, movie, person, partType, character);
                TempData["message"] = $"You added {person.FirstName} {person.Surname} to {movie.Name} as {(PartType) partType}"; 
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
            var partType = int.Parse(Request.Form["PartType"]);

            if (ModelState.IsValid)
            {
                FilmItemMethods.EditFilmItemCredit(_context, filmItemCredit, partType, character);
                
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

        [HttpPost("movies/{Slug}/credits")]
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

        [HttpGet("movies/{Slug}/comments")]
        [AllowAnonymous]
        public ViewResult Comments(string Slug)
        {
            var movie = _context.Movies.Where(m => m.Slug == Slug).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(r => r.FilmItem == movie).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.FilmItem = movie.Name;
            ViewBag.ReleaseYear = movie.ReleaseDate.Value.ToString("yyyy");

            return View(comments);
        }

        [HttpPost("movies/{Slug}/addReview")]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview(string Slug)
        {
            var applicationUser = _userManager.GetUserId(User);
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);

            Review review = new Review
            {
                ApplicationUserId = applicationUser,
                FilmItem = movie,
                Comment = Request.Form["Comment"]
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Details", "Movie", new { Slug = Slug });
        }

        [HttpGet("movies/{Slug}/comments/{Id}/edit")]
        public IActionResult EditComment(string Slug, int Id)
        {
            var comment = _context.Reviews.Include(f => f.FilmItem).FirstOrDefault(r => r.Id == Id);

            if (comment != null && comment.ApplicationUserId == _userManager.GetUserId(User))
            {
                return View(comment);
            } else
            {
                return RedirectToAction("Details", new { Slug = Slug });
            }
        }

        [HttpPost("movies/{Slug}/comments/{Id}/edit")]
        public IActionResult EditComment(string Slug, int Id, int i = 0)
        {
            var comment = _context.Reviews.FirstOrDefault(r => r.Id == Id);

            _context.Reviews.Attach(comment);
            comment.Comment = Request.Form["Comment"];
            comment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Details", new { Slug = Slug});
        }

        [HttpPost("movies/{Slug}/comments/{Id}/delete")]
        public IActionResult DeleteComment(string Slug, int Id)
        {
            var comment = _context.Reviews.Include(f => f.FilmItem).FirstOrDefault(r => r.Id == Id);

            if (comment != null)
            {
                _context.Reviews.Remove(comment);
                _context.SaveChanges();

                TempData["message"] = $"Your comment on {comment.FilmItem.Name} was deleted";
            } 
            return RedirectToAction("Details", new { Slug = Slug});
        }
    }
}
