using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using RealEstateMarketplace.Application.Auth.Commands;
using RealEstateMarketplace.Application.DTOs;
using RealEstateMarketplace.Application.Interfaces.Services;
using RealEstateMarketplace.Domain.Entities;
using RealEstateMarketplace.Infrastructure.Persistence;
using RealEstateMarketplace.Infrastructure.Services;
using RealEstateMarketplace.Api.Security;
using System.Security.Claims;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Auth")]
[Produces("application/json")]
[ProducesResponseType(
    typeof(ProblemDetails),
    StatusCodes.Status500InternalServerError)]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ICloudinaryService _cloudinaryService;

    public AuthController(
        ISender sender,
        UserManager<User> userManager,
        ApplicationDbContext context,
        ITokenService tokenService,
        ICloudinaryService cloudinaryService)
    {
        _sender = sender;
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
        _cloudinaryService = cloudinaryService;
    }

    [HttpPost("login")]
[AllowAnonymous]
[ProducesResponseType(
    typeof(AuthResponseDto),
    StatusCodes.Status200OK)]
[ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
[ProducesResponseType(
    StatusCodes.Status400BadRequest)]
public async Task<ActionResult<AuthResponseDto>> Login(
    [FromBody] LoginRequestDto request,
    CancellationToken cancellationToken)

    {
        var result = await _sender.Send(
            new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error!.Code == "account_locked")
            {
                return StatusCode(
                    StatusCodes.Status423Locked,
                    new
                    {
                        message = result.Error.Message
                    });
            }

            return Unauthorized(new
            {
                message = result.Error.Message
            });
        }

        var user = await _userManager.FindByEmailAsync(
            request.Email);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid credentials."
            });
        }

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        // Store the refresh token for rotation and revocation.
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Ok(new
        {
            accessToken = result.Value!.Token,
            refreshToken = refreshToken.Token,

            token = result.Value.Token,

            userId = result.Value.UserId,
            fullName = result.Value.FullName,
            email = result.Value.Email,
            role = result.Value.Role
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(AuthResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new RegisterCommand
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber
            },
            cancellationToken);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new
        {
            message = result.Error!.Message
        });
    }

    public record RefreshRequest(string RefreshToken);

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshRequest request,
        CancellationToken cancellationToken)
    {
        var storedToken =
            await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    x => x.Token == request.RefreshToken,
                    cancellationToken);

        if (storedToken is null)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token."
            });
        }

        if (storedToken.IsUsed)
        {
            // Revoke all sessions when a refresh token is reused.
            var userTokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.UserId == storedToken.UserId)
                    .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return Unauthorized(new
            {
                message =
                    "Token theft detected. All user sessions revoked."
            });
        }

        if (storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message =
                    "Refresh token expired or revoked."
            });
        }

        var user =
            await _userManager.FindByIdAsync(
                storedToken.UserId.ToString());

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "User not found."
            });
        }

        var roles = new List<string> { user.Role.ToString() };

        storedToken.IsUsed = true;

        // Rotate refresh tokens after each successful renewal.
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(
            newRefreshToken);

        var newAccessToken =
            _tokenService.GenerateAccessToken(
                user,
                roles);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordDto request,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new ForgotPasswordCommand { Email = request.Email }, cancellationToken);

        return Ok(new
        {
            message = "If an account exists with this email, password reset instructions have been sent."
        });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new ResetPasswordCommand
            {
                Email = request.Email,
                Token = request.Token,
                NewPassword = request.NewPassword
            },
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                message = result.Error!.Message
            });
        }

        return Ok(new
        {
            message = "Your password has been reset successfully."
        });
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile(CancellationToken cancellationToken)
    {
        // Resolve the user from the authenticated claim.
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(ToProfileDto(user));
    }

    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileDto>> UpdateMyProfile(
        [FromBody] UpdateProfileDto request,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized();
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _userManager.FindByEmailAsync(request.Email);
            if (existing is not null && existing.Id != user.Id)
            {
                return BadRequest(new { message = "This email is already in use by another account." });
            }

            await _userManager.SetEmailAsync(user, request.Email);
            await _userManager.SetUserNameAsync(user, request.Email);
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message });
        }

        return Ok(ToProfileDto(user));
    }

    [HttpPost("me/photo")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileDto>> UploadMyProfilePhoto(
        IFormFile photo,
        CancellationToken cancellationToken)
    {
        if (photo is null || photo.Length == 0)
        {
            return BadRequest(new { message = "A photo file is required." });
        }

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(photo.ContentType.ToLowerInvariant()))
        {
            return BadRequest(new { message = "Only JPEG, PNG, and WebP images are allowed." });
        }

        var user = await GetCurrentUserAsync();
        if (user is null)
        {
            return Unauthorized();
        }

        var uploadResult = await _cloudinaryService.UploadImageAsync(photo);
        user.ProfileImageUrl = uploadResult.ImageUrl;

        await _userManager.UpdateAsync(user);

        return Ok(ToProfileDto(user));
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(currentUserId, out var userId)
            ? await _userManager.FindByIdAsync(userId.ToString())
            : null;
    }

    private static UserProfileDto ToProfileDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email ?? string.Empty,
        PhoneNumber = user.PhoneNumber,
        ProfileImageUrl = user.ProfileImageUrl,
        Role = user.Role.ToString()
    };

    [HttpGet("users")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType(
        typeof(IReadOnlyList<UserSummaryDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserSummaryDto>>> GetUsers(
        CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .OrderByDescending(u => u.Id)
            .Select(u => new UserSummaryDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                Role = u.Role.ToString()
            })
            .ToListAsync(cancellationToken);

        return Ok(users);
    }
}
