using System;
using System.Collections.Generic;

namespace OnWeb.Core.Modules
{
    using Items;

    interface IModuleCoreInternal
    {
        IReadOnlyDictionary<ItemBase, Uri> GenerateLinks(IEnumerable<ItemBase> items);

        int IdModule { get; }
    }
}
