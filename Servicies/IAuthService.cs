/// <summary>Handles user authentication and registration</summary>
public interface IAuthService
{
    /// <summary>Creates a new user account</summary>
    /// <param name="model">User registration details</param>
    /// <returns>Authentication result with success status</returns>
    Task<AuthResponseDto> RegisterAsync(RegisterDto model);

    /// <summary>Authenticates user credentials</summary>
    /// <param name="model">User login details</param>
    /// <returns>Authentication result with JWT token if successful</returns>
    Task<AuthResponseDto> LoginAsync(LoginDto model);
}