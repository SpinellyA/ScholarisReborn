// Shared logic for the two submission commands: resolve the signed-in scholar, find their
// school's currently-open term, and find-or-create their TermRecord for it. A record's
// GradesRequired is set the first time it's created, from whether the scholar has a prior-term
// PoR (returning scholar) or not (first-year — PoR alone is a complete submission).
internal static class SubmissionResolver
{
    public static async Task<(Scholar scholar, School school, Term openTerm, TermRecord record)>
        ResolveOrCreateRecordAsync(IUnitOfWork uow, Guid scholarUserId, CancellationToken ct)
    {
        var scholars = await uow.ScholarRepository.FindAsync(s => s.UserId == scholarUserId, ct);
        var scholar = scholars.SingleOrDefault()
            ?? throw new DomainException("No scholar profile is associated with your account.");

        var school = await uow.SchoolRepository.GetById(scholar.SchoolId)
            ?? throw new DomainException("Your school could not be found.");

        var openTerm = school.Terms.FirstOrDefault(t => t.IsOpen)
            ?? throw new DomainException("There is no open term for your school right now. Submissions are closed.");

        var existing = await uow.TermRepository.FindAsync(
            r => r.ScholarId == scholar.Id && r.TermId == openTerm.Id, ct);
        var record = existing.FirstOrDefault();

        if (record is null)
        {
            var gradesRequired = await HasPriorCoursesAsync(uow, scholar.Id, school, openTerm, ct);
            record = TermRecord.Create(scholar.Id, openTerm.Id, gradesRequired);
            uow.TermRepository.Add(record);
        }

        return (scholar, school, openTerm, record);
    }

    public static async Task<bool> HasPriorCoursesAsync(
        IUnitOfWork uow, Guid scholarId, School school, Term openTerm, CancellationToken ct)
    {
        var priorTermIds = school.Terms
            .Where(t => t.TermNumber < openTerm.TermNumber)
            .Select(t => t.Id)
            .ToHashSet();

        if (priorTermIds.Count == 0)
            return false;

        var records = await uow.TermRepository.FindAsync(r => r.ScholarId == scholarId, ct);
        return records.Any(r => priorTermIds.Contains(r.TermId)
            && r.ProofOfRegistration is not null
            && r.ProofOfRegistration.Courses.Count > 0);
    }
}
