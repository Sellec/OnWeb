using System;
using System.Collections.Generic;

namespace OnWeb.Core.Modules
{
    using Items;

    interface IModuleCoreInternal
    {
        IReadOnlyDictionary<ItemBase, Uri> GenerateLinks(IEnumerable<ItemBase> items);

        Extensions.ExtensionUrl.ExtensionUrl Urls { get; }

        int IdModule { get; }
    }
}
