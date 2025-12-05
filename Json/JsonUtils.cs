namespace _PRO.Json;

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class JsonUtils
{
    // ----------< JSON Options >----------
    /*private static readonly JsonSerializerOptions PrettyOptions = new(JsonSerializerDefaults.General)
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true
    };*/

    // ----------< Background Save Queue >----------
    // Single-threaded async queue for non-blocking saves.
    private static readonly Channel<Func<Task>> SaveChannel = Channel.CreateUnbounded<Func<Task>>();
    private static readonly CancellationTokenSource SaveCts = new();

    static JsonUtils()
    {
        _ = Task.Run(ProcessSaveQueueAsync); // Start background worker
    }

    public static async Task ShutdownSaveExecutorAsync()
    {
        await SaveCts.CancelAsync();
        SaveChannel.Writer.Complete();
        await Task.Delay(100);
    }

    private static async Task ProcessSaveQueueAsync()
    {
        try
        {
            await foreach (var action in SaveChannel.Reader.ReadAllAsync(SaveCts.Token))
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    LogError($"Error during JSON save task: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            /* normal exit */
        }
    }


    // ----------< Public API >----------
    public static T Load<T>(FileInfo file, T defaultValue)
    {
        if (!file.Exists)
            return defaultValue;

        try
        {
            var json = File.ReadAllText(file.FullName);
            //var obj = JsonConvert.Deserialize<T>(json, PrettyOptions);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, JsonSerializerExtensions.Options);
            return obj == null ? defaultValue : obj;
        }
        catch (Exception ex)
        {
            LogError($"Failed to load JSON file \"{file.FullName}\": {ex.Message}");
            return defaultValue;
        }
    }

    public static void SaveAsync(object value, FileInfo file)
    {
        SaveChannel.Writer.TryWrite(() => WriteJsonFileAsync(value, file));
    }


    // ----------< Utilities >----------

    private static async Task WriteJsonFileAsync(object value, FileInfo file)
    {
        try
        {
            //var bytes = JsonSerializer.SerializeToUtf8Bytes(value, PrettyOptions);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(value, JsonSerializerExtensions.Options);
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            await WriteBytesToFileAtomicallyAsync(file, bytes);
        }
        catch (Exception ex)
        {
            LogError($"Failed to save JSON file \"{file.FullName}\": {ex.Message}");
        }
    }

    private static async Task WriteBytesToFileAtomicallyAsync(FileInfo file, byte[] bytes)
    {
        var dir = file.DirectoryName ?? Directory.GetCurrentDirectory();
        Directory.CreateDirectory(dir);

        var tempPath = Path.Combine(dir, $"{file.Name}.{Guid.NewGuid():N}.tmp");

        await File.WriteAllBytesAsync(tempPath, bytes);
        File.Move(tempPath, file.FullName, overwrite: true);
    }

    private static void LogError(string message)
    {
        Console.Error.WriteLine($"[JsonUtils] {message}");
    }
}