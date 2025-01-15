using Lab2.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Lab2.Data.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly OrderLineContext _context;

        public UsersRepository(OrderLineContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            // Search for the email in the Users table
            return await _context.Users
                .AnyAsync(u => u.email == email);
        }

        public async Task<bool> PhoneNrExistsAsync(string phoneNr)
        {
            // Search for the phone number in the Users table
            return await _context.Users
                .AnyAsync(u => u.phonenr == phoneNr);
        }
    }
}