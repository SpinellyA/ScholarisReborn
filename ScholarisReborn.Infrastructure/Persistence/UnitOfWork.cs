using System.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly MyDbContext _context;
    private readonly IPublisher _publisher;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public IInvitationRepository InvitationRepository => new InvitationRepository(_context);
    public IUserRepository UserRepository => new UserRepository(_context);
    public ITermRepository TermRepository => new TermRecordRepository(_context);
    public IScholarRepository ScholarRepository => new ScholarRepository(_context);
    public IScholarshipRepository ScholarshipRepository => new ScholarshipRepository(_context);
    public ISchoolRepository SchoolRepository => new SchoolRepository(_context);
    public IStipendDropRepository StipendDropRepository => new StipendDropRepository(_context);

    public UnitOfWork(MyDbContext context, IPublisher publisher)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _currentTransaction;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
       
        await PublishDomainEventsAsync(cancellationToken);

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken); // Fail-safe check
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.Entity.ClearEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
            var notification = (INotification)Activator.CreateInstance(notificationType, domainEvent)!;
            await _publisher.Publish(notification, cancellationToken);
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync();
        }
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}