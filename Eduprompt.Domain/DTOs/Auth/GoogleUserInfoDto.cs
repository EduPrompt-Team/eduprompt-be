namespace Eduprompt.Domain.DTOs.Auth;

/// <summary>
/// DTO for Google user information from Google API
/// </summary>
public class GoogleUserInfoDto
{
    /// <summary>
    /// Google user ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// User's full name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string GivenName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string FamilyName { get; set; } = string.Empty;
    
    /// <summary>
    /// User's profile picture URL
    /// </summary>
    public string Picture { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether email is verified
    /// </summary>
    public bool VerifiedEmail { get; set; }
}
