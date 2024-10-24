using System;

public abstract class Country
{
    public string Name { get; private set; }
    public decimal Income { get; set; }
    public int Population { get; private set; }
    public string CurrentLeader { get; private set; }
    public int WarheadsCapacity { get; private set; } // Correct property name

    protected Country(string name, decimal income, int population, string currentLeader, int warheadsCapacity)
    {
        Name = name;
        Income = income;
        Population = population;
        CurrentLeader = currentLeader;
        WarheadsCapacity = warheadsCapacity; // Initialize the warheads capacity
    }

    public abstract decimal CalculateIncome();

    public void DisplayInfo()
    {
        Console.WriteLine($"Country: {Name}");
        Console.WriteLine($"Income: {Income:C}");
        Console.WriteLine($"Population: {Population}");
        Console.WriteLine($"Current Leader: {CurrentLeader}");
        Console.WriteLine($"Warheads Capacity: {WarheadsCapacity}");
    }
}