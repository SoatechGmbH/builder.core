using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soatech.Builder.Core.Test.TestObjects
{
    public class ComplexObject
    {
        public string Param1 { get; private set; }
        public string Param2 { get; private set; }
        public string SomeProperty { get; set; }

        public void Init(string param1, string param2)
        {
            Param1 = param1;
            Param2 = param2;
        }
    }
}
