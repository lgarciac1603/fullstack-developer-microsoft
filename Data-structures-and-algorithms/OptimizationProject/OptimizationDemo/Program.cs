using System.Diagnostics;
using OptimizationDemo;

Console.WriteLine("=== PROYECTO DE OPTIMIZACIÓN DE ALGORITMOS ===\n");

// ========================================================
// 1. BINARY TREE OPTIMIZED (Árbol Binario Balanceado)
// ========================================================
Console.WriteLine("1. OPTIMIZED BINARY TREE (AVL)");
Console.WriteLine("--------------------------------");
var avlTree = new AVLTree();
var random = new Random();
Console.WriteLine("Insertando 1000 elementos aleatorios...");
for (int i = 0; i < 1000; i++)
    avlTree.Insert(random.Next(1, 10000));

var sw = Stopwatch.StartNew();
bool found = avlTree.Search(5000);
sw.Stop();
Console.WriteLine($"Búsqueda de valor 5000: {(found ? "Encontrado" : "No encontrado")} - Tiempo: {sw.ElapsedTicks} ticks");
Console.WriteLine($"Altura del árbol: {avlTree.Height()}");
Console.WriteLine();

// ========================================================
// 2. TASK SCHEDULING OPTIMIZED (Priority Queue)
// ========================================================
Console.WriteLine("2. OPTIMIZED TASK SCHEDULING");
Console.WriteLine("-----------------------------");
var scheduler = new OptimizationDemo.TaskScheduler();
scheduler.AddTask(new ScheduledTask("Tarea A", 5, 10));
scheduler.AddTask(new ScheduledTask("Tarea B", 3, 20));
scheduler.AddTask(new ScheduledTask("Tarea C", 8, 15));
scheduler.AddTask(new ScheduledTask("Tarea D", 2, 30));
scheduler.AddTask(new ScheduledTask("Tarea E", 6, 25));

sw.Restart();
var scheduled = scheduler.Schedule();
sw.Stop();
Console.WriteLine($"Tiempo de scheduling: {sw.ElapsedTicks} ticks");
foreach (var task in scheduled)
    Console.WriteLine($"  - {task.Name}: Prioridad {task.Priority}, Recompensa {task.Reward}");
Console.WriteLine($"Recompensa total: {scheduler.CalculateTotalReward(scheduled)}");
Console.WriteLine();

// ========================================================
// 3. SORTING ALGORITHM IMPROVED (O(n²) -> O(n log n))
// ========================================================
Console.WriteLine("3. SORTING ALGORITHM: BUBBLE SORT vs QUICKSORT");
Console.WriteLine("----------------------------------------------");
int[] smallArray = GenerateRandomArray(500);
int[] largeArray = GenerateRandomArray(10000);

// Bubble Sort O(n²)
sw.Restart();
BubbleSort.Sort((int[])smallArray.Clone());
sw.Stop();
Console.WriteLine($"Bubble Sort (500 elementos): {sw.ElapsedMilliseconds} ms");

// QuickSort O(n log n)
sw.Restart();
QuickSort.Sort((int[])largeArray.Clone(), 0, largeArray.Length - 1);
sw.Stop();
Console.WriteLine($"QuickSort (10000 elementos): {sw.ElapsedMilliseconds} ms");
Console.WriteLine();

// ========================================================
// 4. DEBUGGED TASK EXECUTION
// ========================================================
Console.WriteLine("4. DEBUGGED TASK EXECUTION WITH ERROR HANDLING");
Console.WriteLine("---------------------------------------------");
var executor = new TaskExecutor();
executor.AddExecutableTask(new ExecutableTask("Tarea 1", () => Console.WriteLine("  ✓ Tarea 1 completada")));
executor.AddExecutableTask(new ExecutableTask("Tarea 2", () => throw new InvalidOperationException("Error simulado")));
executor.AddExecutableTask(new ExecutableTask("Tarea 3", () => Console.WriteLine("  ✓ Tarea 3 completada")));

await executor.ExecuteAllAsync();
Console.WriteLine();

// ========================================================
// 5. PERFORMANCE REPORT
// ========================================================
Console.WriteLine("5. PERFORMANCE REPORT");
Console.WriteLine("---------------------");
Console.WriteLine("| Algoritmo           | Complejidad Antes | Complejidad Después | Mejora     |");
Console.WriteLine("|---------------------|-------------------|---------------------|------------|");
Console.WriteLine("| Binary Tree (BST)   | O(n) peor caso    | O(log n) AVL        | Significativa |");
Console.WriteLine("| Task Scheduling     | O(n²) greedy      | O(n log n) con Heap | Notable    |");
Console.WriteLine("| Sorting (Bubble)    | O(n²)             | O(n log n) QuickSort| Óptima     |");
Console.WriteLine("| Task Execution      | Sin manejo errores| Con try-catch/logging| Robusta    |");
Console.WriteLine();

// ========================================================
// 6. LLM CONTRIBUTION REFLECTION
// ========================================================
Console.WriteLine("6. LLM CONTRIBUTION REFLECTION");
Console.WriteLine("-------------------------------");
Console.WriteLine("Microsoft Copilot contribuyó en:");
Console.WriteLine("1. Generación del código base para AVL Tree con balanceo");
Console.WriteLine("2. Optimización del scheduler usando Priority Queue (Heap)");
Console.WriteLine("3. Refactorización de Bubble Sort a QuickSort O(n log n)");
Console.WriteLine("4. Adición de manejo de excepciones y logging en task execution");
Console.WriteLine("5. Sugerencias para medir performance con Stopwatch");
Console.WriteLine("6. Documentación y comentarios del código");
Console.WriteLine("\n✅ Proyecto completado exitosamente!");

static int[] GenerateRandomArray(int size)
{
    var rand = new Random();
    var arr = new int[size];
    for (int i = 0; i < size; i++)
        arr[i] = rand.Next(1, size * 10);
    return arr;
}