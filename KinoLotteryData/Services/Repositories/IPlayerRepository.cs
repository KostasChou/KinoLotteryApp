using KinoLotteryData.Data;
using KinoLotteryData.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KinoLotteryData.Services.Repositories
{
    public interface IPlayerRepository
    {
        Task<Player> GetPlayerByUserNameAsync(string userName);

        Task CreatePlayerAsync(Player player);

        Task UpdatePlayerAsync(Player player);
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly KinoLotteryContext _context;
        public PlayerRepository(KinoLotteryContext context)
        {
            _context = context;
        }

        public Task CreatePlayerAsync(Player player)
        {
            if (player == null)
                throw new ArgumentException(nameof(player));
            try
            {
                _context.Players.Add(player);
                return _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<Player> GetPlayerByUserNameAsync(string userName)
        {
            return _context.Players.FirstOrDefaultAsync(p => p.UserName == userName);
        }

        public Task UpdatePlayerAsync(Player player)
        {
            throw new System.NotImplementedException();
        }
    }
}
