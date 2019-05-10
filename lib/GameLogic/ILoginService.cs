using System.Threading.Tasks;

namespace Muwesome.GameLogic {
  public interface ILoginService {
    Task<AccountOrLoginResult> Login(string username, string password);
  }
}