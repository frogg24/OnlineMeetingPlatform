using BusinessLogic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"[LOG] [NotificationBackgroundService] Starting background service execution loop.");

            // Запускаем задачу проверки уведомлений каждый час
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"[LOG] [NotificationBackgroundService] Iteration started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}.");
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                        Console.WriteLine($"[LOG] [NotificationBackgroundService] Calling CheckAndSendMeetingNotificationsAsync...");
                        await notificationService.CheckAndSendMeetingNotificationsAsync();
                        Console.WriteLine($"[LOG] [NotificationBackgroundService] CheckAndSendMeetingNotificationsAsync completed.");
                    }

                    Console.WriteLine($"[LOG] [NotificationBackgroundService] Iteration completed. Waiting for 1 hour before next check...");
                    // Ждем 10 минут перед следующей проверкой
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] [NotificationBackgroundService] Exception occurred in execution loop: {ex.Message}");
                    Console.WriteLine($"[LOG] [NotificationBackgroundService] Waiting for 5 minutes before retrying...");
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"[LOG] [NotificationBackgroundService] Stop cancellation requested during delay. Exiting loop.");
                        break;
                    }
                }
            }

            Console.WriteLine($"[LOG] [NotificationBackgroundService] Execution loop stopped.");
        }
    }
}