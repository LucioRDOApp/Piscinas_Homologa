using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace laudosPiscinasProject.Api.Models
{
    public class PagedList
    {
        public int currentPage { get; set; }
        public List<int> pages { get; set; }
        public object list { get; set; }

        public static PagedList create(int page, int pagesize, object list)
        {
            PagedList pagedList = new PagedList();
            try
            {
                page = page - 1;
                int total = (list as IList).Count;
                int maxPages = (int)Math.Ceiling((double)total / (double)pagesize);
                if(page >= maxPages)
                {
                    page = maxPages-1;
                }
                if(page < 0)
                {
                    page = 0;
                }

                pagedList.pages = new List<int>();
                pagedList.currentPage = page + 1;

                int lastcount = 0;
                pagedList.pages.Add(pagedList.currentPage);
                while (pagedList.pages.Count != lastcount && pagedList.pages.Count < 5)
                {
                    lastcount = pagedList.pages.Count;
                    pagedList.pages.Insert(0, pagedList.pages.First()-1);
                    pagedList.pages.Add(pagedList.pages.Last() + 1);
                    pagedList.pages.RemoveAll(p => p <= 0);
                    pagedList.pages.RemoveAll(p => p > maxPages);
                }

                pagedList.list = (list as IEnumerable<object>).Skip(page * pagesize).Take(pagesize).ToList();
                return pagedList;
            }
            catch (Exception)
            {
                pagedList.currentPage = 1;
                pagedList.pages = new List<int>();
                pagedList.list = list;
                return pagedList;
            }
        }
    }
}