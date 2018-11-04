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
    public class PeopleController : Controller
    {
        private readonly MovieContext _context;

        public PeopleController(MovieContext context)
        {
            _context = context;
        }

        [HttpGet("people")]
        public ViewResult Index()
        {
            var people = _context.Persons.OrderBy(n => n.Surname).ToList();

            return View(people);
        }

        [HttpGet("people/{Slug}")]
        public ViewResult Details(string Slug)
        {
            var person = _context.Persons.Include(mc => mc.MovieCredits).ThenInclude(c => c.Movie)
                                         .Include(ec => ec.EpisodeCredits).ThenInclude(ep => ep.Episode).ThenInclude(s => s.Season).ThenInclude(srs => srs.Series)
                                         .FirstOrDefault(p => p.Slug == Slug);

            return View(person);
        }

        [HttpGet("people/create")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost("people/create")]
        public IActionResult Create(Person person)
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("people/{Slug}/edit")]
        public ViewResult Edit(string Slug)
        {
            

            return View();
        }

        [HttpPost("people/{Slug}/edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditPersonInfoViewModel editPersonViewModel)
        {
            
            return View();
        }

        [HttpPost("people/{id}/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
