using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melon.Reports
{
    public class DataReaderAdapter : IDataReaderAdapter
    {
        private IDataReader dataReader;

        public DataReaderAdapter(IDataReader dataReader)
        {
            this.dataReader = dataReader;
        }

        public IEnumerable<object> GetData()
        {
            while (dataReader.Read())
             {
                    yield return dataReader;
            }
        }

        public object GetValue(string name)
        {
            return (dataReader)[name];
        }
    }
}
