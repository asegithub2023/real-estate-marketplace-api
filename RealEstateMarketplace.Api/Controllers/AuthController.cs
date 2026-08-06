using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateMarketplace.Application.Common;
using RealEstateMarketplace.Application.Auth.Commands;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
//[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
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
