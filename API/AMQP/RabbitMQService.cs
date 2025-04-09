namespace API.Services;

public class RabbitMQService : IHostedService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly ILogger<RabbitMQService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMQService(
        ILogger<RabbitMQService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public class PostLog
    {
        public string DeviceId { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ArmedTime { get; set; }
        public DateTime DisarmedTime { get; set; }
        public DateTime? TriggeredTime { get; set; }
        public int Movement { get; set; }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = "10.135.41.100",
                Port = 5672,
                UserName = "admin",
                Password = "admin",
                RequestedHeartbeat = TimeSpan.FromSeconds(600)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("amq.topic", ExchangeType.Topic, true);
            _channel.QueueDeclare(
                queue: "ArduinoData",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(
                queue: "ArduinoData",
                exchange: "amq.topic",
                routingKey: "ArduinoData");

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"Received raw message: {message}");

                try
                {
                    var olcData = JsonSerializer.Deserialize<PostLog>(
                        message,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (olcData == null)
                    {
                        _logger.LogError("Failed to deserialize message");
                        return;
                    }

                    // Validate required fields
                    if (string.IsNullOrEmpty(olcData.DeviceId))
                    {
                        _logger.LogError("DeviceId is missing in the message");
                        return;
                    }

                    // Handle dates with fallbacks
                    var logDate = olcData.Date.HasValue
                        ? DateOnly.FromDateTime(olcData.Date.Value)
                        : DateOnly.FromDateTime(DateTime.Now);

                    var endDate = olcData.EndDate.HasValue
                        ? DateOnly.FromDateTime(olcData.EndDate.Value)
                        : logDate; // Fallback to logDate if null

                    // Determine if triggered
                    bool isTriggered = olcData.Movement > 0 || olcData.TriggeredTime.HasValue;

                    _logger.LogInformation($"Processing data for device: {olcData.DeviceId}");
                    _logger.LogInformation($"Log Date: {logDate}, End Date: {endDate}");
                    _logger.LogInformation($"Triggered: {isTriggered}");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();

                        var newLog = new Log
                        {
                            DeviceId = olcData.DeviceId,
                            Date = logDate,
                            EndDate = endDate,
                            ArmedTime = TimeOnly.FromDateTime(olcData.ArmedTime),
                            DisarmedTime = TimeOnly.FromDateTime(olcData.DisarmedTime),
                            IsTriggered = isTriggered,
                            TriggeredTime = olcData.TriggeredTime.HasValue
                                ? TimeOnly.FromDateTime(olcData.TriggeredTime.Value)
                                : null,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await dbContext.Logs.AddAsync(newLog);
                        await dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Successfully saved log for device: {newLog.DeviceId}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"JSON parsing error: {ex.Message}\nRaw message: {message}");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Database error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unexpected error: {ex.Message}");
                }
                finally
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            _channel.BasicConsume(
                queue: "ArduinoData",
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("RabbitMQ consumer started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Failed to start RabbitMQ service: {ex.Message}");
            throw;
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ connection closed gracefully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while closing RabbitMQ connection: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}