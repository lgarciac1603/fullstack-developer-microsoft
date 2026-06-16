namespace OptimizationDemo;

public class ExecutableTask
{
    public string Name { get; }
    private readonly Action _action;

    public ExecutableTask(string name, Action action)
    {
        Name = name;
        _action = action;
    }

    public void Execute() => _action();
}

public class TaskExecutor
{
    private readonly List<ExecutableTask> _tasks = new();
    private readonly List<string> _errorLog = new();

    public void AddExecutableTask(ExecutableTask task) => _tasks.Add(task);

    public async Task ExecuteAllAsync()
    {
        foreach (var task in _tasks)
        {
            try
            {
                Console.Write($"Ejecutando {task.Name}... ");
                await Task.Run(() => task.Execute());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR");
                _errorLog.Add($"{DateTime.Now}: Error en {task.Name} - {ex.Message}");
                Console.WriteLine($"  ⚠️ Error registrado: {ex.Message}");
            }
        }

        if (_errorLog.Any())
        {
            Console.WriteLine($"\n📋 Resumen de errores ({_errorLog.Count}):");
            foreach (var error in _errorLog)
                Console.WriteLine($"  {error}");
        }
    }
}