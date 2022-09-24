namespace MiEstudio.Shared.Data.Core
{
    public interface IFilterCollection
    {
        IEnumerable<KeyValuePair<string, string>> GetFilterCollection();
    }
}