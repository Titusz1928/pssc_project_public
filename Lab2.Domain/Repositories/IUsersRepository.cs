using System.Threading.Tasks;

namespace Lab2.Data.Repositories
{
    public interface IUsersRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNrExistsAsync(string phoneNr);
    }
}