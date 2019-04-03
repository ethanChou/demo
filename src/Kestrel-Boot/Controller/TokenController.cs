using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HttpBroker
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        //Get 地址  备注
        //[HttpGet]	/api/apple
        //[HttpGet("add")]	/api/apple/add
        //[HttpGet("add/{id}")]	/api/apple/add/4396	4396可以为其他值，当做参数传给方法
        //[HttpGet("add/id={id}")]	/api/apple/add/id=4396	4396可以为其他值，当做参数传给方法
        //[HttpGet("add/{id=4396}")]	/api/apple/add 如果不传入参数，id默认为4396，id为参数
        //[HttpGet("add/{id?}")]	/api/apple/add 在add后面可以输入也可以不输入，不输入则参数默认为0
        //[HttpGet("add/{name}/id")]	/api/apple/add/zhangsan/id zhangsan会作为参数传给方法

        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/values
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            //if (User.Identity.IsAuthenticated)
            return new string[] { "value1", "value2" };
        }

        public class UserInfo
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }
        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]UserInfo userInfo)
        {
            if (userInfo.Account == "admin" && userInfo.Password == "123456")
            {
                // push the user’s name into a claim, so we can identify the user later on.
                var claims = new[]
                {
                   new Claim(ClaimTypes.Name, userInfo.Account)
               };
                //sign the token using a secret key.This secret will be shared between your API and anything that needs to check that the token is legit.
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                //.NET Core’s JwtSecurityToken class takes on the heavy lifting and actually creates the token.
                /**
                 * Claims (Payload)
                    Claims 部分包含了一些跟这个 token 有关的重要信息。 JWT 标准规定了一些字段，下面节选一些字段:

                    iss: The issuer of the token，token 是给谁的  发送者
                    audience: 接收的
                    sub: The subject of the token，token 主题
                    exp: Expiration Time。 token 过期时间，Unix 时间戳格式
                    iat: Issued At。 token 创建时间， Unix 时间戳格式
                    jti: JWT ID。针对当前 token 的唯一标识
                    除了规定的字段外，可以包含其他任何 JSON 兼容的字段。
                 * */
                var token = new JwtSecurityToken(
                    issuer: "jwttest",
                    audience: "jwttest",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
                Response.Cookies.Append("token", tokenStr);

                return Ok(new
                {
                    code = 0,
                    token = tokenStr
                });
            }

            return BadRequest("用户名密码错误");
        }


    }
}
