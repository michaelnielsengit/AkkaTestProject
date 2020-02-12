using System;
using System.Collections.Generic;
using System.Text;


using Akka;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;


namespace AkkaTestProject
{



    public class IotApp
    {
        public static void Init()
        {
            Console.WriteLine("Started");
            using (var system = ActorSystem.Create("iot-system"))
            {
                // Create top level supervisor
                var supervisor = system.ActorOf(IotSupervisor.Props(), "iot-supervisor");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }
        }
    }



    public class IotSupervisor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("IoT Application started");
        protected override void PostStop() => Log.Info("IoT Application stopped");

        // No need to handle any messages
        protected override void OnReceive(object message)
        {
        }

        public static Props Props() => Akka.Actor.Props.Create<IotSupervisor>();
    }

















    public class PrintMyActorRefActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "printit":
                    IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
                    Console.WriteLine($"Second: {secondRef}");
                    break;
            }
        }
    }


    public class StartStopActor1 : UntypedActor
    {
        protected override void PreStart()
        {
            Console.WriteLine("first started");
            Context.ActorOf(Props.Create<StartStopActor2>(), "second");
        }

        protected override void PostStop() => Console.WriteLine("first stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "stop":
                    Context.Stop(Self);
                    break;
            }
        }
    }

    public class StartStopActor2 : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("second started");
        protected override void PostStop() => Console.WriteLine("second stopped");

        protected override void OnReceive(object message)
        {
        }
    }

}
