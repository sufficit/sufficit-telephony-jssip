using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public enum JsSIPSessionCause
    {
        [Description("Address Incomplete")]
        ADDRESS_INCOMPLETE,

        [Description("Authentication Error")]
        AUTHENTICATION_ERROR,

        [Description("Bad Media Description")]
        BAD_MEDIA_DESCRIPTION,

        [Description("Busy")]
        BUSY,

        [Description("Terminated")]
        BYE,

        [Description("Canceled")]
        CANCELED,

        [Description("Connection Error")]
        CONNECTION_ERROR,

        [Description("Dialog Error")]
        DIALOG_ERROR,

        [Description("Expires")]
        EXPIRES,

        [Description("Incompatible SDP")]
        INCOMPATIBLE_SDP,

        [Description("Internal Error")]
        INTERNAL_ERROR,

        [Description("Missing SDP")]
        MISSING_SDP,

        [Description("Not Found")]
        NOT_FOUND,

        [Description("No ACK")]
        NO_ACK,

        [Description("No Answer")]
        NO_ANSWER,

        [Description("Redirected")]
        REDIRECTED,

        [Description("Rejected")]
        REJECTED,

        [Description("Request Timeout")]
        REQUEST_TIMEOUT,

        [Description("RTP Timeout")]
        RTP_TIMEOUT,

        [Description("SIP Failure Code")]
        SIP_FAILURE_CODE,

        [Description("Unavailable")]
        UNAVAILABLE,

        [Description("User Denied Media Access")]
        USER_DENIED_MEDIA_ACCESS,

        [Description("WebRTC Error")]
        WEBRTC_ERROR
    }
}
