using TypicalTechTools.Models.DTOs;

namespace TypicalTechTools.Models.Repositories
{
    public interface IAuthenticationRepository
    {
        AppUser Authenticate(LoginDTO loginDto);
        AppUser CreateUser(CreateUserDTO userDto);
    }
}
