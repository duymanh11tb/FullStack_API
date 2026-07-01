using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.SignalR;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Domain.Notifications.Enums;
using QLDA_PCCV.Infrastructure.Persistence;
using QLDA_PCCV.Hubs;

namespace QLDA_PCCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<NotificationHub> _hubContext;

    public AuthController(AppDbContext context, IConfiguration configuration, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _configuration = configuration;
        _hubContext = hubContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest(new { Message = "Username already exists." });
        }

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { Message = "Email already exists." });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            FullName = request.FullName,
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Initialize default notification settings for user
        user.NotificationSetting = new NotificationSetting
        {
            UserId = user.Id,
            OnComment = true,
            OnAssigned = true,
            OnStatusChanged = true,
            OnMention = true,
            OnSprintStarted = true,
            OnMemberAdded = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Registration successful." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid username/email or password." });
        }

        if (!user.IsActive)
        {
            return BadRequest(new { Message = "Account is inactive." });
        }

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            Token = token,
            User = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Role
            }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FullName,
                u.AvatarUrl,
                u.JobTitle,
                u.Department,
                u.Bio,
                u.GitHubUsername,
                u.Role,
                u.CreatedAt
            })
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        return Ok(user);
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        user.FullName = request.FullName;
        user.AvatarUrl = request.AvatarUrl;
        user.JobTitle = request.JobTitle;
        user.Department = request.Department;
        user.Bio = request.Bio;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Profile updated successfully." });
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        if (!VerifyPassword(request.OldPassword, user.PasswordHash))
        {
            return BadRequest(new { Message = "Mật khẩu hiện tại không chính xác." });
        }

        user.PasswordHash = HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Password changed successfully." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);
        if (user == null)
        {
            return NotFound(new { Message = "Tài khoản với email hoặc tên đăng nhập này không tồn tại." });
        }

        // For demo, reset to a default temporary password
        var tempPassword = "TempPassword123!";
        user.PasswordHash = HashPassword(tempPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Khôi phục mật khẩu thành công.", TempPassword = tempPassword });
    }

    [Authorize]
    [HttpGet("users/search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string? query = null)
    {
        var dbQuery = _context.Users.Where(u => u.IsActive);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lowerQuery = query.ToLower();
            dbQuery = dbQuery.Where(u => u.Username.ToLower().Contains(lowerQuery) || 
                                         u.Email.ToLower().Contains(lowerQuery) || 
                                         u.FullName.ToLower().Contains(lowerQuery));
        }

        var users = await dbQuery
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FullName
            })
            .Take(20)
            .ToListAsync();

        return Ok(users);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        return HashPassword(password) == passwordHash;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.FullName,
                u.Role,
                u.IsActive,
                u.CreatedAt,
                u.UpdatedAt
            })
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        if (user.Username != request.Username && await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest(new { Message = "Username already exists." });
        }
        if (user.Email != request.Email && await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { Message = "Email already exists." });
        }

        var oldRole = user.Role;
        var isRoleChanged = oldRole != request.Role;

        user.Username = request.Username;
        user.Email = request.Email;
        user.FullName = request.FullName;
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();

        if (isRoleChanged)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = NotificationType.MemberAdded,
                Message = $"Tài khoản của bạn đã được phân quyền thành {request.Role} bởi Admin.",
                ReferenceId = user.Id,
                ReferenceType = ReferenceType.Project,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            
            try
            {
                await _hubContext.Clients.Group(user.Id.ToString().ToLower()).SendAsync("ReceiveNotification", new
                {
                    id = notification.Id,
                    userId = notification.UserId,
                    type = notification.Type.ToString(),
                    message = notification.Message,
                    referenceId = notification.ReferenceId,
                    referenceType = notification.ReferenceType.ToString(),
                    isRead = notification.IsRead,
                    createdAt = notification.CreatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing real-time notification: {ex.Message}");
            }
        }

        return Ok(new { Message = "User updated successfully." });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.NotificationSetting)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdString, out var currentUserId) && currentUserId == id)
        {
            return BadRequest(new { Message = "You cannot delete your own account." });
        }

        var comments = _context.Comments.Where(c => c.UserId == id);
        _context.Comments.RemoveRange(comments);

        var logs = _context.ActivityLogs.Where(l => l.UserId == id);
        _context.ActivityLogs.RemoveRange(logs);

        var notifies = _context.Notifications.Where(n => n.UserId == id);
        _context.Notifications.RemoveRange(notifies);

        if (user.NotificationSetting != null)
        {
            _context.NotificationSettings.Remove(user.NotificationSetting);
        }
        _context.RefreshTokens.RemoveRange(user.RefreshTokens);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User deleted successfully." });
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromBody] ExternalLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            return BadRequest(new { Message = "Authorization code is required." });
        }

        var googleClientId = _configuration["Authentication:Google:ClientId"] 
            ?? "919341718553-3gdvt6v4d6t6m5s4i12o1i7hr8khm06m.apps.googleusercontent.com";
        var googleClientSecret = _configuration["Authentication:Google:ClientSecret"] 
            ?? Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

        if (string.IsNullOrEmpty(googleClientSecret))
        {
            return BadRequest(new { Message = "Google Client Secret is not configured on the server." });
        }

        try
        {
            using var httpClient = new HttpClient();
            
            // 1. Exchange authorization code for token
            var tokenParams = new Dictionary<string, string>
            {
                { "code", request.Code },
                { "client_id", googleClientId },
                { "client_secret", googleClientSecret },
                { "redirect_uri", request.RedirectUri },
                { "grant_type", "authorization_code" }
            };
            
            var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenParams));
            var tokenRawResult = await tokenResponse.Content.ReadAsStringAsync();
            if (!tokenResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"Google token exchange failed: {tokenRawResult}" });
            }

            var tokenJson = System.Text.Json.Nodes.JsonNode.Parse(tokenRawResult);
            var accessToken = tokenJson?["access_token"]?.ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(new { Message = "Access token was not returned from Google." });
            }

            // 2. Fetch user profile info
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var profileResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            var profileRawResult = await profileResponse.Content.ReadAsStringAsync();
            if (!profileResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"Failed to retrieve Google profile: {profileRawResult}" });
            }

            var profileJson = System.Text.Json.Nodes.JsonNode.Parse(profileRawResult);
            var email = profileJson?["email"]?.ToString();
            var fullName = profileJson?["name"]?.ToString() ?? email?.Split('@')[0] ?? "Google User";
            var picture = profileJson?["picture"]?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { Message = "Email address not provided by Google OAuth." });
            }

            // 3. Authenticate or Register User
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                var username = email.Split('@')[0];
                // Resolve username collision
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    username = $"{username}_{Guid.NewGuid().ToString("N").Substring(0, 5)}";
                }

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    PasswordHash = string.Empty, // External login, password not used
                    FullName = fullName,
                    AvatarUrl = picture,
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                user.NotificationSetting = new NotificationSetting
                {
                    UserId = user.Id,
                    OnComment = true,
                    OnAssigned = true,
                    OnStatusChanged = true,
                    OnMention = true,
                    OnSprintStarted = true,
                    OnMemberAdded = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update avatar if changed
                if (!string.IsNullOrEmpty(picture) && user.AvatarUrl != picture)
                {
                    user.AvatarUrl = picture;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            if (!user.IsActive)
            {
                return BadRequest(new { Message = "Account is inactive." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred during Google OAuth: {ex.Message}" });
        }
    }

    [HttpPost("github-login")]
    public async Task<IActionResult> GithubLogin([FromBody] ExternalLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            return BadRequest(new { Message = "Authorization code is required." });
        }

        var githubClientId = _configuration["Authentication:GitHub:ClientId"] ?? "Ov23lirIaP5cgYJ7mvKt";
        var githubClientSecret = _configuration["Authentication:GitHub:ClientSecret"] 
            ?? Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");

        if (string.IsNullOrEmpty(githubClientSecret))
        {
            return BadRequest(new { Message = "GitHub Client Secret is not configured on the server." });
        }

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "ProTask-App");

            // 1. Exchange code for access token
            var tokenParams = new Dictionary<string, string>
            {
                { "code", request.Code },
                { "client_id", githubClientId },
                { "client_secret", githubClientSecret },
                { "redirect_uri", request.RedirectUri }
            };

            var tokenResponse = await httpClient.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(tokenParams));
            var tokenRawResult = await tokenResponse.Content.ReadAsStringAsync();
            if (!tokenResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"GitHub token exchange failed: {tokenRawResult}" });
            }

            var tokenJson = System.Text.Json.Nodes.JsonNode.Parse(tokenRawResult);
            var accessToken = tokenJson?["access_token"]?.ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(new { Message = "Access token was not returned from GitHub." });
            }

            // 2. Fetch GitHub user profile
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var profileResponse = await httpClient.GetAsync("https://api.github.com/user");
            var profileRawResult = await profileResponse.Content.ReadAsStringAsync();
            if (!profileResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"Failed to retrieve GitHub profile: {profileRawResult}" });
            }

            var profileJson = System.Text.Json.Nodes.JsonNode.Parse(profileRawResult);
            var githubLogin = profileJson?["login"]?.ToString();
            var fullName = profileJson?["name"]?.ToString() ?? githubLogin ?? "GitHub User";
            var email = profileJson?["email"]?.ToString();
            var avatarUrl = profileJson?["avatar_url"]?.ToString();

            // 3. Fallback: retrieve email if private
            if (string.IsNullOrEmpty(email))
            {
                var emailsResponse = await httpClient.GetAsync("https://api.github.com/user/emails");
                if (emailsResponse.IsSuccessStatusCode)
                {
                    var emailsRaw = await emailsResponse.Content.ReadAsStringAsync();
                    var emailsArray = System.Text.Json.Nodes.JsonNode.Parse(emailsRaw)?.AsArray();
                    if (emailsArray != null && emailsArray.Count > 0)
                    {
                        // Look for primary email or the first one
                        foreach (var emailNode in emailsArray)
                        {
                            var isPrimary = emailNode?["primary"]?.GetValue<bool>() ?? false;
                            if (isPrimary || string.IsNullOrEmpty(email))
                            {
                                email = emailNode?["email"]?.ToString();
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { Message = "Email address not found or private in GitHub account." });
            }

            // 4. Authenticate or Register User
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                var username = githubLogin ?? email.Split('@')[0];
                // Resolve username collision
                if (await _context.Users.AnyAsync(u => u.Username == username))
                {
                    username = $"{username}_{Guid.NewGuid().ToString("N").Substring(0, 5)}";
                }

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = email,
                    PasswordHash = string.Empty, // External OAuth
                    FullName = fullName,
                    AvatarUrl = avatarUrl,
                    Role = UserRole.User,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                user.NotificationSetting = new NotificationSetting
                {
                    UserId = user.Id,
                    OnComment = true,
                    OnAssigned = true,
                    OnStatusChanged = true,
                    OnMention = true,
                    OnSprintStarted = true,
                    OnMemberAdded = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update avatar if changed
                if (!string.IsNullOrEmpty(avatarUrl) && user.AvatarUrl != avatarUrl)
                {
                    user.AvatarUrl = avatarUrl;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }

            if (!user.IsActive)
            {
                return BadRequest(new { Message = "Account is inactive." });
            }

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.Role
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred during GitHub OAuth: {ex.Message}" });
        }
    }

    [Authorize]
    [HttpPost("link-github")]
    public async Task<IActionResult> LinkGithub([FromBody] ExternalLoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            return BadRequest(new { Message = "Authorization code is required." });
        }

        var githubClientId = _configuration["Authentication:GitHub:ClientId"] ?? "Ov23lirIaP5cgYJ7mvKt";
        var githubClientSecret = _configuration["Authentication:GitHub:ClientSecret"] 
            ?? Environment.GetEnvironmentVariable("GITHUB_CLIENT_SECRET");

        if (string.IsNullOrEmpty(githubClientSecret))
        {
            return BadRequest(new { Message = "GitHub Client Secret is not configured on the server." });
        }

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "ProTask-App");

            // 1. Exchange code for access token
            var tokenParams = new Dictionary<string, string>
            {
                { "code", request.Code },
                { "client_id", githubClientId },
                { "client_secret", githubClientSecret },
                { "redirect_uri", request.RedirectUri }
            };

            var tokenResponse = await httpClient.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(tokenParams));
            var tokenRawResult = await tokenResponse.Content.ReadAsStringAsync();
            if (!tokenResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"GitHub token exchange failed: {tokenRawResult}" });
            }

            var tokenJson = System.Text.Json.Nodes.JsonNode.Parse(tokenRawResult);
            var accessToken = tokenJson?["access_token"]?.ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest(new { Message = "Access token was not returned from GitHub." });
            }

            // 2. Fetch GitHub user profile
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var profileResponse = await httpClient.GetAsync("https://api.github.com/user");
            var profileRawResult = await profileResponse.Content.ReadAsStringAsync();
            if (!profileResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { Message = $"Failed to retrieve GitHub profile: {profileRawResult}" });
            }

            var profileJson = System.Text.Json.Nodes.JsonNode.Parse(profileRawResult);
            var githubLogin = profileJson?["login"]?.ToString();

            if (string.IsNullOrEmpty(githubLogin))
            {
                return BadRequest(new { Message = "Failed to retrieve username from GitHub profile." });
            }

            // 3. Save GitHubUsername to current user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            user.GitHubUsername = githubLogin;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "GitHub linked successfully.", GitHubUsername = githubLogin });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"An error occurred during GitHub link: {ex.Message}" });
        }
    }

    [Authorize]
    [HttpPost("unlink-github")]
    public async Task<IActionResult> UnlinkGithub()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        user.GitHubUsername = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "GitHub unlinked successfully." });
    }
}

public class ExternalLoginRequest
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UpdateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public string? Password { get; set; }
}

public class UpdateProfileRequest
{
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Bio { get; set; }
}

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
