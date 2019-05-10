using Muwesome.DomainModel.Entities;
using OneOf;

namespace Muwesome.GameLogic {
  public class AccountOrLoginError : OneOfBase<Account, Actions.LoginResult> {
  }
}