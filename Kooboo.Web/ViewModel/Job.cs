//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using Kooboo.IndexedDB.Schedule;
using System;
using System.Collections.Generic;

namespace Kooboo.Web.ViewModel
{
    public class JobEditViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CodeId { get; set; }

        public Guid JobId { get; set; }

        public string Script { get; set; }

        public Dictionary<string, string> Config { get; set; }

        public bool IsRepeat { get; set; }

        public DateTime StartTime { get; set; }

        public int FrequenceUnit { get; set; }

        public string Frequence { get; set; }
    }

    public class JobViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }

        /// <summary>
        /// Whether this is a repeating job or one time job.
        /// </summary>
        public bool IsRepeat { get; set; }

        /// <summary>
        /// The name of this job, this is defined in the job class.
        /// </summary>
        public string JobName { get; set; }

        public string Script { get; set; }

        public RepeatFrequence Frequence { get; set; }

        public int FrequenceUnit { get; set; }

        public int DayInt { get; set; }

        public int SecondOfDay { get; set; }

        public long BlockPosition { get; set; }

        public string CodeName { get; set; }

        public Guid CodeId { get; set; }

        public JobViewModel()
        {
        }

        public JobViewModel(ScheduleItem<Job> scheduleJob)
        {
            Description = scheduleJob.Item.Description;
            StartTime = scheduleJob.ScheduleTime;
            IsRepeat = false;
            JobName = scheduleJob.Item.JobName;
            this.CodeId = scheduleJob.Item.CodeId;
            this.DayInt = scheduleJob.DayInt;
            this.BlockPosition = scheduleJob.BlockPosition;
            this.SecondOfDay = scheduleJob.SecondOfDay;
            this.Script = scheduleJob.Item.Script;

            this.Id = scheduleJob.Item.Id;
        }

        public JobViewModel(RepeatItem<Job> repeatjob)
        {
            Description = repeatjob.Item.Description;
            StartTime = repeatjob.StartTime;
            IsRepeat = true;
            Frequence = repeatjob.Frequence;
            FrequenceUnit = repeatjob.FrequenceUnit;
            JobName = repeatjob.Item.JobName;

            this.Id = repeatjob.Item.Id;

            this.CodeId = repeatjob.Item.CodeId;
        }
    }

    public class JobDeleteViewModel
    {
        public long Id { get; set; }

        public bool IsRepeat { get; set; }

        public int DayInt { get; set; }

        public int SecondOfDay { get; set; }

        public long BlockPosition { get; set; }
    }
}