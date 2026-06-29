
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MyDbContext _context;
    protected readonly DbSet<T> _set;
    public Repository(MyDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }
    public virtual void Add(T entity)
        => _set.Add(entity);

    public virtual void Delete(T entity)
        => _set.Remove(entity);

    public virtual IEnumerable<T> GetAll()
        => _set
        .AsNoTracking()
        .AsQueryable();

    public virtual async Task<T?> GetById(Guid id)
        => await _set.FindAsync(id);

    // Tracked (like GetById): these are repository methods used by command handlers that mutate
    // and save the returned aggregates within the same unit of work. AsNoTracking here would lose
    // shadow-key values for owned collections (e.g. ScholarStatus) and break re-attachment on save.
    // Read-only call sites should query through IApplicationDbContext instead.
    public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _set.ToListAsync(cancellationToken);

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        => await _set.Where(predicate).ToListAsync(cancellationToken);

    public virtual void Update(T entity)
        => _set.Update(entity);
}

public class InvitationRepository : Repository<Invitation>, IInvitationRepository
{
    public InvitationRepository(MyDbContext context) : base(context)
    {

    }
}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(MyDbContext context) : base(context)
    {

    }
}

public class TermRecordRepository : Repository<TermRecord>, ITermRepository
{
    public TermRecordRepository(MyDbContext context) : base(context)
    {

    }
}

public class ScholarRepository : Repository<Scholar>, IScholarRepository
{
    public ScholarRepository(MyDbContext context) : base(context)
    {

    }
}

public class ScholarshipRepository : Repository<Scholarship>, IScholarshipRepository
{
    public ScholarshipRepository(MyDbContext context) : base(context)
    {
    }
}

public class SchoolRepository : Repository<School>, ISchoolRepository
{
    public SchoolRepository(MyDbContext context) : base(context)
    {
    }
}

public class StipendDropRepository : Repository<StipendDrop>, IStipendDropRepository
{
    public StipendDropRepository(MyDbContext context) : base(context)
    {
    }
}

