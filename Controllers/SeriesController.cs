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
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class SeriesController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeriesController(MovieContext context, IHostingEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
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
            } 
            
            return RedirectToAction(nameof(Index));
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
        [AllowAnonymous]
        public ViewResult Credits(string Slug)
        {
            var series = _context.Series.Include(fic => fic.FilmItemCredits).ThenInclude(p => p.Person).FirstOrDefault(m => m.Slug == Slug);

            return View(series);
        }
        
        [HttpGet("series/{Slug}/credits/add")]
        public ViewResult AddCredit(string Slug)
        {
            ViewBag.FilmItem  = _context.Series.FirstOrDefault(m => m.Slug == Slug);

            return View();
        }

        [HttpGet("series/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            return View(filmItemCredit);
        }

        [HttpGet("series/{Slug}/comments")]
        [AllowAnonymous]
        public ViewResult Comments(string Slug)
        {
            var series = _context.Series.Where(m => m.Slug == Slug).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(r => r.FilmItem == series).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.FilmItem = series;
            ViewBag.ReleaseYear = series.ReleaseDate.Value.ToString("yyyy");

            return View(comments);
        }
    }
}
