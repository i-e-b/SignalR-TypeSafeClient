namespace TypeSafeClientTests
{
    using System.Reflection;
    using Microsoft.AspNet.SignalR.Client;
    using Microsoft.AspNet.SignalR.Client.Hubs;
    using Moq;
    using NUnit.Framework;
    using TypeSafeClient;

    [TestFixture]
    public class TypeSafeHubClientTests
    {
        Mock<IConnection> _connectionMock;
        Mock<IHubProxy> _proxyMock;
        IHubClient<ISampleCalls, ISampleEvents> _subject;
        Subscription _subscription;


        [SetUp]
        public void Setup()
        {
            _subscription = new Subscription();

            _proxyMock = new Mock<IHubProxy>();
            _proxyMock.Setup(m => m.Subscribe(It.IsAny<string>())).Returns(_subscription);
            _connectionMock = new Mock<IConnection>();
            _connectionMock.Setup(m => m.State).Returns(ConnectionState.Connected);

            _subject = new HubClient<ISampleCalls, ISampleEvents>(_proxyMock.Object, _connectionMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _subject.Dispose();
        }

        [Test]
        public void disposing_of_a_hub_client_should_close_the_connection_sending_an_abort_message_to_the_hub()
        {
            _subject.Dispose();

            _connectionMock.Verify(m => m.Stop());
        }

        [Test]
        public void making_a_call_to_the_hub_causes_an_invoke_on_the_hub_proxy_with_the_correct_target_name()
        {
            _subject.SendToHub(hub => hub.CanHazWurk());
            _proxyMock.Verify(p => p.Invoke("CanHazWurk", It.IsAny<object[]>()));
        }

        [Test]
        public void making_a_value_returning_call_to_the_hub_causes_an_invoke_on_the_hub_proxy_with_the_correct_target_name()
        {
            _subject.RequestFromHub(hub => hub.GuessNumber(10));
            _proxyMock.Verify(p => p.Invoke<int>("GuessNumber", It.Is<object[]>(args => args[0].ToString() == "10")));
        }

        [Test]
        public void registering_an_event_handler_creates_a_subscription_on_the_proxy()
        {
            Assert.IsFalse(HasBinding(_subscription));
            _subject.BindEventHandler<string, int>(e => e.VisibulWurk, DummyWorker);

            _proxyMock.Verify(p => p.Subscribe("VisibulWurk"));
            Assert.IsTrue(HasBinding(_subscription));
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void DummyWorker(string arg1, int arg2) { }

        public bool HasBinding(Subscription s)
        {
            // test unfriendly base class :-(
            var ty = typeof(Subscription);
            var field = ty.GetField("Received", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) Assert.Inconclusive("Could not reflect properties of event subscription");
            return null != field.GetValue(s);
        }
    }

}
