// Obsolete

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PsiBB.DataAccess
{
    public interface IHasTimestamps
    {
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
    }
}
