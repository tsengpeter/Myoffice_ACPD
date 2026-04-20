using Microsoft.EntityFrameworkCore;
using myofficeacpd.Data;
using myofficeacpd.Data.Entities;
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

        public async Task<PostAcpdResultModel> CreateAsync(PostAcpdRequestModel request)
        {
            var existing = await _db.MyOfficeAcpds
                .FirstOrDefaultAsync(x => x.AcpdLoginId == request.LoginId);

            if (existing is not null && existing.AcpdStop == false)
                throw new InvalidOperationException($"LoginId '{request.LoginId}' 已存在且為啟用狀態。");

            var sid = await GenerateNewSidAsync();

            var entity = new MyOfficeAcpd
            {
                AcpdSid         = sid,
                AcpdCname       = request.Cname,
                AcpdEname       = request.Ename,
                AcpdSname       = request.Sname,
                AcpdEmail       = request.Email,
                AcpdStatus      = request.Status ?? 0,
                AcpdStop        = request.Stop ?? false,
                AcpdStopMemo    = request.StopMemo,
                AcpdLoginId     = request.LoginId,
                AcpdLoginPwd    = request.LoginPwd,
                AcpdMemo        = request.Memo,
                AcpdNowDateTime = DateTime.Now,
                AcpdNowId       = "SYS",
                AcpdUpdDateTime = DateTime.Now,
                AcpdUpdId       = "SYS",
            };

            _db.MyOfficeAcpds.Add(entity);
            await _db.SaveChangesAsync();

            return new PostAcpdResultModel
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

        public async Task<PutAcpdResultModel?> UpdateAsync(string sid, PutAcpdRequestModel request)
        {
            var entity = await _db.MyOfficeAcpds.FindAsync(sid);

            if (entity is null)
                return null;

            if (entity.AcpdStop == true)
                throw new InvalidOperationException($"SID '{sid}' 已停用，無法更新。");

            entity.AcpdCname       = request.Cname;
            entity.AcpdEname       = request.Ename;
            entity.AcpdSname       = request.Sname;
            entity.AcpdEmail       = request.Email;
            entity.AcpdStatus      = request.Status ?? entity.AcpdStatus;
            entity.AcpdStop        = request.Stop ?? entity.AcpdStop;
            entity.AcpdStopMemo    = request.StopMemo;
            entity.AcpdLoginId     = request.LoginId;
            entity.AcpdMemo        = request.Memo;
            entity.AcpdUpdDateTime = DateTime.Now;
            entity.AcpdUpdId       = "SYS";

            if (!string.IsNullOrWhiteSpace(request.LoginPwd))
                entity.AcpdLoginPwd = request.LoginPwd;

            await _db.SaveChangesAsync();

            return new PutAcpdResultModel
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

        public async Task<bool> DeleteAsync(string sid)
        {
            var entity = await _db.MyOfficeAcpds.FindAsync(sid);

            if (entity is null)
                return false;

            entity.AcpdStop        = true;
            entity.AcpdUpdDateTime = DateTime.Now;
            entity.AcpdUpdId       = "SYS";

            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 依照 NEWSID SP 演算法產生 20 碼唯一主鍵：
        /// 2碼年份(Base36) + 3碼年內第幾天 + 5碼當天秒數 + 10碼亂數
        /// </summary>
        private async Task<string> GenerateNewSidAsync()
        {
            const string alphabets = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string sid;
            do
            {
                var now         = DateTime.Now;
                var currentYear = Math.Min(now.Year - 2000, 1295);
                var dayOfYear   = now.DayOfYear;
                var secondOfDay = now.Second + now.Minute * 60 + now.Hour * 3600;

                var firstDigit  = alphabets[(currentYear / 36) % 36];
                var secondDigit = alphabets[currentYear % 36];
                var prefix      = $"{firstDigit}{secondDigit}";
                var dayCode     = dayOfYear.ToString().PadLeft(3, '0');
                var secondCode  = secondOfDay.ToString().PadLeft(5, '0');
                var random      = Math.Abs(BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0)) % 10_000_000_000L;
                var randomValue = random.ToString().PadLeft(10, '0');

                sid = prefix + dayCode + secondCode + randomValue;
            }
            while (await _db.MyOfficeAcpds.AnyAsync(x => x.AcpdSid == sid));

            return sid;
        }
    }
}
