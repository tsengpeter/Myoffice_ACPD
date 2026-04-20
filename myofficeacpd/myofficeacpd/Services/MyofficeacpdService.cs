using Microsoft.EntityFrameworkCore;
using myofficeacpd.Data;
using myofficeacpd.Interfaces;
using myofficeacpd.Models;

namespace myofficeacpd.Services
{
    public class MyofficeacpdService : IMyofficeacpdService
    {
        private readonly MyofficeAcpdDbContext _db;

        public MyofficeacpdService(MyofficeAcpdDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<GetAcpdResultModel>> GetAllAsync(GetAcpdQueryModel query)
        {
            var q = _db.MyOfficeAcpds.AsQueryable();

            if (query.Status.HasValue)
                q = q.Where(x => x.AcpdStatus == query.Status.Value);

            if (query.Stop.HasValue)
                q = q.Where(x => x.AcpdStop == query.Stop.Value);

            if (!string.IsNullOrWhiteSpace(query.Keyword))
                q = q.Where(x =>
                    x.AcpdCname!.Contains(query.Keyword) ||
                    x.AcpdEname!.Contains(query.Keyword) ||
                    x.AcpdLoginId!.Contains(query.Keyword));

            return await q.Select(x => new GetAcpdResultModel
            {
                Sid      = x.AcpdSid,
                Cname    = x.AcpdCname,
                Ename    = x.AcpdEname,
                Sname    = x.AcpdSname,
                Email    = x.AcpdEmail,
                Status   = x.AcpdStatus,
                Stop     = x.AcpdStop,
                StopMemo = x.AcpdStopMemo,
                LoginId  = x.AcpdLoginId,
                Memo     = x.AcpdMemo,
            }).ToListAsync();
        }

        public async Task<GetAcpdResultModel?> GetBySidAsync(string sid)
        {
            var entity = await _db.MyOfficeAcpds.FindAsync(sid);

            if (entity is null)
                return null;

            return new GetAcpdResultModel
            {
                Sid      = entity.AcpdSid,
                Cname    = entity.AcpdCname,
                Ename    = entity.AcpdEname,
                Sname    = entity.AcpdSname,
                Email    = entity.AcpdEmail,
                Status   = entity.AcpdStatus,
                Stop     = entity.AcpdStop,
                StopMemo = entity.AcpdStopMemo,
                LoginId  = entity.AcpdLoginId,
                Memo     = entity.AcpdMemo,
            };
        }
    }
}
