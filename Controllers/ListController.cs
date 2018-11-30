using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public ViewResult Details(string Slug, string listName)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);

            var list = _context.Lists.Include(li => li.ListItems).ThenInclude(f => f.FilmItem)
                                     .Where(u => u.ApplicationUserId == user.Id)
                                     .FirstOrDefault(l => l.Slug == listName);
                                     
            return View(list);
        }

        [HttpGet("users/{Slug}/lists/create")]
        public IActionResult Create(string Slug)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            if (user.Id == _userManager.GetUserId(User))
            {
                return View();
            } else
            {
                return RedirectToAction("Index", new { Slug = Slug});
            }
        }

        [HttpPost("users/{Slug}/lists/create")]
        public IActionResult Create(string Slug, List list)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);

            if (user.Id == _userManager.GetUserId(User))
            {
                list.Slug = UrlEncoder.ListSlugSearch(_context, list, user.Id);
                list.ApplicationUserId = user.Id;

                _context.Lists.Add(list);
                _context.SaveChanges();
                TempData["message"] = $"{list.Name} has been created";

            } 
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("users/{Slug}/lists/{listName}/edit")]
        public IActionResult Edit(string Slug, string listName)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            var list = _context.Lists.Where(l => l.Slug == listName).Where(u => u.ApplicationUserId == user.Id).FirstOrDefault();

            if (user.Id == _userManager.GetUserId(User))
            {
                return View(list);
            } else
            {
                return RedirectToAction("Details", new { Slug = Slug, listName = listName});
            }
        }

        [HttpPost("users/{Slug}/lists/{listName}/edit")]
        public IActionResult Edit(string Slug, List tempList)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            var list = _context.Lists.FirstOrDefault(l => l.Id == tempList.Id);

            if (ModelState.IsValid && user.Id == _userManager.GetUserId(User))
            {
                _context.Lists.Attach(list);
                list.Name = tempList.Name;
                list.Slug = UrlEncoder.ListSlugSearch(_context, tempList, user.Id);
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
        public IActionResult Delete(string Slug, int id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            var list = _context.Lists.Find(id);

            if (list != null && user.Id == list.ApplicationUserId)
            {
                _context.Lists.Remove(list);
                _context.SaveChanges();

                TempData["message"] = $"{list.Name} was deleted";
            }

            return RedirectToAction("Index", new { Slug = Slug});
        }

        [HttpGet("users/{Slug}/lists/{listName}/add")]
        public IActionResult AddListItem(string Slug, string listName)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);

            if (user.Id == _userManager.GetUserId(User))
            {
                return View();
            } else
            {
                return RedirectToAction("Details", new { Slug = Slug, listName = listName});
            }
        }

        [HttpPost("users/{Slug}/lists/{listName}/add")]
        public IActionResult AddListItem(string Slug, string listName, string id = null)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            var list = _context.Lists.Where(l => l.Slug == listName).Where(u => u.ApplicationUserId == user.Id).FirstOrDefault();
            var filmItem = _context.FilmItem.Where(f => f.Name == Request.Form["FilmItem"].ToString()).FirstOrDefault();

            if (user.Id == _userManager.GetUserId(User))
            {
                SaveListItem(list, filmItem);
                TempData["message"] = $"{filmItem.Name} is added to {list.Name}";
            }

            return RedirectToAction("Details", "List", new { listName = listName });
        }

        [HttpPost("users/{Slug}/lists/{listName}/{Id}/delete")]
        public IActionResult DeleteListItem(string Slug, string listName, int Id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Slug == Slug);
            var listItem = _context.ListItems.Include(l => l.List).Include(f => f.FilmItem).FirstOrDefault(li => li.Id == Id);

            if (listItem != null && user.Id == _userManager.GetUserId(User))
            {
                EditListAfterDeletingListItem(listItem);
                _context.ListItems.Remove(listItem);
                _context.SaveChanges();
                TempData["message"] = $"Removed {listItem.FilmItem.Name} from '{listItem.List.Name}'"; 
            } 

            return RedirectToAction("Details", "List", new { listName = listName });
        }

        public void SaveListItem(List list, FilmItem filmItem)
        {
            var filmItemAlreadyInList = _context.ListItems.Where(l => l.List == list).Where(f => f.FilmItem == filmItem).FirstOrDefault();

            if (filmItem != null && filmItemAlreadyInList == null)
            {
                ListItem li = new ListItem()
                {
                    FilmItem = filmItem,
                    List = list
                };

                _context.ListItems.Add(li);
                _context.SaveChanges();

                _context.Lists.Attach(list);
                list.ItemCount++;
                list.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void EditListAfterDeletingListItem(ListItem listItem)
        {
            var list = listItem.List;
            _context.Lists.Attach(list);
            listItem.List.ItemCount--;
            _context.SaveChanges();
        }
    }
}
