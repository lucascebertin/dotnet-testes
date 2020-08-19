using Xunit;

namespace Sample.Testes.Aceitacao.Infra
{
    [CollectionDefinition(Name)]
    public class TestsCollection : ICollectionFixture<TestsFixture>
    {
        public const string Name = nameof(TestsCollection);
    }
}
