# Soatech.Builder.Core
Core components for utilizing the builder pattern in test  object construction.

Designed to 
1. Create objects using default Builder without the need of a  separate class: 
``` csharp
var simpleObject = Builder<SimpleObject>.Create();

var simpleObject = Builder<SimpleObject>.Create(ob => ob
    .With(o => o.Prop1 = "Test")
    .With(o => o.Prop2 = 2));
```
2. Create more complex objects fluently:
``` csharp
var complexObject = Builder<ComplexObject>.Create(b => b
    .WithCtorArg(() => Builder<SimpleObject>.Create())
    .With(co => co.List = ListBuilder.Create(lb => lb
        .Add(Builder<ItemObject>.Create())
        .Add(Builder<ItemObject>.Create()))));
```
3. Easily create own builders to increase readability:
``` csharp

public class ComplexObjectBuilder : Builder<ComplexObjectBuilder, ComplexObject>
{    
    public ComplexObjectBuilder CallInit()
    {
        return With(o => 
        {
            o.Init("SET", "VAL1");
        });
    }

    protected override OnBuild(ComplexObject obj) 
    {
        if (obj.Param1 == "")
            obj.Param1 = "UNSET";        
    }

    protected override Instance() 
    {
        return this;
    }
}

[TestClass]
public class ComplexObjectBuilder
{
    [Test]
    public void TestMethod()
    {
        var testObj = ComplexObjectBuilder.Create(cb => cb
            .With(o => o.SomeProperty = "Value")
            .CallInit());
        
        Assert.IsTrue("SET", testObj.Param1);
    }
}
```