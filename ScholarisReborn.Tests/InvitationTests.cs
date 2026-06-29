

using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ScholarisReborn.Tests
{
    public class InvitationTests
    {
        private DbContextOptions<MyDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldWriteRecordToDatabase()
        {
            Console.WriteLine("Hello world!");
            var options = CreateNewContextOptions();
            var mockPublisher = new Mock<IPublisher>();

            using (var context = new MyDbContext(options))
            {
                var uow = new UnitOfWork(context, mockPublisher.Object);
            }
        }

        [Fact]
        public async Task Handle_ShouldSendEmailOnCreation()
        {
            var options = CreateNewContextOptions();
            var mockPublisher = new Mock<IPublisher>();

            var command = new 

        }
    }
}
