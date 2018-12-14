using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GymProcess.Models;

namespace GymProcess.ViewModel
{
    public class SchemeDropdown
    {
        public IEnumerable <Tbl_Member> members { get; set; }
        public IEnumerable <Scheme> schemes { get; set; }


    }
}