using DotaApiCore.MatchHistory;
using DotaApiCore.Requests;
using DotaApiCore.SharedLib;
using DotaApiUnitTests.MockData.MatchHistoryService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace DotaApiUnitTests
{
    [TestCategory("Test MatchHistoryService")]
    [TestClass()]
    public class TestMatchHistoryService
    {
        private const string MockApiKey = "MockApiKey";
        private const int MockAccountId = 322;
        private Mock<IHttpHandler> _httpClientMock;

        [TestInitialize]
        public void Init()
        {
            _httpClientMock = new Mock<IHttpHandler>();
        }

        [TestMethod]
        public void GetMatchHistory_ValidRequest_MapsResponseCorrectly()
        {
            _httpClientMock.Setup(client => client.SendRequest(It.IsAny<string>()))
                .Returns(MatchHistoryServiceMockData.GenericSuccess);

            var service = new MatchHistoryService(_httpClientMock.Object, MockApiKey);
            var result = service.GetMatchHistory(accountId: MockAccountId, matchesRequested: 1).Result;

            Assert.AreEqual(result.NumResults, 1);
            Assert.AreEqual(result.Status, 1);
            Assert.IsNull(result.StatusDetail);
            Assert.AreEqual(result.TotalResults, 500);
            Assert.AreEqual(result.ResultsRemaining, result.TotalResults - result.NumResults);
            Assert.IsNotNull(result.Matches);
            Assert.AreEqual(result.Matches.Count, 1);

            var match = result.Matches.FirstOrDefault();
            Assert.AreEqual(match.MatchId, 3088091200);
            Assert.AreEqual(match.MatchSequenceNumber, 2696663841);
            Assert.AreEqual(match.StartTime, SharedFunctions.UnixTimeStampToDateTime(1490921858));

            var players = match.Players;
            Assert.AreEqual(players.Count, 10);
            Assert.IsTrue(players.Any(x => x.AccountId == MockAccountId));

            foreach (var player in players)
            {
                Assert.IsNotNull(player.AccountId);
                Assert.IsNotNull(player.PlayerSlot);
                Assert.IsNotNull(player.HeroId);
            }
        }

        [TestMethod]
        public void GetMatchHistory_InvalidAccountId_MapsErrorResponse()
        {
            _httpClientMock.Setup(client => client.SendRequest(It.IsAny<string>()))
                .Returns(MatchHistoryServiceMockData.NotFound);

            var service = new MatchHistoryService(_httpClientMock.Object, MockApiKey);
            var result = service.GetMatchHistory(MockAccountId).Result;

            Assert.AreEqual(result.Status, 15);
            Assert.IsNotNull(result.StatusDetail);
            Assert.IsNull(result.Matches);
            Assert.AreEqual(result.NumResults, 0);
            Assert.AreEqual(result.ResultsRemaining, 0);
            Assert.AreEqual(result.TotalResults, 0);
        }
    }
}
