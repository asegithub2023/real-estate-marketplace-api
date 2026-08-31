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
using RealEstateMarketplace.Api.Security;

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

    public AuthController(
        ISender sender,
        UserManager<User> userManager,
        ApplicationDbContext context,
        ITokenService tokenService)
    {
        _sender = sender;
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
    }

    // =========================================================
    // LOGIN
    // =========================================================

    [HttpPost("login")]
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

        // Create initial refresh token
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync(
            cancellationToken);

        return Ok(new
        {
            accessToken = result.Value!.Token,
            refreshToken = refreshToken.Token,

            // Temporary compatibility with existing Angular
            token = result.Value.Token,

            userId = result.Value.UserId,
            fullName = result.Value.FullName,
            email = result.Value.Email,
            role = result.Value.Role
        });
    }

    // =========================================================
    // REGISTER
    // =========================================================

    [HttpPost("register")]
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

    // =========================================================
    // REFRESH TOKEN
    // =========================================================

    public record RefreshRequest(string RefreshToken);

    [HttpPost("refresh")]
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

        // Token does not exist
        if (storedToken is null)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token."
            });
        }

        // Reusing an already-used token = token theft
        if (storedToken.IsUsed)
        {
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

        // Expired or revoked
        if (storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message =
                    "Refresh token expired or revoked."
            });
        }

        // Find user
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

        // Get user's roles
        var roles =
            await _userManager.GetRolesAsync(user);

        // Mark old refresh token as used
        storedToken.IsUsed = true;

        // Create new refresh token
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

        // Generate new access token
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

    // =========================================================
    // ADMIN: LIST USERS
    // =========================================================

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