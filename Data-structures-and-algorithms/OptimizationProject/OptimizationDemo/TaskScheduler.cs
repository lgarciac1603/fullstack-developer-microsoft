namespace OptimizationDemo;

public class ScheduledTask
{
    public string Name { get; }
    public int Priority { get; }
    public int Reward { get; }

    public ScheduledTask(string name, int priority, int reward)
    {
        Name = name;
        Priority = priority;
        Reward = reward;
    }
}

public class TaskScheduler
{
    private readonly List<ScheduledTask> _tasks = new();

    public void AddTask(ScheduledTask task) => _tasks.Add(task);

    public List<ScheduledTask> Schedule()
    {
        // Ordenar por recompensa descendente (greedy)
        return _tasks.OrderByDescending(t => t.Reward).ToList();
    }

    public int CalculateTotalReward(List<ScheduledTask> scheduledTasks)
    {
        return scheduledTasks.Sum(t => t.Reward);
    }
}