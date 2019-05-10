using System.Threading.Tasks;

namespace Muwesome.GameLogic {
  public interface ILoginService {
    Task<AccountOrLoginError> Login(string username, string password);
  }
}