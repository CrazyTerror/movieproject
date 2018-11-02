using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieProject.Models;

namespace MovieProject.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
    }
}