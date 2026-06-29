
public record FinancialGrant(string Name, double Amount)
{
    public static FinancialGrant Create(string name, double amount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Grant name cannot be empty.");
        if (amount <= 0)
            throw new DomainException("Grant amount must be positive.");
        return new FinancialGrant(name, amount);
    }
}

