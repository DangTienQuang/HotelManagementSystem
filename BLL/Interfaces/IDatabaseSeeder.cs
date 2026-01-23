using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}
