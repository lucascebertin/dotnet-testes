using System;
using WireMock.Server;
using Xunit;

namespace Sample.Testes.Aceitacao.Infra
{
    [Collection(nameof(TestsCollection))]
    public class TestBase : IDisposable
    {
        public readonly TestsFixture Fixture;
        public readonly WireMockServer Server;

        protected TestBase(TestsFixture fixture) 
        {
            Server = fixture.Server;
        }

        public void Dispose()
        {
            Server.Reset();
        }
    }
}
