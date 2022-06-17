using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.Entities
{
    public class ListEntityItem
    {
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidCastException($"{nameof(ListEntityItem)}.{nameof(Value)} can't be empty or null.");
                _value = value;
            }
        }
        public string[] Synonyms { get; set; }
    }
}
