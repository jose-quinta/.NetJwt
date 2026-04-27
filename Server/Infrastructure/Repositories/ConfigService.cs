using Microsoft.Extensions.Configuration;
using Server.Application.Interfaces;

namespace Server.Infrastructure.Repositories {
    public class ConfigService : IConfigService {
        private readonly IConfiguration _configuration;

        public ConfigService(IConfiguration configuration) {
            _configuration = configuration;
        }

        public string GetValue(string key) => _configuration[key]!;
    }
}