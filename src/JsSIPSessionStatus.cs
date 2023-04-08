namespace Sufficit.Telephony.JsSIP
{
    public enum JsSIPSessionStatus
    {
        /// <summary>
        /// On Progress
        /// </summary>
        STATUS_1XX_RECEIVED = 2,

        STATUS_ANSWERED = 5,
        STATUS_CANCELED = 7,

        /// <summary>
        /// On Confirmed
        /// </summary>
        STATUS_CONFIRMED = 9,

        STATUS_INVITE_RECEIVED = 3,

        /// <summary>
        /// On Accepted
        /// </summary>
        STATUS_INVITE_SENT = 1,

        STATUS_NULL = 0,
        STATUS_TERMINATED = 8,
        STATUS_WAITING_FOR_ACK = 6,
        STATUS_WAITING_FOR_ANSWER = 4
    }
}
