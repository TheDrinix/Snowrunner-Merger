using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Api.Services;
using SnowrunnerMerger.Api.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Google;
using SnowrunnerMerger.Api.Models.Auth.OAuth;

namespace SnowrunnerMerger.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        IAuthService authService, 
        IUserService userService, 
        IEmailSender emailSender,
        IOAuthServiceFactory oauthServiceFactory
    ) : ControllerBase
    {
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Logs in a user", Description = "Logs in a user with provided credentials and returns a JWT token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Logs in successfully", typeof(LoginResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Email not verified")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto data)
        {
            return Ok(await authService.Login(data));
        }
        
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers a new user", Description = "Registers a new user with provided details and sends a confirmation email")]
        [SwaggerResponse(StatusCodes.Status201Created, "User registered successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request body or password requirements not met")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "User with email already exists")]
        public async Task<IActionResult> Register([FromBody] RegisterDto data)
        {
            var confirmationToken = await authService.Register(data);
            
            var confirmationUrl = new Uri($"{Request.Headers.Origin}/auth/confirm-email?token={WebUtility.UrlEncode(confirmationToken.Token)}");
            
            var html = $"""
                        <html>
                            <body>        
                                <h2>Verify your email</h2>
                                <p>
                                    Please verify your email by clicking <a href="{confirmationUrl}">here</a>.
                                </p>
                                <p>The link will be valid for an hour.</p>
                                <p>If you did not register, please ignore this email.</p>
                            </body>
                        </html>
                      """;
            
            await emailSender.SendEmailAsync(data.Email, "Verify your email", html);

            return Created();
        }

        [HttpGet("refresh")]
        [SwaggerOperation(Summary = "Gets long-lived refresh token", Description = "Gets long-lived refresh token for a user to use in frontend (desktop app)")]
        [SwaggerResponse(StatusCodes.Status200OK, "Refresh token retrieved successfully", typeof(RefreshTokenDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<RefreshTokenDto>> GetLongLivedRefreshToken()
        {
            var userData = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userData is null) return Unauthorized();
            
            var data = await authService.GetLongLivedRefreshToken(Guid.Parse(userData));
            
            return Ok(data);
        }
        
        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Refreshes JWT token", Description = "Refreshes JWT token using refresh token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Token refreshed successfully", typeof(RefreshResponseDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid refresh token")]
        public async Task<ActionResult<RefreshResponseDto>> RefreshToken(RefreshDto body)
        {
            var isCookieToken = body.Token is null;
            var token = body.Token ?? Request.Cookies["refresh_token"];
            
            if (string.IsNullOrEmpty(token)) return Unauthorized();
            
            var data = await authService.RefreshToken(token, isCookieToken);
            
            return Ok(data);
        }
        
        [HttpPost("verify-email")]
        [SwaggerOperation(Summary = "Verifies user's email", Description = "Verifies user's email using provided token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Email verified successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid token")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto data)
        {
            var verified = await authService.VerifyEmail(data.Token);

            return verified ? Ok() : BadRequest();
        }

        [HttpGet("{provider}/signin")]
        [SwaggerOperation(Summary = "Initiates OAuth sign-in", Description = "Initiates OAuth sign-in flow for the specified provider and returns the sign-in URL")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns OAuth provider signin url", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Unsupported OAuth provider")]
        public IActionResult OAuthSignIn([FromQuery] string? callbackUrl, [FromRoute] string provider)
        {
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }
            
            var hashedState = authService.GenerateOauthStateToken();

            var currentUrl = $"{Request.Scheme}://{Request.Host}";
            
            var url = oauthService.GetSignInUrl(currentUrl, hashedState, callbackUrl);
            
            return Ok(url);
        }
        
        [HttpGet("{provider}/callback")]
        [SwaggerOperation(Summary = "Handles OAuth sign-in callback", Description = "Handles OAuth sign-in callback and signs in the user")]
        [SwaggerResponse(StatusCodes.Status200OK, "Sign-in successful", typeof(LoginResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid state token or error during OAuth sign-in")]
        public async Task<IActionResult> OAuthSignInCallback(string? code, string state, string? error, string? callbackUrl, [FromRoute] string provider)
        {
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }
            
            if (!authService.ValidateOauthStateToken(WebUtility.UrlDecode(state)))
            {
                return BadRequest();
            }            
            
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }
            
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            
            var redirectUrl = callbackUrl ?? oauthService.GetCallbackUrl(baseUrl);
            
            var res = await oauthService.OAuthSignIn(code!, redirectUrl);

            return res switch
            {
                OAuthSignInResult.OAuthSignInAccountSetupRequired oAuthSignInAccountSetupRequired => Ok(new
                {
                    tokenType = OAuthResultTokenType.CompletionToken,
                    data = oAuthSignInAccountSetupRequired.completionToken
                }),
                OAuthSignInResult.OAuthSignInLinkRequired oAuthSignInLinkRequired => Ok(new
                {
                    tokenType = OAuthResultTokenType.LinkingToken,
                    data = oAuthSignInLinkRequired.linkingToken,
                }),
                OAuthSignInResult.OAuthSignInSuccess oAuthSignInSuccess => Ok(new
                {
                    tokenType = OAuthResultTokenType.AccessToken,
                    data = oAuthSignInSuccess.data
                }),
                _ => StatusCode(500)
            };
        }

        [HttpGet("{provider}/link/callback")]
        [Authorize]
        [SwaggerOperation(Summary = "Handles OAuth account linking callback", Description = "Handles OAuth provider account linking callback and links the OAuth provider account to the current user")]
        [SwaggerResponse(StatusCodes.Status200OK, "Account linked successfully", typeof(User))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid state token or error during OAuth account linking")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "There is already a user with the OAuth provider account linked")]
        public async Task<IActionResult> LinkOAuthAccountCallback(string? code, string state, string? error,
            string callbackUrl, [FromRoute] string provider)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest();
            }
            
            if (!authService.ValidateOauthStateToken(state))
            {
                return BadRequest();
            }
            
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }

            var user = await userService.GetCurrentUser();

            var updatedUser = await oauthService.LinkOAuthProvider(user, code!, callbackUrl);

            return Ok(updatedUser);
        }

        [HttpPost("{provider}/link-account")]
        [SwaggerOperation(Summary = "Links OAuth provider account", Description = "Links OAuth provider account to an existing user account")]
        [SwaggerResponse(StatusCodes.Status200OK, "Account linked successfully and logs user in", typeof(LoginResponseDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired linking token")]
        public async Task<IActionResult> LinkAccount([FromBody] LinkAccountDto data, [FromRoute] string provider)
        {
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }

            var accessTokenData = await oauthService.LinkOAuthProvider(data.LinkingToken);
            
            return Ok(accessTokenData);
        }
        
        [HttpPost("{provider}/finish-account-setup")]
        [SwaggerOperation(Summary = "Finishes account setup", Description = "Finishes account setup for a new user created via OAuth sign-in")]
        [SwaggerResponse(StatusCodes.Status200OK, "Account setup finished successfully and logs user in", typeof(LoginResponseDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid account data or password requirements not met")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid or expired account completion token")]
        public async Task<IActionResult> FinishAccountSetup([FromBody] FinishAccountSetupDto data, [FromRoute] string provider)
        {
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }
            
            var accessTokenData = await authService.FinishAccountSetup(data, oauthService);
            
            return Ok(accessTokenData);
        }
        
        [HttpPost("{provider}/unlink")]
        [Authorize]
        [SwaggerOperation(Summary = "Unlinks OAuth provider account", Description = "Unlinks OAuth provider account from the current user")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Account unlinked successfully")]
        public async Task<IActionResult> UnlinkGoogleAccount([FromRoute] string provider)
        {
            OAuthService oauthService;
            try
            {
                oauthService = oauthServiceFactory.GetService(provider);
            } catch (NotSupportedException)
            {
                return BadRequest("Unsupported OAuth provider");
            }
            
            var user = await userService.GetCurrentUser();
            
            await oauthService.UnlinkOAuthProvider(user);
            
            return NoContent();
        }

        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(Summary = "Logs out a user", Description = "Logs out a user and invalidates refresh token")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Logged out successfully")]
        public async Task<IActionResult> Logout()
        {
            await authService.Logout();
            
            return NoContent();
        }

        [HttpPost("resend-confirmation")]
        [SwaggerOperation(Summary = "Resends confirmation email", Description = "Resends confirmation email to user")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Email was sent if a user with provided email exists")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationDto body)
        {
            var confirmationToken = await authService.GenerateConfirmationToken(body.Email);
            
            if (confirmationToken is null)
            {
                return NoContent();
            }
            
            var confirmationUrl = new Uri($"{Request.Headers.Origin}/auth/confirm-email?token={WebUtility.UrlEncode(confirmationToken.Token)}");
            
            var html = $"""
                          <html>
                              <body>        
                                  <h2>Verify your email</h2>
                                  <p>
                                      Please verify your email by clicking <a href="{confirmationUrl}">here</a>.
                                  </p>
                                  <p>The link will be valid for an hour.</p>
                                  <p>If you did not register, please ignore this email.</p>
                              </body>
                          </html>
                        """;
            
            await emailSender.SendEmailAsync(body.Email, "Verify your email", html);
            
            return NoContent();
        }

        [HttpPost("request-password-reset")]
        [SwaggerOperation(Summary = "Requests password reset", Description = "Requests password reset and sends an email with reset link")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Email was sent if a user with provided email exists")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestResetPasswordDto body, [FromQuery] string? origin)
        {
            var resetToken = await authService.GeneratePasswordResetToken(body.Email);
            
            if (resetToken is null)
            {
                return NoContent();
            }
            
            if (string.IsNullOrEmpty(origin)) 
            {
                origin = Request.Headers.Origin;
            }
            
            var resetUrl = new Uri($"{origin}/auth/reset-password?token={WebUtility.UrlEncode(resetToken.Token)}");

            var html = $"""
                            <html>
                                <body>
                                    <h2>Password reset</h2>
                                    <p>
                                        Click <a href="{resetUrl}">here</a> to reset your password.
                                    </p>
                                    <p>The link will be valid for 30 minutes.</p>
                                    <p>If you did not request a password reset, please ignore this email.</p>
                                </body>
                            </html>
                        """;
            
            await emailSender.SendEmailAsync(body.Email, "Reset your password", html);
            
            return NoContent();
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Resets user's password", Description = "Resets user's password using provided token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Password reset successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid token")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto body)
        {
            await authService.ResetPassword(body);
            
            return Ok();
        }
        
        [HttpGet("oauth/providers")]
        [SwaggerOperation(Summary = "Gets available OAuth providers", Description = "Gets a list of available OAuth providers")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns a list of available OAuth providers", typeof(IEnumerable<string>))]
        public IActionResult GetAvailableOAuthProviders()
        {
            var providers = oauthServiceFactory.ProviderNames;
            
            return Ok(providers);
        }
    }
}
