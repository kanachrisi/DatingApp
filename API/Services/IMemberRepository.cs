using API.Entities;

namespace API.Services
{
    public interface IMemberRepository
    {
        void Update(Member user);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<Member>> GetMembersAsync();

        Task<Member> GetMemberByIdAsync(int id);

        Task<Member> GetMemberByUsernameAsync(string username);
    }
}
