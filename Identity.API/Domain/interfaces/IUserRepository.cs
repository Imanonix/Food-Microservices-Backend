using Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.interfaces
{
    public interface IUserRepository
    {
        // ==========================================
        // Core CRUD Operations
        // ==========================================

        /// <summary> Retrieves a user by their unique identifier. </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary> Retrieves a user by their email address. </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary> Retrieves all users. </summary>
        Task<List<User>?> GetAllAsync();

        /// <summary> Adds a new user entity to the database. </summary>
        Task AddAsync(User user);

        /// <summary> Marks a user entity as modified. </summary>
        void Update(User user);

        /// <summary> Hard Remove /// </summary>
        void Delete(User user);

        /// <summary> Persists all changes made in the context to the database. </summary>
        Task SaveChangesAsync();

        Task<int> CountUserAsync();
        // ==========================================
        // Identity & Security Specifics
        // ==========================================

        /// <summary> Finds a user associated with a specific password reset token. </summary>
        Task<User?> GetByForgotPasswordTokenAsync(string token);

        /// <summary> Retrieves a list of the most recently registered users. </summary>
        Task<List<User>> GetLatestUsersAsync(int count);


        // ==========================================
        // Specialized Queries (Projection/Include)
        // ==========================================

        /// <summary> Retrieves a user and eagerly loads their associated addresses. </summary>
        Task<UserProfileInfo?> GetUserWithAddressesByIdAsync(Guid id);

        /// <summary> Retrieves a user by their email and eagerly loads their associated addresses. </summary>
        Task<UserProfileInfo?> GetUserWithAddressesByEmailAsync(string email);
    }
}
