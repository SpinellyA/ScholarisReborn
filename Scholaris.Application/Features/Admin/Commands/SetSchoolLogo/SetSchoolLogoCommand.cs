using MediatR;

public record SetSchoolLogoCommand(Guid SchoolId, byte[] Content, string ContentType) : ICommand;
