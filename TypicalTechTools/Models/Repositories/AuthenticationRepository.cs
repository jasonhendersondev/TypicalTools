using TypicalTechTools.Models.Data;
using TypicalTechTools.Models.DTOs;

namespace TypicalTechTools.Models.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        //Readonly variable to store a reference to context class
        private readonly TypistTechToolsDBContext _context;
        //Request the context from the dependency injection by naming it in the constructor
        public AuthenticationRepository(TypistTechToolsDBContext context)
        {
            _context = context;
        }

        public AppUser Authenticate(LoginDTO loginDto)
        {
            //Find the user that has the same username as the one provided in the login DTO
            var userDetails = _context.AppUsers.Where(u => u.UserName.Equals(loginDto.UserName)).FirstOrDefault();
            //If no user was found, return null to let the caller know that the login failed.
            if (userDetails == null)
            {
                return null;
            }
            //Use Bcrypt to check the password provided in the user DTO against the hashed password stored in the
            //user account we just retrieved.
            if (BCrypt.Net.BCrypt.EnhancedVerify(loginDto.Password, userDetails.Password))
            {
                //If they match, return the user details to the caller to let them know it worked.
                return userDetails;
            }
            //If the check failed, return null to let caller know the login failed
            return null;
        }

        public AppUser CreateUser(CreateUserDTO userDto)
        {
            //Find the user that has the same username as the one provided in the login DTO
            var userDetails = _context.AppUsers.Where(u => u.UserName.Equals(userDto.UserName)).FirstOrDefault();
            //If the username returns a record, meaning the username is already taken.
            if (userDetails != null)
            {
                //Return null to the caller to let them know the account couldn't be created.
                return null;
            }

            var user = new AppUser
            {
                Email = userDto.Email,
                UserName = userDto.UserName,
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDto.Password),
                Role = "GUEST"
            };
            //Add the user to the context class then save the changes to the database
            _context.AppUsers.Add(user);
            _context.SaveChanges();

            //Return the user details to the caller to confirm it worked.
            return user;
        }
    }
}
