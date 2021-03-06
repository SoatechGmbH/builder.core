﻿using System;
using System.Collections.Generic;

namespace Soatech.Builder.Core
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

        public TBuilder Add(int count, Func<int, Builder<TItem>, Builder<TItem>> setups)
        {
            return With(c =>
            {
                for (int i = 0; i < count; i++)
                {
                    c.Add(Builder<TItem>.Create(b => setups(i, b)));
                }
            });
        }
    }

    public class CollectionBuilder<TCollection, TItem> : CollectionBuilder<CollectionBuilder<TCollection, TItem>, TCollection, TItem>
        where TCollection : ICollection<TItem>, new()
    {
    }
}
