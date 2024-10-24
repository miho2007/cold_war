using System.Collections.Generic;

public class USSR : Country
{
    public string Leader { get; private set; }

    public USSR(int population, List<string> majorCities, int warheadsCapacity, string currentLeader)
        : base("USSR", 0, population, currentLeader, warheadsCapacity)
    {
        Leader = currentLeader; // Assigning the current leader
    }

    public override decimal CalculateIncome()
    {
        return 2500000; // Example income increment
    }
}
