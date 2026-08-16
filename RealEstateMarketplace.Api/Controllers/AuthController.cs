using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Auth.Commands;
using RealEstateMarketplace.Application.DTOs;
using Scalar.AspNetCore;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Auth")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
[ApiVersion("1.0")]
//[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Log in a user")]
    [EndpointDescription("Authenticates a user and returns a JWT response on success.")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        }, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!) 
            : result.Error!.Code == "invalid_credentials"
                ? Unauthorized(result.Error.Message)
                : BadRequest(result.Error.Message);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [EndpointSummary("Register a new user")]
    [EndpointDescription("Creates a new user account and returns authentication details.")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new RegisterCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            PhoneNumber = request.PhoneNumber
        }, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value!)
            : BadRequest(result.Error!.Message);
    }
}
