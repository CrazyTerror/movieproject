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
        [AllowAnonymous]
        public ViewResult Details(int id)
        {
            var review = _context.Reviews.Include(r => r.FilmItem).FirstOrDefault(r => r.Id == id);
            var replies = _context.Reviews.Where(r => r.ShoutId == review.Id).OrderBy(r => r.CreatedAt).ToList();

            ReviewDetailsViewModel reviewDetailsViewModel = new ReviewDetailsViewModel
            {
                Review = review,
                Replies = replies
            };

            return View(reviewDetailsViewModel);
        }
        
        [HttpPost("addReview")]
        [ValidateAntiForgeryToken]
        public IActionResult AddReview()
        {
            var applicationUser = _userManager.GetUserId(User);
            
            int? reviewId, listId, filmItemId;
            FilmItem filmItem = new FilmItem();
            if (!string.IsNullOrWhiteSpace(Request.Form["ReviewId"]))
            {
                reviewId = int.Parse(Request.Form["ReviewId"]);
            } else
            {
                reviewId = null;
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["ListId"]))
            {
                listId = int.Parse(Request.Form["ListId"]);
            } else {
                listId = null;
            }

            if (!string.IsNullOrWhiteSpace(Request.Form["FilmItemId"]))
            {
                filmItemId = int.Parse(Request.Form["FilmItemId"]);
                filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            } else {
                filmItemId = null;
            }
            
            Review review = new Review
            {
                ApplicationUserId = applicationUser,
                FilmItemId = filmItemId,
                Comment = Request.Form["Comment"],
                ShoutId = reviewId,
                ListId = listId
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            
            if (listId != null)
            {
                var list = _context.Lists.FirstOrDefault(l => l.Id == listId);
                var user = _userManager.Users.FirstOrDefault(u => u.Id == list.ApplicationUserId);
                return RedirectToAction("Comments", "List", new { Slug = user, listName = list.Slug });
            }

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
