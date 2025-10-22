using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dr_Poro.Services;
public class WeeklyAvailabilityClear : BackgroundService
{
    private readonly IAvailability _availabilityService;
    private readonly ILogger<WeeklyAvailabilityClear> _logger;

    public WeeklyAvailabilityClear(IAvailability availabilityService, ILogger<WeeklyAvailabilityClear> logger)
    {
        _availabilityService = availabilityService;
        _logger = logger;
    }
    

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DateTime now = DateTime.Now;
            DateTime nextSaturday = now.AddDays((6 - (int)now.DayOfWeek) % 7);
            DateTime nextClearTime = nextSaturday.Date.AddHours(23).AddMinutes(59);

            if (nextClearTime <= now)
                nextClearTime = nextClearTime.AddDays(7);

            TimeSpan delay = nextClearTime - now;
            
            _logger.LogInformation($"Next availability clear scheduled for {nextClearTime}");
            
            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) 
                return;
            
            await _availabilityService.ClearAllAvailabilitiesAsync();
            _logger.LogInformation("Weekly availabilities cleared.");
        }
    }
}