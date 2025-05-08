using System.Text;
using EventBooking.ApI.DTOs.IdentityDTOs;
using EventBooking.DAL.Models;
using EventBooking.DAL.Services.Contract;
using EventBooking.DAL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace EventBooking.ApI.Controllers
{
    public class AccountController : BaseAPIControllercs
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailProvider _emailProvider;

        public AccountController(IAuthService authService, UserManager<AppUser> userManager
            , SignInManager<AppUser> signInManager, IEmailProvider emailProvider)
        {
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailProvider = emailProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                PhoneNumber = registerDto.PhoneNumber,
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);
            else
            await  _userManager.AddToRoleAsync(user, SD.CustomerRole);//.Wait();
            var token = await _authService.CreateTokenAsync(user);
            var tokene = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = user.Id, token = tokene },
                protocol: HttpContext.Request.Scheme);
            Console.WriteLine($"Generated Confirmation Link: {confirmationLink}");

            var emailResult= await _emailProvider.SendConfirmAccount(user.Email, confirmationLink);

            if (emailResult != "Done")
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new 
                    { 
                        Message = "Failed to send confirmation email.",
                        Token = token,
                        ConfirmationLink = confirmationLink
                    });
            }
            return Ok(new UserDto()
            {
                Token = token,
                DisplayName = user.DisplayName,
                Email = user.Email,

            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.FindByEmailAsync(loginDto.EmailOrUserName) ??
                 await _userManager.FindByNameAsync(loginDto.EmailOrUserName);

            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new { Message = "Please confirm your email before signing in." });
            }
            if (user == null) return Unauthorized(new
            {
                Message = "UserName or Email is InValid"
            });

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new
                {

                    Message = "Invalid Password !"
                });

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user),
                Image = user.Photo
            });
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            Console.WriteLine($"ConfirmEmail called with userId: {userId}, token: {token}");
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Invalid confirmation request: userId or token is empty");
                return BadRequest("Invalid confirmation request");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");
            try 
            { 
            
            var result = await _userManager.ConfirmEmailAsync(user, token);
            //var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Console.WriteLine("Email confirmed successfully");
                return Ok("Email confirmed successfully");
            }
            Console.WriteLine("Email confirmation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            return BadRequest("Email confirmation failed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token or confirming email: {ex.Message}");
                return BadRequest("Error processing confirmation request");
            }
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new
            {
                Message = "Logout Successfully"
            });
        }


        [HttpPost("send_reset_code")]
        public async Task<IActionResult> SendResetCode(SendPINDto model, [FromServices] IEmailProvider _emailProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid ModelState"
                });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                return NotFound(new
                {
                    status = 404,
                    errorMessage = "Email Not Found!"
                });
            }

            int pin = await _emailProvider.SendResetCode(model.Email);
            if (pin == -1)
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, errorMessage = "Failed to send reset code." });
            #region Hash Pin
            //Hash the pin before storing it
            //string hashedPin = BCrypt.Net.BCrypt.HashPassword(pin.ToString());
            //user.PasswordResetPin = int.Parse(hashedPin.Substring(0, 9)); 
            #endregion
            //Store the pin and expiration time in the user object
            user.PasswordResetPin = pin;
            user.ResetExpires = DateTime.Now.AddMinutes(15);
            //var expireTime = user.ResetExpires.Value.ToString("hh:mm tt");
            //await _userManager.UpdateAsync(user);
            #region Update user 
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Update failed: {string.Join(", ", result.Errors)}");
                    return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, errorMessage = "Failed to update user." });
                }
                Console.WriteLine("Update succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = 500, errorMessage = "Error updating user." });
            }

            var expireTime = user.ResetExpires.Value.ToString("hh:mm tt");
            #endregion

            return Ok(new
            {
                status = 200,
                ExpireAt = "expired at " + expireTime,
                email = model.Email,
            });
        }

        [HttpPost("verify_pin/{email}")]
        public async Task<IActionResult> VerifyPin([FromBody] VerfiyPINDto model, [FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid ModelState"
                });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return NotFound(new
                {
                    status = 404,
                    errorMessage = "Email Not Found!"
                });
            }
            if (user.ResetExpires < DateTime.Now || user.ResetExpires is null)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Time Expired try to send new pin"
                });
            }
            if (user.PasswordResetPin != model.pin)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid pin"
                });
            }
            #region Check Hashing
            //if (BCrypt.Net.BCrypt.Verify(model.pin.ToString(), user.PasswordResetPin.ToString()))
            //{
            //    #region Delete pin
            //    user.ResetExpires = null;
            //    user.PasswordResetPin = null;
            //    await _userManager.UpdateAsync(user);
            //    return Ok(new
            //    {
            //        status = 200,
            //        message = "PIN verified successfully",
            //        email = user.Email,
            //    });
            //    #endregion
            //}
            //else
            //{
            //    return BadRequest(new
            //    {
            //        status = 400,
            //        errorMessage = "Invalid pin"
            //    });
            //}
            #endregion

            #region Delete pin
            user.ResetExpires = null;
            user.PasswordResetPin = null;
            await _userManager.UpdateAsync(user);
            return Ok(new
            {
                status = 200,
                message = "PIN verified successfully",
                email = user.Email,
            });
            #endregion
        }

        [HttpPost("forget_password/{email}")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgetPassDto model, [FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Invalid model state."
                });
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "New password and confirm new password do not match."
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return BadRequest(new
                {
                    status = 400,
                    errorMessage = "Email Not Found!"
                });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    status = 200,
                    message = "Password changed successfully"
                });
            }
            return BadRequest(new
            {
                status = 400,
                errorMessage = "Invalid model state."
            });
        }

        [Authorize]
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return BadRequest(new
                {
                    message = "User Not Authorized !"
                });
            }

            var checkoldpass = _userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!checkoldpass.Result)
            {
                return BadRequest(new
                {
                    message = "Old Password is not correct !"
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Message = "Failure in Change Password!"
                });

            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);

                return Ok(new
                {

                    Message = "Password Change Sucessfully !"
                });

            }
        }

    }
}
