using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OnlineRegistration.Server.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class BiometricDevice
    {
        //gayahin sa db
        public string BiometricDeviceID { get; set; }
        public string DeviceType { get; set; }
        public string IPAddress { get; set; }
        public int PortNumber { get; set; }
        public int Mode { get; set; }
        public int Status { get; set; }

    }


}
