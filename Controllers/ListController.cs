using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Data;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class ListController : Controller
    {
        private readonly MovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ListController(MovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("users/{Slug}/lists")]
        public ViewResult Index(string Slug)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);

            if (user.Id != _userManager.GetUserId(User) || User.Identity.IsAuthenticated == false)
            {
                var lists = _context.Lists.Where(u => u.ApplicationUserId == user.Id).Where(p => p.Privacy == false).OrderBy(l => l.Name);
                return View(lists.ToList());
            } else 
            {
                var lists = _context.Lists.Where(u => u.ApplicationUserId == user.Id).OrderBy(l => l.Name);
                return View(lists.ToList());
            }
        }

        [HttpGet("users/{Slug}/lists/{listName}")]
        public ViewResult Details(string listName)
        {
            var list = _context.Lists.Include(li => li.ListItems).ThenInclude(f => f.FilmItem)
                                     .Where(u => u.ApplicationUserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                                     .FirstOrDefault(l => l.Slug == listName);

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
            list.Slug = UrlEncoder.ListSlugSearch(_context, list, userId);
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
            var list = _context.Lists.Where(l => l.Slug == listName).Where(u => u.ApplicationUserId == userId).FirstOrDefault();

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
                list.Name = tempList.Name;
                list.Slug = UrlEncoder.ListSlugSearch(_context, tempList, userId);
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

        [HttpGet("users/{Slug}/lists/{listName}/add")]
        public ViewResult AddListItem()
        {
            return View();
        }

        [HttpPost("users/{Slug}/lists/{listName}/add")]
        public IActionResult AddListItem(string listName)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var list = _context.Lists.Where(l => l.Slug == listName).Where(u => u.ApplicationUserId == userId).FirstOrDefault();
            var filmItem = _context.FilmItem.Where(f => f.Name == Request.Form["FilmItem"].ToString()).FirstOrDefault();

            ListMethods.SaveListItem(_context, list, filmItem);
            TempData["message"] = $"{filmItem.Name} is added to {list.Name}";

            return RedirectToAction("Details", "List", new { listName = listName });
        }

        [HttpPost("users/{Slug}/lists/{listName}/{Id}/delete")]
        public IActionResult DeleteListItem(string listName, int Id)
        {
            var listItem = _context.ListItems.Include(l => l.List).Include(f => f.FilmItem).FirstOrDefault(li => li.Id == Id);

            if (listItem != null)
            {
                ListMethods.EditListAfterDeletingListItem(_context, listItem);
                _context.ListItems.Remove(listItem);
                _context.SaveChanges();
                TempData["message"] = $"Removed {listItem.FilmItem.Name} from '{listItem.List.Name}'"; 
            } 

            return RedirectToAction("Details", "List", new { listName = listName });
        }
    }
}
