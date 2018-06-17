using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
namespace CoreHelper.QuartzScheduler
{
	/// <summary>
	/// 任务接口
	/// 优先使用Cron表达式,如果为空,则使用重复规则
	/// </summary>
	public abstract class QuartzJob : IJob
	{
		//string JobName { get; }
		//string JobGroup { get; }
		/// <summary>
		/// Cron表达式,如果为空,则按重复间隔
		/// </summary>
		public string CronExpression;
		/// <summary>
		/// 重复间隔
		/// </summary>
		public TimeSpan RepeatInterval;
		/// <summary>
		/// 重复次数,-1为不限次数
		/// </summary>
        public int RepeatCount = -1;

		/// <summary>
		/// 执行的任务委托
		/// </summary>
		public abstract void DoWork();
        static object lockObj = new object();
        protected void Log(string message)
        {
            EventLog.Log(message, "Task");
        }
		public void Execute(Quartz.JobExecutionContext context)
		{
            string name = context.JobDetail.Name;
            if (QuartzWorker.workCache[name])
            {
                return;
            }
            QuartzWorker.workCache[name] = true;
            try
            {
                DoWork();
            }
            catch (Exception ero)
            {
                Log(GetType() + "执行出错:" + ero.Message);
            }
            QuartzWorker.workCache[name] = false;
		}
	}
}
