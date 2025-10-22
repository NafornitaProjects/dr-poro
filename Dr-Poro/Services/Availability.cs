using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Dr_Poro.Services;

public interface IAvailability
{
    Task<string> GetAllAvailabilitiesTextAsync();
    Task<string> GetAvailabilityTextAsync(string username);

    Task ClearAllAvailabilitiesAsync();
}

public class Availability : IAvailability
{
    private readonly string? _availabilityFilePath;

    public Availability(IConfiguration configuration)
    {
        _availabilityFilePath = configuration["FilePaths:AvailabilityFile"];
    }
    private async Task<Dictionary<string, Dictionary<string, string>>> ReadContentsFromJsonFileAsync()
    {
        try
        {
            if (!File.Exists(_availabilityFilePath))
                return new();
        
            string json = await File.ReadAllTextAsync(_availabilityFilePath);
            return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json) ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading availability file: {ex.Message}");
            return new();
        }
    }
    
    
    public async Task<string> GetAllAvailabilitiesTextAsync()
    {
        Dictionary<string, Dictionary<string, string>> userAvailabilityData = await ReadContentsFromJsonFileAsync();
        
        if (userAvailabilityData.Count == 0)
            return "No availabilities set yet.";
        
        var result = userAvailabilityData
            .Select(user => $"**User {user.Key}:**\n" + string.Join("\n", user.Value
                .Select(day => $"{day.Key}: {day.Value}")))
            .Aggregate((acc, current) => acc + "\n\n" + current);
        
        return result;
    }

    public async Task<string> GetAvailabilityTextAsync(string username)
    {
        Dictionary<string, Dictionary<string, string>> userAvailabilityData = await ReadContentsFromJsonFileAsync();
        
        if (!userAvailabilityData.TryGetValue(username, out var userAvailability))
            return "No availability set.";
        
        var result = "**Weekly Schedule:**\n" + string.Join("\n", userAvailability
                         .Select(day => $"{day.Key}: {day.Value}"));
        
        return result;
    }   
    
    public async Task ClearAllAvailabilitiesAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_availabilityFilePath))
            {
                Console.WriteLine("Error: AvailabilityFile path not configured!");
                return;
            }
        
            Dictionary<string, Dictionary<string, string>> emptyData = new();
            string jsonOutput = JsonSerializer.Serialize(emptyData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_availabilityFilePath, jsonOutput);
        
            Console.WriteLine("Availability file cleared successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing availability file: {ex.Message}");
        }
    }
}