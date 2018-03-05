using System.Collections.Generic;

namespace Builder.Core.Test.TestObjects
{
    public class ParentObject
    {
        public int Val1 { get; set; }
        public string Val2 { get; set; }
        public List<ChildObject> List { get; set; }
    }
}
