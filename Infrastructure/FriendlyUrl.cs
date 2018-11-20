using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public static class UrlEncoder
    {
        public static string ToFriendlyUrl (string title) 
        { 
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || 
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        private static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }

        public static string IsSlugAvailable(MovieContext _ctx, string table, string title, int year = 0, string user = null)
        {
            var slug = "";
            if (table == "filmitem")
            {
                slug = FilmItemSlugSearch(_ctx, title, year);
            } else if (table == "person")
            {
                slug = PersonSlugSearch(_ctx, title, year);
            } 
            return slug;
        }

        private static string FilmItemSlugSearch(MovieContext _ctx, string title, int year = 0)
        {
            var slug = ToFriendlyUrl(title);
            var slugAlreadyInUse = _ctx.FilmItem.Where(x => x.Slug == slug).FirstOrDefault();
            if (slugAlreadyInUse != null && year != 0)
            {
                slug = ToFriendlyUrl(title + " " + year);
                var slugYearAlreadyInUse = _ctx.FilmItem.Where(x => x.Slug == slug).FirstOrDefault();
                if (slugYearAlreadyInUse != null)
                {
                    var randomString = RandomString();
                    slug = ToFriendlyUrl(title + " " + RandomString() + " " + RandomString() + " " + RandomString() + " " + RandomString());
                }
            } else if (slugAlreadyInUse != null && year == 0)
            {
                var randomString = RandomString();
                slug = ToFriendlyUrl(title + " " + RandomString() + " " + RandomString() + " " + RandomString() + " " + RandomString());
            }

            return slug;
        }

        private static string PersonSlugSearch(MovieContext _ctx, string title, int year = 0)
        {
            var slug = ToFriendlyUrl(title);
            var slugAlreadyInUse = _ctx.Persons.Where(x => x.Slug == slug).FirstOrDefault();
            if (slugAlreadyInUse != null && year != 0)
            {
                slug = ToFriendlyUrl(title + " " + year);
                var slugYearAlreadyInUse = _ctx.Persons.Where(x => x.Slug == slug).FirstOrDefault();
                if (slugYearAlreadyInUse != null)
                {
                    slug = ToFriendlyUrl(title + " " + RandomString() + " " + RandomString() + " " + RandomString() + " " + RandomString());
                } 
            } else if (slugAlreadyInUse != null && year == 0)
            {
                var randomString = RandomString();
                slug = ToFriendlyUrl(title + " " + RandomString() + " " + RandomString() + " " + RandomString() + " " + RandomString());
            }

            return slug;
        }

        public static string ListSlugSearch(MovieContext _ctx, List list, string user)
        {   
            var slug = ToFriendlyUrl(list.Name);
            var slugAlreadyInUse = _ctx.Lists.Where(l => l.Name == slug).Where(u => u.ApplicationUserId == user).Where(lu => lu.Id != list.Id).FirstOrDefault(); 
            
            if (slugAlreadyInUse != null)
            {
                slug = ToFriendlyUrl(list.Name + " " + RandomString());
            }

            return slug;
        }

        private static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }
    }
}