using myofficeacpd.Models;

namespace myofficeacpd.Interfaces
{
    public interface IMyofficeacpdService
    {
        Task<IEnumerable<GetAcpdResultModel>> GetAllAsync(GetAcpdQueryModel query);
        Task<GetAcpdResultModel?> GetBySidAsync(string sid);
        Task<PostAcpdResultModel> CreateAsync(PostAcpdRequestModel request);
    }
}
