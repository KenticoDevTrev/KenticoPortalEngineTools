using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HBS_GiftCards;
using CMS.Scheduler;
using CMS;

[assembly: RegisterCustomClass("GiftCardScheduledTasks", typeof(GiftCardScheduledTasks))]
namespace HBS_GiftCards
{
    class GiftCardScheduledTasks : ITask
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="ti">Info object representing the scheduled task</param>
        public string Execute(TaskInfo ti)
        {
            string Results = "";
            switch (ti.TaskName)
            {
                case "ExpireGiftCards":
                    HBS_GiftCards.GiftCardHelper.ExpireGiftCards(ref Results);
                    break;
            }
            ti.TaskLastResult = Results;
            return Results;
        }
    }
}
