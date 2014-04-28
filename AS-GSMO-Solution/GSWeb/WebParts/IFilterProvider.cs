using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSWeb.WebParts
{
    public interface IFilterProvider
    {
        int CurrentListItemId { get; }
    }
}
