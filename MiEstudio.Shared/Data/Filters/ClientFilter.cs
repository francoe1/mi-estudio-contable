using MiEstudio.Shared.Data.Core;
using System.Collections.Generic;

namespace MiEstudio.Shared.Data.Filters
{
    public enum ClientSort
    {
        Name,
        Type,
        Balance,
        Expense,
        City
    }

    public class ClientFilter : Paginate
    {
        public string Search { get; set; }
        public ClientSort Sort { get; set; }

        protected override IEnumerable<KeyValuePair<string, string>> FilterCollection()
        {
            yield return new KeyValuePair<string, string>("Search", Search);
            yield return new KeyValuePair<string, string>("Sort", Sort.ToString());
        }
    }
}