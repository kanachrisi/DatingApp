using API.Entities;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MemberRepository : IMemberRepository
    {
        private readonly DataContext _context;

        public MemberRepository(DataContext context)
        {
            _context = context;
        }

        async Task<Member> IMemberRepository.GetMemberByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        async Task<Member> IMemberRepository.GetMemberByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == username);
        }

        async Task<IEnumerable<Member>> IMemberRepository.GetMembersAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        async Task<bool> IMemberRepository.SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        void IMemberRepository.Update(Member user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
