using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.NLP.Entities
{
    public abstract class AEntity
    {
        public bool IsOutput { get; set; } = true;
        public bool Required { get; set; } = false;
    }
}
