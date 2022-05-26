using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine
{
    public static class ErrorExtension
    {
        public static string GetErrorMessage(this Error error)
        {
            if (error is BadFormatTokenError bfte)
                return $"Token '{bfte.Token}' is not recognized";
            else if (error is MissingValueOptionError mvoe)
                return $"Option '{mvoe.NameInfo.NameText}' has no value";
            else if (error is UnknownOptionError uoe)
                return $"Option '{uoe.Token}' is unknown";
            else if (error is MissingRequiredOptionError mroe)
                return mroe.NameInfo.Equals(NameInfo.EmptyName)
                    ? "A required value not bound to option name is missing"
                    : $"Required option '{mroe.NameInfo.NameText}' is missing";
            else if (error is BadFormatConversionError bfce)
                return bfce.NameInfo.Equals(NameInfo.EmptyName)
                    ? "A value not bound to option name is defined with a bad format"
                    : $"Option '{bfce.NameInfo.NameText}' is defined with a bad format";
            else if (error is SequenceOutOfRangeError sofre)
                return sofre.NameInfo.Equals(NameInfo.EmptyName)
                    ? "A sequence value not bound to option name is defined with few items than required"
                    : $"A sequence option '{sofre.NameInfo.NameText}' is defined with fewer or more items than required";
            else if (error is BadVerbSelectedError bvse)
                return $"Verb '{bvse.Token}' is not recognized";
            else if (error is NoVerbSelectedError)
                return $"No verb selected";
            else if (error is RepeatedOptionError roe)
                return $"Option '{roe.NameInfo.NameText}' is defined multiple times";
            else if (error is SetValueExceptionError sve)
                return $"Error setting value to option '{sve.NameInfo.NameText}': {sve.Exception.Message}";
            else if (error is MissingGroupOptionError mgoe)
                return $"At least one option from group '{mgoe.Group}' ({string.Join(",", mgoe.Names.Select(n => n.NameText))}) is required";
            else if (error is GroupOptionAmbiguityError goae)
                return $"Both SetName and Group are not allowed in option: ({goae.Option.NameText})";
            else if (error is MultipleDefaultVerbsError mdve)
                return MultipleDefaultVerbsError.ErrorMessage;

            throw new InvalidOperationException();
        }
    }
}
