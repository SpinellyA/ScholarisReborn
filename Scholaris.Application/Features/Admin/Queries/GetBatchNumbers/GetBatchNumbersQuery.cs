public record GetBatchNumbersQuery(Guid SchoolId) : IQuery<List<int>>;
