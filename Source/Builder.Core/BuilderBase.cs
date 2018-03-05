using System;
using System.Collections.Generic;

namespace Builder.Core
{
    public abstract class BuilderBase<TBuilder, TObject>
        where TBuilder : BuilderBase<TBuilder, TObject>, new()
    {

        protected abstract TBuilder Instance();

        protected abstract TObject CreateObjectInstance();

        protected IList<Action<TObject>> Setups = new List<Action<TObject>>();

        public TBuilder With(Action<TObject> setup)
        {
            Setups.Add(setup);
            return Instance();
        }

        public static TObject Create(Func<TBuilder, TBuilder> setup = null)
        {
            if (setup == null)
                setup = b => b;

            var builder = setup(new TBuilder());
            return builder.Build();
        }

        public TObject Build()
        {
            var obj = CreateObjectInstance();
            foreach (var setup in Setups)
            {
                setup(obj);
            }
            OnBuild(obj);
            return obj;
        }

        protected virtual void OnBuild(TObject dto)
        {
        }
    }
}
