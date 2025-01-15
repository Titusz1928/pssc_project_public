using static Lab2.Domain.Models.Awb;

using Lab2.Domain.Models;

namespace Lab2.Domain.Repositories
{
    public interface IAwbRepository
    {
        Task AddAwb(IAwb awb);
    }
}