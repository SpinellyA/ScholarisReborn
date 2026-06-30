// Periods-per-year and human labels for the different term systems. A "period" is a semester /
// trimester / quarter; the academic year is written "AY {start}-{start+1}".
public static class TermSystemInfo
{
    public static int PeriodsPerYear(TermSystem termSystem) => termSystem switch
    {
        TermSystem.Semestral => 2,
        TermSystem.Trimestral => 3,
        TermSystem.QuarterSystem => 4,
        _ => 2
    };

    public static string PeriodName(TermSystem termSystem) => termSystem switch
    {
        TermSystem.Semestral => "Semester",
        TermSystem.Trimestral => "Trimester",
        TermSystem.QuarterSystem => "Quarter",
        _ => "Term"
    };

    public static string Ordinal(int n) => n switch
    {
        1 => "1st",
        2 => "2nd",
        3 => "3rd",
        _ => $"{n}th"
    };

    public static string Label(TermSystem termSystem, int academicYearStart, int periodNumber)
        => $"{Ordinal(periodNumber)} {PeriodName(termSystem)}, AY {academicYearStart}-{academicYearStart + 1}";
}
