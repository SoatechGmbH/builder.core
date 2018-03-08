[<img src="https://soatech.visualstudio.com/_apis/public/build/definitions/d4cb291c-743a-44b0-91fe-6139e3974dd2/4/badge" />](https://soatech.visualstudio.com/Soatech%20Builder%20Tools/_build/index?definitionId=4)

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
    .With(co => co.List = ListBuilder<ItemObject>.Create(lb => lb
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

    protected override void OnBuild(ComplexObject obj)
    {
        if (obj.Param1 == "")
            obj.Init("UNSET", null);
    }
}

[TestFixture]
public class ComplexObjectBuilderTest
{
    [Test]
    public void TestMethod()
    {
        var testObj = ComplexObjectBuilder.Create(cb => cb
            .With(o => o.SomeProperty = "Value")
            .CallInit());

        Assert.AreEqual("SET", testObj.Param1);
    }
}
```