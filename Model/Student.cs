using System;
using System.Collections.Generic;
using System.Text;

namespace Ins_Assignment_3.Model
{
    public class Student
    {
        public String StudentId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String ImageData { get; set; }
        public bool MyRecord { get; set; }

        public string _DateOfBirth;
        public string DateOfBirth
        {
            get { return _DateOfBirth; }
            set
            {
                _DateOfBirth = value;

                //Convert DateOfBirth to DateTime
                DateTime dtOut;
                DateTime.TryParse(_DateOfBirth, out dtOut);
                DateOfBirthDT = dtOut;
            }
        }

       public DateTime DateOfBirthDT { get; set; }

        public override string ToString()
        {
            return $"{StudentId} {FirstName} {LastName}";
        }

        public string ToCSV()
        {
            return $"{StudentId},{FirstName},{LastName}, {DateOfBirth}";
        }

        public virtual int Age
        {
            get
            {
                DateTime Now = DateTime.Now;
                int Years = new DateTime(DateTime.Now.Subtract(DateOfBirthDT).Ticks).Year - 1;
                DateTime PastYearDate = DateOfBirthDT.AddYears(Years);
                int Months = 0;
                for (int i = 1; i <= 12; i++)
                {
                    if (PastYearDate.AddMonths(i) == Now)
                    {
                        Months = i;
                        break;
                    }
                    else if (PastYearDate.AddMonths(i) >= Now)
                    {
                        Months = i - 1;
                        break;
                    }
                }
                int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
                int Hours = Now.Subtract(PastYearDate).Hours;
                int Minutes = Now.Subtract(PastYearDate).Minutes;
                int Seconds = Now.Subtract(PastYearDate).Seconds;
                return Years;
            }
        }

    }

}
