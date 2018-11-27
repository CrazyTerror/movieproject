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
            var ratings = _context.UserRatings.Where(u => u.ApplicationUserId == user.Id).ToList();

            return View(ratings);
        }

        [HttpGet("users/{slug}/comments")]
        public ViewResult Comments(string Slug)
        {
            var user = _userManager.Users.Where(u => u.Slug == Slug).FirstOrDefault();
            return View();
        }
    }
}
