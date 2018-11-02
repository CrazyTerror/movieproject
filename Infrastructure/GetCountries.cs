using System.Collections.Generic;
using System.Globalization;

namespace MovieProject.Infrastructure
{
    public static class GetCountries
    {
        public static Dictionary<string, string> AllCountries()
        {
            Dictionary<string, string> cultureList = new Dictionary<string, string>();

            CultureInfo[] getCultureInfo = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo getCulture in getCultureInfo)
            {
                RegionInfo getRegionInfo = new RegionInfo(getCulture.LCID);

                if (!(cultureList.ContainsKey(getRegionInfo.Name)))
                {
                    cultureList.Add(getRegionInfo.Name, getRegionInfo.EnglishName);
                }
            }

            return cultureList;
        }
    }
}