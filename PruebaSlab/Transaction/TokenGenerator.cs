using Microsoft.IdentityModel.Tokens;
using PruebaSlab.Models;
using PruebaSlab.Models.DB;
using PruebaSlab.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace PruebaSlab.Transaction
{
    public class TokenGenerator
    {
        SlabEntities DB = null;
        Response response = null;
        public Response GenerateTokenAwt(LoginRequest login) 
        {
            var secretKey = ConfigurationManager.AppSettings["JWT_SECRET_KEY"];
            var audienceToken = ConfigurationManager.AppSettings["JWT_AUDIENCE_TOKEN"];
            var issuerToken = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            var expireTime = ConfigurationManager.AppSettings["JWT_EXPIRE_MINUTES"];

            //VALIDAR LA VERICIDAD DE LOS DATOS DADOS EN EL LOGIN
            DB = new SlabEntities();
            response = new Response();
            Usuario us = DB.Usuario.Where(x => x.Usuario1.Equals(login.UserName) && x.Contrasena.Equals(login.Password)&&x.Estado==true).FirstOrDefault();

            if (us != null)
            {
                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, us.Id + "|" + us.Usuario1 + '|' + us.Rol_Id) });
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: audienceToken,
                    issuer: issuerToken,
                    subject: claimsIdentity,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                    signingCredentials: signingCredentials);
                var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
                response.Successfully = true;
                response.Code = 200;
                response.Message = "Login Realizado con exito";
                response.Result = "Bearer " + jwtTokenString;
            }
            else 
            {
                response.Successfully = true;
                response.Code = 401;
                response.Message = "Login incorrecto, verifique usuario y contraseña";
                response.Result = "";
            }
            return response;
        }
        public Response DecodeTokenAwt(string token) 
        {
            response = new Response();
            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var StringTokenAwt = tokenHandler.ReadJwtToken(token);
                var ValueUser = StringTokenAwt.Claims.ToList()[0].Value;
                response.Successfully = true;
                response.Result = ValueUser;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Successfully = true;
                response.Message = ex.Message.ToString();
                response.Result = null;
            }
            return response;
        }
    }
}