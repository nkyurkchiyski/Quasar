﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quasar.Web.Utils
{
    public class PaginatedList<T> : List<T>
    {

        public PaginatedList(ICollection<T> items, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public int PageIndex { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(
            ICollection<T> source, int pageIndex, int pageSize)
        {
            if (!source.Any() || source == null)
            {
                return null;
            }

            var count = source.Count;

            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
