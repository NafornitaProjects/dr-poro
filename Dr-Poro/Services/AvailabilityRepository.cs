namespace Dr_Poro.Services;
using System.Text.Json;

public interface IAvailabilityRepository
{
    Task SaveUserAvailabilityAsync(string username, Dictionary<string, string> availability);
    Task<Dictionary<string, Dictionary<string, string>>> LoadAllAvailabilitiesAsync();
}

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly string? _filePath;

    public AvailabilityRepository(string? filePath)
    {
        _filePath = filePath;
    }

    public async Task SaveUserAvailabilityAsync(string username, Dictionary<string, string> availability)
    {
        try
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                Console.WriteLine("No file path configured for availability storage");
                return;
            }

            Dictionary<string, Dictionary<string, string>> allAvailabilities = await LoadAllAvailabilitiesAsync();
            allAvailabilities[username] = availability;

            string jsonOutput = JsonSerializer.Serialize(allAvailabilities, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_filePath, jsonOutput);
            Console.WriteLine($"Saved availability for {username}");
            Console.WriteLine($"Saving to: {Path.GetFullPath(_filePath)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving availability: {ex.Message}");
        }
    }

    public async Task<Dictionary<string, Dictionary<string, string>>> LoadAllAvailabilitiesAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_filePath) || !File.Exists(_filePath))
                return new();

            string existingJson = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(existingJson) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading availabilities: {ex.Message}");
            return new();
        }
    }
}