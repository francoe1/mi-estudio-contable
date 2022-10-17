using System;
using System.Collections.Generic;

namespace MiEstudio.Shared.Data.Core
{
    public class Paginate : IFilterCollection
    {
        private int _page { get; set; }
        private int _pageSize { get; set; } = 30;

        public int PerPage
        {
            get => Math.Max(_pageSize, 1);
            set => _pageSize = value;
        }

        public int Page
        {
            get => Math.Max(_page, 1);
            set => _page = value;
        }

        public int Skip => PerPage * (Page - 1);
        public Guid FromRequestUserId { get; set; }
        public string SearchFilter { get; set; }

        public static Paginate Default { get; } = new Paginate { PerPage = 30 };

        IEnumerable<KeyValuePair<string, string>> IFilterCollection.GetFilterCollection()
        {
            yield return new KeyValuePair<string, string>("PerPage", PerPage.ToString());
            yield return new KeyValuePair<string, string>("Page", Page.ToString());
            if (!string.IsNullOrEmpty(SearchFilter)) yield return new KeyValuePair<string, string>("SearchFilter", SearchFilter);
            foreach (KeyValuePair<string, string> param in FilterCollection()) yield return param;
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> FilterCollection()
        {
            yield break;
        }
    }
}