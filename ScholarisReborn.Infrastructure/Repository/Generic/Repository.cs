
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

    public virtual void Update(T entity)
        => _set.Update(entity);
}

public class InvitationRepository : Repository<Invitation>, IInvitationRepository
{
    public InvitationRepository(MyDbContext context) : base(context)
    {
        
    }
}

public interface IInvitationRepository : IRepository<Invitation>
{

}

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(MyDbContext context) : base(context)
    {
        
    }
}

public interface IUserRepository : IRepository<User> { }

public class TermRecordRepository : Repository<TermRecord>, ITermRepository
{
    public TermRecordRepository(MyDbContext context) : base(context)
    {
         
    }
}

public interface ITermRepository : IRepository<TermRecord> { }

public class ScholarRepository : Repository<Scholar>, IScholarRepository
{
    public ScholarRepository(MyDbContext context) : base(context)
    {
         
    }
}

public interface IScholarRepository : IRepository<Scholar>
{

}

public class ScholarshipRepository : Repository<Scholarship>, IScholarshipRepository
{
    public ScholarshipRepository(MyDbContext context) : base(context)
    {
    }
}

public interface IScholarshipRepository : IRepository<Scholarship> { }

public class SchoolRepository : Repository<School>, ISchoolRepository
{
    public SchoolRepository(MyDbContext context) : base(context)
    {
    }
}

public interface ISchoolRepository : IRepository<School> { }

public class StipendDropRepository : Repository<StipendDrop>, IStipendDropRepository
{
    public StipendDropRepository(MyDbContext context) : base(context)
    {
    }
}

public interface IStipendDropRepository : IRepository<StipendDrop> { }

