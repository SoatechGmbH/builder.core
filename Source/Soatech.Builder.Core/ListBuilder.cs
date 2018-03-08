using System.Collections.Generic;

namespace Soatech.Builder.Core
{
    public abstract class ListBuilder<TBuilder, TItem> : CollectionBuilder<TBuilder, List<TItem>, TItem>
        where TBuilder : ListBuilder<TBuilder, TItem>, new()
    {
    }

    public class ListBuilder<TItem> : ListBuilder<ListBuilder<TItem>, TItem>
    {
    }
}
