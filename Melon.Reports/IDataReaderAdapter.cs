using System.Collections.Generic;

namespace Melon.Reports
{
    public interface IDataReaderAdapter
    {
        IEnumerable<object> GetData();
        object GetValue(string name);
    }
}