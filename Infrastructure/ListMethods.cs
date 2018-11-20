using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MovieProject.Models;

namespace MovieProject.Infrastructure
{
    public class ListMethods
    {
        public static void SaveListItem(MovieContext _ctx, List list, FilmItem filmItem)
        {
            var filmItemAlreadyInList = _ctx.ListItems.Where(l => l.List == list).Where(f => f.FilmItem == filmItem).FirstOrDefault();

            if (filmItem != null && filmItemAlreadyInList == null)
            {
                ListItem li = new ListItem()
                {
                    FilmItem = filmItem,
                    List = list
                };

                _ctx.ListItems.Add(li);
                _ctx.SaveChanges();

                _ctx.Lists.Attach(list);
                list.ItemCount++;
                list.UpdatedAt = DateTime.Now;
                _ctx.SaveChanges();
            }
        }

        public static void EditListAfterDeletingListItem(MovieContext _ctx, ListItem listItem)
        {
            var list = listItem.List;
            _ctx.Lists.Attach(list);
            listItem.List.ItemCount--;
            _ctx.SaveChanges();
        }
    }
}