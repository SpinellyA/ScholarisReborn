// Resolves a friendly display name for a domain User (profile name, falling back to email).
public static class UserDisplay
{
    public static string NameOf(User? user)
    {
        if (user is null)
            return "—";
        if (user.Profile is not null && !string.IsNullOrWhiteSpace(user.Profile.LastName))
            return $"{user.Profile.FirstName} {user.Profile.LastName}".Trim();
        return user.Email;
    }
}
