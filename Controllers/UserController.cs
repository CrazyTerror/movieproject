using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Data;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class UserController : Controller
    {
        private readonly MovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(MovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admins")]
        public ViewResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet("users/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();

            if (user == null)
            {
                return View("Index", "Home");
            }

            return View(user);
        }

        [HttpGet("users/{slug}/history")]
        public ViewResult History(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();



            return View();
        }

        [HttpGet("users/{slug}/ratings")]
        public ViewResult Ratings(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var ratings = _context.UserRatings.Include(u => u.FilmItem).Where(u => u.ApplicationUserId == user.Id).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.User = user.UserName;

            return View(ratings);
        }

        [HttpGet("users/{slug}/comments")]
        public ViewResult Comments(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            var comments = _context.Reviews.Include(r => r.FilmItem).Where(u => u.ApplicationUserId == user.Id).OrderByDescending(x => x.CreatedAt).ToList();
            ViewBag.User = user.UserName;

            return View(comments);
        }

        [HttpPost("addRating")]
        [ValidateAntiForgeryToken]
        public IActionResult AddRating()
        {
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            var userRating = int.Parse(Request.Form["Rating"]);
            var user = _userManager.GetUserId(User);
            
            FilmItemMethods.AddRating(_context, filmItem, userRating, user);
            FilmItemMethods.AlterFilmItemAverage(_context, filmItem, userRating);
            _context.SaveChanges();

            return Redirect(Request.Headers["Referer"]);
        }

        [HttpPost("deleteRating")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteRating()
        {
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            var userRating = _context.UserRatings.Include(f => f.FilmItem).Where(m => m.FilmItem == filmItem).Where(u => u.ApplicationUserId == _userManager.GetUserId(User)).FirstOrDefault();

            if (userRating != null)
            {
                _context.UserRatings.Remove(userRating);
                FilmItemMethods.CalculateFilmItemAverageAfterDeletion(_context, filmItem, userRating.Rating);
                _context.SaveChanges();
                TempData["message"] = $"Your rating on {userRating.FilmItem.Name} was deleted";
            }

            return Redirect(Request.Headers["Referer"]);
        }
    }
}
