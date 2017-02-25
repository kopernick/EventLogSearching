using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogSearching.Service
{
    public class SearchElement
    {
        public string Name { get; private set; }
        public string FieldName { get; private set; }
        public string[] keyword { get; private set; }

       
        public SearchElement(string[] value)
        {
            keyword = value;
        }
        public SearchElement(string[] value, string _fieldName)
            : this(value)
        {
            FieldName = _fieldName;
        }
        public SearchElement(string[] value, string _fieldName,string _name)
           : this(value, _fieldName)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return FieldName;
        }
    }
}
