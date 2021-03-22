using System.Threading.Tasks;
using Entity;

namespace BusinessLogic.Interfaces
{
    public interface ITokenService
    {
         Task<string> CreateToken(AppUser user);
    }
}