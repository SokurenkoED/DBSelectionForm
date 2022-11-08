using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSelectionForm.Models
{
    public class InfoData
    {
        public string SensorName { get; set; }

        public string TimeFrom { get; set; }

        public string TimeTo { get; set; }

        public string PathToFolder { get; set; }

        public string PathToListFile { get; set; }

        public string PathToDataFile { get; set; }

        public string DayFrom { get; set; }

        public string DayTo { get; set; }

        public string PathToFolderForListBD { get; set; }

        public string EndDayForListBD { get; set; }

        public string EndTimeForListBD { get; set; }

        public string SlicePathDB { get; set; }
    }
}
