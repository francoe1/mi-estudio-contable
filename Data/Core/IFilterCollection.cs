namespace MiEstudio.Data.Core
{
    public interface IFilterCollection
    {
        IEnumerable<KeyValuePair<string, string>> GetFilterCollection();
    }
}