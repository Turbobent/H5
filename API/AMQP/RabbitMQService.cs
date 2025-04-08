namespace API.Services;

public class RabbitMQService : IHostedService
{
    private IConnection _connection;
    private IModel _channel;
    private readonly ILogger<RabbitMQService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    // Tilføj denne klasse for at repræsentere OLC-dataene

    public RabbitMQService(
        ILogger<RabbitMQService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public class IncomingData
    {
        public long Timestamp { get; set; }
        public int Movement { get; set; }
        public int TotalAcceleration { get; set; }
    }

    public DateOnly ConvertTimestampToDate(object timestamp)
    {
        // If you're getting a string timestamp
        if (timestamp is string timestampStr)
        {
            DateTime dateTime = DateTime.Parse(timestampStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return DateOnly.FromDateTime(dateTime);
        }
        // If you're getting a Unix timestamp as long (seconds)
        if (timestamp is long unix)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unix).DateTime;
            return DateOnly.FromDateTime(dateTime);
        }

        throw new ArgumentException("Unsupported timestamp format");
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

            _channel.QueueDeclare(queue: "ArduinoData",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

            _channel.QueueBind(queue: "ArduinoData",
                             exchange: "amq.topic",
                             routingKey: "ArduinoData");

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Udskriv den rå besked først for at hjælpe med fejlfinding
                _logger.LogInformation($" [x] Rå OLC data: {message}");

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var olcData = JsonSerializer.Deserialize<PostLog>(message, options);

                    if (olcData != null)
                    {
                        IncomingData incomingData = new();
                        var isTriggered = incomingData.Movement > 0;

                        // Check if Date is null, and use current date as fallback if it is
                        DateOnly timestamp;
                        if (olcData.Date != null)
                        {
                            // Use Date from the PostLog object
                            var dateTime = new DateTime(olcData.Date.Year, olcData.Date.Month, olcData.Date.Day);
                            timestamp = ConvertTimestampToDate(dateTime);
                        }
                        else
                        {
                            // Fallback to current date if Date is null
                            timestamp = DateOnly.FromDateTime(DateTime.Now);
                            _logger.LogWarning("OLC data Date is null, using current date as fallback.");
                        }

                        _logger.LogInformation($"Movement: {incomingData.Movement}, TotalAcceleration: {incomingData.TotalAcceleration}, IsTriggered: {isTriggered}");

                        // Log timestamp for debugging
                        _logger.LogInformation($"Timestamp: {timestamp}");

                        // Gem data i database
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
                            var log = await dbContext.Logs.FirstOrDefaultAsync();

                            // Opret et nyt PostLog-objekt med de nødvendige værdier
                            var newLog = new Log
                            {
                                DeviceId = log.DeviceId,
                                Date = new DateOnly(log.Date.Year, log.Date.Month, log.Date.Day),
                                EndDate = new DateOnly(log.EndDate.Year, log.EndDate.Month, log.EndDate.Day),
                                ArmedTime = new TimeOnly(log.ArmedTime.Hour, log.ArmedTime.Minute),
                                DisarmedTime = new TimeOnly(log.DisarmedTime.Hour, log.DisarmedTime.Minute),
                                IsTriggered = isTriggered,  // Set based on movement
                                TriggeredTime = log.TriggeredTime.HasValue
                                ? new TimeOnly(log.TriggeredTime.Value.Hour, log.TriggeredTime.Value.Minute)
                                : null,
                                UpdatedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow
                            };

                            // Tilføj til database og gem
                            await dbContext.Logs.AddAsync(newLog);
                            await dbContext.SaveChangesAsync();

                            _logger.LogInformation($"Data gemt i database med ID: {newLog.DeviceId}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($" [!] Kunne ikke deserialisere OLC data: {message}");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError($" [!] Fejl ved parsing af OLC data: {ex.Message}. Rå data: {message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Fejl ved håndtering af besked: {ex.Message}");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: "ArduinoData",
                                autoAck: false,
                                consumer: consumer);

            _logger.LogInformation(" [*] Venter på ArduinoData beskeder...");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Der opstod en fejl: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}
