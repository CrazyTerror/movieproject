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
    [Authorize(Roles = "Admins, Users")]
    public class ReviewController : Controller
    {
        private readonly MovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewController(MovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("reviews/{id}")]
        public ViewResult Details(int id)
        {
            var review = _context.Reviews.FirstOrDefault(r => r.Id == id);

            return View(review);
        }
        
        [HttpPost("addReview")]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview()
        {
            var applicationUser = _userManager.GetUserId(User);
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            
            Review review = new Review
            {
                ApplicationUserId = applicationUser,
                FilmItem = filmItem,
                Comment = Request.Form["Comment"]
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            
            if (filmItem.Discriminator == "Season") {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug, SeasonNumber = filmItem.Season_SeasonNumber });
            } else if (filmItem.Discriminator == "Episode") {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug, SeasonNumber = filmItem.Episode_SeasonNumber, EpisodeNumber = filmItem.Episode_EpisodeNumber });
            } else {
                return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug });
            }
        }

        [HttpGet("comment/{id}")]
        public IActionResult EditReview(int Id)
        {
            var comment = _context.Reviews.Include(f => f.FilmItem).FirstOrDefault(r => r.Id == Id);

            if (comment != null && comment.ApplicationUserId == _userManager.GetUserId(User))
            {
                return View(comment);
            } else
            {
                return RedirectToAction("Details", new { Slug = comment.FilmItem.Slug });
            }
        }

        [HttpPost("comment/{id}")]
        public IActionResult EditReview()
        {
            var comment = _context.Reviews.Include(f => f.FilmItem).FirstOrDefault(r => r.Id == int.Parse(Request.Form["ReviewId"]));

            _context.Reviews.Attach(comment);
            comment.Comment = Request.Form["Comment"];
            comment.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("Details", comment.FilmItem.Discriminator, new { Slug = comment.FilmItem.Slug});
        }

        [HttpPost("deleteReview")]
        public IActionResult DeleteReview()
        {
            var comment = _context.Reviews.Include(f => f.FilmItem).FirstOrDefault(r => r.Id == int.Parse(Request.Form["ReviewId"]));

            if (comment != null)
            {
                _context.Reviews.Remove(comment);
                _context.SaveChanges();

                TempData["message"] = $"Your comment on {comment.FilmItem.Name} was deleted";
            } 
            return RedirectToAction("Details", comment.FilmItem.Discriminator, new { Slug = comment.FilmItem.Slug});
        }
    }
}
