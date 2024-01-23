using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using NewsBackend.Data;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Azure.Core;
using LiveStreamingBackend.Data;

namespace LiveStreamingBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthorizationController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto2 request)
        {
            bool isGoodSize = Functions.MyFunctions.checkMessageSizeUserRegister(request);
            if(!isGoodSize)
            {
                return BadRequest("Inputs too Long");

            }
            string sqlQuery = "SELECT COUNT(*) FROM [dbo].[StreamingCreds] WHERE Name = @searchString;";
            string sqlQuery2 = "SELECT COUNT(*) FROM [dbo].[StreamingCreds] WHERE Email = @searchString;";
            using var connection = new SqlConnection(_config["sqlConnectionString"]);

            int count = await connection.ExecuteScalarAsync<int>(sqlQuery, new { searchString = request.Name });

            int count2 = await connection.ExecuteScalarAsync<int>(sqlQuery2, new { searchString = request.Email });
            if (count >= 1)
            {
                return BadRequest("Username Taken");
            }
            if (count2 >= 1)
            {
                return BadRequest("Email Taken");
            }
            UserStorage user1 = new UserStorage();
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user1.Name = request.Name;
            user1.PasswordHash = Convert.ToBase64String(passwordHash);
            user1.PasswordSalt = Convert.ToBase64String(passwordSalt);
            user1.Roles = "User";
            user1.Email = request.Email;
            await connection.ExecuteAsync("INSERT INTO [dbo].[StreamingCreds] (Name,PasswordSalt,PasswordHash,Roles, Email) VALUES(@Name,@PasswordSalt,@PasswordHash,@Roles,@Email)",
                user1);
            JwtSecurityToken token = CreateToken(user1);
            var cookie = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Response.Cookies.Append("creds", tokenString, cookie);
            return Ok("Account Created");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            bool isGoodSize = Functions.MyFunctions.checkMessageSizeUserLogin(request);
            if (!isGoodSize)
            {
                return BadRequest("Inputs too Long");

            }
            string sqlQuery = "SELECT COUNT(*) FROM [dbo].[StreamingCreds] WHERE Name = @searchString;";
            string sqlQuery2 = "SELECT COUNT(*) FROM [dbo].[StreamingCreds] WHERE Email = @searchString;";
            //using var connection = new SqlConnection("Data Source=LAPTOP-64PNHRUC\\SQLEXPRESS;Initial Catalog=Streaming;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            
            
            using var connection = new SqlConnection(_config["sqlConnectionString"]);
            int count = await connection.ExecuteScalarAsync<int>(sqlQuery, new { searchString = request.Name });

            int count2 = await connection.ExecuteScalarAsync<int>(sqlQuery2, new { searchString = request.Name });
            if (count == 0 && count2 == 0)
            {
                return BadRequest("User does not exist");
            }
            string QueryUser = "Name";
            if(count == 0 && count2 != 0)
            {
                QueryUser = "Email";
            }
            string sqlQueryUser = $"SELECT TOP (1) [Name],[PasswordHash],[PasswordSalt],[Roles],[Email] FROM [dbo].[StreamingCreds] WHERE {QueryUser} = @Name";
            UserStorage? user1 = await connection.QueryFirstOrDefaultAsync<UserStorage>(sqlQueryUser,
                new { Name = request.Name });
            byte[] bytepasswordhash = Convert.FromBase64String(user1?.PasswordHash);
            byte[] bytepasswordsalt = Convert.FromBase64String(user1?.PasswordSalt);

            if (!VerifyPasswordHash(request.Password, bytepasswordhash, bytepasswordsalt))
            {
                return BadRequest("Password Incorrect");
            }
            JwtSecurityToken token = CreateToken(user1);
            var cookie = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1),
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Response.Cookies.Append("creds", tokenString, cookie);
            return Ok("Password Accepted");

        }

        private JwtSecurityToken CreateToken(UserStorage user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Roles),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes((_config["jwtSecret"])));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            return token;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var checkhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return checkhash.SequenceEqual(passwordHash);
            }
        }



        [HttpPost("validateGoogle")]
        public async Task<ActionResult<object>> ValidateGoogle(GoogleDto googleDto)
        {
            string accessToken = googleDto.AccessToken;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response2 = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");
                    if (response2.IsSuccessStatusCode)
                    {
                        string userInfoJson = await response2.Content.ReadAsStringAsync();
                        UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo>(userInfoJson);
                        string email = userInfo.Email;
                        string sqlQuery = "SELECT COUNT(*) FROM [dbo].[StreamingCreds] WHERE Email = @searchString;";
                        using var connection = new SqlConnection(_config["sqlConnectionString"]);

                        int count = await connection.ExecuteScalarAsync<int>(sqlQuery, new { searchString = email });
                        if (count == 0)
                        {
                            string message = "New Account";
                            return Ok(new {message, email});
                        }
                        else
                        {
                            string sqlQueryUser = $"SELECT TOP (1) [Name],[PasswordHash],[PasswordSalt],[Roles],[Email] FROM [dbo].[StreamingCreds] WHERE Email = @Name";
                            UserStorage? user1 = await connection.QueryFirstOrDefaultAsync<UserStorage>(sqlQueryUser,
                                new { Name = email });
                            JwtSecurityToken token = CreateToken(user1);
                            var cookie = new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(1),
                                Path = "/",
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None
                            };
                            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                            Response.Cookies.Append("creds", tokenString, cookie);
                            string message = "Password Accepted";
                            return Ok(new { message, email });
                        }
                    }
                    else
                    {
                        string message = "Something Went Wrong";
                        return Ok(new { message });
                    }


                }
            }
            catch
            {
                string message = "Something Went Wrong";
                return Ok(new { message });
            }
        }




        [HttpPost("findUserDetails"), Authorize]
        public ActionResult<object> FindUserDetails(UserDto request)
        {
            var username = User?.FindFirstValue(ClaimTypes.Name);
            var role = User?.FindFirstValue(ClaimTypes.Role);
            var email = User?.FindFirstValue(ClaimTypes.Email);


            if (username != null)
            {
            return Ok(new { username, role, email});
            }
            else
            {
                return null;
            }
        }


        [HttpPost("Logout"), Authorize]
        public ActionResult Logout(UserDto request)
        {

            var cookie = new CookieOptions
            {
                Expires = DateTime.UnixEpoch,
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            };

            var tokenString = "empty";
            Response.Cookies.Append("creds", tokenString, cookie);
            return Ok("Successful Logout");
        }

        


    }
}
