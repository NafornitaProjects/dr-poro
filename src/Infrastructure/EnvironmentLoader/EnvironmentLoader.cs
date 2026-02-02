namespace DrPoro.Infrastructure.EnvironmentLoader;

public class EnvironmentLoader
{
    public static void Load(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file '{filePath}' was not found.");

        foreach (string line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue; // skip empty lines and comments

            string[] parts = line.Split("=", 2);
            
            if (parts.Length != 2)
                continue; // skip lines that are not key-value pairs

            string key = parts[0].Trim();
            string value = parts[1].Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
