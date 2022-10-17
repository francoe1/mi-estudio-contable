using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace MiEstudio.Server.Reports
{
    public class FileCSV
    {
        private const char DELIMETER = ';';
        private StringBuilder _content { get; } = new StringBuilder();

        public void AddHeader(params object[] headers)
        {
            AddRow(headers);
        }

        public void AddRow(params object[] values)
        {
            _content.AppendLine(string.Join(DELIMETER, values.Select(x => x == null ? "" : x.ToString())));
        }

        public override string ToString() => _content.ToString();

        public byte[] ToBytes() => Encoding.UTF8.GetBytes(ToString());
    }
}