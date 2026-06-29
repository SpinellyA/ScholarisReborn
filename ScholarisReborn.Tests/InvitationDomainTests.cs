public class InvitationDomainTests
{
    [Fact]
    public void Create_AdminInvitation_WithoutSchoolOrScholarship_Succeeds()
    {
        var invitation = Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid());

        Assert.Equal(InvitationType.Admin, invitation.Type);
        Assert.Null(invitation.SchoolId);
        Assert.Null(invitation.ScholarshipId);
        Assert.Equal(InvitationStatus.Pending, invitation.Status);
    }

    [Fact]
    public void Create_AdminInvitation_WithSchoolId_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid(), schoolId: Guid.NewGuid()));
    }

    [Fact]
    public void Create_ScholarInvitation_WithSchoolScholarshipBatchAndProgram_Succeeds()
    {
        var schoolId = Guid.NewGuid();
        var scholarshipId = Guid.NewGuid();

        var invitation = Invitation.Create("scholar@example.com", InvitationType.Scholar, Guid.NewGuid(), schoolId, scholarshipId, 2024, "BS Computer Science");

        Assert.Equal(schoolId, invitation.SchoolId);
        Assert.Equal(scholarshipId, invitation.ScholarshipId);
        Assert.Equal(2024, invitation.BatchNumber);
        Assert.Equal("BS Computer Science", invitation.DegreeProgram);
    }

    [Fact]
    public void Create_ScholarInvitation_WithoutBatchNumber_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Invitation.Create("scholar@example.com", InvitationType.Scholar, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), degreeProgram: "BS CS"));
    }

    [Fact]
    public void Create_ScholarInvitation_WithoutDegreeProgram_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Invitation.Create("scholar@example.com", InvitationType.Scholar, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), batchNumber: 2024));
    }

    [Fact]
    public void Create_ScholarInvitation_WithoutSchoolId_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Invitation.Create("scholar@example.com", InvitationType.Scholar, Guid.NewGuid(), scholarshipId: Guid.NewGuid()));
    }

    [Fact]
    public void Create_ScholarInvitation_WithoutScholarshipId_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Invitation.Create("scholar@example.com", InvitationType.Scholar, Guid.NewGuid(), schoolId: Guid.NewGuid()));
    }

    [Fact]
    public void Revoke_PendingInvitation_SetsRevokedStatus()
    {
        var invitation = Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid());

        invitation.Revoke();

        Assert.True(invitation.IsRevoked);
        Assert.Equal(InvitationStatus.Revoked, invitation.Status);
    }

    [Fact]
    public void Revoke_AlreadyRevokedInvitation_Throws()
    {
        var invitation = Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid());
        invitation.Revoke();

        Assert.Throws<DomainException>(() => invitation.Revoke());
    }

    [Fact]
    public void MarkAsUsed_RevokedInvitation_Throws()
    {
        var invitation = Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid());
        invitation.Revoke();

        Assert.Throws<DomainException>(() => invitation.MarkAsUsed());
    }

    [Fact]
    public void MarkAsUsed_PendingInvitation_SetsAcceptedStatus()
    {
        var invitation = Invitation.Create("admin@example.com", InvitationType.Admin, Guid.NewGuid());

        invitation.MarkAsUsed();

        Assert.True(invitation.IsUsed);
        Assert.Equal(InvitationStatus.Accepted, invitation.Status);
    }
}
