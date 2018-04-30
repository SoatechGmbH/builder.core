using System;

namespace Soatech.Builder.Core.Test.TestObjects
{
    public class ObjectWithInterfaceConstructor
    {
        public IFormatProvider FormatProvider { get; }

        public ObjectWithInterfaceConstructor(IFormatProvider formatProvider)
        {
            FormatProvider = formatProvider;
        }
    }
}
