using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Andead.Chat.Client;
using Andead.Chat.Client.Wcf;
using Andead.Chat.Client.Wcf.ChatService;
using Moq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ClientsTests
{
    [TestFixture]
    public class WcfServiceClientTests
    {
        private static ServiceClient CreateClient(IChatService chatService)
        {
            var clientMock = new Mock<ServiceClient> {CallBase = true};
            clientMock.Setup(c => c.Service).Returns(chatService);
            return clientMock.Object;
        }

        [Test]
        public void ServiceClient_AfterSignIn_HasCorrectSignedInValue()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false},
                new SignInResponse {Success = true}
            };

            foreach (SignInResponse response in responses)
            {
                var serviceMock = new Mock<IChatService>();
                serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                ServiceClient client = CreateClient(serviceMock.Object);

                client.SignIn(string.Empty);

                Assert.AreEqual(response.Success, client.SignedIn);
            }
        }

        [Test]
        public async Task ServiceClient_AfterSignInAsync_HasCorrectSignedInValue()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false},
                new SignInResponse {Success = true}
            };

            foreach (SignInResponse response in responses)
            {
                var serviceMock = new Mock<IChatService>();
                serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                ServiceClient client = CreateClient(serviceMock.Object);

                await client.SignInAsync(string.Empty);

                Assert.AreEqual(response.Success, client.SignedIn);
            }
        }

        [Test]
        public void ServiceClient_AfterSignOut_HasFalseSignedInValue()
        {
            ServiceClient client = CreateClient(Mock.Of<IChatService>());

            client.SignOut();

            Assert.IsFalse(client.SignedIn);
        }

        [Test]
        public async Task ServiceClient_AfterSignOutAsync_HasFalseSignedInValue()
        {
            ServiceClient client = CreateClient(Mock.Of<IChatService>());

            await client.SignOutAsync();

            Assert.IsFalse(client.SignedIn);
        }

        [Test]
        public void ServiceClient_OnDispose_DisposesChatService()
        {
            var mock = new Mock<ICommunicationObject>();
            Mock<IChatService> serviceMock = mock.As<IChatService>();

            ServiceClient client = CreateClient(serviceMock.Object);

            client.Disconnect();

            mock.Verify(s => s.Close(It.IsAny<TimeSpan>()));
        }

        [Test]
        public void ServiceClient_OnReceiveMessage_RaisesMessageReceived()
        {
            const string testMessage = "Test message";
            string receivedMessage = null;

            var client = new ServiceClient();

            client.MessageReceived += (sender, args) => receivedMessage = args.Message;

            ((IChatServiceCallback) client).ReceiveMessage(testMessage);

            Assert.AreEqual(testMessage, receivedMessage);
        }

        [Test]
        public void ServiceClient_Send_PassesCorrectData()
        {
            const string testMessage = "Test message";
            string receivedMessage = null;

            var serviceMock = new Mock<IChatService>();
            serviceMock.Setup(s => s.SendMessage(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => new SendMessageResponse {Success = true})
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);
            serviceMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => Task.FromResult(new SendMessageResponse {Success = true}))
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);

            ServiceClient client = CreateClient(serviceMock.Object);

            client.Send(testMessage);

            Assert.AreEqual(testMessage, receivedMessage);
        }

        [Test]
        public async Task ServiceClient_SendAsync_PassesCorrectData()
        {
            const string testMessage = "Test message";
            string receivedMessage = null;

            var serviceMock = new Mock<IChatService>();
            serviceMock.Setup(s => s.SendMessage(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => new SendMessageResponse {Success = true})
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);
            serviceMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => Task.FromResult(new SendMessageResponse {Success = true}))
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);

            ServiceClient client = CreateClient(serviceMock.Object);

            await client.SendAsync(testMessage);

            Assert.AreEqual(testMessage, receivedMessage);
        }

        [Test]
        public void ServiceClient_SignIn_SendsCorrectNameInRequest()
        {
            const string testName = "Test name";
            string receivedName = null;

            var serviceMock = new Mock<IChatService>();
            serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name)
                .Returns<SignInRequest>(r => Task.FromResult(new SignInResponse()));
            serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name)
                .Returns<SignInRequest>(r => new SignInResponse());

            ServiceClient client = CreateClient(serviceMock.Object);

            client.SignIn(testName);
            Assert.AreEqual(testName, receivedName);
        }

        [Test]
        public async Task ServiceClient_SignInAsync_SendsCorrectNameInRequest()
        {
            const string testName = "Test name";
            string receivedName = null;

            var serviceMock = new Mock<IChatService>();
            serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name)
                .Returns<SignInRequest>(r => Task.FromResult(new SignInResponse()));
            serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name);

            ServiceClient client = CreateClient(serviceMock.Object);

            await client.SignInAsync(testName);
            Assert.AreEqual(testName, receivedName);
        }

        [Test]
        public void SignIn_ReturnsCorrectSignInResult()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false, Message = "Test message 1"},
                new SignInResponse {Success = true, Message = "Test message 2"}
            };

            foreach (SignInResponse response in responses)
            {
                var serviceMock = new Mock<IChatService>();
                serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                ServiceClient client = CreateClient(serviceMock.Object);

                SignInResult signInResult = client.SignIn(string.Empty);

                Assert.AreEqual(response.Success, signInResult.Success);
                Assert.AreEqual(response.Message, signInResult.Message);
            }
        }

        [Test]
        public async Task SignInAsync_ReturnsCorrectSignInResult()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false, Message = "Test message 1"},
                new SignInResponse {Success = true, Message = "Test message 2"}
            };

            foreach (SignInResponse response in responses)
            {
                var serviceMock = new Mock<IChatService>();
                serviceMock.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                serviceMock.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                ServiceClient client = CreateClient(serviceMock.Object);

                SignInResult signInResult = await client.SignInAsync(string.Empty);

                Assert.AreEqual(response.Success, signInResult.Success);
                Assert.AreEqual(response.Message, signInResult.Message);
            }
        }
    }
}