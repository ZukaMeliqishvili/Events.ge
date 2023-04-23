namespace BookingBackgroundApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("Worker running at: {time}", DateTimeOffset.Now);
                try     
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<ServiceClient>();

                        await service.RemoveExpiredBookings();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                 
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}