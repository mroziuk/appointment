using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Data;
using Appointment.Domain.DTO.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Appointment.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/identity")]
public class IdentityController : BaseController
{
    IIdentityService _identityService;
    IRefreshTokenService _refreshTokenService;
    ILogger<IdentityController> _logger;

    public IdentityController(CalendarContext context, IIdentityService identityService, IRefreshTokenService refreshTokenService,ILogger<IdentityController> logger) : base(context)
    {
        _identityService = identityService;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }
    // GET: api/identity/ping
    [HttpGet]
    [AllowAnonymous]
    [Route("ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
    // POST: api/identity/register
    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] SignUpDto signInDto)
    {
        var user = _identityService.SignUp(signInDto);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetUserById", new { id = user.Id }, user.Id);
    }
    // POST: api/identity/login
    [HttpPost]
    [AllowAnonymous]
    [Route("login")]
    public async Task<ActionResult<string>> Login([FromBody] SignInDto signInDto)
    {
        var token =  await _identityService.SignInAsync(signInDto);
        _logger.LogInformation("User {Email} logged in", signInDto.Login);
        // TODO redirect to activation page
        var options = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            Expires = DateTime.Now.AddDays(1),
            IsEssential = true,
            Path = "/",
        };
        Response.Headers.AccessControlExposeHeaders = HeaderNames.SetCookie;
        //Response.Cookies.Append("X-Access-Token", token.AccessToken, options);
        //Response.Cookies.Append("X-Role", token.Role, options);
        Response.Cookies.Append("X-Refresh-Token", token.RefreshToken, options);
        return Ok(token.AccessToken);
    }
    // POST: api/identity/refresh
    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    public async Task<ActionResult<string>> Refresh()
    {   
        var refreshToken = Request.Cookies["X-Refresh-Token"];
        if(string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest();
        }
        var token = await _identityService.Refresh(new RefreshTokenDto() { RefreshToken = refreshToken});
        //Response.Cookies.Append("X-Access-Token", token.AccessToken, new CookieOptions() { HttpOnly = false, SameSite = SameSiteMode.None,Secure = true });
        //Response.Cookies.Append("X-Role", token.Role, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
        Response.Cookies.Append("X-Refresh-Token", token.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
        return Ok(token.AccessToken);
    }
    // POST: api/identity/activate{token}
    [HttpPost]
    [AllowAnonymous]
    [Route("activate")]
    public async Task<IActionResult> Activate(string token)
    {
        await _identityService.ActivateAccount(new() { Token = token});
        // TODO redirect to login page
        return Ok();
    }
    // POST: api/identity/password/forgot
    [HttpPost]
    [AllowAnonymous]
    [Route("password/forgot")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        await _identityService.ForgotPassword(forgotPasswordDto);
        return Ok();
    }
    // POST: api/identity/password/reset
    [HttpPost]
    [AllowAnonymous]
    [Route("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        await _identityService.ResetPassword(resetPasswordDto);
        return Ok();
    }
    // POST: api/identity/password
    [HttpPost]
    [AllowAnonymous]
    [Route("password")]
    public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto setPasswordDto)
    {
        await _identityService.SetPassword(GetUserId(), setPasswordDto);
        return Ok();
    }

}
