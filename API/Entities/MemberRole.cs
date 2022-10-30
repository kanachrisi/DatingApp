using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class MemberRole : IdentityUserRole<int>
    {
        public Member Member { get; set; }

        public AppRole Role { get; set; }
    }
}
