

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
        public async Task Handle_ShouldCreateAdminInvitation()
        {
            var options = CreateNewContextOptions();
            var mockPublisher = new Mock<IPublisher>();

            using var context = new MyDbContext(options);
            var uow = new UnitOfWork(context, mockPublisher.Object);
            var handler = new InviteAdminCommandHandler(uow);

            var command = new InviteAdminCommand(Guid.NewGuid(), "newadmin@example.com");

            await handler.Handle(command, CancellationToken.None);

            var invitation = Assert.Single(await uow.InvitationRepository.GetAllAsync());
            Assert.Equal("newadmin@example.com", invitation.Email);
            Assert.Equal(InvitationType.Admin, invitation.Type);
        }
    }
}
