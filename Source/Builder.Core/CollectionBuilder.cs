using System;
using System.Collections.Generic;

namespace Builder.Core
{
    public abstract class CollectionBuilder<TBuilder, TCollection, TItem> : Builder<TBuilder, TCollection>
        where TBuilder : CollectionBuilder<TBuilder, TCollection, TItem>, new()
        where TCollection : ICollection<TItem>, new()
    {
        public TBuilder Add(TItem item)
        {
            return With(c =>
            {
                c.Add(item);
            });
        }

        public TBuilder Add(Func<Builder<TItem>, Builder<TItem>> setup = null)
        {
            return With(c =>
            {
                c.Add(Builder<TItem>.Create(setup));
            });
        }

        public TBuilder Add(int count, Func<Builder<TItem>, Builder<TItem>> setup = null)
        {
            return With(c =>
            {
                for (int i = 0; i < count; i++)
                {
                    c.Add(Builder<TItem>.Create(setup));
                }
            });
        }

        public TBuilder Add(int count, Func<int, Func<Builder<TItem>, Builder<TItem>>> setups)
        {
            return With(c =>
            {
                for (int i = 0; i < count; i++)
                {
                    Func<Builder<TItem>, Builder<TItem>> setup = setups == null ? b => b : setups(i);
                    c.Add(Builder<TItem>.Create(setup));
                }
            });
        }
    }

    public class CollectionBuilder<TCollection, TItem> : CollectionBuilder<CollectionBuilder<TCollection, TItem>, TCollection, TItem>
        where TCollection : ICollection<TItem>, new()
    {
        protected override CollectionBuilder<TCollection, TItem> Instance()
        {
            return this;
        }
    }
}
