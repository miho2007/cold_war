using System.Collections.Generic;

public class USA : Country
{
    public string President { get; private set; }

    public USA(int population, List<string> majorCities, int warheadsCapacity, string currentLeader)
        : base("USA", 0, population, currentLeader, warheadsCapacity)
    {
        President = currentLeader; // Assuming the current leader is the President
    }

    public override decimal CalculateIncome()
    {
        return 3000000; // Example income increment
    }
}
