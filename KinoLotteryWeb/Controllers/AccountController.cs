﻿using KinoLotteryData.Data.Entities;
using KinoLotteryData.Dtos.PlayerDtos;
using KinoLotteryData.Services;
using KinoLotteryData.Services.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KinoLotteryWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IPlayerRepository _repo;
        public AccountController(IPlayerRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LogIn(LogInDto playerLogin)
        {
            var player = await _repo.GetPlayerByUserNameAsync(playerLogin.UserName);
            if (player == null)
                return BadRequest();
            if (!EncryptionService.VerifyPasswordHash(playerLogin.Password, player.Password, player.Salt))
                return BadRequest();


            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity));
            var claims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Name, playerLogin.UserName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true,
            });

            return Ok();

        }

        [HttpPost("register")]

        public async Task<ActionResult> Register(RegisterDto playerRegister)
        {
            if (playerRegister == null)
                return NotFound();
            Player player = playerRegister;
            byte[] hash;
            byte[] salt;
            EncryptionService.CreatePasswordHash(playerRegister.Password, out hash, out salt);
            player.Password = hash;
            player.Salt = salt;
            await _repo.CreatePlayerAsync(player);
            return Ok(player);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return NoContent();
        }
    }
}