using System.Collections.Generic;

namespace Builder.Core
{
    public abstract class ListBuilder<TBuilder, TItem> : CollectionBuilder<TBuilder, List<TItem>, TItem>
        where TBuilder : ListBuilder<TBuilder, TItem>, new()
    {
    }

    public class ListBuilder<TItem> : ListBuilder<ListBuilder<TItem>, TItem>
    {
        protected override ListBuilder<TItem> Instance()
        {
            return this;
        }
    }
}
