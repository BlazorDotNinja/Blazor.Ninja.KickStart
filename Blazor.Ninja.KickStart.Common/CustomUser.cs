using Blazor.Ninja.Common.Data;

namespace Blazor.Ninja.KickStart.Common
{
    public class CustomUser: User
    {
        public long Followers { get; set; }
        public long Following { get; set; }
    }
}
