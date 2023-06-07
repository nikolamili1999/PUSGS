using EmailService;
using EmailService.Interfaces;
using IdentityApi.Dto;
using IdentityApi.Interfaces;
using IdentityApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IdentityApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailService;
        private readonly IConfiguration _config;

        public UsersController(IUserService userService, IEmailSender emailSender, IConfiguration configuration)
        {
            _userService = userService;
            _emailService = emailSender;
            _config = configuration;
        }

        [HttpGet]
        public ActionResult GetUser()
        {
            var userEmail = GetUserEmail();
            try
            {
                return Ok(_userService.GetUserByEmail(userEmail));
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpPost("login")]
        public ActionResult Login(LoginDto dto)
        {
            var retVal = _userService.LoginUser(dto);
            if (retVal == null) return BadRequest(new { message = "Wrong password/email" });
            return Ok(retVal);
        }

        [HttpPost]
        public ActionResult AddUser([FromBody] RegisterDto dto)
        {
            try
            {
                
                _userService.AddUser(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            return Ok();
        }
        [HttpPut]
        [Authorize]
        public ActionResult UpdateUser([FromBody] UpdateUserDto dto)
        {
            var userEmail = GetUserEmail();
            try
            {
                _userService.UpdateUser(userEmail, dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            return Ok();
        }

        [HttpGet("sellers")]
        [Authorize(Roles = "Admin")]
        public ActionResult Getsellers()
        {
            return Ok(_userService.GetSellers());
        }

        [HttpPost("sellers")]
        [Authorize(Roles = "User")]
        public ActionResult ApplyForSeller()
        {
            var userEmail = GetUserEmail();
            try
            {
                _userService.UpdatePendingStatus(userEmail, ERequestStatus.WAITING_FOR_RESPONSE);
                // send email
                try
                {
                    var message = new Message(userEmail, "You apply is register", "Your apply for becoming seller is registred. It will be responded in shortest time.");
                    _emailService.SendEmail(message);
                }catch(Exception ex) { }
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            return Ok();
        }
        [HttpPut("sellers")]
        [Authorize(Roles = "Admin")]
        public ActionResult Updatesellerstatus([FromBody] UpdateStatusDto updateStatusDto)
        {
            try
            {
                _userService.UpdateSellerStatus(updateStatusDto);
                try
                {
                    if (updateStatusDto.Approved)
                    {

                        var message = new Message(updateStatusDto.User, "Seller application update", "Wellcome to seller community.");
                        _emailService.SendEmail(message);
                    }
                    else
                    {
                        var message = new Message(updateStatusDto.User, "Seller application update", "We are sorry but your seller apply is denied:/");
                        _emailService.SendEmail(message);
                    }
                }catch(Exception ex) { }
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }

            return Ok();
        }

        [HttpPost("image")]
        [Authorize]
        public async Task<IActionResult> OnPostUploadAsync()
        {
            var email = GetUserEmail();

            var files = Request.Form.Files;

            if (files.Count != 1)
                return BadRequest();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(_config["StoredFilesPath"], email);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            return Ok();
        }
        [HttpGet("image/me")]
        [Authorize]
        public async Task<IActionResult> DownloadImage(string filename)
        {
            var email = GetUserEmail();

            var path = Path.Combine(_config["StoredFilesPath"], email);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/png", Path.GetFileName(path));
        }
        [HttpGet("image")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownloadImageByEmail(string email)
        {

            var path = Path.Combine(_config["StoredFilesPath"], email);
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, "image/png", Path.GetFileName(path));
        }
        [HttpPost("login-google")]
        public ActionResult LoginGoogle([FromBody] GoogleLoginDto dto) => Ok(_userService.LoginGoogle(dto));

        [NonAction]
        private string GetUserEmail()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
