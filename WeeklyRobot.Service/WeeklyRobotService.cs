using System;
using System.Collections.Generic;

namespace WeeklyRobot.Service
{
    public class WeeklyRobotService
    {
        public void Start()
        {
            ActionScheduler writeReportAction = new ActionScheduler();
            ActionScheduler resetAction = new ActionScheduler();


            Reset();
            WriteContent();



            writeReportAction.Start(TimeSpan.FromHours(1), () => WriteContent());
            resetAction.Start(TimeSpan.FromMinutes(13), () => Reset());
        }

        public void Stop()
        {

        }

        private void WriteReport(ReportType type)
        {
            Logger.Debug($"WriteReport\t{type}");
            if (DateTime.Now.Hour > 14 && !Config.TryGet(type.ToString(), true))
            {
                Config.Set(type.ToString(), true);
                var result = HttpHelper.Post(Config.Get<string>("Url"), new PostBody(DateTime.Now.ToString("yyyy-MM-dd"), new string[] { "占座占座" }, (int)type));
                Logger.Debug(result);
                SmtpHelper.SendMail(type.ToString(), result);
            }
        }

        private void WriteContent()
        {
            Logger.Debug($"WriteContent");
            var list = Judge();
            foreach (var type in list)
            {
                WriteReport(type);
            }
        }

        private List<ReportType> Judge()
        {
            var list = new List<ReportType>() { ReportType.Day };
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                list.Add(ReportType.Week);
            }
            if (DateTime.Now.Day >= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - 2)
            {
                list.Add(ReportType.Month);
            }
            if (DateTime.Now.DayOfYear < 2)
            {
                list.Add(ReportType.Year);
            }
            return list;
        }

        private void Reset()
        {
            Logger.Debug($"Reset");
            var time = Config.Get<DateTime>("Time");
            if (time.DayOfYear != DateTime.Now.DayOfYear)
            {
                Config.Set(nameof(ReportType.Day), false);
                Config.Set("Time", DateTime.Now.ToString("yyyy-MM-dd"));
            }
            if (time.DayOfWeek != DayOfWeek.Friday)
            {
                Config.Set(nameof(ReportType.Week), false);
            }
            if (DateTime.Now.Day < DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) - 10)
            {
                Config.Set(nameof(ReportType.Month), false);
            }
            if (DateTime.Now.DayOfYear > 2)
            {
                Config.Set(nameof(ReportType.Year), false);
            }

        }

    }
}
