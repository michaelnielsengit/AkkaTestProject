using System;
using System.Collections.Generic;
using System.Text;

using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.TestKit;

namespace AkkaTestProject
{
    public class Device : UntypedActor
    {
        private double? _lastTemperatureReading = null;

        public Device(string groupId, string deviceId)
        {
            GroupId = groupId;
            DeviceId = deviceId;
        }

        protected override void PreStart() => Log.Info($"Device actor {GroupId}-{DeviceId} started");
        protected override void PostStop() => Log.Info($"Device actor {GroupId}-{DeviceId} stopped");

        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        protected string GroupId { get; }
        protected string DeviceId { get; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case MainDevice.ReadTemperature read:
                    Sender.Tell(new RespondTemperature(read.RequestId, _lastTemperatureReading));
                    break;
            }
        }

        public static Props Props(string groupId, string deviceId) =>
            Akka.Actor.Props.Create(() => new Device(groupId, deviceId));
    }




    public class TestKitSampleTest : TestKit
    {
       // [Fact]
        public void Device_actor_must_reply_with_empty_reading_if_no_temperature_is_known()
        {
            var probe = CreateTestProbe();
            var deviceActor = system.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new ReadTemperature(requestId: 42), probe.Ref);
            var response = probe.ExpectMsg<RespondTemperature>();
            response.RequestId.Should().Be(42);
            response.Value.Should().Be(null);
        }
    }
}
