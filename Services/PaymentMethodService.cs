using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IRepository<PaymentMethod> _repo;

        public PaymentMethodService(IRepository<PaymentMethod> repo)
        {
            _repo = repo;
        }

        public async Task<GeneralResponse<IEnumerable<PaymentMethod>>> GetAllAsync()
        {
            var methods = await _repo.GetAllAsync();
            return new GeneralResponse<IEnumerable<PaymentMethod>>
            {
                Success = true,
                Message = "Payment methods retrieved",
                Data = methods
            };
        }

        public async Task<GeneralResponse<PaymentMethod>> GetByIdAsync(string id)
        {
            var method = await _repo.GetByIdAsync(id);
            if (method == null) return new GeneralResponse<PaymentMethod> { Success = false, Message = "Not found" };
            return new GeneralResponse<PaymentMethod> { Success = true, Data = method };
        }

        public async Task<GeneralResponse<PaymentMethod>> CreateAsync(PaymentType type)
        {
            var method = new PaymentMethod { Name = type };
            var response = await _repo.AddAsync(method);
            return new GeneralResponse<PaymentMethod> { Success = true, Data = response.Data };
        }

        public async Task<GeneralResponse<PaymentMethod>> UpdateAsync(string id, PaymentType type)
        {
            var method = await _repo.GetByIdAsync(id);
            if (method == null) return new GeneralResponse<PaymentMethod> { Success = false, Message = "Not found" };

            method.Name = type;
            var response = await _repo.UpdateAsync(method);
            return new GeneralResponse<PaymentMethod> { Success = true, Data = response.Data };
        }

        public async Task<GeneralResponse<bool>> DeleteAsync(string id)
        {
            var response = await _repo.DeleteAsync(id);
            return new GeneralResponse<bool> { Success = response.Success, Data = response.Success };
        }
    }
}
