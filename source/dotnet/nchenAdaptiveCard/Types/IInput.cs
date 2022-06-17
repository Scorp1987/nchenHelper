using AdaptiveCards.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveCards.Types
{
    public interface IInput
    {
        ElementType Type { get; }
        string ID { get; set; }
        TextRun[] Title { get; }
        TextRun[] ErrorMessage { get; set; }
    }
}
