using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Sample.Testes.Unidade
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var jsonText = @"{ ""msg"": ""Hello world!"" }";

            using var httpTest = new HttpTest();
            httpTest.RespondWith(jsonText, 200);

            var json = await "https://api.com"
                .AppendPathSegment("foo")
                .SetQueryParams(new { fail = "false" })
                .GetStringAsync();

            httpTest.ShouldHaveCalled("https://api.com/foo")
                .WithVerb(HttpMethod.Get)
                .WithQueryParam("fail");

           json.Should().Be(jsonText);
        }
    }
}
