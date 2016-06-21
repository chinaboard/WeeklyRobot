using System;
using Topshelf;
using Topshelf.ServiceConfigurators;
using WeeklyRobot.Service;

namespace WeeklyRobot
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service((ServiceConfigurator<WeeklyRobotService> s) =>
                {
                    s.ConstructUsing(() => new WeeklyRobotService());
                    s.WhenStarted(ch => ch.Start());
                    s.WhenStopped(ch => ch.Stop());
                });
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription(nameof(WeeklyRobotService));
                x.SetDisplayName(nameof(WeeklyRobotService));
                x.SetServiceName(nameof(WeeklyRobotService));
            });
            Console.Read();
        }
    }
}
