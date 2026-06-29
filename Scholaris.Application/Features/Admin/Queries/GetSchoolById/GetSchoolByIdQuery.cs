public record GetSchoolByIdQuery(Guid SchoolId) : IQuery<SchoolDetailsDto?>;
