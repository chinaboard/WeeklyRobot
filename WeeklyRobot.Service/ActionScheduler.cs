using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeeklyRobot.Service
{
    public sealed class ActionScheduler
    {
        private CancellationTokenSource token = null;

        public void Start(TimeSpan interval, Action action)
        {
            Start(interval, t =>
            {
                if (!t.IsCancellationRequested)
                {
                    action();
                }
            });
        }

        public void Start(TimeSpan interval, Action<CancellationToken> action)
        {
            Start(interval, t =>
            {
                action(t);
                return Task.FromResult(true);
            });
        }

        public void Start(TimeSpan interval, Func<Task> action)
        {
            Start(interval, t => t.IsCancellationRequested ? action() : Task.FromResult(true));
        }

        public void Start(TimeSpan interval, Func<CancellationToken, Task> action)
        {
            if (interval.TotalSeconds == 0)
            {
                throw new ArgumentException("interval must be > 0 seconds", "interval");
            }

            if (this.token != null)
            {
                throw new InvalidOperationException("Scheduler is already started.");
            }

            this.token = new CancellationTokenSource();

            RunScheduler(interval, action, this.token);
        }

        private static void RunScheduler(TimeSpan interval, Func<CancellationToken, Task> action, CancellationTokenSource token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(interval, token.Token).ConfigureAwait(false);
                        try
                        {
                            await action(token.Token).ConfigureAwait(false);
                        }
                        catch (Exception x)
                        {
                            Logger.Error($"Error while executing action scheduler.\r\n{x.ToString()}");
                            SmtpHelper.SendMail("error", x.ToString());
                            token.Cancel();
                        }
                    }
                    catch (TaskCanceledException) { }
                }
            }, token.Token);
        }

        public void Stop()
        {
            if (token != null)
            {
                token.Cancel();
            }
        }

        public void Dispose()
        {
            if (token != null)
            {
                token.Cancel();
                token.Dispose();
            }
        }
    }
}
