using Confluent.Kafka;
using Microsoft.Extensions.Options;
namespace OrderService
{
  public class KafkaProducer
    {
        private readonly string _topic;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducer(IConfiguration config)
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();
            _topic = config["Kafka:Topic"];
        }

        public async Task SendAsync(string message)
        {
            try
            {
                await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"Produced message to Kafka: {message}");
            }
            catch(Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
            
        }
    }

}
