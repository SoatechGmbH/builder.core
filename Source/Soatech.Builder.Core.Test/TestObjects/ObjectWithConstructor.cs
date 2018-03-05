using System.Collections.Generic;

namespace Soatech.Builder.Core.Test.TestObjects
{
    public class ObjectWithConstructor
    {
        public int Val1 { get; }
        public int Val2 { get; }
        public string Val3 { get; }
        public List<string> List { get; }

        public ObjectWithConstructor(int val1, int val2, string val3, List<string> list)
        {
            Val1 = val1;
            Val2 = val2;
            Val3 = val3;
            List = list;
        }
    }
}
