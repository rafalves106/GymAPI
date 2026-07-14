using GymAPI.Application.DTOs;
using GymAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthUseCases _authUseCases;

    public AuthController(IAuthUseCases authUseCases)
    {
        _authUseCases = authUseCases;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await _authUseCases.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authUseCases.LoginAsync(request);
        return Ok(response);
    }
}
