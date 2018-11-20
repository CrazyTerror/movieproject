using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MovieProject.Models;

namespace MovieProject.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //public string Slug { get; set; }
        //public ICollection<UserRating> UserRatings { get; set; }
        //public ICollection<List> Lists { get; set; }
    }
}
