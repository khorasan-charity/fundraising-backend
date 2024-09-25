using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mahak.Main.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Mahak.Main;

public class ParbadEntityFrameworkStorage(
    MainDbContext dbContext,
    IRepository<Payments.Payment, long> paymentRepository,
    IRepository<Transactions.Transaction, long> transactionRepository,
    IObjectMapper objectMapper)
    : Parbad.Storage.Abstractions.IStorage, ITransientDependency
{
    public virtual IQueryable<Parbad.Storage.Abstractions.Models.Payment> Payments =>
        dbContext.Payments.Select(x =>
            objectMapper.Map<Payments.Payment, Parbad.Storage.Abstractions.Models.Payment>(x));

    public virtual IQueryable<Parbad.Storage.Abstractions.Models.Transaction> Transactions =>
        dbContext.Transactions.Select(x =>
            objectMapper.Map<Transactions.Transaction, Parbad.Storage.Abstractions.Models.Transaction>(x));

    public virtual async Task CreatePaymentAsync(Parbad.Storage.Abstractions.Models.Payment? payment,
        CancellationToken cancellationToken = default)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        var entity = objectMapper.Map<Parbad.Storage.Abstractions.Models.Payment, Payments.Payment>(payment);
        await paymentRepository.InsertAsync(entity, true, cancellationToken);

        payment.Id = entity.Id;
    }

    public virtual async Task UpdatePaymentAsync(Parbad.Storage.Abstractions.Models.Payment payment,
        CancellationToken cancellationToken = default)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        var entity = await paymentRepository.GetAsync(payment.Id, true, cancellationToken);

        objectMapper.Map(payment, entity);

        await paymentRepository.UpdateAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task DeletePaymentAsync(Parbad.Storage.Abstractions.Models.Payment payment,
        CancellationToken cancellationToken = default)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment));
        }

        await paymentRepository.DeleteAsync(payment.Id, cancellationToken: cancellationToken);
    }

    public virtual async Task CreateTransactionAsync(Parbad.Storage.Abstractions.Models.Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var entity =
            objectMapper.Map<Parbad.Storage.Abstractions.Models.Transaction, Transactions.Transaction>(transaction);

        await transactionRepository.InsertAsync(entity, true, cancellationToken);

        transaction.Id = entity.Id;
    }

    public virtual async Task UpdateTransactionAsync(Parbad.Storage.Abstractions.Models.Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var entity = await transactionRepository.GetAsync(transaction.Id, cancellationToken: cancellationToken);

        objectMapper.Map(transaction, entity);

        await transactionRepository.UpdateAsync(entity, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteTransactionAsync(Parbad.Storage.Abstractions.Models.Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        await transactionRepository.DeleteAsync(transaction.Id, cancellationToken: cancellationToken);
    }
}