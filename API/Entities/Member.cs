
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class Member : IdentityUser<int>
    {
        public DateTime DateOfBirth { get; set; }

        public string KnownAs { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public ICollection<Photo> Photos { get; set; }

        public ICollection<MemberLike> LikedByMembers { get; set;}

        public ICollection<MemberLike> LikedMembers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }

        public ICollection<Message> MessagesReceived { get; set; }

        public ICollection<MemberRole> MemberRoles { get; set; }


        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }

    }
}
