using GameManagement.Shared.Entities;
using GameManagement.Shared.Helpers;
using GameManagement.Shared.Models;

namespace GameManagement.Api.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> companyPropertyMapping = new(StringComparer.OrdinalIgnoreCase)
            {
                {"Id", new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyName", new PropertyMappingValue(new List<string>{"Name"}) },
                {"Country", new PropertyMappingValue(new List<string>{"Country"}) },
                {"Introduction", new PropertyMappingValue(new List<string>{"Introduction"})}
            };

        private readonly Dictionary<string, PropertyMappingValue> gamePropertyMapping = new(StringComparer.OrdinalIgnoreCase)
            {
                 //需要适配修改。。。。。
                {"Id", new PropertyMappingValue(new List<string>{"Id"}) },
                {"CompanyId", new PropertyMappingValue(new List<string>{"CompanyId"}) },
                {"Tiltle", new PropertyMappingValue(new List<string>{"Tiltle"})},
                {"Subtiltle", new PropertyMappingValue(new List<string>{"Subtiltle"})},
                {"TitleAndPrice", new PropertyMappingValue(new List<string>{"Tiltle", "Price"})},
                {"CoverUrl", new PropertyMappingValue(new List<string>{"CoverUrl"})},
                {"Price", new PropertyMappingValue(new List<string>{"Price"})},
                {"Introduction", new PropertyMappingValue(new List<string>{"Introduction"})},
                {"ReleaseDate", new PropertyMappingValue(new List<string>{"ReleaseDate"}, true)}
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<GameDto, Game>(gamePropertyMapping));
            _propertyMappings.Add(new PropertyMapping<CompanyDto, Company>(companyPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            var propertyMappings = matchingMapping.ToList();
            return propertyMappings.Count == 1
                ? propertyMappings.First().MappingDictionary
                : throw new Exception($"无法找到唯一的映射关系：{typeof(TSource)}, {typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string? fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldAfterSplit = fields.Split(",");
            foreach (var field in fieldAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimmedField
                    : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
