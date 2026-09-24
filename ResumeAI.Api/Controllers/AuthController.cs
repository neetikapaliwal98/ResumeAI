using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeAI.Application.Features.Auth.DTOs;
using ResumeAI.Application.Interfaces;
using ResumeAI.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ResumeAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;
    public AuthController(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService,
    IConfiguration configuration)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return BadRequest(new
            {
                message = "A user with this email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new
        {
            message = "User registered successfully."
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    RefreshTokenRequest request)
    {
        await _refreshTokenService.RevokeRefreshTokenAsync(
            request.UserId,
            request.RefreshToken);

        return Ok(new
        {
            message = "Logged out successfully."
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        RefreshTokenRequest request)
    {
        var isValid =
            await _refreshTokenService.ValidateRefreshTokenAsync(
                request.UserId,
                request.RefreshToken);

        if (!isValid)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired refresh token."
            });
        }

        var user =
            await _userManager.FindByIdAsync(
                request.UserId.ToString());

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "User not found."
            });
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        // Revoke the old refresh token
        await _refreshTokenService.RevokeRefreshTokenAsync(
            request.UserId,
            request.RefreshToken);

        // Generate new access token
        var accessToken =
            _jwtTokenService.GenerateToken(
                user.Id,
                user.Email!,
                roles);

        // Generate new refresh token
        var newRefreshToken =
            await _refreshTokenService.CreateRefreshTokenAsync(
                user.Id);

        return Ok(new
        {
            accessToken,
            refreshToken = newRefreshToken,
            expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "30"))
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var roles =
            await _userManager.GetRolesAsync(user);

        var accessToken =
            _jwtTokenService.GenerateToken(
                user.Id,
                user.Email!,
                roles);
        var refreshToken =
    await _refreshTokenService.CreateRefreshTokenAsync(user.Id);
        var response = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse( _configuration["Jwt:ExpirationMinutes"] ?? "30")),
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return Ok(response);
    }
}