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
            var rating = int.Parse(Request.Form["Rating"]);
            var user = _userManager.GetUserId(User);
            
            UserRating userRating = new UserRating
            {
                ApplicationUserId = user,
                FilmItem = filmItem,
                Rating = rating
            };
            _context.UserRatings.Add(userRating);
            
            AlterFilmItemAverage(filmItem, rating);
            _context.SaveChanges();

            return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug});
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
                CalculateFilmItemAverageAfterDeletion(filmItem, userRating.Rating);
                _context.SaveChanges();
                TempData["message"] = $"Your rating on {userRating.FilmItem.Name} was deleted";
            }

            return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug});
        }

        public void AlterFilmItemAverage(FilmItem filmItem, int rating)
        {
            _context.Attach(filmItem);
            if (filmItem.VoteCount == null && filmItem.VoteAverage == null)
            {
                filmItem.VoteCount = 1;
                filmItem.VoteAverage = rating;
            } else {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount++;
                filmItem.VoteAverage = (totalRating + rating) / filmItem.VoteCount;
            }
            
            filmItem.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
        }

        public void CalculateFilmItemAverageAfterDeletion(FilmItem filmItem, int rating)
        {
            _context.Attach(filmItem);

            if (filmItem.VoteCount == 1)
            {
                filmItem.VoteCount = null;
                filmItem.VoteAverage = null;
            } else 
            {
                var totalRating = filmItem.VoteAverage * filmItem.VoteCount;
                filmItem.VoteCount--;
                filmItem.VoteAverage = (totalRating - rating) / filmItem.VoteCount;
            }
            filmItem.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
        }
    }
}
