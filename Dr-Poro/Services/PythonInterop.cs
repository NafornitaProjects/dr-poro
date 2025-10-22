using System.Diagnostics;

namespace Dr_Poro.Services;

public interface IPythonInterop
{
    Task<string?> GenerateAvailabilityImageAsync(string availabilityJson);
}

public class PythonInterop : IPythonInterop
{
    public async Task<string?> GenerateAvailabilityImageAsync(string availabilityJson)
    {
        try
        {
            string pythonScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "python_scripts", "generate_availability_image.py");
            string outputImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "availability.png");
            string escapedJson = availabilityJson.Replace("\"", "\\\"");
            
            Directory.CreateDirectory(Path.GetDirectoryName(outputImagePath)!);
            
            if (!File.Exists(pythonScriptPath))
            {
                Console.WriteLine($"ERROR: Python script not found at {pythonScriptPath}");
                return null;
            }

            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = "python3",
                Arguments = $"\"{pythonScriptPath}\" \"{outputImagePath}\" \"{escapedJson}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            
            using (Process? process = Process.Start(processInfo))
            {
                string error = await process.StandardError.ReadToEndAsync();
                
                await process.WaitForExitAsync();
                
                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Python script error: {error}");
                    return null;
                }

                if (!File.Exists(outputImagePath))
                {
                    Console.WriteLine($"File Does Not Exist: {outputImagePath}");
                    return null;
                }
            
                Console.WriteLine($"Image generated successfully: {outputImagePath}");
                return outputImagePath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating availability image: {ex.Message}");
            return null;
        }
    }
}