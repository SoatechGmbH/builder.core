using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soatech.Builder.Core.Test.TestObjects
{
    public class ComplexObjectBuilder : Builder<ComplexObjectBuilder, ComplexObject>
    {
        public ComplexObjectBuilder CallInit()
        {
            return With(o =>
            {
                o.Init("SET", "VAL1");
            });
        }

        protected override void OnBuild(ComplexObject obj)
        {
            if (obj.Param1 == "")
                obj.Init("UNSET", null);
        }
    }
}
