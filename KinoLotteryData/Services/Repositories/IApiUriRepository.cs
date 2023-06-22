using KinoLotteryData.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinoLotteryData.Services.Repositories
{
    public interface IApiUriRepository
    {
        string GetApiUriString();
    }
    public class ApiUriRepository : IApiUriRepository
    {
        private readonly KinoLotteryContext _context;
        public ApiUriRepository(KinoLotteryContext context)
        {
            _context = context;
        }
        public string GetApiUriString()
        {
            return _context.APIURIEntities.Select(a => a.URIString).FirstOrDefault();
        }
    }
}
