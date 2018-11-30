using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class CreditController : Controller
    {
        private readonly MovieContext _context;

        public CreditController(MovieContext context)
        {
            _context = context;
        }

        [HttpPost("addCredit")]
        public IActionResult AddCredit()
        {
            var filmItem = _context.FilmItem.FirstOrDefault(f => f.Id == int.Parse(Request.Form["FilmItemId"]));
            var person = _context.Persons.Where(fn => fn.FirstName == Request.Form["Firstname"]).Where(sn => sn.Surname == Request.Form["Surname"]).FirstOrDefault();
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (filmItem != null && person != null && character != null && (partType >= 1 && partType <= 7 ))
            {
                FilmItemMethods.SaveFilmItemCredits(_context, filmItem, person, partType, character);
                TempData["message"] = $"You added {person.FirstName} {person.Surname} to {filmItem.Name} as {(PartType) partType}"; 
            } else
            {
                TempData["message"] = $"You made an error filling in the Person or Character"; 
            }
            
            return RedirectToAction("Details", filmItem.Discriminator, new { Slug = filmItem.Slug });
        }

        [HttpPost("editCredit")]
        public IActionResult EditCredit(EditMovieCreditViewModel edc)
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == int.Parse(Request.Form["FicId"]));
            var character = Request.Form["Character"].ToString();
            var partType = int.Parse(Request.Form["PartType"]);

            if (ModelState.IsValid)
            {
                FilmItemMethods.EditFilmItemCredit(_context, filmItemCredit, partType, character);
                
                TempData["message"] = $"Edited {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} as '{character}'";  
            } else 
            {
                TempData["message"] = $"Something went wrong";
            }
            
            return RedirectToAction("Details", filmItemCredit.FilmItem.Discriminator, new { Slug = filmItemCredit.FilmItem.Slug });
        }

        [HttpPost("deleteCredit")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCredit()
        {
            var filmItemCredit = _context.FilmItemCredits.Include(p => p.Person).Include(f => f.FilmItem).FirstOrDefault(fic => fic.Id == int.Parse(Request.Form["FicId"]));

            if (filmItemCredit != null)
            {
                _context.FilmItemCredits.Remove(filmItemCredit);
                _context.SaveChanges();
                TempData["message"] = $"Removed {filmItemCredit.Person.FirstName} {filmItemCredit.Person.Surname} from '{filmItemCredit.FilmItem.Name}'"; 
            } 

            return RedirectToAction("Details", filmItemCredit.FilmItem.Discriminator, new { Slug = filmItemCredit.FilmItem.Slug });
        }
    }
}
