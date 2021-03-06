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
                                       .Include(l => l.ListItems).ThenInclude(l => l.List)
                                       .FirstOrDefault(m => m.Slug == Slug);

            MovieDetailsViewModel movieDetailsViewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Genres = movie.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray(),
                ReleaseYear = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "Unknown"),
                ReleaseDate = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("dd MMMM, yyyy") : "Unknown"),
                Directors = movie.FilmItemCredits.Where(p => p.PartType == PartType.Director).OrderBy(x => x.Person.Surname).ToList(), 
                Producers = movie.FilmItemCredits.Where(p => p.PartType == PartType.Producer).ToList(),
                Writers = movie.FilmItemCredits.Where(p => p.PartType == PartType.Writer).ToList(),
                CommentCount = movie.Reviews.Count,
                Lists = _context.Lists.Where(l => l.ApplicationUserId == _userManager.GetUserId(User)).ToList(),
                ListCount = movie.ListItems.Select(l => l.List).ToList().Count
            };

            if (movie == null)
            {
                return View(nameof(Index));
            }

            return View(movieDetailsViewModel);
        }

        [HttpGet("movies/create")]
        public ViewResult Create()
        {
            FilmItemCreateParentsViewModel createViewModel = new FilmItemCreateParentsViewModel
            {
                Genres = new SelectList((_context.Genres.OrderBy(g => g.Name)), "Id", "Name"),
            };

            return View(createViewModel);
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
            FilmItemMethods.AddMediaEntry(_context, movie);
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

            return RedirectToAction("Details", new { Slug = movie.Slug});
        }

        [HttpGet("movies/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var movie = _context.Movies.Include(mg => mg.FilmItemGenres).ThenInclude(g => g.Genre).FirstOrDefault(m => m.Slug == Slug);

            MovieDetailsViewModel movieDetailsViewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                ReleaseYear = (movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value.ToString("yyyy") : "Unknown")
            };

            return View(movieDetailsViewModel);
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

                return RedirectToAction("Details", new { Slug = movie.Slug});
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
            } 

            return RedirectToAction(nameof(Index));
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
            FilmItemAddGenresViewModel filmItemAddGenresViewModel = new FilmItemAddGenresViewModel 
            { 
                FilmItem = _context.Movies.FirstOrDefault(m => m.Slug == Slug),
                Genres = new SelectList((_context.Genres.OrderBy(x => x.Name)), "Id", "Name")
            };
            
            return View(filmItemAddGenresViewModel);
        }

        [HttpGet("movies/{Slug}/credits/add")]
        public ViewResult AddCredit(string Slug)
        {
            var filmItem = _context.FilmItem.FirstOrDefault(m => m.Slug == Slug);

            return View(filmItem);
        }

        [HttpGet("movies/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            return View(filmItemCredit);
        }

        [HttpGet("movies/{Slug}/credits")]
        [AllowAnonymous]
        public ViewResult Credits(string Slug)
        {
            var movie = _context.Movies.Include(fic => fic.FilmItemCredits).ThenInclude(p => p.Person).FirstOrDefault(m => m.Slug == Slug);

            return View(movie);
        }

        [HttpGet("movies/{Slug}/comments")]
        [AllowAnonymous]
        public ViewResult Comments(string Slug)
        {
            var movie = _context.Movies.Where(m => m.Slug == Slug).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(r => r.FilmItem == movie).OrderByDescending(x => x.CreatedAt).ToList();
            
            FilmItemCommentsViewModel filmItemCommentsViewModel = new FilmItemCommentsViewModel
            {
                Comments = comments,
                FilmItem = movie,
                ReleaseYear = movie.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemCommentsViewModel);
        }

        [HttpGet("movies/{Slug}/media")]
        public ViewResult Media(string Slug)
        {
            var movie = _context.Movies.Where(m => m.Slug == Slug).FirstOrDefault();
            var media = _context.Media.Include(r => r.FilmItem).Where(r => r.FilmItem == movie).FirstOrDefault();

            return View(media);
        }

        [HttpGet("movies/{Slug}/lists")]
        [AllowAnonymous]
        public ViewResult Lists(string Slug)
        {
            var movie = _context.Movies.Include(f => f.ListItems).ThenInclude(li => li.List).FirstOrDefault(m => m.Slug == Slug);
            
            FilmItemListsViewModel filmItemListsViewModel = new FilmItemListsViewModel
            {
                FilmItem = movie,
                ReleaseYear = movie.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemListsViewModel);
        }

        [HttpGet("movies/{Slug}/listsModal")]
        public IActionResult ListsModal(string Slug)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            var lists = _context.Lists.Include(li => li.ListItems).ThenInclude(m => m.FilmItem).Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).ToList();

            FilmItemListsModalViewModel filmItemListsViewModel = new FilmItemListsModalViewModel
            {
                Lists = lists,
                FilmItem = movie.Name,
                FilmItemId = movie.Id,
                ListsWithFilmItem = FilmItemMethods.ListHavingFilmItem(lists, movie)
            };

            return PartialView("_FilmItemListsModalPartial", filmItemListsViewModel);
        }

        [HttpPost("movies/{Slug}/listsModal")]
        public IActionResult ListsModal(string Slug, bool check = false)
        {
            var listsChecked = Request.Form["Lists"].ToList();
            var movie = _context.Movies.FirstOrDefault(m => m.Slug == Slug);
            var lists = _context.Lists.Include(li => li.ListItems).Where(u => u.ApplicationUserId == _userManager.GetUserId(User)).ToList();

            foreach (var list in lists)
            {
                var itemInList = list.ListItems.Where(m => m.FilmItem == movie).FirstOrDefault();
                if (itemInList != null && !listsChecked.Contains(list.Id.ToString())) //unchecked -> checked
                {
                    FilmItemMethods.RemoveListItem(_context, itemInList);
                } else if (itemInList == null && listsChecked.Contains(list.Id.ToString())) //checked -> unchecked
                {
                    FilmItemMethods.SaveListItem(_context, list, movie);
                }
            }

            return RedirectToAction("Details", new { Slug = Slug });
        }
    }
}
