using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineAbit2013.Models
{
    public enum StudyLevel
    {
        Secondary = 1,
        Bak = 2,
        Mag = 3,
        Spec = 4,
        PostPhD = 5,
        Doctoral = 6,
        ShortCourses = 7
    }

    public class ForeignPersonModel : BaseModel
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public string NationalityId { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }

        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string PassportDate { get; set; }
        public string PassportExpire { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }

        public string VisaCountryName { get; set; }
        public string VisaTownName { get; set; }
        public string VisaPostAddress { get; set; }

        public string StudyLevelId { get; set; }
        public string ObtainingLevelId { get; set; }

        public string StudyPlace { get; set; }
        public string StudyStart { get; set; }
        public string StudyFinish { get; set; }


        public string Works { get; set; }

        public bool HostelAbit { get; set; }

        public List<SelectListItem> lCountries { get; set; }
        public List<SelectListItem> lStudyLevels { get; set; }
        public List<SelectListItem> lLanguageLevels { get; set; }
        public Dictionary<int, string> dicLanguages { get; set; }

        public bool IsLocked { get; set; }
        public bool Agree { get; set; }
    }

    public class ForeignMain : BaseModel
    {
        public string Surname { get; set; }
        public string Name { get; set; }

        public List<SimpleApplication> lApps { get; set; }
        public List<PersonalMessage> Messages { get; set; }
    }

    public class ForeignNewApplicationModel : BaseModel
    {
        public List<SelectListItem> StudyForms { get; set; }
        public int EntryType { get; set; }
        public string StudyFormId { get; set; }
        public string ObtainingLevel { get; set; }

        public List<SelectListItem> StudyBasises { get; set; }
        public string StudyBasisId { get; set; }
    }
}