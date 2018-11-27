using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Infrastructure;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    [Authorize(Roles = "Admins, Users")]
    public class DashboardController : Controller
    {
        private readonly MovieContext _context;

        public DashboardController(MovieContext context)
        {
            _context = context;

        }

        [HttpGet("dashboard")]
        public ViewResult Index()
        {
            return View();
        }
    }
}
