public class UserDomainTests
{
    [Fact]
    public void CreateWithId_SetsIdAndProfile()
    {
        var id = Guid.NewGuid();
        var profile = UserProfile.Create("Juan", "Dela Cruz");

        var user = User.CreateWithId(id, "juan@example.com", profile);

        Assert.Equal(id, user.Id);
        Assert.Equal(profile, user.Profile);
    }

    [Fact]
    public void CreateWithId_EmptyId_Throws()
    {
        Assert.Throws<DomainException>(() => User.CreateWithId(Guid.Empty, "juan@example.com"));
    }

    [Fact]
    public void SetProfile_UpdatesProfile()
    {
        var user = User.CreateWithId(Guid.NewGuid(), "juan@example.com");
        var profile = UserProfile.Create("Juan", "Dela Cruz");

        user.SetProfile(profile);

        Assert.Equal(profile, user.Profile);
    }

    [Fact]
    public void UserProfile_Create_RequiresFirstAndLastName()
    {
        Assert.Throws<DomainException>(() => UserProfile.Create("", "Dela Cruz"));
        Assert.Throws<DomainException>(() => UserProfile.Create("Juan", ""));
    }
}
