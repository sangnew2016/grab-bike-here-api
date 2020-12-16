using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using DataAccessLayer;
using DataAccessLayer.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using myApi.DTO;

namespace myApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IConfiguration _config;       

        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(Login login) {
            // check exists account
            var password = Security.EncryptString(CONSTANTS.KEY_ENCRYPT_DECRYPT_STRING, login.Password);
            var datatable = _Data.BaseMYSQL.getDataTable($@"
                SELECT A.id, A.email, A.phone, A.type, P.full_name, P.id_card, P.address, P.avatar
                FROM Accounts A LEFT JOIN PersonInfo P ON A.id = P.user_id
                WHERE A.email = '{login.Email}' and A.password = '{password}'
            ");             // `is_confirmed` = 1 for verify email

            if (datatable == null || datatable.Rows.Count == 0)
            {
                throw new Exception("Email has not existed to login.");
            }

            // get token
            var id = Convert.ToInt32(datatable.Rows[0]["id"]);
            var fullName = Convert.ToString(datatable.Rows[0]["full_name"]);
            var email = Convert.ToString(datatable.Rows[0]["email"]);
            var phone = Convert.ToString(datatable.Rows[0]["phone"]);
            var type = Convert.ToString(datatable.Rows[0]["type"]);
            var idCard = Convert.ToString(datatable.Rows[0]["id_card"]);
            var address = Convert.ToString(datatable.Rows[0]["address"]);
            var avatar = Convert.ToString(datatable.Rows[0]["avatar"]);
            var userRole = "User";            
            var token = GenerateJWTToken(login.Email, fullName, userRole);

            return Ok(new { 
                token, id, fullName, email, phone, type, idCard, address, avatar
            });
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Policy = Policies.User)]
        public ActionResult<int> UpdateAccount(UpdateAccount account)
        {
            // check exists account            
            var userid = _Data.BaseMYSQL.GetScalarValue($@"
                SELECT id FROM Accounts WHERE email = '{account.Email}'
            ");

            if (userid == null || Convert.ToString(userid) == "")
            {
                throw new Exception("Email has not existed to login.");
            }

            var sql = $@"
                START TRANSACTION;

                UPDATE `Accounts` SET
                    phone = '{account.Phone}',
                    email = '{account.Email}'
                WHERE id = {Convert.ToString(userid)};

                UPDATE `PersonInfo` SET 
	                full_Name = '{account.FullName}',                    
                    id_card = '{account.IdCard}',
                    address = '{account.Address}',
                    avatar = '{account.Avatar}'
                WHERE user_id = {Convert.ToString(userid)};

                COMMIT;
            ";
            var result = _Data.BaseMYSQL.SaveChanges(sql);

            return result;
        }


        [HttpPost]
        [Route("register")]
        public ActionResult<int> Register(Register account)
        {                        
            // validate email, phone -> not duplicate
            var id = _Data.BaseMYSQL.GetScalarValue($"SELECT `id` FROM `Accounts` WHERE `phone` = '{account.Phone}' or `email` = '{account.Email}'");
            if (id != null)
            {
                throw new Exception("Phone or Email has existed");
            }

            var password = Security.EncryptString(CONSTANTS.KEY_ENCRYPT_DECRYPT_STRING, account.Password);
            string insert_personInfo_clause;
            if (string.IsNullOrEmpty(account.IdCard))
            {
                insert_personInfo_clause = $@"
                    INSERT INTO `PersonInfo`(`user_id`, `full_name`, `address`, `avatar`) 
	                    SELECT LAST_INSERT_ID(), '{account.FullName}', '{account.Address}', '{account.Avatar}'
                        WHERE NOT EXISTS(SELECT * FROM `PersonInfo` WHERE `user_id` = LAST_INSERT_ID());
                ";
            } else {
                insert_personInfo_clause = $@"
                    INSERT INTO `PersonInfo`(`user_id`, `full_name`, `id_card`, `address`, `avatar`) 
	                    SELECT LAST_INSERT_ID(), '{account.FullName}', '{account.IdCard}', '{account.Address}', '{account.Avatar}'
                        WHERE NOT EXISTS(SELECT * FROM `PersonInfo` WHERE `user_id` = LAST_INSERT_ID());
                ";
            }
            
            var sql = $@"
                    START TRANSACTION;

                    INSERT INTO `Accounts`(`email`, `phone`, `password`, `type`) 
                        SELECT '{account.Email}', '{account.Phone}', '{password}', '{account.Type}' 
                        WHERE NOT EXISTS(SELECT * FROM `Accounts` WHERE `email` = '{account.Email}');

                    {insert_personInfo_clause}

                    COMMIT;
                    ";
            var affectedRows = _Data.BaseMYSQL.SaveChanges(sql);

            // correct, must have 2 row affected
            return (affectedRows == 2 ? 1 : 0);
        }       
       
        private string GenerateJWTToken(string email, string fullName, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim("fullName", fullName),
                new Claim("role", userRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }
}
