using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class PersonMethods
    {
        public static int CalculatePersonAge(Person person)
        {
            var today = DateTime.Today;
            var birthDate = person.BirthDate;

            var age = today.Year - birthDate.Value.Year;

            if (birthDate > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}