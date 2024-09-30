using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parbad.Storage.Abstractions;
using Parbad.Storage.Abstractions.Models;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Volo.Abp.ObjectMapping;

namespace Mahak.Main;

public class ParbadEntityFrameworkStorageManager(
    IStorage storage,
    IRepository<Payments.Payment, long> paymentRepository,
    IRepository<Transactions.Transaction, long> transactionRepository,
    IAsyncQueryableExecuter asyncExecutor,
    IObjectMapper objectMapper)
    : IStorageManager, ITransientDependency
{
    public virtual Task CreatePaymentAsync(Parbad.Storage.Abstractions.Models.Payment payment,
        CancellationToken cancellationToken = default)
    {
        return storage.CreatePaymentAsync(payment, cancellationToken);
    }

    public virtual Task UpdatePaymentAsync(Parbad.Storage.Abstractions.Models.Payment payment,
        CancellationToken cancellationToken = default)
    {
        return storage.UpdatePaymentAsync(payment, cancellationToken);
    }

    public virtual Task CreateTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        return storage.CreateTransactionAsync(transaction, cancellationToken);
    }

    public virtual async Task<Parbad.Storage.Abstractions.Models.Payment?> GetPaymentByTrackingNumberAsync(
        long trackingNumber, CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.FindAsync(x => x.TrackingNumber == trackingNumber,
            cancellationToken: cancellationToken);

        return objectMapper.Map<Payments.Payment?, Parbad.Storage.Abstractions.Models.Payment?>(payment);
    }

    public virtual async Task<Parbad.Storage.Abstractions.Models.Payment?> GetPaymentByTokenAsync(string paymentToken,
        CancellationToken cancellationToken = default)
    {
        var payment = await paymentRepository.FindAsync(x => x.Token == paymentToken,
            cancellationToken: cancellationToken);

        return objectMapper.Map<Payments.Payment?, Parbad.Storage.Abstractions.Models.Payment?>(payment);
    }

    public virtual async Task<bool> DoesPaymentExistAsync(long trackingNumber,
        CancellationToken cancellationToken = default)
    {
        var q = await paymentRepository.GetQueryableAsync();
        return await asyncExecutor.AnyAsync(q, x => x.TrackingNumber == trackingNumber, cancellationToken);
    }

    public virtual async Task<bool> DoesPaymentExistAsync(string paymentToken,
        CancellationToken cancellationToken = default)
    {
        var q = await paymentRepository.GetQueryableAsync();
        return await asyncExecutor.AnyAsync(q, x => x.Token == paymentToken, cancellationToken);
    }

    public virtual async Task<List<Transaction>> GetTransactionsAsync(
        Parbad.Storage.Abstractions.Models.Payment payment, CancellationToken cancellationToken = default)
    {
        var transactions = await transactionRepository.GetListAsync(x =>
            x.PaymentId == payment.Id, cancellationToken: cancellationToken);

        return objectMapper.Map<List<Transactions.Transaction>, List<Transaction>>(transactions);
    }
}