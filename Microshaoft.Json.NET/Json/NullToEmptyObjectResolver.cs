namespace Microshaoft
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;
    using System.Reflection;

    public class NullToEmptyObjectResolver : DefaultContractResolver
    {
        private readonly Type[] _types;

        public NullToEmptyObjectResolver(params Type[] types)
        {
            _types = types;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties().Select(p =>
            {
                var jp = base.CreateProperty(p, memberSerialization);
                jp.ValueProvider = new NullToEmptyValueProvider(p, _types);
                return jp;
            }).ToList();
        }
    }

    public class NullToEmptyValueProvider : IValueProvider
    {
        readonly PropertyInfo _memberInfo;
        private readonly Type[] _types;

        public NullToEmptyValueProvider(PropertyInfo memberInfo, params Type[] types)
        {
            _memberInfo = memberInfo;
            _types = types;
        }

        public object GetValue(object target)
        {
            var result = _memberInfo.GetValue(target);

            if (_types.Contains(_memberInfo.PropertyType) && result == null)
            {
                result = new object();
            }

            return result!;
        }

        public void SetValue(object target, object? value)
        {
            _memberInfo.SetValue(target, value);
        }
    }
}
namespace Test
{
    using Newtonsoft.Json;
    using Microshaoft;
    public class Main
    {
        public Child First
        {
            get;
            set;
        } = null!;

        public Child Last
        {
            get;
            set;
        } = null!;
    }

    public class Child
    {
        public string Name { get; set; } = null!;
    }

    class Program
    {
        static void Main()
        {
            var m = new Main()
            {
                First = new Child()
                {
                    Name = "C1",
                }
            };

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NullToEmptyObjectResolver(typeof(Child))
            };
            var str = JsonConvert.SerializeObject(m, settings);
            Console.WriteLine(str);
        }
    }
}