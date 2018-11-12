using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public static class GetCountries
    {
        public static void LoadCountries(MovieContext context)
        {
            if (!context.Countries.Any())
            {
                List<string> cultureList = new List<string>();

                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                foreach (CultureInfo culture in cultures)
                {
                     RegionInfo getRegionInfo = new RegionInfo(culture.LCID);

                    if (!(cultureList.Contains(getRegionInfo.EnglishName)))
                    {
                        cultureList.Add(getRegionInfo.EnglishName);
                    }
                }

                cultureList.Sort();

                foreach (var country in cultureList)
                {
                    Country c = new Country
                    {
                        Name = country
                    };

                    context.Countries.Add(c);
                }

                context.SaveChanges();
            }
        }
        public static void LoadLanguages(MovieContext context)
        {
            if (!context.Languages.Any())
            {
                List<string> cultureList = new List<string>();

                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

                foreach (CultureInfo culture in cultures)
                {
                    cultureList.Add(culture.EnglishName);
                }

                cultureList.Sort();

                foreach (var language in cultureList)
                {
                    Language l = new Language
                    {
                        Name = language
                    };
                    context.Languages.Add(l);
                }

                context.SaveChanges();
            }
        }
    }
}