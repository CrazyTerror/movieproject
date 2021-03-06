using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieProject.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MovieProject.Data;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class PersonController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public PersonController(MovieContext context, IHostingEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [HttpGet("person")]
        [AllowAnonymous]
        public ViewResult Index()
        {
            var people = _context.Persons.OrderBy(n => n.Surname).ToList();

            return View(people);
        }

        [HttpGet("person/{Slug}")]
        [AllowAnonymous]
        public ViewResult Details(string Slug)
        {
            var person = _context.Persons.Include(mc => mc.FilmItemCredits).ThenInclude(c => c.FilmItem)
                                         .Include(m => m.Media)
                                         .FirstOrDefault(p => p.Slug == Slug);

            var user = _userManager.GetUserId(User);

            PersonDetailsViewModel personDetailsViewModel = new PersonDetailsViewModel
            {
                Person = person,
                FilmItemId = PersonMethods.GetRandomBackground(person),
                Age = PersonMethods.CalculatePersonAge(person),
                PersonWatchedByUser = PersonMethods.GetUserStats(_context, person, user),
                PartTypes = Enum.GetValues(typeof(PartType))
            };

            return View(personDetailsViewModel);
        }

        [HttpGet("person/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost("person/create")]
        public IActionResult Create(Person person)
        {
            var personName = Request.Form["FirstName"] + " " + Request.Form["Surname"];
            //check if slug is available, else with release year
            if (string.IsNullOrWhiteSpace(Request.Form["BirthDate"]))
            {
                person.Slug = UrlEncoder.IsSlugAvailable(_context, "person", personName);
            } else
            {
                person.Slug = UrlEncoder.IsSlugAvailable(_context, "person", personName, DateTime.Parse(Request.Form["BirthDate"]).Year);
            }

            _context.Persons.Add(person);
            FilmItemMethods.AddMediaEntry(_context, null, person);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files;
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "person");
            }

            TempData["message"] = $"{person.FirstName} {person.Surname} has been created";

            return RedirectToAction("Details", "Person", new { Slug = person.Slug });
        }

        [HttpGet("person/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);

            return View(person);
        }

        [HttpPost("person/{Slug}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditPersonInfoViewModel editPersonViewModel, string Slug)
        {
            var person = _context.Persons.FirstOrDefault(m => m.Id == editPersonViewModel.Id);
            
            if (ModelState.IsValid)
            {
                _context.Persons.Attach(person);
                editPersonViewModel.MapToModel(person);
                _context.SaveChanges();

                var images = HttpContext.Request.Form.Files;
                if (images.Count > 0)
                {
                    Images.ReadImages(_context, _env, images, "person");
                }

                TempData["message"] = $"{person.FirstName} {person.Surname} has been changed";

                return RedirectToAction("Details", "Person", new { Slug = Slug });
            } 
            return View(person);
        }

        [HttpPost("person/{id}/Delete")]
        [Authorize(Roles = "Admins")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var person = _context.Persons.Find(id);
            
            if (person != null)
            {
                Images.DeleteAssetImage(_context, _env, "person", person.Id);

                _context.Persons.Remove(person);
                _context.SaveChanges();

                TempData["message"] = $"{person.FirstName} {person.Surname} was deleted";

                return RedirectToAction(nameof(Index));
            } else {
                return View(nameof(Index));
            }
        }

        [HttpGet("person/{Slug}/credits")]
        [AllowAnonymous]
        public ViewResult Credits(string Slug)
        {
            var person = _context.Persons.Include(mc => mc.FilmItemCredits).ThenInclude(c => c.FilmItem).FirstOrDefault(p => p.Slug == Slug);

            return View(person);
        }

        [HttpGet("person/{Slug}/credits/add")]
        public ViewResult AddCredit(string Slug)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);

            return View(person);
        }

        [HttpPost("person/{Slug}/credits/add")]
        public IActionResult AddCredit(string Slug, int i = 0)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);
            var filmItem = _context.FilmItem.Where(f => f.Name == Request.Form["Name"]).DefaultIfEmpty().First();
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (filmItem != null && person != null && character != null && (partType >= 1 && partType <= 7 ))
            {
                //CreditController
                FilmItemMethods.SaveFilmItemCredits(_context, filmItem, person, partType, character);
                TempData["message"] = $"You added {person.FirstName} {person.Surname} to {filmItem.Name} as {(PartType) partType}"; 
            } else
            {
                TempData["message"] = $"You made an error filling in the Film Item or Character"; 
            }
            
            return RedirectToAction("Details", "Person", new { Slug = Slug });
        }

        [HttpGet("person/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);

            return View(filmItemCredit);
        }

        [HttpPost("person/{Slug}/credits/{Id}/edit")]
        public IActionResult EditCredit(EditPersonCreditViewModel edc, string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (ModelState.IsValid)
            {
                FilmItemMethods.EditFilmItemCredit(_context, filmItemCredit, partType, character);
                
                TempData["message"] = $"Edited {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} as '{character}'";  
                return RedirectToAction("Details", "Person", new { Slug = Slug });
            } else 
            {
                TempData["message"] = $"Something went wrong";
                return View(filmItemCredit);
            }
        }

        [HttpPost("person/{Slug}/credits/{Id}/delete")]
        public IActionResult DeleteCredit(string Slug, int Id)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);
            var filmItem = _context.FilmItem.FirstOrDefault(x => x.Id == Id);
            var filmItemCredit = _context.FilmItemCredits.Where(a => a.FilmItemId == Id).Where(p => p.PersonId == person.Id).FirstOrDefault();

            if (filmItemCredit != null)
            {
                _context.FilmItemCredits.Remove(filmItemCredit);
                _context.SaveChanges();

                TempData["message"] = $"{person.FirstName} {person.Surname} was removed from {filmItem.Name}";

                return RedirectToAction("Details", "Person", new { Slug = Slug });
            } else
            {
                return View("Error");
            }
        }

        [HttpGet("person/{Slug}/media")]
        public ViewResult Media(string Slug)
        {
            var person = _context.Persons.Where(m => m.Slug == Slug).FirstOrDefault();
            var media = _context.Media.Include(r => r.Person).Where(r => r.Person == person).FirstOrDefault();

            return View(media);
        }
    }
}
