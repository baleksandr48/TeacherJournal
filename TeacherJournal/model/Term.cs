using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class Term
    {
        public static int ID_FOR_WRITING = 1001;

        public long id { get; set; }
        public String name { get; set; }
        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }
        public int startFromNumerator { get; set; }

        public Term(long id, string name, DateTime beginDate, DateTime endDate, int startFromNumerator)
        {
            this.id = id;
            this.name = name;
            this.beginDate = beginDate;
            this.endDate = endDate;
            this.startFromNumerator = startFromNumerator;
        }

        public Term(long id, string name, String beginDate, String endDate, int startFromNumerator)
        {
            this.id = id;
            this.name = name;
            this.beginDate = DateTime.ParseExact(beginDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            this.endDate = DateTime.ParseExact(endDate, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            this.startFromNumerator = startFromNumerator;
        }

    }
}
