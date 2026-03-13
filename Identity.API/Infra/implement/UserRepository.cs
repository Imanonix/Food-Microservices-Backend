using Domain.interfaces;
using Domain.models;
using Infra.identityDb;
using Microsoft.EntityFrameworkCore;


namespace Infra.implement
{

    public class UserRepository : IUserRepository
    {
        private readonly IdentityDb _context;

        public UserRepository(IdentityDb context) 
        {
            _context = context;
        }

        // ==========================================
        // Core CRUD Operations
        // ==========================================
        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<List<User>?> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
             _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountUserAsync()
        {
            return await _context.Users.CountAsync();
        }

        // ==========================================
        // Identity & Security Specifics
        // ==========================================

        public async Task<User?> GetByForgotPasswordTokenAsync(string forgotPasswordToken)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.ForgotPasswordToken == forgotPasswordToken);
        }

        public async Task<List<User>> GetLatestUsersAsync(int count)
        {
            var users = await _context.Users.OrderByDescending(u => u.CreatedAt).Take(10).ToListAsync();
            return users;
        }

        // ==========================================
        // Specialized Queries (Projection/Include)
        // ==========================================

        public async Task<UserProfileInfo?> GetUserWithAddressesByIdAsync(Guid id)
        {
            var user = await _context.Users.Include( u => u.Addresses ).Where(u => u.Id == id).Select(
                u => new UserProfileInfo
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Addresses = u.Addresses.Select(ua => new AddressInfo
                    {
                        Id = ua.Id,
                        Title = ua.Title,
                        FullAddress = ua.FullAddress,
                        City = ua.City,
                    }).ToList(),

                    }).SingleOrDefaultAsync();
            return user;
        }

        public async Task<UserProfileInfo?> GetUserWithAddressesByEmailAsync(string email)
        {
            var user = await _context.Users.Include(u => u.Addresses).Where(u => u.Email == email).Select(
                u => new UserProfileInfo
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Addresses = u.Addresses.Select(ua => new AddressInfo
                    {
                        Id = ua.Id,
                        Title = ua.Title,
                        FullAddress = ua.FullAddress,
                        City = ua.City,
                    }).ToList(),

                }).SingleOrDefaultAsync();
            return user;
        }
       
    }

}
