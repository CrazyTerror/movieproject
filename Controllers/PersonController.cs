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

namespace MovieProject.Controllers
{
    public class PersonController : Controller
    {
        private readonly MovieContext _context;

        public PersonController(MovieContext context)
        {
            _context = context;
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
                var deletedProduct = _context.Persons.Remove(person);
                _context.SaveChangesAsync();

                TempData["message"] = $"{person.FirstName} {person.Surname} was deleted";

                return RedirectToAction(nameof(Index));
            } else {
                return View(nameof(Index));
            }
        }
    }
}
