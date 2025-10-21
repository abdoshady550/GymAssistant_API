using GymAssistant_API.Model.Entities.Notifications;
using GymAssistant_API.Repository.Interfaces.Notifications;

namespace GymAssistant_API.Infrastructure
{
    /// <summary>
    /// Background service for scheduled notifications
    /// Example: Daily workout reminders, measurement reminders, etc.
    /// </summary>
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public NotificationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessScheduledNotifications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in notification background service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessScheduledNotifications(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider
                .GetRequiredService<IPushNotificationService>();

            var currentHour = DateTime.UtcNow.Hour;

            // Example: Send workout reminders at 8 AM
            if (currentHour == 8)
            {
                await SendWorkoutReminders(notificationService, ct);
            }

            // Example: Send measurement reminders on Mondays at 9 AM
            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Monday && currentHour == 9)
            {
                await SendMeasurementReminders(notificationService, ct);
            }
        }

        private async Task SendWorkoutReminders(
            IPushNotificationService service,
            CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Sending workout reminders");

                // Get users who need workout reminders
                // This is just an example - you'll need to implement your logic
                var usersToNotify = new List<string>(); // Get from database

                foreach (var userId in usersToNotify)
                {
                    await service.SendNotificationAsync(
                        userId,
                        "Workout Reminder 💪",
                        "Don't forget your workout today! Let's crush it!",
                        NotificationType.WorkoutReminder,
                        new Dictionary<string, string>
                        {
                            { "type", "workout_reminder" },
                            { "action", "open_workouts" }
                        },
                        null,
                        ct
                    );
                }

                _logger.LogInformation(
                    "Sent workout reminders to {Count} users",
                    usersToNotify.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending workout reminders");
            }
        }

        private async Task SendMeasurementReminders(
            IPushNotificationService service,
            CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Sending measurement reminders");

                // Get users who need measurement reminders
                var usersToNotify = new List<string>(); // Get from database

                foreach (var userId in usersToNotify)
                {
                    await service.SendNotificationAsync(
                        userId,
                        "Time for Measurements 📏",
                        "Track your progress! Update your measurements this week.",
                        NotificationType.MeasurementReminder,
                        new Dictionary<string, string>
                        {
                            { "type", "measurement_reminder" },
                            { "action", "open_measurements" }
                        }, null,
                        ct
                    );
                }

                _logger.LogInformation(
                    "Sent measurement reminders to {Count} users",
                    usersToNotify.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending measurement reminders");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Notification Background Service stopped");
            await base.StopAsync(cancellationToken);
        }
    }
}
