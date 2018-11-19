using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class ListController : Controller
    {
        private readonly MovieContext _context;

        public ListController(MovieContext context)
        {
            _context = context;

        }

        [HttpGet("users/{Slug}/lists")]
        public ViewResult Index(string Slug)
        {
            var lists = _context.Lists.Where(u => u.ApplicationUserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).OrderBy(l => l.Name).ToList();

            return View(lists);
        }

        [HttpGet("users/{Slug}/lists/{listName}")]
        public ViewResult Details(string listName)
        {
            var list = _context.Lists.Include(li => li.ListItems).ThenInclude(f => f.FilmItem).Where(u => u.ApplicationUserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).FirstOrDefault(l => l.Name == listName);

            return View(list);
        }

        [HttpGet("users/{Slug}/lists/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet("users/{Slug}/lists/{listName}/edit")]
        public ViewResult Edit()
        {
            return View();
        }
    }
}
