using System.Collections.Generic;
using System.Linq;
using Andead.Chat.Common.Logging;
using Andead.Chat.Common.Policy;
using Andead.Chat.Common.Resources.Strings;
using Andead.Chat.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ChatServiceTests
{
    [TestClass]
    public class SignInTests
    {
        [TestMethod]
        public void SignIn_WithEmptyName_ReturnsFailure()
        {
            IChatService service = CreateService();

            IEnumerable<SignInRequest> requests = new[]
            {
                new SignInRequest(),
                new SignInRequest {Name = null},
                new SignInRequest {Name = string.Empty}
            }.Concat(Enumerable.Range(1, Limits.UsernameMaxLength)
                .Select(i => new SignInRequest
                {
                    Name = Enumerable.Repeat(" ", i).Aggregate((s, c) => s + c)
                }));

            foreach (SignInRequest request in requests)
            {
                SignInResponse response = service.SignIn(request);

                Assert.IsNotNull(response);

                Assert.IsFalse(response.Success);
                Assert.AreEqual(Errors.EmptyNameNotAllowed, response.Message);
            }
        }

        [TestMethod]
        public void SignIn_WithUsernameExceedingLimits_ReturnsFailure()
        {
            IChatService service = CreateService();

            string name = string.Join("", Enumerable.Repeat("A", Limits.UsernameMaxLength + 1));

            var request = new SignInRequest {Name = name};

            SignInResponse response = service.SignIn(request);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(Errors.NameLengthExceededLimits, response.Message);
        }

        private static IChatService CreateService()
        {
            var clientProvider = new Mock<IChatClientsProvider>();
            clientProvider.Setup(p => p.GetCurrentClient()).Returns(Mock.Of<IChatClient>());

            IChatService service = new ChatService(clientProvider.Object, Mock.Of<ILogger>());
            return service;
        }
    }
}