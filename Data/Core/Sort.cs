namespace MiEstudio.Data.Core
{
    public class Sort<T>
    {
        public T Property { get; set; }
        public SortOperator Operator { get; set; }
    }
}