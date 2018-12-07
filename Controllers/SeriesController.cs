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
                                        .Include(l => l.ListItems).ThenInclude(l => l.List)
                                        .FirstOrDefault(s => s.Slug == Slug);

            SeriesDetailsViewModel seriesDetailsViewModel = new SeriesDetailsViewModel
            {
                Series = series,
                Genres = series.FilmItemGenres.Select(g => g.Genre.Name).OrderBy(g => g).ToArray(),
                ReleaseYear = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("yyyy") : ""),
                PremiereDate = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("dd MMMM yyyy") : ""),
                TotalRuntime = FilmItemMethods.CalculateSeriesTotalRuntime(series),
                CommentCount = series.Reviews.Count,
                ListCount = series.ListItems.Select(l => l.List).ToList().Count
            };

            return View(seriesDetailsViewModel);
        }

        [HttpGet("series/create")]
        public ViewResult Create()
        {
            FilmItemCreateParentsViewModel createViewModel = new FilmItemCreateParentsViewModel
            {
                Genres = new SelectList((_context.Genres.OrderBy(g => g.Name)), "Id", "Name"),
                Languages = new SelectList((_context.Languages.OrderBy(l => l.Name)), "Id", "Name")
            };

            return View(createViewModel);
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
            FilmItemMethods.AddMediaEntry(_context, series);
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

            SeriesDetailsViewModel seriesDetailsViewModel = new SeriesDetailsViewModel
            {
                Series = series,
                ReleaseYear = (series.FirstAirDate.HasValue ? series.FirstAirDate.Value.ToString("yyyy") : "")
            };

            return View(seriesDetailsViewModel);
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
            FilmItemAddGenresViewModel filmItemAddGenresViewModel = new FilmItemAddGenresViewModel 
            { 
                FilmItem = _context.Series.FirstOrDefault(m => m.Slug == Slug),
                Genres = new SelectList((_context.Genres.OrderBy(x => x.Name)), "Id", "Name")
            };
            
            return View(filmItemAddGenresViewModel);
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
            var filmItem  = _context.Series.Where(s => s.Discriminator == "Series").FirstOrDefault(m => m.Slug == Slug);

            return View(filmItem);
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

            FilmItemCommentsViewModel filmItemCommentsViewModel = new FilmItemCommentsViewModel
            {
                Comments = comments,
                FilmItem = series,
                ReleaseYear = series.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemCommentsViewModel);
        }

        [HttpGet("series/{Slug}/media")]
        public ViewResult Media(string Slug)
        {
            var series = _context.Series.Where(m => m.Slug == Slug).FirstOrDefault();
            var media = _context.Media.Include(r => r.FilmItem).Where(r => r.FilmItem == series).FirstOrDefault();

            return View(media);
        }

        [HttpGet("series/{Slug}/lists")]
        [AllowAnonymous]
        public ViewResult Lists(string Slug)
        {
            var series = _context.Series.Include(f => f.ListItems).ThenInclude(li => li.List).FirstOrDefault(m => m.Slug == Slug);

            FilmItemListsViewModel filmItemListsViewModel = new FilmItemListsViewModel
            {
                FilmItem = series,
                ReleaseYear = series.ReleaseDate.Value.ToString("yyyy")
            };

            return View(filmItemListsViewModel);
        }

        [HttpGet("series/{Slug}/listsModal")]
        public IActionResult ListsModal(string Slug)
        {
            var series = _context.Series.FirstOrDefault(s => s.Slug == Slug);
            var lists = _context.Lists.Include(li => li.ListItems).ThenInclude(m => m.FilmItem).Where(a => a.ApplicationUserId == _userManager.GetUserId(User)).ToList();
            ViewBag.FilmItem = series.Name;
            ViewBag.FilmItemId = series.Id;

            FilmItemListsModalViewModel filmItemListsViewModel = new FilmItemListsModalViewModel
            {
                Lists = lists,
                FilmItem = series.Name,
                FilmItemId = series.Id,
                ListsWithFilmItem = FilmItemMethods.ListHavingFilmItem(lists, series)
            };

            return PartialView("_FilmItemListsModalPartial", filmItemListsViewModel);
        }

        [HttpPost("series/{Slug}/listsModal")]
        public IActionResult ListsModal(string Slug, bool check = false)
        {
            var listsChecked = Request.Form["Lists"].ToList();
            var series = _context.Series.FirstOrDefault(m => m.Slug == Slug);
            var lists = _context.Lists.Include(li => li.ListItems).Where(u => u.ApplicationUserId == _userManager.GetUserId(User)).ToList();

            foreach (var list in lists)
            {
                var itemInList = list.ListItems.Where(m => m.FilmItem == series).FirstOrDefault();
                if (itemInList != null && !listsChecked.Contains(list.Id.ToString())) //unchecked -> checked
                {
                    FilmItemMethods.RemoveListItem(_context, itemInList);
                } else if (itemInList == null && listsChecked.Contains(list.Id.ToString())) //checked -> unchecked
                {
                    FilmItemMethods.SaveListItem(_context, list, series);
                }
            }

            return RedirectToAction("Details", new { Slug = Slug });
        }
    }
}
