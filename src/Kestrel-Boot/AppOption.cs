
namespace HttpBroker
{
    public class Config
    {
        KeyValueConfig _config;

        public Config()
        {
            _config = new KeyValueConfig("config.ini");
            var m1 = _config["MongodbUrl", "mongodb://192.168.0.5:9127"];
            var m0 = _config["VSSE", "http://192.168.0.5:9150"];
            var m2 = _config["PFSIP", "192.168.0.201"];
        }

        public string PFSIP
        {
            get
            {
                return _config["PFSIP"];
            }
            set
            {
                _config["PFSIP"] = value;
            }
        }
        public string MongodbUrl
        {
            get
            {
                return _config["MongodbUrl"];
            }
            set
            {
                _config["MongodbUrl"] = value;
            }
        }

        public string VSSE
        {
            get
            {
                return _config["VSSE"];
            }
            set
            {
                _config["VSSE"] = value;
            }
        }

        public void Update(string key, string value)
        {
            _config[key] = value;
        }
    }

    public interface IOption<T> where T : class
    {
        T Value { get; }
    }

    public class AppOption : IOption<Config>
    {
        public Config Value { get; private set; }

        public AppOption()
        {
            Value = new Config();

            MongodbUrl = Value.MongodbUrl;
        }

        public static string MongodbUrl { get; set; }
    }
}
