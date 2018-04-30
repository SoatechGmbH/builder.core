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
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), () => arg));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(Func<TArg> argFnc)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), () => argFnc()));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(string name, TArg arg)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), name, () => arg));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(string name, Func<TArg> argFnc)
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), name, () => argFnc()));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(Func<Builder<TArg>, Builder<TArg>> setup = null)
            where TArg : new()
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), () => Builder<TArg>.Create(setup)));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg>(string name, Func<Builder<TArg>, Builder<TArg>> setup = null)
            where TArg : new()
        {
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), name, () => Builder<TArg>.Create(setup)));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg, TArgBuilder>(Func<TArgBuilder, TArgBuilder> setup = null)
            where TArgBuilder : BuilderBase<TArgBuilder, TArg>, new()
        {
            if (setup == null)
                setup = b => b;
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), () => setup(new TArgBuilder()).Build()));
            return Instance();
        }

        public TBuilder WithCtorArg<TArg, TArgBuilder>(string name, Func<TArgBuilder, TArgBuilder> setup = null)
            where TArgBuilder : BuilderBase<TArgBuilder, TArg>, new()
        {
            if (setup == null)
                setup = b => b;
            ctorArgsBuilder.Add(new NamedCtorArgsBuilder(typeof(TArg), name, () => setup(new TArgBuilder()).Build()));
            return Instance();
        }

        protected override TObject CreateObjectInstance()
        {
            var args = ctorArgsBuilder
                .Select(c => new Tuple<NamedCtorArgsBuilder, object>(c, c.ArgsBuilder()))
                .GroupBy(k => k.Item1.Type)
                .ToDictionary(k => k.Key, k => k.Select(na => new Tuple<string, object>(na.Item1.Name, na.Item2)).ToList());

            var argNames = ctorArgsBuilder.Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c => c.Name).ToList();

            foreach (var ctor in knownCtors)
            {
                var parameters = ctor.GetParameters();
                if (parameters.Length < ctorArgsBuilder.Count
                    || argNames.Except(parameters.Select(p => p.Name)).Count() > 0)
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

            //No suitable constructor found.
            throw new InvalidOperationException($"No constructor for type {typeof(TObject)} with at least {ctorArgsBuilder.Count} parameters found.");
        }

        private static object Default(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        private class NamedCtorArgsBuilder
        {
            public string Name { get; }
            public Func<object> ArgsBuilder { get; }
            public Type Type { get; }

            public NamedCtorArgsBuilder(Type type, Func<object> argsBuilder) : this(type, null, argsBuilder)
            {
            }

            public NamedCtorArgsBuilder(Type type, string name, Func<object> argsBuilder) 
            {
                Type = type;
                Name = name;
                ArgsBuilder = argsBuilder;
            }
        }
    }

    public class Builder<TObject> : Builder<Builder<TObject>, TObject>
    {
    }
}
