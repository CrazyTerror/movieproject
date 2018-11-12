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

namespace MovieProject.Controllers
{
    public class FilmItemCreditsController : Controller
    {
        private readonly MovieContext _context;

        public FilmItemCreditsController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("person/{Slug}/credits")]
        public ViewResult Index(string Slug)
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

        [HttpGet("person/{Slug}/credits/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost("person/{Slug}/credits/create")]
        public IActionResult Create(string Slug)
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
                return RedirectToAction(nameof(Index));
            } else 
            {
                TempData["message"] = $"{filmItemName} does not exist. Please enter a valid title";
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpPost("person/{Slug}/credits/{Id}/delete")]
        public IActionResult Delete(string Slug, int Id)
        {
            var person = _context.Persons.FirstOrDefault(p => p.Slug == Slug);
            var filmItem = _context.FilmItem.FirstOrDefault(x => x.Id == Id);
            var filmItemCredit = _context.FilmItemCredits.Where(a => a.FilmItemId == Id).Where(p => p.PersonId == person.Id).FirstOrDefault();

            if (filmItemCredit != null)
            {
                _context.FilmItemCredits.Remove(filmItemCredit);
                _context.SaveChanges();

                TempData["message"] = $"{person.FirstName} {person.Surname} was removed from {filmItem.Name}";

                return RedirectToAction(nameof(Index));
            } else
            {
                return View("Error");
            }
        }
    }
}
