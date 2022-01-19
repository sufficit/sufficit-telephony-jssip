using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sufficit.Telephony.JsSIP
{
    public class MediaConstraints : IMediaBasic
    {
        public MediaConstraints()
        {
            Audio = new MediaOptions(true);
        }

        //{ video: true, audio: true }
        //{ video: { deviceId: { exact: cameraDevice.deviceId } } }
        [JsonPropertyName("video")]
        [DataMember(EmitDefaultValue = false)]
        public MediaOptions? Video { get; set; }

        [JsonPropertyName("audio")]
        [DataMember(EmitDefaultValue = false)]
        public MediaOptions Audio { get; set; }

        #region INTERFACE IMediaBasic

        bool IMediaBasic.Video => Video != null;

        bool IMediaBasic.Audio => Audio != null;


        #endregion
    }
}
