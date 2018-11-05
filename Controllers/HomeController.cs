using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace MovieProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieContext _context;

        public HomeController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            var srs = _context.Series.Include(series => series.Seasons).ThenInclude(seasons => seasons.Episodes).ThenInclude(episodes => episodes.FilmItemCredits).ThenInclude(credit => credit.Person)
                                     .Include(series => series.FilmItemGenres).ThenInclude(genre => genre.Genre)
                                     .ToList();

            return View(srs);
        }

        [HttpGet("/contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            //var a = _context.Movies;
            var xy = _context.Movies.Include(movie => movie.FilmItemCredits).ThenInclude(movieCast => movieCast.Person)
                                    .Include(movie => movie.FilmItemGenres).ThenInclude(movieGenre => movieGenre.Genre)
                                    .ToList();
            
            return View(xy);
        }

        [HttpGet("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("/error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
