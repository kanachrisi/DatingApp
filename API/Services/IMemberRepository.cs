using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface IMemberRepository
    {
        public void Update(Member user);

        public Task<bool> SaveAllAsync();

        public Task<PaginatedList<Member>> GetMembersAsync(UserParams userParams);

        public Task<Member> GetMemberByIdAsync(int id);

        public Task<Member> GetMemberByUsernameAsync(string username);
    }
}
