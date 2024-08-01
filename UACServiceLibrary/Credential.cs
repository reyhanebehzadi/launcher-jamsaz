using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace UACServiceLibrary
{
    [DataContract]
    public class Credential
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public int FiscalyearID { get; set; }
    }
}
