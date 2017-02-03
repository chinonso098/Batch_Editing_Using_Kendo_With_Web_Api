using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoStudent.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public int Studentid { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
    }
}