namespace MiEstudio.Shared.Data.Core
{
    public class FilterProperty<T>
    {
        public T Value { get; set; }

        public ComparerOperator Operator { get; set; }
    }
}