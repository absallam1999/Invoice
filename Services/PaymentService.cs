using AutoMapper;
using invoice.Core.DTO;
using invoice.Core.DTO.Payment;
using invoice.Core.Entites;
using invoice.Core.Enums;
using invoice.Core.Interfaces.Services;
using invoice.Repo;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class PaymentService : IPaymentService
{
    private readonly IRepository<Payment> _paymentRepo;
    private readonly IRepository<Invoice> _invoiceRepo;
    private readonly IPaymentGatewayService _gatewayService;
    private readonly IRepository<PaymentMethod> _paymentMethodRepo;
    private readonly IRepository<PaymentLink> _paymentLinkRepo;
    private readonly IMapper _mapper;

    public PaymentService(IRepository<Payment> paymentRepo, IRepository<Invoice> invoiceRepo, IPaymentGatewayService gatewayService, IRepository<PaymentLink> paymentLinkRepo, IRepository<PaymentMethod> paymentMethodRepo, IMapper mapper)
    {
        _gatewayService = gatewayService;
        _invoiceRepo = invoiceRepo;
        _paymentRepo = paymentRepo;
        _paymentLinkRepo = paymentLinkRepo;
        _paymentMethodRepo = paymentMethodRepo;
        _mapper = mapper;
    }

    public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetAllAsync(string userId = null)
    {
        var payments = await _paymentRepo.GetAllAsync(userId, x => x.User, x => x.Invoice, x => x.PaymentMethod, x => x.PaymentLink);
        return new GeneralResponse<IEnumerable<PaymentReadDTO>>
        {
            Success = true,
            Message = payments.Any() ? "Payments retrieved successfully" : "No payments found",
            Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
        };
    }

    public async Task<GeneralResponse<PaymentReadDTO>> GetByIdAsync(string id, string userId = null)
    {
        var payment = await _paymentRepo.GetByIdAsync(id, userId, q => q.Include(x => x.User).Include(x => x.Invoice).Include(x => x.PaymentMethod).Include(x => x.PaymentLink));
        if (payment == null)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

        return new GeneralResponse<PaymentReadDTO>
        {
            Success = true,
            Data = _mapper.Map<PaymentReadDTO>(payment)
        };
    }

    public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByInvoiceIdAsync(string invoiceId, string userId = null)
    {
        var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId), x => x.User, x => x.PaymentMethod, x => x.PaymentLink);
        return new GeneralResponse<IEnumerable<PaymentReadDTO>>
        {
            Success = true,
            Message = payments.Any() ? "Payments retrieved successfully" : "No payments found for this invoice",
            Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
        };
    }

    public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> GetByPaymentMethodIdAsync(string paymentMethodId, string userId = null)
    {
        var payments = await _paymentRepo.QueryAsync(p => p.PaymentMethodId == paymentMethodId && (userId == null || p.UserId == userId), x => x.User, x => x.Invoice, x => x.PaymentLink);
        return new GeneralResponse<IEnumerable<PaymentReadDTO>>
        {
            Success = true,
            Message = payments.Any() ? "Payments retrieved successfully" : "No payments found for this payment method",
            Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(payments)
        };
    }

    public async Task<GeneralResponse<PaymentReadDTO>> CreateAsync(PaymentCreateDTO dto, string userId)
    {
        var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
        if (invoice == null)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Invoice not found" };

        PaymentMethod paymentMethod = null;
        if (!string.IsNullOrEmpty(dto.PaymentMethodId))
        {
            paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
        }

        if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
        {
            paymentMethod = new PaymentMethod
            {
                Name = paymentType
            };
            await _paymentMethodRepo.AddAsync(paymentMethod);
        }

        if (paymentMethod == null)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment method not found or invalid" };

        var paymentLinks = await _paymentLinkRepo.QueryAsync(
            pl => pl.InvoiceId == invoice.Id && pl.Id == dto.PaymentLinkId
        );

        PaymentLink paymentLink = paymentLinks.FirstOrDefault();

        if (paymentLink == null)
        {
            paymentLink = new PaymentLink
            {
                InvoiceId = invoice.Id,
                Value = dto.Cost,
                PaymentsNumber = "1", 
                Description = $"Payment link for invoice {invoice.Id}",
                Message = "Generated automatically",
                Link = string.Empty,
            };

            if (paymentMethod.Name is PaymentType.Stripe or PaymentType.PayPal or PaymentType.ApplePay or PaymentType.GooglePay)
            {
                var linkResponse = await _gatewayService.CreatePaymentSessionAsync(dto, paymentMethod.Name);
                if (!linkResponse.Success)
                    return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to generate payment link" };

                paymentLink.Link = linkResponse.Data;
            }

            await _paymentLinkRepo.AddAsync(paymentLink);
        }

        var payment = _mapper.Map<Payment>(dto);
        payment.UserId = userId;
        payment.PaymentMethodId = paymentMethod.Id;
        payment.PaymentLinkId = paymentLink.Id;

        var response = await _paymentRepo.AddAsync(payment);
        if (!response.Success)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to create payment" };

        var resultDto = _mapper.Map<PaymentReadDTO>(response.Data);

        return new GeneralResponse<PaymentReadDTO>
        {
            Success = true,
            Message = "Payment created successfully",
            Data = resultDto
        };
    }

    public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> CreateRangeAsync(IEnumerable<PaymentCreateDTO> dtos, string userId)
    {
        var createdPayments = new List<Payment>();

        foreach (var dto in dtos)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(dto.InvoiceId);
            if (invoice == null)
                continue;

            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
            {
                paymentMethod = new PaymentMethod { Name = paymentType };
                await _paymentMethodRepo.AddAsync(paymentMethod);
            }

            if (paymentMethod == null)
                continue;

            var paymentLinks = await _paymentLinkRepo.QueryAsync(
                pl => pl.InvoiceId == invoice.Id && pl.Id == dto.PaymentLinkId
            );
            PaymentLink paymentLink = paymentLinks.FirstOrDefault();

            if (paymentLink == null)
            {
                paymentLink = new PaymentLink
                {
                    InvoiceId = invoice.Id,
                    Value = dto.Cost,
                    PaymentsNumber = "1",
                    Description = $"Payment link for invoice {invoice.Id}",
                    Message = "Generated automatically",
                    Link = string.Empty,
                };

                if (paymentMethod.Name is PaymentType.Stripe or PaymentType.PayPal or PaymentType.ApplePay or PaymentType.GooglePay)
                {
                    var linkResponse = await _gatewayService.CreatePaymentSessionAsync(dto, paymentMethod.Name);
                    if (!linkResponse.Success)
                        continue;

                    paymentLink.Link = linkResponse.Data;
                }

                await _paymentLinkRepo.AddAsync(paymentLink);
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.UserId = userId;
            payment.PaymentMethodId = paymentMethod.Id;
            payment.PaymentLinkId = paymentLink.Id;

            var response = await _paymentRepo.AddAsync(payment);
            if (response.Success)
            {
                createdPayments.Add(response.Data);
            }
        }

        return new GeneralResponse<IEnumerable<PaymentReadDTO>>
        {
            Success = true,
            Message = "Payments created successfully",
            Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(createdPayments)
        };
    }

    public async Task<GeneralResponse<PaymentReadDTO>> UpdateAsync(string id, PaymentUpdateDTO dto, string userId)
    {
        var payment = await _paymentRepo.GetByIdAsync(id, userId);
        if (payment == null)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Payment not found" };

        PaymentMethod paymentMethod = null;
        if (!string.IsNullOrEmpty(dto.PaymentMethodId))
        {
            paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
        }

        if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
        {
            paymentMethod = new PaymentMethod { Name = paymentType };
            await _paymentMethodRepo.AddAsync(paymentMethod);
        }

        if (paymentMethod != null)
            payment.PaymentMethodId = paymentMethod.Id;

        _mapper.Map(dto, payment);

        var response = await _paymentRepo.UpdateAsync(payment);
        if (!response.Success)
            return new GeneralResponse<PaymentReadDTO> { Success = false, Message = "Failed to update payment" };

        return new GeneralResponse<PaymentReadDTO>
        {
            Success = true,
            Message = "Payment updated successfully",
            Data = _mapper.Map<PaymentReadDTO>(response.Data)
        };
    }

    public async Task<GeneralResponse<IEnumerable<PaymentReadDTO>>> UpdateRangeAsync(IEnumerable<PaymentUpdateDTO> dtos, string userId)
    {
        var updatedPayments = new List<Payment>();

        foreach (var dto in dtos)
        {
            var payment = await _paymentRepo.GetByIdAsync(dto.Id, userId);
            if (payment == null)
                continue;

            // Handle PaymentMethod update
            PaymentMethod paymentMethod = null;
            if (!string.IsNullOrEmpty(dto.PaymentMethodId))
            {
                paymentMethod = await _paymentMethodRepo.GetByIdAsync(dto.PaymentMethodId);
            }

            if (paymentMethod == null && Enum.TryParse<PaymentType>(dto.Name, out var paymentType))
            {
                paymentMethod = new PaymentMethod { Name = paymentType };
                await _paymentMethodRepo.AddAsync(paymentMethod);
            }

            if (paymentMethod != null)
                payment.PaymentMethodId = paymentMethod.Id;

            _mapper.Map(dto, payment);
            updatedPayments.Add(payment);
        }

        var response = await _paymentRepo.UpdateRangeAsync(updatedPayments);
        if (!response.Success)
            return new GeneralResponse<IEnumerable<PaymentReadDTO>> { Success = false, Message = "Failed to update payments" };

        return new GeneralResponse<IEnumerable<PaymentReadDTO>>
        {
            Success = true,
            Message = "Payments updated successfully",
            Data = _mapper.Map<IEnumerable<PaymentReadDTO>>(response.Data)
        };
    }

    public async Task<GeneralResponse<bool>> DeleteAsync(string id, string userId)
    {
        var payment = await _paymentRepo.GetByIdAsync(id, userId);
        if (payment == null)
            return new GeneralResponse<bool> { Success = false, Message = "Payment not found", Data = false };

        var response = await _paymentRepo.DeleteAsync(id);
        return new GeneralResponse<bool> { Success = response.Success, Message = response.Message, Data = response.Success };
    }

    public async Task<GeneralResponse<bool>> DeleteRangeAsync(IEnumerable<string> ids, string userId)
    {
        var entities = new List<Payment>();
        foreach (var id in ids)
        {
            var payment = await _paymentRepo.GetByIdAsync(id, userId);
            if (payment != null)
                entities.Add(payment);
        }

        if (!entities.Any())
            return new GeneralResponse<bool> { Success = false, Message = "No payments found to delete", Data = false };

        var response = await _paymentRepo.DeleteRangeAsync(entities.Select(e => e.Id));
        return new GeneralResponse<bool> { Success = response.Success, Message = response.Message, Data = response.Success };
    }

    public async Task<bool> ExistsAsync(string id, string userId = null)
    {
        return await _paymentRepo.ExistsAsync(p => p.Id == id && (userId == null || p.UserId == userId));
    }

    public async Task<int> CountAsync(string userId = null)
    {
        return await _paymentRepo.CountAsync(p => userId == null || p.UserId == userId);
    }

    //public async Task<GeneralResponse<string>> ProcessPaymentAsync(string paymentMethodId, PaymentCreateDTO dto, string userId)
    //{
    //    var method = await _paymentRepo.GetQueryable()
    //        .Select(p => p.PaymentMethod)
    //        .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId);

    //    if (method == null)
    //        return new GeneralResponse<string> { Success = false, Message = "Payment method not found" };

    //    switch (method.Name)
    //    {
    //        case PaymentType.Cash:
    //        case PaymentType.Delivery:
    //            var cashPayment = new Payment
    //            {
    //                InvoiceId = dto.InvoiceId,
    //                Cost = dto.Cost,
    //                Name = dto.Name,
    //                PaymentMethodId = paymentMethodId,
    //                UserId = userId
    //            };
    //            await _paymentRepo.AddAsync(cashPayment);
    //            return new GeneralResponse<string> { Success = true, Message = "Payment recorded as cash/delivery" };

    //        case PaymentType.Stripe:
    //            var stripeUrl = await _gatewayService.CreateStripeSessionAsync(dto);
    //            return stripeUrl;

    //        case PaymentType.PayPal:
    //            var payPalUrl = await _gatewayService.CreatePayPalSessionAsync(dto);
    //            return payPalUrl;

    //        case PaymentType.ApplePay:
    //            var applePayUrl = await _gatewayService.CreateApplePaySessionAsync(dto);
    //            return applePayUrl;

    //        case PaymentType.GooglePay:
    //            var googlePayUrl = await _gatewayService.CreateGooglePaySessionAsync(dto);
    //            return googlePayUrl;

    //        case PaymentType.CreditCard:
    //        case PaymentType.DebitCard:
    //        case PaymentType.BankTransfer:
    //        case PaymentType.Mada:
    //        case PaymentType.STCPay:
    //        case PaymentType.Sadad:
    //            // Implement other gateways or return stub for now
    //            return new GeneralResponse<string>
    //            {
    //                Success = true,
    //                Message = $"Payment method {method.Name} not fully implemented yet",
    //                Data = null
    //            };

    //        default:
    //            return new GeneralResponse<string>
    //            {
    //                Success = false,
    //                Message = "Unknown payment method"
    //            };
    //    }
    //}

    public async Task<GeneralResponse<string>> ProcessPaymentAsync(string paymentMethodId, PaymentCreateDTO dto, string userId)
    {
        var method = await _paymentRepo.GetQueryable()
            .Select(p => p.PaymentMethod)
            .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId);

        if (method == null)
            return new GeneralResponse<string> { Success = false, Message = "Payment method not found" };

        if (method.Name == PaymentType.Cash || method.Name == PaymentType.Delivery)
        {
            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                Cost = dto.Cost,
                Name = dto.Name,
                PaymentMethodId = paymentMethodId,
                UserId = userId
            };
            await _paymentRepo.AddAsync(payment);

            return new GeneralResponse<string>
            {
                Success = true,
                Message = $"Payment recorded as {method.Name}"
            };
        }

        var sessionResponse = await _gatewayService.CreatePaymentSessionAsync(dto, method.Name);

        if (!sessionResponse.Success)
            return new GeneralResponse<string>
            {
                Success = false,
                Message = $"Failed to create payment session: {sessionResponse.Message}"
            };

        return new GeneralResponse<string>
        {
            Success = true,
            Message = $"{method.Name} session created successfully",
            Data = sessionResponse.Data
        };
    }

    public async Task<GeneralResponse<decimal>> GetTotalPaidByInvoiceAsync(string invoiceId, string userId = null)
    {
        var payments = await _paymentRepo.QueryAsync(p => p.InvoiceId == invoiceId && (userId == null || p.UserId == userId));
        return new GeneralResponse<decimal> { Success = true, Data = payments.Sum(p => p.Cost) };
    }
}
