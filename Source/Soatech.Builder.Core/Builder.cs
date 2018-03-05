using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Soatech.Builder.Core
{
    public abstract class Builder<TBuilder, TObject> : BuilderBase<TBuilder, TObject>
        where TBuilder : Builder<TBuilder, TObject>, new()
    {
        List<NamedCtorArgsBuilder> ctorArgsBuilder = new List<NamedCtorArgsBuilder>();
        static ConstructorInfo[] knownCtors;

        static Builder()
        {
            var type = typeof(TObject);
            knownCtors = type.GetTypeInfo().DeclaredConstructors
                .Where(c => c.IsPublic).ToArray();
        }

        public TBuilder WithCtorArg<TArg>(TArg arg)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(() => arg));
            return Instance();
        }

        public TBuilder WithCtorArg(Func<object> argFnc)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(argFnc));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(string name, TArg arg)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(name, () => arg));
            return Instance();
        }

        public TBuilder WithCtorArg(string name, Func<object> argFnc)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(name, argFnc));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(Func<Builder<TArg>, Builder<TArg>> setup = null)
            where TArg : new()
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(() => Builder<TArg>.Create(setup)));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(string name, Func<Builder<TArg>, Builder<TArg>> setup = null)
            where TArg : new()
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(name, () => Builder<TArg>.Create(setup)));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg, TArgBuilder>(Func<TArgBuilder, TArgBuilder> setup = null)
            where TArgBuilder : BuilderBase<TArgBuilder, TArg>, new()
        {
            if (setup == null)
                setup = b => b;
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(() => setup(new TArgBuilder()).Build()));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg, TArgBuilder>(string name, Func<TArgBuilder, TArgBuilder> setup = null)
            where TArgBuilder : BuilderBase<TArgBuilder, TArg>, new()
        {
            if (setup == null)
                setup = b => b;
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(name, () => setup(new TArgBuilder()).Build()));
            return Instance();
        }

        protected override TObject CreateObjectInstance()
        {
            var args = ctorArgsBuilder
                .Select(c => new Tuple<string, object>(c.Name, c.ArgsBuilder()))
                .GroupBy(k => k.Item2.GetType())
                .ToDictionary(k => k.Key, k => k.ToList());

            var argNames = ctorArgsBuilder.Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name).ToList();

            foreach (var ctor in knownCtors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length < ctorArgsBuilder.Count
                    || argNames.Except(parameters.Select(p => p.Name)).Count() < 0)
                    continue;

                var usedArgs = new List<object>();

                foreach (var parameter in parameters)
                {
                    if (args.ContainsKey(parameter.ParameterType) 
                        && args[parameter.ParameterType].Count > 0)
                    {
                        var typeArgs = args[parameter.ParameterType];
                        var namedArg = typeArgs.Any(a => a.Item1 == parameter.Name) 
                            ? typeArgs.First(a => a.Item1 == parameter.Name)
                            : typeArgs.FirstOrDefault(a => string.IsNullOrEmpty(a.Item1));

                        usedArgs.Add(namedArg == null ? Default(parameter.ParameterType) : namedArg.Item2);
                        args[parameter.ParameterType].Remove(namedArg);
                    }
                    else
                    {
                        // Create default instance for parameter
                        usedArgs.Add(Default(parameter.ParameterType));
                    }
                }

                return (TObject)ctor.Invoke(usedArgs.ToArray());
            }

            //No suitable constructor found. Return default instance.
            return default(TObject);
        }

        private static object Default(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        private class NamedCtorArgsBuilder
        {
            public string Name { get; }
            public Func<object> ArgsBuilder { get; }
            public NamedCtorArgsBuilder(Func<object> argsBuilder) : this(null, argsBuilder)
            {
            }

            public NamedCtorArgsBuilder(string name, Func<object> argsBuilder) 
            {
                Name = name;
                ArgsBuilder = argsBuilder;
            }
        }
    }

    public class Builder<TObject> : Builder<Builder<TObject>, TObject>
    {
        protected override Builder<TObject> Instance()
        {
            return this;
        }
    }
}
