using Blazor.Ninja.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Ninja.KickStart.Common
{
    public class CustomUser: User
    {
        public long Followers { get; set; }
        public long Following { get; set; }
    }
}
