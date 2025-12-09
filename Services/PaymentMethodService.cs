using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.PaymentMethod;
using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;

namespace invoice.Services
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IRepository<PaymentMethod> _repo;
        private readonly IMapper _mapper;

        public PaymentMethodService(IRepository<PaymentMethod> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }

        public async Task<GeneralResponse<IEnumerable<PaymentMethodReadDTO>>> GetAllAsync()
        {
            var methods = await _repo.GetAllAsync();

            var dto = _mapper.Map<IEnumerable<PaymentMethodReadDTO>>(methods);

            return new GeneralResponse<IEnumerable<PaymentMethodReadDTO>>(true, "Payment methods retrieved successfully", dto);

        }

        public async Task<GeneralResponse<PaymentMethod>> GetByIdAsync(string id)
        {
            var method = await _repo.GetByIdAsync(id);
            if (method == null) return new GeneralResponse<PaymentMethod> { Success = false, Message = "Not found" };
            return new GeneralResponse<PaymentMethod> { Success = true, Data = method };
        }

        public async Task<string> GetIdFromTypeAsync(PaymentType paymentType)
        {
            var methods = await _repo.GetAllAsync();
            var method = methods.FirstOrDefault(m => m.Name == paymentType);

            if (method.Name == null)
                throw new Exception($"Payment method not found for type: {paymentType}");

            return method.Id;
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
