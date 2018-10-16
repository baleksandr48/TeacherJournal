using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal
{
    class Term
    {
        private int id;
        private String name;
        private DateTime begin_date;
        private DateTime end_date;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public DateTime BeginDate
        {
            get
            {
                return begin_date;
            }
            set
            {
                begin_date = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return end_date;
            }
            set
            {
                end_date = value;
            }
        }
    }
}
