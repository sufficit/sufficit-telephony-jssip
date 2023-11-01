using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public static class JsSIPExtensions
    {
        /// <summary>
        ///     Not connected anymore, no active stream
        /// </summary>
        public static bool IsFinished(this JsSIPSessionStatus source)
            => source == JsSIPSessionStatus.STATUS_TERMINATED || source == JsSIPSessionStatus.STATUS_CANCELED;

        /// <inheritdoc cref="IsFinished"/>
        public static bool IsFinished(this JsSIPSessionInfo source)
            => source.Status.IsFinished();

        /// <inheritdoc cref="IsFinished"/>
        public static bool IsFinished(this JsSIPSessionMonitor source)
            => source.Status.IsFinished();

        public static bool CanAnswer(this JsSIPSessionStatus source)
            => source == JsSIPSessionStatus.STATUS_WAITING_FOR_ANSWER;

        public static bool CanAnswer(this JsSIPSessionInfo source)
            => source.Status.CanAnswer();

        public static bool CanAnswer(this JsSIPSessionMonitor source)
            => source.Status.CanAnswer();

        public static bool CanTerminate(this JsSIPSessionStatus source)
            => !source.IsFinished();

        public static bool CanTerminate(this JsSIPSessionInfo source)
            => !source.IsFinished();

        public static bool CanTerminate(this JsSIPSessionMonitor source)
            => !source.IsFinished();
    }
}
