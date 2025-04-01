namespace vz777.Events
{
    public enum Priority
    {
        Lowest = int.MinValue,
        Low = -10000,
        Unset = 0,
        High = 10000,
        Highest = int.MaxValue,
    }
}