using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using MovieProject.Models;

namespace MovieProject.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Slug { get; set; }
    }
}