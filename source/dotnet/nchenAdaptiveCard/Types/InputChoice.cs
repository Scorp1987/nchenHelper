using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveCards.Types
{
    public class InputChoice
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public override string ToString() => Title;
    }
}
