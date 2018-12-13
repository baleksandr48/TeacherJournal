using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    class Teacher
    {
        public String institute { get; set; }
        public String faculty { get; set; }
        public String department { get; set; }
        public String fullName { get; set; }
        public String headFullName { get; set; }
        public String academicRank { get; set; }
        public String post { get; set; }

        public Teacher() { }

        public Teacher(string institute, string faculty, string department, string fullName, string headFullName, string academicRank, string post)
        {
            this.institute = institute;
            this.faculty = faculty;
            this.department = department;
            this.fullName = fullName;
            this.headFullName = headFullName;
            this.academicRank = academicRank;
            this.post = post;
        }
    }
}
