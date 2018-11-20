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

        [HttpPost("users/{Slug}/lists/create")]
        public IActionResult Create(List list)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            list.Name = UrlEncoder.ListSlugSearch(_context, list, userId);
            list.ApplicationUserId = userId;

            _context.Lists.Add(list);
            _context.SaveChanges();
            TempData["message"] = $"{list.Name} has been created";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("users/{Slug}/lists/{listName}/edit")]
        public ViewResult Edit(string listName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var list = _context.Lists.Where(l => l.Name == listName).Where(u => u.ApplicationUserId == userId).FirstOrDefault();

            return View(list);
        }

        [HttpPost("users/{Slug}/lists/{listName}/edit")]
        public IActionResult Edit(List tempList)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var list = _context.Lists.FirstOrDefault(l => l.Id == tempList.Id);

            if (ModelState.IsValid)
            {
                _context.Lists.Attach(list);
                list.Name = UrlEncoder.ListSlugSearch(_context, tempList, userId);
                list.Description = tempList.Description;
                list.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                
                TempData["message"] = $"{list.Name} has been changed";
                return RedirectToAction(nameof(Index));
            } else
            {
                return View(list);
            }
        }

        [HttpPost("users/{Slug}/lists/{id}/delete")]
        public IActionResult Delete(int id)
        {
            var list = _context.Lists.Find(id);

            if (list != null)
            {
                _context.Lists.Remove(list);
                _context.SaveChanges();

                TempData["message"] = $"{list.Name} was deleted";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
