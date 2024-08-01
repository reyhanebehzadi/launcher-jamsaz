using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jamsaz.Launcher.BusinessObject.Data;

namespace Jamsaz.Launcher.Classes
{
    public static class UAC
    {
        public static List<BusinessApplication> BusinessApplications { get; set; }

        public static List<BusinessApplication> AllowedBusinessApplications { get; set; }
    }
}
