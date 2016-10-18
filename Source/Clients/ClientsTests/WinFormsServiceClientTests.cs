using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Andead.Chat.Client.Entities;
using Andead.Chat.Client.ServiceModel.ChatService;
using Andead.Chat.Client.ServiceModel.Entities;
using Andead.Chat.Client.ServiceModel.Interfaces;
using Andead.Chat.Client.ServiceModel.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClientsTests
{
    [TestClass]
    public class WinFormsServiceClientTests
    {
        [TestMethod]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void ServiceClientCtor_OnArgumentsNull_ThrowsException()
        {
            var calls = new Action[]
            {
                () => new ServiceClient(null, Mock.Of<IChatServiceFactory>()),
                () => new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(), null)
            };

            foreach (Action action in calls)
            {
                try
                {
                    action();
                }
                catch (ArgumentNullException)
                {
                    continue;
                }
                catch
                {
                    Assert.Fail("Another exception type has been thrown.");
                }

                Assert.Fail("No exception has been thrown.");
            }
        }

        [TestMethod]
        public async Task ServiceClient_SignIn_SendsCorrectNameInRequest()
        {
            const string testName = "Test name";
            string receivedName = null;

            var chatService = new Mock<IChatService>();
            chatService.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name)
                .Returns<SignInRequest>(r => Task.FromResult(new SignInResponse()));
            chatService.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                .Callback<SignInRequest>(r => receivedName = r.Name);

            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(() => chatService.Object);

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            await client.SignInAsync(testName);
            Assert.AreEqual(testName, receivedName);
        }

        [TestMethod]
        public async Task SignInAsync_ReturnsCorrectSignInResult()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false, Message = "Test message 1"},
                new SignInResponse {Success = true, Message = "Test message 2"}
            };

            foreach (SignInResponse response in responses)
            {
                var chatService = new Mock<IChatService>();
                chatService.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                chatService.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                var chatServiceFactory = new Mock<IChatServiceFactory>();
                chatServiceFactory.Setup(
                        f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                    .Returns(() => chatService.Object);

                var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                    chatServiceFactory.Object);

                SignInResult signInResult = await client.SignInAsync(string.Empty);

                Assert.AreEqual(response.Success, signInResult.Success);
                Assert.AreEqual(response.Message, signInResult.Message);
            }
        }

        [TestMethod]
        public async Task ServiceClient_AfterSignInAsync_HasCorrectSignedInValue()
        {
            var responses = new[]
            {
                new SignInResponse {Success = false},
                new SignInResponse {Success = true}
            };

            foreach (SignInResponse response in responses)
            {
                var chatService = new Mock<IChatService>();
                chatService.Setup(s => s.SignIn(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => response);
                chatService.Setup(s => s.SignInAsync(It.IsAny<SignInRequest>()))
                    .Returns<SignInRequest>(request => Task.FromResult(response));

                var chatServiceFactory = new Mock<IChatServiceFactory>();
                chatServiceFactory.Setup(
                        f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                    .Returns(() => chatService.Object);

                var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                    chatServiceFactory.Object);

                await client.SignInAsync(string.Empty);

                Assert.AreEqual(response.Success, client.SignedIn);
            }
        }

        [TestMethod]
        public async Task ServiceClient_AfterSignOutAsync_HasFalseSignedInValue()
        {
            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(Mock.Of<IChatService>());

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            await client.SignOutAsync();

            Assert.IsFalse(client.SignedIn);
        }

        [TestMethod]
        public async Task ServiceClient_SendAsync_PassesCorrectData()
        {
            const string testMessage = "Test message";
            string receivedMessage = null;

            var chatService = new Mock<IChatService>();
            chatService.Setup(s => s.SendMessage(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => new SendMessageResponse {Success = true})
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);
            chatService.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>()))
                .Returns<SendMessageRequest>(request => Task.FromResult(new SendMessageResponse {Success = true}))
                .Callback<SendMessageRequest>(request => receivedMessage = request.Message);

            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(() => chatService.Object);

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            await client.SendAsync(testMessage);

            Assert.AreEqual(testMessage, receivedMessage);
        }

        [TestMethod]
        public void ServiceClient_OnReceiveMessage_RaisesMessageReceived()
        {
            const string testMessage = "Test message";
            string receivedMessage = null;

            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(Mock.Of<IChatService>());

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            client.MessageReceived += (sender, args) => receivedMessage = args.Message;

            ((IChatServiceCallback) client).ReceiveMessage(testMessage);

            Assert.AreEqual(testMessage, receivedMessage);
        }

        [TestMethod]
        public void ServiceClient_OnDispose_DisposesChatService()
        {
            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(Mock.Of<IChatService>());

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            client.Dispose();

            chatServiceFactory.Verify(s => s.Dispose(It.IsAny<IChatService>(),
                It.IsAny<ConnectionConfiguration>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task ServiceClient_GetOnlineCount_ReturnsCorrectValue()
        {
            int? testValue = 3;

            var chatService = new Mock<IChatService>();
            chatService.Setup(s => s.GetOnlineCount())
                .Returns(testValue);
            chatService.Setup(s => s.GetOnlineCountAsync())
                .Returns(() => Task.FromResult(testValue));

            var chatServiceFactory = new Mock<IChatServiceFactory>();
            chatServiceFactory.Setup(
                    f => f.Create(It.IsAny<ConnectionConfiguration>(), It.IsAny<IChatServiceCallback>()))
                .Returns(() => chatService.Object);

            var client = new ServiceClient(Mock.Of<IConnectionConfigurationProvider>(),
                chatServiceFactory.Object);

            int? result = await client.GetOnlineCountAsync();

            Assert.AreEqual(testValue, result);
        }
    }
}