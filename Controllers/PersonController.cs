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
using System.Data.SqlClient;

namespace MovieProject.Controllers
{
    public class PersonController : Controller
    {
        private readonly MovieContext _context;
        private readonly IHostingEnvironment _env;

        public PersonController(MovieContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("person")]
        public ViewResult Index()
        {
            var people = _context.Persons.OrderBy(n => n.Surname).ToList();

            return View(people);
        }

        [HttpGet("person/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var person = _context.Persons.Include(mc => mc.FilmItemCredits).ThenInclude(c => c.FilmItem)
                                         .FirstOrDefault(p => p.Slug == Slug);
            
            var filmItems = from p in _context.Persons
                            join fc in _context.FilmItemCredits on p.Id equals fc.PersonId
                            join f in _context.FilmItem on fc.FilmItemId equals f.Id
                            where p.Slug == Slug
                            where f.Discriminator == "Series" || f.Discriminator == "Movie"
                            orderby f.ReleaseDate descending
                            select new FilmItemRelease {
                                Id = f.Id,
                                ReleaseDate = f.ReleaseDate,
                                Discriminator = f.Discriminator,
                                Slug = f.Slug,
                                Name = f.Name,
                                Character = fc.Character
                            };
                            
            List<int> filmItemIds = new List<int>();
            foreach (var filmItem in filmItems)
            {
                filmItemIds.Add(filmItem.Id);
            }
            
            foreach (var item in filmItemIds)
            {
                System.Console.WriteLine(item);
            }
            
            if (filmItemIds.Count == 1)
            {
                ViewBag.FilmItemId = filmItemIds.First();
            } else if (filmItemIds.Count > 1)
            {
                Random rand = new Random();
                int i = rand.Next(filmItemIds.Count) + 1;
                ViewBag.FilmItemId = i;
            }
            
            ViewBag.FilmItems = filmItems;

            return View(person);
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
            var slug = UrlEncoder.ToFriendlyUrl(personName);
            person.Slug = slug;

            _context.Persons.Add(person);
            _context.SaveChanges();

            var images = HttpContext.Request.Form.Files;
            if (images.Count > 0)
            {
                Images.ReadImages(_context, _env, images, "person");
            }

            TempData["message"] = $"{person.FirstName} {person.Surname} has been created";

            return RedirectToAction("Details", "Person", new { Slug = slug});
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
            
            System.Console.WriteLine("------------" + editPersonViewModel.DeathDate);
            if (ModelState.IsValid)
            {
                _context.Persons.Attach(person);

                person.FirstName = editPersonViewModel.FirstName;
                person.Surname = editPersonViewModel.Surname;
                person.BirthDate = editPersonViewModel.BirthDate;
                person.DeathDate = editPersonViewModel.DeathDate;
                person.UpdatedAt = DateTime.Now;

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
        public ViewResult Credits(string Slug)
        {
            var filmItems = from f in _context.FilmItem
                            join fc in _context.FilmItemCredits on f.Id equals fc.FilmItemId
                            join p in _context.Persons on fc.PersonId equals p.Id
                            where p.Slug == Slug
                            where f.Discriminator == "Series" || f.Discriminator == "Movie"
                            orderby f.ReleaseDate descending
                            select new FilmItemRelease {
                                Id = f.Id,
                                ReleaseDate = f.ReleaseDate,
                                Discriminator = f.Discriminator,
                                Slug = f.Slug,
                                Name = f.Name,
                                Character = fc.Character
                            };

            return View(filmItems.ToList());
        }

        [HttpGet("person/{Slug}/credits/add")]
        public ViewResult AddCredit()
        {
            return View();
        }

        [HttpPost("person/{Slug}/credits/add")]
        public IActionResult AddCredit(string Slug)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);
            var filmItemName = Request.Form["Name"];
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Name == filmItemName);

            if (filmItem != null && person != null)
            {
                FilmItemCredits fic = new FilmItemCredits
                {
                    FilmItem = filmItem,
                    Person = person,
                    Character = Request.Form["Character"]
                };

                _context.FilmItemCredits.Add(fic);
                _context.SaveChanges();
                
                TempData["message"] = $"{person.FirstName} {person.Surname} is added to {filmItem.Name}";
                return RedirectToAction("Details", "Person", new { Slug = Slug });
            } else 
            {
                TempData["message"] = $"{filmItemName} does not exist. Please enter a valid title";
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpGet("person/{Slug}/credits/{Id}/edit")]
        public ViewResult EditCredit(string Slug, int Id)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == Id);
            var filmItemCredit = _context.FilmItemCredits.Where(p => p.PersonId == person.Id).Where(f => f.FilmItemId == filmItem.Id).FirstOrDefault();

            return View(filmItemCredit);
        }

        [HttpPost("person/{Slug}/credits/{Id}/edit")]
        public IActionResult EditCredit(EditPersonCreditViewModel edc, string Slug, int Id)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == Id);
            var character = Request.Form["Character"].ToString();

            if (ModelState.IsValid)
            {
                _context.FilmItemCredits.Attach(filmItemCredit);

                if (character != null)
                {
                    filmItemCredit.Character = edc.Character;
                }

                _context.SaveChanges();
                
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
    }
}
