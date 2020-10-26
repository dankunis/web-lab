using Xunit;

namespace Homework1.Test
{
    [CollectionDefinition(Name)]
    public class ApiCollectionFixture : ICollectionFixture<ApiFixture>
    {
        public const string Name = "Api collection";
    }
}