using System.Linq;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Sample.Testes.Aceitacao.Infra;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xbehave;

namespace Sample.Testes.Aceitacao
{
    public class UnitTest1 : TestBase
    {
        public UnitTest1(TestsFixture fixture): base(fixture) { }

        [Scenario]
        public void Test1(string jsonEsperado, string json)
        {
            "Dado que uma integração responde um json por <dominio>/foo".x(x => 
            {
                jsonEsperado = @"{ ""msg"": ""Hello world!"" }";

                var request = Request.Create()
                    .WithPath("/foo")
                    .WithParam("fail", new ExactMatcher("false"))
                    .UsingGet();

                var response = Response.Create()
                    .WithStatusCode(200)
                    .WithBody(jsonEsperado);

                Server.Given(request).RespondWith(response);
            });

            "Quando fizer uma chamada para <dominio>/foo, como GET e com querystring fail=\"false\"".x(async () => 
            {
                var port = Server.Ports.First();

                json = await $"http://localhost:{port}"
                    .AppendPathSegment("foo")
                    .SetQueryParams(new { fail = "false" })
                    .GetStringAsync();
            });
            
            
            "Então, o json retornado deve ser idêntico ao esperado".x(() => 
            {
                json.Should().Be(jsonEsperado);
            });
        }
    }
}

