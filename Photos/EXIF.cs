using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photos
{
    //EXIF Model
    public class Exif
    {
        public int ExifId { get; set; }

        public string PhotoId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public short DataType { get; set; }

        
    }
}
