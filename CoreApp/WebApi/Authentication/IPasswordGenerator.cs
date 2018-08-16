using Microsoft.AspNetCore.Identity;

namespace WebApi.Authentication
{
    public interface IPasswordGenerator
    {
        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        string Generate(PasswordOptions opts = null);
    }
}