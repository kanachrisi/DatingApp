using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public async Task<Member> GetMemberByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Member> GetMemberByUsernameAsync(string username)
        {
            return await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<PaginatedList<Member>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                 .Include(p => p.Photos)
                 .AsNoTracking()
                 .AsQueryable();

            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            return await PaginatedList<Member>.CreatePaginatedListAsync(query, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Member user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}
