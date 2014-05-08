namespace TypeSafeClientTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNet.SignalR.Client;
    using Microsoft.AspNet.SignalR.Client.Hubs;
    using Moq;
    using NUnit.Framework;
    using TypeSafeClient;

    [TestFixture]
    public class DisconnectionWaitTests
    {
        Mock<IConnection> _connectionMock;
        Mock<IHubProxy> _proxyMock;
        IHubClient<ISampleCalls, ISampleEvents> _subject;
        Subscription _subscription;
        TimeSpan _timeoutValue;

        private interface ISampleCalls
        {
            void CanHazWurk();
            int GuessNumber(int max);
        }

        private interface ISampleEvents
        {
            void VisibulWurk(string data, int moreData);
        }

        [SetUp]
        public void Setup()
        {
            _subscription = new Subscription();

            _proxyMock = new Mock<IHubProxy>();
            _proxyMock.Setup(m => m.Subscribe(It.IsAny<string>())).Returns(_subscription);

            _timeoutValue = TimeSpan.FromSeconds(0.5);
            _connectionMock = new Mock<IConnection>();
            _connectionMock.Setup(m => m.TotalTransportConnectTimeout).Returns(_timeoutValue);

            _subject = new HubClient<ISampleCalls, ISampleEvents>(_proxyMock.Object, _connectionMock.Object);
        }

        [TearDown]
        public void Cleanup()
        {
            _subject.Dispose();
        }

        [Test]
        public void SendToHub_waits_for_connection_to_be_open()
        {
            _connectionMock.Setup(m => m.State).Returns(ReconnectingStates());
            
            _subject.SendToHub(hub => hub.CanHazWurk());
        
            _proxyMock.Verify(p => p.Invoke("CanHazWurk", It.IsAny<object[]>()));
        }

        [Test]
        public void RequestFromHub_waits_for_connection_to_be_open()
        {
            _connectionMock.Setup(m => m.State).Returns(ReconnectingStates());

            _subject.RequestFromHub(hub => hub.GuessNumber(10));

            _proxyMock.Verify(p => p.Invoke<int>("GuessNumber", It.Is<object[]>(args => args[0].ToString() == "10")));
        }



        [Test]
        public void SendToHub_fails_after_a_timeout()
        {
            _connectionMock.Setup(m => m.State).Returns(ConnectionState.Disconnected);

            var sw = new Stopwatch();
            sw.Start();

            var ex = Assert.Throws<TimeoutException>(() => _subject.SendToHub(hub => hub.CanHazWurk()));
            Assert.That(ex.Message, Is.StringContaining("Connection was disconnected during an attempted request"));

            sw.Stop();
            Assert.That(sw.Elapsed, Is.GreaterThanOrEqualTo(_timeoutValue));
        }

        [Test]
        public void RequestFromHub_fails_after_a_timeout()
        {
            _connectionMock.Setup(m => m.State).Returns(ConnectionState.Disconnected);

            var sw = new Stopwatch();
            sw.Start();

            var ex = Assert.Throws<TimeoutException>(() => _subject.RequestFromHub(hub => hub.GuessNumber(10)));
            Assert.That(ex.Message, Is.StringContaining("Connection was disconnected during an attempted request"));

            sw.Stop();
            Assert.That(sw.Elapsed, Is.GreaterThanOrEqualTo(_timeoutValue));
        }

        static Func<ConnectionState> ReconnectingStates()
        {
            return new Queue<ConnectionState>(new[]{ConnectionState.Disconnected, ConnectionState.Connecting, ConnectionState.Connected}).Dequeue;
        }
    }
}