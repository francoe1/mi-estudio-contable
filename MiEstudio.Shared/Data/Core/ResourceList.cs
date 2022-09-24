namespace MiEstudio.Shared.Data.Core
{
    public partial class ResourceList<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
        public int PerPage { get; set; } = 30;
        public ICollection<T> Data { get; set; } = new List<T>();

        public ResourceList()
        { }

        public ResourceList(ICollection<T> data, int total, int page, int perPage)
        {
            Total = total;
            Data = data;
            Page = page;
            PerPage = perPage;
            Pages = Total / PerPage;
        }
    }
}