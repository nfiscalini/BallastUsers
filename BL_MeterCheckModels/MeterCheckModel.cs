using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL_MeterCheckModels
{
    public class MeterCheckModel
    {
        public int Check_id { get; set; }
        public int Customer_Id { get; set; }
        public DateTime Date { get; set; }
        public float Measure { get; set; }
    }
}
