using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineAbit2013.Models;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace OnlineAbit2013.Controllers
{
//    public class SPOController : Controller
//    {
//        //
//        // GET: /SPO/

//        public ActionResult Index(string step)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            if (Util.CheckIsNew(PersonId))
//                return RedirectToAction("OpenPersonalAccount");

//            int AbitType = Util.CheckAbitType(PersonId);
//            switch (AbitType)
//            {
//                case 1: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
//                case 2: { return RedirectToAction("Index", "ForeignAbiturient", new RouteValueDictionary() { { "step", step }}); }
//                case 3: { return RedirectToAction("Index", "Transfer", new RouteValueDictionary() { { "step", step }}); }
//                case 4: { return RedirectToAction("Index", "TransferForeign", new RouteValueDictionary() { { "step", step }}); }
//                case 5: { return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", step }}); }
//                case 6: { return RedirectToAction("Index", "ChangeStudyForm", new RouteValueDictionary() { { "step", step }}); }
//                case 7: { return RedirectToAction("Index", "ChangeObrazProgram", new RouteValueDictionary() { { "step", step }}); ; }
//                case 8: { return RedirectToAction("Index", "AG", new RouteValueDictionary() { { "step", step }}); ; }
//                case 9: { break; }
//                default: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
//            }

//            int stage = 0;
//            if (!int.TryParse(step, out stage))
//                stage = 1;

//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                string query;
//                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
//                if (Person == null)//paranoia
//                    return RedirectToAction("OpenPersonalAccount", "Abiturient");

//                if (Person.RegistrationStage == 0)
//                    stage = 1;
//                else if (Person.RegistrationStage < stage)
//                    stage = Person.RegistrationStage;

//                PersonalOffice_SPO model = new PersonalOffice_SPO() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
//                if (model.Stage == 1)
//                {
//                    model.PersonInfo = new InfoPerson();

//                    model.PersonInfo.Surname = Server.HtmlDecode(Person.Surname);
//                    model.PersonInfo.Name = Server.HtmlDecode(Person.Name);
//                    model.PersonInfo.SecondName = Server.HtmlDecode(Person.SecondName);
//                    model.PersonInfo.Sex = (Person.Sex ?? false) ? "M" : "F";
//                    model.PersonInfo.Nationality = Person.NationalityId.ToString();
//                    model.PersonInfo.BirthPlace = Server.HtmlDecode(Person.BirthPlace);
//                    model.PersonInfo.BirthDate = Person.BirthDate.HasValue ? Person.BirthDate.Value.ToString("dd.MM.yyyy") : "";

//                    model.PersonInfo.NationalityList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
//                    model.PersonInfo.SexList = new List<SelectListItem>()
//                    {
//                        new SelectListItem() { Text = LangPack.GetValue(5, model.Lang), Value = "M" }, 
//                        new SelectListItem() { Text = LangPack.GetValue(6, model.Lang), Value = "F" }
//                    };
//                }
//                else if (model.Stage == 2)
//                {
//                    model.PassportInfo = new PassportPerson();
//                    DataTable tblPsp = Util.AbitDB.GetDataTable("SELECT Id, Name FROM PassportType WHERE 1=@x", new SortedList<string, object>() { { "@x", 1 } });
//                    model.PassportInfo.PassportTypeList =
//                        (from DataRow rw in tblPsp.Rows
//                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>("Name") }).
//                        ToList();

//                    model.PassportInfo.PassportType = (Person.PassportTypeId ?? 1).ToString();
//                    model.PassportInfo.PassportSeries = Server.HtmlDecode(Person.PassportSeries);
//                    model.PassportInfo.PassportNumber = Server.HtmlDecode(Person.PassportNumber);
//                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(Person.PassportAuthor);
//                    model.PassportInfo.PassportDate = Person.PassportDate.HasValue ? Person.PassportDate.Value.ToString("dd.MM.yyyy") : "";
//                    model.PassportInfo.PassportCode = Server.HtmlDecode(Person.PassportCode);
//                }
//                else if (model.Stage == 3)
//                {
//                    model.ContactsInfo = new ContactsPerson();
//                    var PersonContacts = Person.PersonContacts;
//                    if (PersonContacts == null)
//                        PersonContacts = new PersonContacts();

//                    model.ContactsInfo.MainPhone = Server.HtmlDecode(PersonContacts.Phone);
//                    model.ContactsInfo.SecondPhone = Server.HtmlDecode(PersonContacts.Mobiles);
//                    model.ContactsInfo.CountryId = PersonContacts.CountryId.ToString();
//                    model.ContactsInfo.RegionId = PersonContacts.RegionId.ToString();

//                    model.ContactsInfo.PostIndex = Server.HtmlDecode(PersonContacts.Code);
//                    model.ContactsInfo.City = Server.HtmlDecode(PersonContacts.City);
//                    model.ContactsInfo.Street = Server.HtmlDecode(PersonContacts.Street);
//                    model.ContactsInfo.House = Server.HtmlDecode(PersonContacts.House);
//                    model.ContactsInfo.Korpus = Server.HtmlDecode(PersonContacts.Korpus);
//                    model.ContactsInfo.Flat = Server.HtmlDecode(PersonContacts.Flat);

//                    model.ContactsInfo.PostIndexReal = Server.HtmlDecode(PersonContacts.CodeReal);
//                    model.ContactsInfo.CityReal = Server.HtmlDecode(PersonContacts.CityReal);
//                    model.ContactsInfo.StreetReal = Server.HtmlDecode(PersonContacts.StreetReal);
//                    model.ContactsInfo.HouseReal = Server.HtmlDecode(PersonContacts.HouseReal);
//                    model.ContactsInfo.KorpusReal = Server.HtmlDecode(PersonContacts.KorpusReal);
//                    model.ContactsInfo.FlatReal = Server.HtmlDecode(PersonContacts.FlatReal);

//                    model.ContactsInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

//                    query = "SELECT Id, Name FROM Region WHERE RegionNumber IS NOT NULL";
//                    model.ContactsInfo.RegionList =
//                        (from DataRow rw in Util.AbitDB.GetDataTable(query, null).Rows
//                         select new SelectListItem()
//                         {
//                             Value = rw.Field<int>("Id").ToString(),
//                             Text = rw.Field<string>("Name")
//                         }).ToList();
//                }
//                else if (model.Stage == 4)
//                {
//                    model.EducationInfo = new EducationPerson();
//                    var PersonEducationDocument = Person.PersonEducationDocument;
//                    var PersonHighEducationInfo = Person.PersonHighEducationInfo;

//                    if (PersonEducationDocument == null)
//                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
//                    if (PersonHighEducationInfo == null)
//                        PersonHighEducationInfo = new OnlineAbit2013.PersonHighEducationInfo();

//                    model.EducationInfo.QualificationList = Util.QualifaicationAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
//                    model.EducationInfo.SchoolTypeList = Util.SchoolTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
//                    model.EducationInfo.StudyFormList = Util.StudyFormAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
//                    model.EducationInfo.LanguageList = Util.LanguagesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
//                    model.EducationInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

//                    query = "SELECT Id, Name FROM SchoolTypeAll";
//                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
//                    model.EducationInfo.SchoolTypeList =
//                        (from DataRow rw in _tblT.Rows
//                         select new SelectListItem()
//                         {
//                             Value = rw.Field<int>("Id").ToString(),
//                             Text = rw.Field<string>("Name")
//                         }).ToList();

//                    model.EducationInfo.SchoolName = Server.HtmlDecode(PersonEducationDocument.SchoolName);
//                    model.EducationInfo.SchoolNumber = Server.HtmlDecode(PersonEducationDocument.SchoolNum);
//                    model.EducationInfo.SchoolExitYear = Server.HtmlDecode(PersonEducationDocument.SchoolExitYear);
//                    model.EducationInfo.SchoolCity = Server.HtmlDecode(PersonEducationDocument.SchoolCity);

//                    model.EducationInfo.AvgMark = PersonEducationDocument.AvgMark.HasValue ? PersonEducationDocument.AvgMark.Value.ToString() : "";
//                    model.EducationInfo.IsExcellent = PersonEducationDocument.IsExcellent ?? false;
//                    model.EducationInfo.StartEnglish = PersonEducationDocument.StartEnglish ?? false;
//                    model.EducationInfo.EnglishMark = PersonEducationDocument.EnglishMark.ToString();

//                    model.EducationInfo.AttestatRegion = Server.HtmlDecode(PersonEducationDocument.AttestatRegion);
//                    model.EducationInfo.AttestatSeries = Server.HtmlDecode(PersonEducationDocument.AttestatSeries);
//                    model.EducationInfo.AttestatNumber = Server.HtmlDecode(PersonEducationDocument.AttestatNumber);

//                    model.EducationInfo.HEExitYear = Server.HtmlDecode(PersonHighEducationInfo.ExitYear.ToString());
//                    model.EducationInfo.HEEntryYear = Server.HtmlDecode(PersonHighEducationInfo.EntryYear.ToString());

//                    model.EducationInfo.DiplomSeries = Server.HtmlDecode(PersonEducationDocument.Series);
//                    model.EducationInfo.DiplomNumber = Server.HtmlDecode(PersonEducationDocument.Number);
//                    model.EducationInfo.ProgramName = Server.HtmlDecode(PersonHighEducationInfo.ProgramName);
//                    model.EducationInfo.DiplomTheme = Server.HtmlDecode(PersonHighEducationInfo.DiplomaTheme);

//                    model.EducationInfo.SchoolTypeId = (PersonEducationDocument.SchoolTypeId ?? 1).ToString();
//                    model.EducationInfo.PersonStudyForm = (PersonHighEducationInfo.StudyFormId ?? 1).ToString();
//                    model.EducationInfo.PersonQualification = (PersonHighEducationInfo.QualificationId ?? 1).ToString();
//                    model.EducationInfo.LanguageId = (PersonEducationDocument.LanguageId ?? 1).ToString();
//                    model.EducationInfo.CountryEducId = (PersonEducationDocument.CountryEducId ?? 193).ToString();

//                    model.EducationInfo.Barcode = Person.Barcode.ToString();
//                    query = "SELECT Id, Name FROM SPO_SchoolExitClass";
//                    _tblT = Util.AbitDB.GetDataTable(query, null);
//                    model.EducationInfo.SchoolExitClassList =
//                        (from DataRow rw in _tblT.Rows
//                         select new SelectListItem()
//                         {
//                             Value = rw.Field<int>("Id").ToString(),
//                             Text = rw.Field<string>("Name")
//                         }).ToList();

//                    string qEgeMarks = "SELECT EgeMark.Id, EgeCertificate.Number, EgeExam.Name, EgeMark.Value FROM Person " +
//                        " INNER JOIN EgeCertificate ON EgeCertificate.PersonId = Person.Id INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
//                        " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE Person.Id=@Id";
//                    DataTable tblEge = Util.AbitDB.GetDataTable(qEgeMarks, new SortedList<string, object>() { { "@Id", PersonId } });

//                    model.EducationInfo.EgeMarks = new List<EgeMarkModel>();
//                    model.EducationInfo.EgeMarks =
//                        (from DataRow rw in tblEge.Rows
//                         select new EgeMarkModel()
//                         {
//                             Id = rw.Field<Guid>("Id"),
//                             CertificateNum = rw.Field<string>("Number"),
//                             ExamName = rw.Field<string>("Name"),
//                             Value = rw.Field<int?>("Value").ToString()
//                         }).ToList();

//                    Person_SPOSchoolExitClass Person_SPOSchoolExitClass = Person.Person_SPOSchoolExitClass;
//                    if (Person_SPOSchoolExitClass == null)
//                        Person_SPOSchoolExitClass = new Person_SPOSchoolExitClass();

//                    model.EducationInfo.SchoolExitClassId = Person_SPOSchoolExitClass.SchoolExitClassId.ToString();
//                }
//                else if (model.Stage == 5)
//                {

//                    PersonWork PersonWork = Person.PersonWork.DefaultIfEmpty(null).First();
//                    if (PersonWork == null)
//                        PersonWork = new OnlineAbit2013.PersonWork();

//                    PersonSportQualification PersonSportQualification = Person.PersonSportQualification;
//                    if (PersonSportQualification == null)
//                        PersonSportQualification = new OnlineAbit2013.PersonSportQualification();

//                    model.PrivelegeInfo = new SPO_PersonPrivileges();
//                    model.PrivelegeInfo.Stag = PersonWork.Stage;
//                    model.PrivelegeInfo.WorkPlace = PersonWork.WorkPlace;
//                    model.PrivelegeInfo.SportQualification = PersonSportQualification.OtherSportQualification;
//                    model.PrivelegeInfo.SportQualificationId = PersonSportQualification.SportQualificationId.ToString();
//                    model.PrivelegeInfo.SportQualificationLevel = PersonSportQualification.SportQualificationLevel;

//                    query = "SELECT Id, Name FROM SportQualification";
//                    DataTable tbl = Util.AbitDB.GetDataTable(query, null);
//                    model.PrivelegeInfo.SportQualificationList =
//                        (from DataRow rw in tbl.Rows
//                         select new SelectListItem()
//                         {
//                             Value = rw.Field<int>("Id").ToString(),
//                             Text = rw.Field<string>("Name")
//                         }).ToList();
//                }
//                else if (model.Stage == 6)
//                {
//                    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                        return RedirectToAction("LogOn", "Account");

//                    var AddInfo = Person.PersonAddInfo;
//                    if (AddInfo == null)
//                        AddInfo = new PersonAddInfo();

//                    model.AddInfo = new AdditionalInfoPerson()
//                    {
//                        FZ_152Agree = false,
//                        ExtraInfo = Server.HtmlDecode(AddInfo.AddInfo),
//                        HasPrivileges = AddInfo.HasPrivileges ?? false,
//                        HostelAbit = AddInfo.HostelAbit ?? false,
//                        ContactPerson = Server.HtmlDecode(AddInfo.Parents)
//                    };
//                }
//                return View("PersonalOffice", model);
//            }
//        }

//        [HttpPost]
//        [ValidateInput(false)]
//        public ActionResult NextStep(PersonalOffice_SPO model)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
//                int iRegStage = Person.RegistrationStage;

//                if (iRegStage == 0)
//                    iRegStage = 1;

//                if (Util.CheckPersonReadOnlyStatus(PersonId))
//                {
//                    if (++(model.Stage) <= 6)
//                        return RedirectToAction("Index", "SPO", new RouteValueDictionary() { { "step", model.Stage } });
//                    else
//                        return RedirectToAction("Main", "SPO");
//                }

//                if (model.Stage == 1)
//                {
//                    DateTime bdate;
//                    if (!DateTime.TryParse(model.PersonInfo.BirthDate, CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat, DateTimeStyles.None, out bdate))
//                        bdate = DateTime.Now.Date;

//                    if (bdate.Date > DateTime.Now.Date)
//                        bdate = DateTime.Now.Date;

//                    int NationalityId = 193;
//                    if (!int.TryParse(model.PersonInfo.Nationality, out NationalityId))
//                        NationalityId = 193;

//                    Person.Surname = model.PersonInfo.Surname;
//                    Person.Name = model.PersonInfo.Name;
//                    Person.SecondName = model.PersonInfo.SecondName;
//                    Person.BirthDate = bdate;
//                    Person.BirthPlace = model.PersonInfo.BirthPlace;
//                    Person.NationalityId = NationalityId;
//                    Person.Sex = model.PersonInfo.Sex == "M" ? true : false;
//                    Person.RegistrationStage = iRegStage < 2 ? 2 : iRegStage;

//                    context.SaveChanges();
//                }
//                else if (model.Stage == 2)
//                {
//                    int iPassportType = 1;
//                    if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
//                        iPassportType = 1;

//                    DateTime? dtPassportDate, dtPassportValid;
//                    try
//                    {
//                        dtPassportDate = Convert.ToDateTime(model.PassportInfo.PassportDate,
//                            System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
//                    }
//                    catch { dtPassportDate = DateTime.Now; }

//                    try
//                    {
//                        dtPassportValid = Convert.ToDateTime(model.PassportInfo.PassportValid,
//                            System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
//                    }
//                    catch { dtPassportValid = null; }

//                    if (dtPassportDate.Value.Date > DateTime.Now.Date)
//                        dtPassportDate = DateTime.Now.Date;

//                    Person.PassportTypeId = iPassportType;
//                    Person.PassportSeries = model.PassportInfo.PassportSeries;
//                    Person.PassportNumber = model.PassportInfo.PassportNumber;
//                    Person.PassportAuthor = model.PassportInfo.PassportAuthor;
//                    Person.PassportDate = dtPassportDate;
//                    Person.PassportCode = model.PassportInfo.PassportCode;
//                    Person.PassportValid = dtPassportValid;

//                    Person.RegistrationStage = iRegStage < 3 ? 3 : iRegStage;

//                    context.SaveChanges();
//                }
//                else if (model.Stage == 3)
//                {
//                    int iCountryId = 0;
//                    if (!int.TryParse(model.ContactsInfo.CountryId, out iCountryId))
//                        iCountryId = 193;//Russia

//                    int iRegionId = 0;
//                    if (!int.TryParse(model.ContactsInfo.RegionId, out iRegionId))
//                        iRegionId = 0;//unnamed

//                    int? altRegionId = context.Country.Where(x => x.Id == iCountryId).Select(x => x.RegionId).FirstOrDefault();
//                    if (altRegionId.HasValue && iRegionId == 0)
//                    {
//                        if (iCountryId != 193)
//                            iRegionId = altRegionId.Value;//RegionValue
//                        else
//                            iRegionId = 3;//Russia
//                    }
//                    else
//                        if (iCountryId != 193)
//                            iRegionId = 11;//Далн. зарубеж.

//                    bool bIns = false;
//                    var PersonContacts = Person.PersonContacts;
//                    if (PersonContacts == null)
//                    {
//                        PersonContacts = new PersonContacts();
//                        PersonContacts.PersonId = PersonId;
//                        bIns = true;
//                    }

//                    PersonContacts.Phone = model.ContactsInfo.MainPhone;
//                    PersonContacts.Mobiles = model.ContactsInfo.SecondPhone;
//                    PersonContacts.CountryId = iCountryId;
//                    PersonContacts.RegionId = iRegionId;

//                    PersonContacts.Code = model.ContactsInfo.PostIndex;
//                    PersonContacts.City = model.ContactsInfo.City;
//                    PersonContacts.Street = model.ContactsInfo.Street;
//                    PersonContacts.House = model.ContactsInfo.House;
//                    PersonContacts.Korpus = model.ContactsInfo.Korpus;
//                    PersonContacts.Flat = model.ContactsInfo.Flat;
//                    PersonContacts.CodeReal = model.ContactsInfo.PostIndexReal;
//                    PersonContacts.CityReal = model.ContactsInfo.CityReal;
//                    PersonContacts.StreetReal = model.ContactsInfo.StreetReal;
//                    PersonContacts.HouseReal = model.ContactsInfo.HouseReal;
//                    PersonContacts.KorpusReal = model.ContactsInfo.KorpusReal;
//                    PersonContacts.FlatReal = model.ContactsInfo.FlatReal;

//                    if (bIns)
//                        context.PersonContacts.AddObject(PersonContacts);

//                    Person.RegistrationStage = iRegStage < 4 ? 4 : iRegStage;

//                    context.SaveChanges();
//                }
//                else if (model.Stage == 4)//образование
//                {
//                    int iSchoolTypeId;
//                    int SchoolExitYear;
//                    if (!int.TryParse(model.EducationInfo.SchoolTypeId, out iSchoolTypeId))
//                        iSchoolTypeId = 1;
//                    if (!int.TryParse(model.EducationInfo.SchoolExitYear, out SchoolExitYear))
//                        SchoolExitYear = DateTime.Now.Year;
//                    int iCountryEducId;
//                    if (!int.TryParse(model.EducationInfo.CountryEducId, out iCountryEducId))
//                        iCountryEducId = 1;
//                    int iLanguageId;
//                    if (!int.TryParse(model.EducationInfo.LanguageId, out iLanguageId))
//                        iLanguageId = 1;

//                    double avgBall;
//                    if (!double.TryParse(model.EducationInfo.AvgMark, out avgBall))
//                        avgBall = 0.0;

//                    double EnglishMark;
//                    if (!double.TryParse(model.EducationInfo.EnglishMark, out EnglishMark))
//                        EnglishMark = 0.0;

//                    int sform = 0;
//                    if (!int.TryParse(model.EducationInfo.PersonStudyForm, out sform))
//                        sform = 0;
//                    int qualId = 0;
//                    if (!int.TryParse(model.EducationInfo.PersonQualification, out qualId))
//                        qualId = 0;

//                    var PersonEducationDocument = Person.PersonEducationDocument;
//                    var PersonHighEducationInfo = Person.PersonHighEducationInfo;

//                    //-----------------PersonEducationDocument---------------------
//                    bool bIns = false;
//                    if (PersonEducationDocument == null)
//                    {
//                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
//                        PersonEducationDocument.PersonId = PersonId;
//                        bIns = true;
//                    }

//                    PersonEducationDocument.SchoolTypeId = iSchoolTypeId;
//                    PersonEducationDocument.SchoolName = model.EducationInfo.SchoolName;
//                    PersonEducationDocument.SchoolNum = model.EducationInfo.SchoolNumber;
//                    PersonEducationDocument.SchoolCity = model.EducationInfo.SchoolCity;
//                    PersonEducationDocument.AvgMark = (avgBall == 0.0 ? null : (double?)avgBall);
//                    PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
//                    PersonEducationDocument.CountryEducId = iCountryEducId;
//                    PersonEducationDocument.LanguageId = iLanguageId;
//                    PersonEducationDocument.IsExcellent = model.EducationInfo.IsExcellent;
//                    PersonEducationDocument.EnglishMark = EnglishMark;
//                    PersonEducationDocument.StartEnglish = model.EducationInfo.StartEnglish;

//                    if (iSchoolTypeId == 1)//Pure School
//                    {
//                        PersonEducationDocument.AttestatRegion = model.EducationInfo.AttestatRegion;
//                        PersonEducationDocument.AttestatSeries = model.EducationInfo.AttestatSeries;
//                        PersonEducationDocument.AttestatNumber = model.EducationInfo.AttestatNumber;
//                    }
//                    else
//                    {
//                        PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
//                        PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;
//                    }

//                    if (bIns)
//                    {
//                        context.PersonEducationDocument.AddObject(PersonEducationDocument);
//                        bIns = false;
//                    }

//                    //-----------------PersonHighEducationInfo---------------------
//                    if (PersonHighEducationInfo == null)
//                    {
//                        PersonHighEducationInfo = new PersonHighEducationInfo();
//                        PersonHighEducationInfo.PersonId = PersonId;
//                        bIns = true;
//                    }

//                    int HEEntryYear;
//                    if (!int.TryParse(model.EducationInfo.HEEntryYear, out HEEntryYear))
//                        HEEntryYear = 0;

//                    PersonHighEducationInfo.DiplomaTheme = model.EducationInfo.DiplomTheme;
//                    PersonHighEducationInfo.ProgramName = model.EducationInfo.ProgramName;
//                    PersonHighEducationInfo.EntryYear = (HEEntryYear == 0 ? null : (int?)HEEntryYear);
//                    if (sform != 0)
//                        PersonHighEducationInfo.StudyFormId = sform;
//                    if (qualId != 0)
//                        PersonHighEducationInfo.QualificationId = qualId;

//                    if (iSchoolTypeId == 4)
//                    {
//                        int iEntryYear;
//                        int.TryParse(model.EducationInfo.HEEntryYear, out iEntryYear);
//                        PersonHighEducationInfo.EntryYear = iEntryYear != 0 ? (int?)iEntryYear : null;
//                        PersonHighEducationInfo.ExitYear = SchoolExitYear != 0 ? (int?)SchoolExitYear : null;
//                    }

//                    if (bIns)
//                        context.PersonHighEducationInfo.AddObject(PersonHighEducationInfo);

//                    int iExitClassId;
//                    int.TryParse(model.EducationInfo.SchoolExitClassId, out iExitClassId);
//                    Person_SPOSchoolExitClass Person_SPOSchoolExitClass = Person.Person_SPOSchoolExitClass;
//                    if (Person_SPOSchoolExitClass == null)
//                    {
//                        Person_SPOSchoolExitClass = new Person_SPOSchoolExitClass();
//                        Person_SPOSchoolExitClass.PersonId = PersonId;
//                        Person_SPOSchoolExitClass.SchoolExitClassId = iExitClassId;
//                        context.Person_SPOSchoolExitClass.AddObject(Person_SPOSchoolExitClass);
//                    }
//                    else
//                        Person_SPOSchoolExitClass.SchoolExitClassId = iExitClassId;

//                    Person.RegistrationStage = iRegStage < 5 ? 5 : iRegStage;
//                    context.SaveChanges();
//                }
//                else if (model.Stage == 5)
//                {
//                    PersonWork PersonWork = Person.PersonWork.DefaultIfEmpty(null).First();
//                    bool bIns = false;
//                    if (PersonWork == null)
//                    {
//                        bIns = true;
//                        PersonWork = new PersonWork();
//                        PersonWork.Id = Guid.NewGuid();
//                        PersonWork.PersonId = PersonId;
//                    }
                    
//                    PersonWork.WorkPlace = model.PrivelegeInfo.WorkPlace;
//                    PersonWork.Stage = model.PrivelegeInfo.Stag;
//                    if (bIns)
//                        context.PersonWork.AddObject(PersonWork);

//                    PersonSportQualification PersonSportQualification = Person.PersonSportQualification;
//                    bIns = false;
//                    if (PersonSportQualification == null)
//                    {
//                        bIns = true;
//                        PersonSportQualification = new OnlineAbit2013.PersonSportQualification();
//                        PersonSportQualification.PersonId = PersonId;
//                    }

//                    int iSportQualId;
//                    int.TryParse(model.PrivelegeInfo.SportQualificationId, out iSportQualId);

//                    PersonSportQualification.SportQualificationId = iSportQualId;
//                    PersonSportQualification.SportQualificationLevel = model.PrivelegeInfo.SportQualificationLevel;
//                    PersonSportQualification.OtherSportQualification = model.PrivelegeInfo.SportQualification;
//                    if (bIns)
//                        context.PersonSportQualification.AddObject(PersonSportQualification);

//                    if (iRegStage < 6)
//                        Person.RegistrationStage = 6;

//                    context.SaveChanges();
//                }
//                else if (model.Stage == 6)
//                {
//                    if (!model.AddInfo.FZ_152Agree)
//                    {
//                        ModelState.AddModelError("AddInfo_FZ_152Agree", "Вы должны принять условия");
//                        return View("PersonalOffice", model);
//                    }
//                    bool bIns = false;
//                    var PersonAddInfo = Person.PersonAddInfo;
//                    if (PersonAddInfo == null)
//                    {
//                        PersonAddInfo = new PersonAddInfo();
//                        PersonAddInfo.PersonId = PersonId;
//                        bIns = true;
//                    }

//                    PersonAddInfo.AddInfo = model.AddInfo.ExtraInfo;
//                    PersonAddInfo.Parents = model.AddInfo.ContactPerson;
//                    PersonAddInfo.HasPrivileges = model.AddInfo.HasPrivileges;
//                    PersonAddInfo.HostelAbit = model.AddInfo.HostelAbit;

//                    if (Person.RegistrationStage <= 6)
//                        Person.RegistrationStage = 100;

//                    if (bIns)
//                        context.PersonAddInfo.AddObject(PersonAddInfo);

//                    context.SaveChanges();
//                }

//                if (model.Stage < 6)
//                {
//                    model.Stage++;
//                    return RedirectToAction("Index", "SPO", new RouteValueDictionary() { { "step", model.Stage } });
//                }
//                else
//                    return RedirectToAction("Main", "SPO");
//            }
//        }

//        public ActionResult Main()
//        {
//            //if (Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 && Util.bUseRedirection &&
//            //    Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
//            //    return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

//            //Validation
//            Guid PersonID;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
//                return RedirectToAction("LogOn", "Account");

//            if (Util.CheckIsNew(PersonID))
//                return RedirectToAction("OpenPersonalAccount");

//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
//                if (PersonInfo == null)
//                    return RedirectToAction("Index");

//                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
//                switch (PersonInfo.AbiturientTypeId)
//                {
//                    case 1: { return RedirectToAction("Main", "Abiturient"); }
//                    case 2: { return RedirectToAction("Main", "ForeignAbiturient"); }
//                    case 3: { return RedirectToAction("Main", "Transfer"); }
//                    case 4: { return RedirectToAction("Main", "TransferForeign"); }
//                    case 5: { return RedirectToAction("Main", "Recover"); }
//                    case 6: { return RedirectToAction("Main", "ChangeStudyForm"); }
//                    case 7: { return RedirectToAction("Main", "ChangeObrazProgram"); }
//                    case 8: { return RedirectToAction("Main", "AG"); }
//                    case 9: { break; }
//                    default: { return RedirectToAction("Main", "Abiturient"); }
//                }

//                int regStage = PersonInfo.RegistrationStage;
//                if (regStage < 100)
//                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage.ToString() } });

//                SimplePerson model = new SimplePerson();
//                model.Applications = new List<SimpleApplication>();
//                model.Files = new List<AppendedFile>();

//                string query = "SELECT Surname, Name, SecondName, RegistrationStage FROM PERSON WHERE Id=@Id";
//                SortedList<string, object> dic = new SortedList<string, object>() { { "@Id", PersonID } };
//                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//                if (tbl.Rows.Count != 1)
//                    return RedirectToAction("Index");

//                model.Name = Server.HtmlEncode(PersonInfo.Name);
//                model.Surname = Server.HtmlEncode(PersonInfo.Surname);
//                model.SecondName = Server.HtmlEncode(PersonInfo.SecondName);

//                var Applications = context.Abiturient.Where(x => x.PersonId == PersonID);

//                query = "SELECT [Application].Id, LicenseProgramName, ObrazProgramName, ProfileName, Priority, Enabled, StudyFormName, StudyBasisName FROM [Application] " +
//                    "INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE PersonId=@PersonId";
//                dic.Clear();
//                dic.Add("@PersonId", PersonID);
//                tbl = Util.AbitDB.GetDataTable(query, dic);
//                foreach (var app in Applications)
//                {
//                    model.Applications.Add(new SimpleApplication()
//                    {
//                        Id = app.Id,
//                        Profession = app.LicenseProgramName,
//                        ObrazProgram = app.ObrazProgramName,
//                        Specialization = app.ProfileName,
//                        Priority = app.Priority.HasValue ? app.Priority.Value.ToString() : "1",
//                        Enabled = app.Enabled ?? true,
//                        StudyBasis = app.StudyBasisName,
//                        StudyForm = app.StudyFormName
//                    });
//                }

//                model.Messages = Util.GetNewPersonalMessages(PersonID);
//                if (model.Applications.Count == 0)
//                {
//                    model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, Text = "Для подачи заявления нажмите кнопку <a href=\"" + Util.ServerAddress + "/Abiturient/NewApplication\">\"Подать новое заявление\"</a>" });
//                }

//                return View("Main", model);
//            }
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult NewApplication(params string[] errors)
//        {
//            if (errors != null && errors.Length > 0)
//            {
//                foreach (string er in errors)
//                    ModelState.AddModelError("", er);
//            }

//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
//                if (PersonInfo == null)//а что это могло бы значить???
//                    return RedirectToAction("Index");

//                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
//                switch (PersonInfo.AbiturientTypeId)
//                {
//                    case 1: { return RedirectToAction("NewApplication", "Abiturient"); }
//                    case 2: { return RedirectToAction("NewApplication", "ForeignAbiturient"); }
//                    case 3: { return RedirectToAction("NewApplication", "Transfer"); }
//                    case 4: { return RedirectToAction("NewApplication", "TransferForeign"); }
//                    case 5: { return RedirectToAction("NewApplication", "Recover"); }
//                    case 6: { return RedirectToAction("NewApplication", "ChangeStudyForm"); }
//                    case 7: { return RedirectToAction("NewApplication", "ChangeObrazProgram"); }
//                    case 8: { return RedirectToAction("NewApplication", "AG"); }
//                    case 9: { break; }
//                    default: { break; }
//                }
                
//                SPO_NewApplicationModel model = new SPO_NewApplicationModel();

//                int iObrazProgramId = 1;
//                string query = @"SELECT SchoolTypeId, SchoolExitClassId FROM PersonEducationDocument 
//                    LEFT JOIN Person_SPOSchoolExitClass ON Person_SPOSchoolExitClass.PersonId=PersonEducationDocument.PersonId 
//                    WHERE PersonEducationDocument.PersonId=@Id";
//                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
//                if (tbl.Rows[0].Field<int?>("SchoolTypeId") == 1)
//                {
//                    //у школьников жёсткая привязка
//                    iObrazProgramId = tbl.Rows[0].Field<int>("SchoolExitClassId");
//                    model.CanChooseExitClass = false;
//                    model.ObrazProgramId = iObrazProgramId.ToString();
//                    model.ObrazPrograms = new List<SelectListItem>();
//                    query = "SELECT ObrazProgramName FROM Entry WHERE ObrazProgramId=@Id";
//                    model.ObrazProgramName = Util.AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Id", iObrazProgramId } });
//                }
//                else
//                {
//                    model.CanChooseExitClass = true;
//                    query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName FROM Entry WHERE SemesterId=1 AND CampaignYear=2013 AND StudyLevelId IN (8,10)";
//                    SortedList<string, object> dic = new SortedList<string, object>();
//                    tbl = Util.AbitDB.GetDataTable(query, null);
//                    model.ObrazPrograms = (from DataRow rw in tbl.Rows
//                                           select new SelectListItem()
//                                           {
//                                               Value = rw.Field<int>("ObrazProgramId").ToString(),
//                                               Text = rw.Field<string>("ObrazProgramName")
//                                           }).ToList();
//                }

//                //одно заявление -- уже не актуально
//                //query = "SELECT COUNT(Id) FROM [Application] WHERE PersonId=@PersonId AND Enabled='True'";
//                //int cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
//                //if (cnt > 0)
//                //    ModelState.AddModelError("", "Вы уже подали заявление на программы среднего профессионального образования. " +
//                //        "Допускается подача только одного заявления");

//                //model.ObrazProgramId = tbl.Rows[0].Field<string>("EntryClassName");

//                return View("NewApplication", model);
//            }
//        }

//        [HttpPost]
//        public ActionResult NewApp()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            string profession = Request.Form["Profession"];
//            string entryclass = Request.Form["ObrazProgram"];
//            bool needHostel = string.IsNullOrEmpty(Request.Form["NeedHostel"]) ? false : true;
//            string studyFromId = Request.Form["StudyForm"];
//            string studyBasisId = Request.Form["StudyBasis"];

//            int iObrazProgramId = Util.ParseSafe(entryclass);
//            int iProfession = Util.ParseSafe(profession);
//            int iStudyFormId = Util.ParseSafe(studyFromId);
//            int iStudyBasisId = Util.ParseSafe(studyBasisId);

//            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
//            string query = "SELECT Id FROM Entry WHERE LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyFormId=@StudyFormId " +
//                " AND StudyBasisId=@StudyBasisId AND SemesterId=1 AND CampaignYear=@CampaignYear AND StudyLevelId IN (8,10)";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@LicenseProgramId", iProfession);
//            dic.Add("@ObrazProgramId", iObrazProgramId);
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@CampaignYear", Util.iPriemYear);
//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            if (tbl.Rows.Count > 1)
//                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Неоднозначный выбор учебного плана (найдено " + tbl.Rows.Count + ")" } });
//            if (tbl.Rows.Count == 0)
//                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Не найден учебный план" } });

//            Guid? EntryId = tbl.Rows[0].Field<Guid?>("Id");

//            query = "SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
//            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
//            var eIds =
//                from DataRow rw in tbl.Rows
//                select rw.Field<Guid?>("EntryId");
//            if (eIds.Contains(EntryId))
//                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

//            query = "SELECT DateOfClose FROM [Entry] WHERE Id=@Id";
//            DateTime DateOfClose = (DateTime)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", EntryId } });

//            if (DateTime.Now > DateOfClose)
//                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на данное направление прекращена " + DateOfClose.ToString("dd.MM.yyyy") } });


//            DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [Application] WHERE PersonId=@PersonId AND Enabled=@Enabled",
//                new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
//            int? PriorMax =
//                (from DataRow rw in tblPriors.Rows
//                 select rw.Field<int?>("Priority")).Max();

//            Guid appId = Guid.NewGuid();
//            query = "INSERT INTO [Application] (Id, PersonId, EntryId, HostelEduc, Enabled, Priority, EntryType, DateOfStart) " +
//                "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @Priority, @EntryType, @DateOfStart)";
//            SortedList<string, object> prms = new SortedList<string, object>()
//            {
//                { "@Id", appId },
//                { "@PersonId", PersonId },
//                { "@EntryId", EntryId },
//                { "@HostelEduc", needHostel },
//                { "@Enabled", true },
//                { "@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1 },
//                { "@EntryType", 3 },
//                { "@DateOfStart", DateTime.Now }
//            };

//            Util.AbitDB.ExecuteQuery(query, prms);
//            query = "SELECT Person.Surname, Person.Name, Person.SecondName, Entry.LicenseProgramName, Entry.ObrazProgramName " +
//                " FROM [Application] INNER JOIN Person ON Person.Id=[Application].PersonId " +
//                " INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE Application.Id=@AppId";
//            DataTable Tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", appId } });
//            var fileInfo =
//                (from DataRow rw in Tbl.Rows
//                 select new
//                 {
//                     Surname = rw.Field<string>("Surname"),
//                     Name = rw.Field<string>("Name"),
//                     SecondName = rw.Field<string>("SecondName"),
//                     Profession = rw.Field<string>("LicenseProgramName"),
//                     ObrazProgram = rw.Field<string>("ObrazProgramName")
//                 }).FirstOrDefault();

//            //byte[] pdfData = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
//            //DateTime dateTime = DateTime.Now;

//            //query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileExtention, FileData, FileSize, IsReadOnly, LoadDate, Comment, MimeType) " +
//            //    " VALUES (@Id, @PersonId, @FileName, @FileExtention, @FileData, @FileSize, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
//            //prms.Clear();
//            //prms.Add("@Id", Guid.NewGuid());
//            //prms.Add("@PersonId", appId);
//            //prms.Add("@FileName", fileInfo.Surname + " " + fileInfo.Name.FirstOrDefault() + " - Заявление [" + dateTime.ToString("dd.MM.yyyy HH.mm") + "].pdf");
//            //prms.Add("@FileExtention", ".pdf");
//            //prms.Add("@FileData", pdfData);
//            //prms.Add("@FileSize", pdfData.Length);
//            //prms.Add("@IsReadOnly", true);
//            //prms.Add("@LoadDate", dateTime);
//            //prms.Add("@Comment", "Заявление на направление " + fileInfo.Profession + ", " + fileInfo.ObrazProgram + " от " + dateTime.ToShortDateString());
//            //prms.Add("@MimeType", "[Application]/pdf");
//            //Util.AbitDB.ExecuteQuery(query, prms);

//            return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", appId.ToString("N") } });
//        }

//        public ActionResult MotivateMail()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            string query = "SELECT Id, Priority, Profession, ObrazProgram, Specialization FROM extApplicationAll WHERE PersonId=@PersonId AND Enabled=@Enabled";
//            SortedList<string, object> dic = new SortedList<string, object>()
//            {
//                {"@PersonId", PersonId },
//                {"@Enabled", true }
//            };
//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var apps = (from DataRow rw in tbl.Rows
//                        select new SimpleApplication()
//                        {
//                            Id = rw.Field<Guid>("Id"),
//                            Priority = rw.Field<int>("Priority").ToString(),
//                            Profession = rw.Field<string>("Profession"),
//                            ObrazProgram = rw.Field<string>("ObrazProgram"),
//                            Specialization = rw.Field<string>("Specialization")
//                        }).ToList();

//            MotivateMailModel mdl = new MotivateMailModel()
//            {
//                Apps = apps
//            };
//            return View(mdl);
//        }

//        public ActionResult PriorityChanger()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");
//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
//                if (PersonInfo == null)//а что это могло бы значить???
//                    return RedirectToAction("Index");

//                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
//                switch (PersonInfo.AbiturientTypeId)
//                {
//                    case 1: { break; }
//                    case 2: { return RedirectToAction("PriorityChanger", "ForeignAbiturient"); }
//                    //case 3: { return RedirectToAction("PriorityChanger", "Transfer"); }
//                    //case 4: { return RedirectToAction("PriorityChanger", "TransferForeign"); }
//                    //case 5: { return RedirectToAction("PriorityChanger", "Recover"); }
//                    //case 6: { return RedirectToAction("PriorityChanger", "ChangeStudyForm"); }
//                    //case 7: { return RedirectToAction("PriorityChanger", "ChangeObrazProgram"); }
//                    //case 8: { return RedirectToAction("PriorityChanger", "AG"); }
//                    default: { break; }
//                }

//                string query = "(SELECT [Application].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [Application] " +
//                    " INNER JOIN Entry ON [Application].EntryId=Entry.Id " +
//                    " WHERE PersonId=@PersonId AND Enabled=@Enabled " +
//                    " UNION " +
//                    " SELECT [ForeignApplication].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [ForeignApplication] " +
//                    " INNER JOIN Entry ON [ForeignApplication].EntryId=Entry.Id " +
//                    " WHERE PersonId=@PersonId AND Enabled=@Enabled) ORDER BY Priority ";
//                SortedList<string, object> dic = new SortedList<string, object>()
//                {
//                    {"@PersonId", PersonId },
//                    {"@Enabled", true }
//                };
//                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//                var apps = (from DataRow rw in tbl.Rows
//                            select new SimpleApplication()
//                            {
//                                Id = rw.Field<Guid>("Id"),
//                                Priority = rw.Field<int>("Priority").ToString(),
//                                Profession = rw.Field<string>("LicenseProgramName"),
//                                ObrazProgram = rw.Field<string>("ObrazProgramName"),
//                                Specialization = rw.Field<string>("ProfileName")
//                            }).ToList();

//                MotivateMailModel mdl = new MotivateMailModel()
//                {
//                    Apps = apps,
//                    UILanguage = Util.GetUILang(PersonId)
//                };
//                return View(mdl);
//            }
//        }

//        [HttpPost]
//        public ActionResult ChangePriority(MotivateMailModel model)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");
//            int prior = 0;
//            string[] allKeys = Request.Form.AllKeys;
//            foreach (string key in allKeys)
//            {
//                Guid appId;
//                if (!Guid.TryParse(key, out appId))
//                    continue;

//                string query = "UPDATE [Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId";
//                SortedList<string, object> dic = new SortedList<string, object>();
//                dic.AddItem("@Priority", ++prior);
//                dic.AddItem("@Id", appId);
//                dic.AddItem("@PersonId", PersonId);

//                try
//                {
//                    Util.AbitDB.ExecuteQuery(query, dic);
//                }
//                catch { }
//            }
//            return RedirectToAction("PriorityChanger");
//        }

//        public ActionResult AddFiles()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            string query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
//            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

//            List<AppendedFile> lst =
//                (from DataRow rw in tbl.Rows
//                 select new AppendedFile() { Id = rw.Field<Guid>("Id"), FileName = rw.Field<string>("FileName"), FileSize = rw.Field<int>("FileSize"), Comment = rw.Field<string>("Comment") })
//                .ToList();

//            AppendFilesModel model = new AppendFilesModel() { Files = lst };
//            return View(model);
//        }

//        public ActionResult AddSharedFiles()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return RedirectToAction("LogOn", "Account");

//            using (OnlinePriemEntities context = new OnlinePriemEntities())
//            {
//                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
//                if (PersonInfo == null)//а что это могло бы значить???
//                    return RedirectToAction("Index");

//                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
//                switch (PersonInfo.AbiturientTypeId)
//                {
//                    case 1: { break; }
//                    case 2: { return RedirectToAction("AddSharedFiles", "ForeignAbiturient"); }
//                    //case 3: { return RedirectToAction("AddSharedFiles", "Transfer"); }
//                    //case 4: { return RedirectToAction("AddSharedFiles", "TransferForeign"); }
//                    //case 5: { return RedirectToAction("AddSharedFiles", "Recover"); }
//                    //case 6: { return RedirectToAction("AddSharedFiles", "ChangeStudyForm"); }
//                    //case 7: { return RedirectToAction("AddSharedFiles", "ChangeObrazProgram"); }
//                    case 8: { return RedirectToAction("AddSharedFiles", "AG"); }
//                    default: { break; }
//                }

//                Util.SetThreadCultureByCookies(Request.Cookies);

//                string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
//                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

//                List<AppendedFile> lst =
//                    (from DataRow rw in tbl.Rows
//                     select new AppendedFile()
//                     {
//                         Id = rw.Field<Guid>("Id"),
//                         FileName = rw.Field<string>("FileName"),
//                         FileSize = rw.Field<int>("FileSize"),
//                         Comment = rw.Field<string>("Comment"),
//                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
//                            rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
//                     }).ToList();

//                AppendFilesModel model = new AppendFilesModel() { Files = lst };
//                return View(model);
//            }
//        }

//        [HttpPost]
//        public ActionResult AddSharedFile()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json("Ошибка авторизации");

//            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
//                return Json(Resources.ServerMessages.EmptyFileError);

//            string fileName = Request.Files["File"].FileName;
//            int lastSlashPos = 0;
//            lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
//            if (lastSlashPos > 0)
//                fileName = fileName.Substring(lastSlashPos);
//            string fileComment = Request.Form["Comment"];
//            int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
//            byte[] fileData = new byte[fileSize];
//            //читаем данные из ПОСТа
//            Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
//            string fileext = "";
//            try
//            {
//                fileext = fileName.Substring(fileName.LastIndexOf('.'));
//            }
//            catch
//            {
//                fileext = "";
//            }

//            try
//            {
//                string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, LoadDate, Comment, MimeType) " +
//                    " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @LoadDate, @Comment, @MimeType)";
//                SortedList<string, object> dic = new SortedList<string, object>();
//                dic.Add("@Id", Guid.NewGuid());
//                dic.Add("@PersonId", PersonId);
//                dic.Add("@FileName", fileName);
//                dic.Add("@FileData", fileData);
//                dic.Add("@FileSize", fileSize);
//                dic.Add("@FileExtention", fileext);
//                dic.Add("@LoadDate", DateTime.Now);
//                dic.Add("@Comment", fileComment);
//                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

//                Util.AbitDB.ExecuteQuery(query, dic);
//            }
//            catch
//            {
//                return Json("Ошибка при записи файла");
//            }

//            return RedirectToAction("AddSharedFiles");
//        }

//        [HttpPost]
//        public ActionResult AddFile()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json("Ошибка авторизации");

//            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
//                return Json("Файл не приложен или пуст");

//            string fileName = Request.Files["File"].FileName;
//            int lastSlashPos = 0;
//            lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
//            if (lastSlashPos > 0)
//                fileName = fileName.Substring(lastSlashPos);
//            string fileComment = Request.Form["Comment"];
//            int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
//            byte[] fileData = new byte[fileSize];
//            //читаем данные из ПОСТа
//            Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
//            string fileext = "";
//            try
//            {
//                fileext = fileName.Substring(fileName.LastIndexOf('.'));
//            }
//            catch
//            {
//                fileext = "";
//            }

//            try
//            {
//                string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType) " +
//                    " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
//                SortedList<string, object> dic = new SortedList<string, object>();
//                dic.Add("@Id", Guid.NewGuid());
//                dic.Add("@PersonId", PersonId);
//                dic.Add("@FileName", fileName);
//                dic.Add("@FileData", fileData);
//                dic.Add("@FileSize", fileSize);
//                dic.Add("@FileExtention", fileext);
//                dic.Add("@IsReadOnly", false);
//                dic.Add("@LoadDate", DateTime.Now);
//                dic.Add("@Comment", fileComment);
//                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

//                Util.AbitDB.ExecuteQuery(query, dic);
//            }
//            catch
//            {
//                return Json("Ошибка при записи файла");
//            }

//            return RedirectToAction("AddFiles");
//        }

//        public ActionResult GetFile(string id)
//        {
//            Guid FileId = new Guid();
//            if (!Guid.TryParse(id, out FileId))
//                return Content("Некорректный идентификатор файла");

//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Content("Authorization required");

//            DataTable tbl = Util.AbitDB.GetDataTable("SELECT FileName, FileData, MimeType, FileExtention FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id",
//                new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", FileId } });

//            if (tbl.Rows.Count == 0)
//                return Content("Файл не найден");

//            string fileName = tbl.Rows[0].Field<string>("FileName");
//            string contentType = tbl.Rows[0].Field<string>("MimeType");
//            byte[] content = tbl.Rows[0].Field<byte[]>("FileData");
//            string ext = tbl.Rows[0].Field<string>("FileExtention");


//            if (string.IsNullOrEmpty(contentType))
//            {
//                if (string.IsNullOrEmpty(ext))
//                    contentType = "application/octet-stream";
//                else
//                    contentType = Util.GetMimeFromExtention(ext);
//            }
//            bool openMenu = true;
//            if (ext.IndexOf("jpg", StringComparison.OrdinalIgnoreCase) != -1)
//                openMenu = false;
//            if (ext.IndexOf("jpeg", StringComparison.OrdinalIgnoreCase) != -1)
//                openMenu = false;
//            if (ext.IndexOf("gif", StringComparison.OrdinalIgnoreCase) != -1)
//                openMenu = false;
//            if (ext.IndexOf("png", StringComparison.OrdinalIgnoreCase) != -1)
//                openMenu = false;

//            //var file = Util.ABDB.PersonFile.Where(x => x.PersonId == PersonId && x.Id == FileId).
//            //    Select(x => new { RealName = x.FileName, x.FileData }).FirstOrDefault();

//            try
//            {
//                if (openMenu)
//                    return File(content, contentType, fileName);
//                else
//                    return File(content, contentType);
//            }
//            catch
//            {
//                return Content("Ошибка при чтении файла");
//            }
//        }

//        public ActionResult GetMotivationMailPDF(string id)
//        {
//            Guid personId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
//                return Content("Authorization required");
//            string fontspath = Server.MapPath("~/Templates/times.ttf");
//            return File(PDFUtils.GetMotivateMail(id, fontspath), "application/pdf", "MotivateEdit.pdf");
//        }

//        public ActionResult FilesList(string id)
//        {
//            Guid PersonId;
//            Guid ApplicationId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Content(Resources.ServerMessages.AuthorizationRequired);
//            if (!Guid.TryParse(id, out ApplicationId))
//                return Content(Resources.ServerMessages.IncorrectGUID);
//            string fontspath = Server.MapPath("~/Templates/times.ttf");
//            return File(PDFUtils.GetFilesList(PersonId, ApplicationId, fontspath), "application/pdf", "FilesList.pdf");
//        }

//        public ActionResult MotivatePost()
//        {
//            string appId = Request.Form["AppId"];
//            string mailId = Request.Form["MailId"];
//            string mailText = Request.Form["MailText"];

//            Guid Id;
//            Guid.TryParse(mailId, out Id);
//            Guid ApplicationId;
//            Guid.TryParse(appId, out ApplicationId);

//            string query = "";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            if (Id == Guid.Empty && ApplicationId == Guid.Empty)
//                return RedirectToAction("Main");
//            else if (Id == Guid.Empty)
//            {
//                query = "INSERT INTO MotivateMail (Id, ApplicationId, MailText) VALUES (@Id, @ApplicationId, @MailText)";
//                dic.Add("@Id", Guid.NewGuid());
//                dic.AddItem("@ApplicationId", ApplicationId);
//                dic.AddItem("@MailText", mailText);
//            }
//            else
//            {
//                query = "UPDATE MotivateMail SET MailText=@MailText WHERE Id=@Id";
//                dic.Add("@Id", Guid.NewGuid());
//                dic.AddItem("@MailText", mailText);
//            }

//            Util.AbitDB.ExecuteQuery(query, dic);

//            if (ApplicationId != Guid.Empty)
//                return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", appId } });
//            else
//                return RedirectToAction("Main");
//        }

//        #region Ajax

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult GetFacs(string studyform, string studybasis, string entry)
//        {
//            int iStudyFormId;
//            int iStudyBasisId;
//            if (!int.TryParse(studyform, out iStudyFormId))
//                iStudyFormId = 1;
//            if (!int.TryParse(studybasis, out iStudyBasisId))
//                iStudyBasisId = 1;
//            int iEntryId = 1;
//            if (!int.TryParse(entry, out iEntryId))
//                iEntryId = 1;

//            string query = string.Format("SELECT DISTINCT FacultyId, FacultyName FROM {0} WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId " +
//                "AND IsSecond=@IsSecond ORDER BY FacultyId", iEntryId == 2 ? "extStudyPlan" : "extStudyPlan1K");
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@IsSecond", iEntryId == 3 ? true : false);

//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var facs =
//                from DataRow rw in tbl.Rows
//                select new { Id = rw.Field<int>("FacultyId"), Name = rw.Field<string>("FacultyName") };
//            return Json(facs);
//        }

//        public ActionResult GetLicenseProgramList(string slId)
//        {
//            Guid PersonId;
//            Util.CheckAuthCookies(Request.Cookies, out PersonId);

//            int iStudyLevelId;
//            if (!int.TryParse(slId, out iStudyLevelId))
//                iStudyLevelId = 1;

//            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM Entry WHERE StudyLevelId=@StudyLevelId ORDER BY LicenseProgramCode, LicenseProgramName";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@StudyLevelId", iStudyLevelId);

//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var profs =
//                (from DataRow rw in tbl.Rows
//                 select new
//                 {
//                     Id = rw.Field<int>("LicenseProgramId"),
//                     Name = "(" + rw.Field<string>("LicenseProgramCode") + ") " + rw.Field<string>("LicenseProgramName")
//                 }).OrderBy(x => x.Name);
//            return Json(profs);
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult GetProfs(string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
//        {
//            Guid PersonId;
//            Util.CheckAuthCookies(Request.Cookies, out PersonId);

//            int iStudyFormId;
//            int iStudyBasisId;
//            int iEntryId = 1;
//            int iSemesterId;
//            if (!int.TryParse(studyform, out iStudyFormId))
//                iStudyFormId = 1;
//            if (!int.TryParse(studybasis, out iStudyBasisId))
//                iStudyBasisId = 1;
//            if (!int.TryParse(entry, out iEntryId))
//                iEntryId = 1;
//            if (!int.TryParse(semesterId, out iSemesterId))
//                iSemesterId = 1;

//            bool bIsSecond = isSecond == "1" ? true : false;
//            bool bIsReduced = isReduced == "1" ? true : false;
//            bool bIsParallel = isParallel == "1" ? true : false;

//            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
//                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel " +
//                "AND IsReduced=@IsReduced AND [CampaignYear]=@Year AND SemesterId=@SemesterId";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);//2 == mag, 1 == 1kurs
//            dic.Add("@IsSecond", bIsSecond);
//            dic.Add("@IsParallel", bIsParallel);
//            dic.Add("@IsReduced", bIsReduced);
//            dic.Add("@Year", Util.iPriemYear);
//            dic.Add("@SemesterId", iSemesterId);

//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var profs =
//                (from DataRow rw in tbl.Rows
//                 select new
//                 {
//                     Id = rw.Field<int>("LicenseProgramId"),
//                     Name = "(" + rw.Field<string>("LicenseProgramCode") + ") " + rw.Field<string>("LicenseProgramName")
//                 }).OrderBy(x => x.Name);
//            return Json(profs);
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult GetObrazPrograms(string prof, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
//        {
//            Guid PersonId;
//            Util.CheckAuthCookies(Request.Cookies, out PersonId);

//            int iStudyFormId;
//            int iStudyBasisId;
//            if (!int.TryParse(studyform, out iStudyFormId))
//                iStudyFormId = 1;
//            if (!int.TryParse(studybasis, out iStudyBasisId))
//                iStudyBasisId = 1;
//            int iEntryId = 1;
//            if (!int.TryParse(entry, out iEntryId))
//                iEntryId = 1;
//            int iProfessionId = 1;
//            if (!int.TryParse(prof, out iProfessionId))
//                iProfessionId = 1;

//            bool bIsSecond = isSecond == "1" ? true : false;
//            bool bIsReduced = isReduced == "1" ? true : false;
//            bool bIsParallel = isParallel == "1" ? true : false;

//            string query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
//                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
//                "AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced AND DateOfClose>GETDATE() ";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@LicenseProgramId", iProfessionId);
//            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
//            dic.Add("@IsSecond", bIsSecond);
//            dic.Add("@IsParallel", bIsParallel);
//            dic.Add("@IsReduced", bIsReduced);

//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var OPs = from DataRow rw in tbl.Rows
//                      select new { Id = rw.Field<int>("ObrazProgramId"), Name = rw.Field<string>("ObrazProgramName") };

//            return Json(new { NoFree = OPs.Count() > 0 ? false : true, List = OPs });
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult GetSpecializations(string prof, string obrazprogram, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0")
//        {
//            Guid PersonId;
//            Util.CheckAuthCookies(Request.Cookies, out PersonId);

//            int iStudyFormId;
//            int iStudyBasisId;
//            if (!int.TryParse(studyform, out iStudyFormId))
//                iStudyFormId = 1;
//            if (!int.TryParse(studybasis, out iStudyBasisId))
//                iStudyBasisId = 1;
//            int iEntryId = 1;
//            if (!int.TryParse(entry, out iEntryId))
//                iEntryId = 1;
//            int iProfessionId = 1;
//            if (!int.TryParse(prof, out iProfessionId))
//                iProfessionId = 1;
//            int iObrazProgramId = 1;
//            if (!int.TryParse(obrazprogram, out iObrazProgramId))
//                iObrazProgramId = 1;

//            bool bIsSecond = isSecond == "1" ? true : false;
//            bool bIsReduced = isReduced == "1" ? true : false;
//            bool bIsParallel = isParallel == "1" ? true : false;

//            string query = "SELECT DISTINCT ProfileId, ProfileName FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = qEntry.StudyLevelId WHERE StudyFormId=@StudyFormId " +
//                "AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@StudyLevelGroupId " +
//                "AND qEntry.Id NOT IN (SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL) " +
//                "AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced  AND DateOfClose>GETDATE() ";

//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@LicenseProgramId", iProfessionId);
//            dic.Add("@ObrazProgramId", iObrazProgramId);
//            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
//            dic.Add("@IsSecond", bIsSecond);
//            dic.Add("@IsParallel", bIsParallel);
//            dic.Add("@IsReduced", bIsReduced);

//            DataTable tblSpecs = Util.AbitDB.GetDataTable(query, dic);
//            var Specs =
//                from DataRow rw in tblSpecs.Rows
//                select new { SpecId = rw.Field<Guid?>("ProfileId"), SpecName = rw.Field<string>("ProfileName") };

//            var ret = new
//            {
//                NoFree = Specs.Count() == 0 ? true : false,
//                List = Specs.Select(x => new { Id = x.SpecId, Name = x.SpecName })
//            };
//            return Json(ret);
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult GetAbitCertsAndExams()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json("", JsonRequestBehavior.AllowGet);

//            string query = "SELECT DISTINCT Number FROM EgeCertificate WHERE PersonId=@PersonId";
//            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
//            List<string> certs = (from DataRow rw in tbl.Rows
//                                  select rw.Field<string>("Number")).ToList();

//            query = "SELECT EgeExam.Id, EgeExam.Name FROM EgeCertificate INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
//                " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE EgeCertificate.PersonId=@PersonId";
//            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

//            List<KeyValuePair<int, string>> exams =
//                Util.EgeExamsAll.Except(
//                (from DataRow rw in tbl.Rows
//                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).
//                 ToDictionary(x => x.Id, y => y.Name)).ToList();

//            var result = new { Certs = certs, Exams = exams };

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult AddMark(string certNumber, string examName, string examValue)
//        {

//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(result);
//            }

//            int iExamId = 0;
//            if (!int.TryParse(examName, out iExamId))
//                iExamId = 0;
//            int iExamValue = 0;
//            if (!int.TryParse(examValue, out iExamValue))
//                iExamValue = 0;

//            string query;
//            SortedList<string, object> dic = new SortedList<string, object>();
//            Guid EgeCertificateId = Guid.Empty;
//            query = "SELECT DISTINCT Id, PersonId FROM EgeCertificate WHERE Number=@Number";
//            dic.Add("@Number", certNumber);
//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var certs = (from DataRow rw in tbl.Rows
//                         select new { Id = rw.Field<Guid>("Id"), PersonId = rw.Field<Guid>("PersonId") }).ToList();
//            //номер должен быть уникальным
//            if (certs.Count() > 1)
//                return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });//Это косяк, двух не может быть!!!
//            if (certs.Count() == 1)
//            {
//                if (certs[0].PersonId != PersonId)
//                    return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });
//                else
//                    EgeCertificateId = certs[0].Id;
//            }

//            query = "SELECT EgeMark.Value FROM EgeMark INNER JOIN EgeCertificate ON EgeCertificate.Id=EgeMark.EgeCertificateId " +
//                " WHERE EgeCertificate.PersonId=@PersonId AND EgeMark.EgeExamId=@ExamId";
//            dic.Clear();
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@ExamId", iExamId);
//            string MarkVal = Util.AbitDB.GetStringValue(query, dic);

//            if (!string.IsNullOrEmpty(MarkVal))
//                return Json(new { IsOk = false, ErrorMessage = "Оценка по данному предмету уже введена" });

//            try
//            {
//                if (EgeCertificateId == Guid.Empty)
//                {
//                    EgeCertificateId = Guid.NewGuid();
//                    query = "INSERT INTO EgeCertificate (Id, PersonId, Number) VALUES (@CertId, @PersonId, @Number)";
//                    dic.Clear();
//                    dic.Add("@CertId", EgeCertificateId);
//                    dic.Add("@PersonId", PersonId);
//                    dic.Add("@Number", certNumber);
//                    Util.AbitDB.ExecuteQuery(query, dic);
//                }
//                Guid MarkId = Guid.NewGuid();
//                query = "INSERT INTO EgeMark (Id, EgeCertificateId, EgeExamId, [Value]) VALUES (@Id, @CertId, @ExamId, @Value)";
//                dic.Clear();
//                dic.Add("@Id", MarkId);
//                dic.Add("@CertId", EgeCertificateId);
//                dic.Add("@ExamId", iExamId);
//                dic.Add("@Value", iExamValue);
//                Util.AbitDB.ExecuteQuery(query, dic);
//                string exName = Util.EgeExamsAll[iExamId] == null ? "" : Util.EgeExamsAll[iExamId];
//                var res = new
//                {
//                    IsOk = true,
//                    Data = new
//                    {
//                        Id = MarkId.ToString(),
//                        CertificateNumber = certNumber,
//                        ExamName = exName,
//                        ExamMark = iExamValue.ToString()
//                    },
//                    ErrorMessage = ""
//                };
//                return Json(res);
//            }
//            catch
//            {
//                return Json(new { IsOk = false, ErrorMessage = "Ошибка при сохранении оценки." });
//            }
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult DeleteEgeMark()
//        {
//            string mId = Request.Params["mId"];
//            Guid id;
//            if (!Util.CheckAuthCookies(Request.Cookies, out id))
//            {
//                var result = new { IsOk = false, ErrorMsg = "Ошибка авторизации" };
//                return Json(result);
//            }

//            Guid markId;
//            if (!Guid.TryParse(mId, out markId))
//            {
//                var result = new { IsOk = false, ErrorMsg = "Некорректный идентификатор" };
//                return Json(result);
//            }

//            try
//            {
//                Util.AbitDB.ExecuteQuery("DELETE FROM EgeMark WHERE Id=@Id", new SortedList<string, object>() { { "@Id", markId } });
//                var res = new { IsOk = true, ErrorMsg = "" };
//                return Json(res);
//            }
//            catch
//            {
//                var result = new { IsOk = false, ErrorMsg = "Ошибка при обновлении" };
//                return Json(result);
//            }

//            //using (AbitDB db = new AbitDB())
//            //{

//            //    EgeMark Mark = db.EgeMark.Where(x => x.Id == markId).DefaultIfEmpty(null).First();

//            //    if (Mark != null)
//            //        db.EgeMark.DeleteObject(Mark);

//            //    try
//            //    {
//            //        db.SaveChanges(System.Data.Objects.SaveOptions.None);
//            //    }
//            //    catch
//            //    {
//            //        var result = new { IsOk = false, ErrorMsg = "Ошибка при обновлении" };
//            //        return Json(result);
//            //    }

//            //    var res = new { IsOk = true, ErrorMsg = "" };
//            //    return Json(res);
//            //}
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult UpdateScienceWorks(string ScWorkInfo, string ScWorkType)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMsg = "Ошибка авторизации" };
//                return Json(result);
//            }

//            int iScWorkType = 1;
//            if (!(int.TryParse(ScWorkType, out iScWorkType)))
//                iScWorkType = 1;

//            Guid wrkId = Guid.NewGuid();

//            string query = "INSERT INTO PersonScienceWork (Id, PersonId, WorkTypeId, WorkInfo) VALUES (@Id, @PersonId, @WorkTypeId, @WorkInfo)";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@Id", wrkId);
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@WorkTypeId", iScWorkType);
//            dic.Add("@WorkInfo", ScWorkInfo);
//            try
//            {
//                Util.AbitDB.ExecuteQuery(query, dic);
//                string scType = Util.ScienceWorkTypeAll[iScWorkType];
//                string scInfo = HttpUtility.HtmlEncode(ScWorkInfo);
//                var res = new { IsOk = true, Data = new { Id = wrkId.ToString("N"), Type = scType, Info = scInfo }, ErrorMsg = "" };
//                return Json(res);
//            }
//            catch
//            {
//                var result = new { IsOk = false, ErrorMsg = "Ошибка при сохранении данных" };
//                return Json(result);
//            }
//            //using (AbitDB db = new AbitDB())
//            //{
//            //    PersonScienceWork psw = new PersonScienceWork()
//            //    {
//            //        Id = Guid.NewGuid(),
//            //        PersonId = PersonId,
//            //        WorkTypeId = iScWorkType,
//            //        WorkInfo = ScWorkInfo
//            //    };

//            //    try
//            //    {
//            //        db.PersonScienceWork.AddObject(psw);
//            //        db.SaveChanges(System.Data.Objects.SaveOptions.None);
//            //    }
//            //    catch
//            //    {
//            //        var result = new { IsOk = false, ErrorMsg = "Ошибка при сохранении данных" };
//            //        return Json(result);
//            //    }
//            //    string scType = Util.ScienceWorkTypeAll[iScWorkType];
//            //    string scInfo = HttpUtility.HtmlEncode(ScWorkInfo);
//            //    var res = new { IsOk = true, Data = new { Id = psw.Id, Type = scType, Info = scInfo }, ErrorMsg = "" };
//            //    return Json(res);
//            //}
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public ActionResult DeleteScienceWorks(string id)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(result);
//            }

//            Guid wrkId = Guid.Empty;
//            if (!Guid.TryParse(id, out wrkId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Некорректный идентификатор" };
//                return Json(result);
//            }
//            try
//            {
//                Util.AbitDB.ExecuteQuery("DELETE FROM PersonScienceWork WHERE Id=@Id", new SortedList<string, object>() { { "@Id", wrkId } });
//                var res = new { IsOk = true, ErrorMessage = "" };
//                return Json(res);
//            }
//            catch
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении данных" };
//                return Json(result);
//            }
//            //using (AbitDB db = new AbitDB())
//            //{
//            //    PersonScienceWork psw = 
//            //        db.PersonScienceWork.Where(x => x.Id == wrkId && x.PersonId == PersonId).DefaultIfEmpty(null).First();

//            //    if (psw == null)
//            //    {
//            //        var result = new { IsOk = false, ErrorMessage = "Запись не найдена" };
//            //        return Json(result);
//            //    }

//            //    try
//            //    {
//            //        db.PersonScienceWork.DeleteObject(psw);
//            //        db.SaveChanges();
//            //    }
//            //    catch
//            //    {
//            //        var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении данных" };
//            //        return Json(result);
//            //    }

//            //    var res = new { IsOk = true, ErrorMessage = "" };
//            //    return Json(res);
//            //}
//        }

//        public JsonResult LoadVuzNames(string schoolType)
//        {
//            int iSchoolType;
//            int.TryParse(schoolType, out iSchoolType);
//            string query = @"SELECT SchoolName, count(SchoolName) as cnt 
//FROM EducationDocument 
//WHERE SchoolName IS NOT NULL AND SchoolTypeId=@SchTypeId 
//group by SchoolName
//Order by cnt desc";
//            DataTable tbl = Util.StudDB.GetDataTable(query, new SortedList<string, object>() { { "@SchTypeId", iSchoolType } });
//            List<string> vals =
//                (from DataRow rw in tbl.Rows
//                 select rw.Field<string>("SchoolName")).ToList();
//            return Json(new { IsOk = true, Values = vals });
//        }

//        public ActionResult AddWorkPlace(string WorkStag, string WorkPlace, string WorkProf, string WorkSpec)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(result);
//            }

//            Guid workId = Guid.NewGuid();
//            string query = "INSERT INTO PersonWork(Id, PersonId, Stage, WorkPlace, WorkProfession, WorkSpecifications) " +
//                " VALUES (@Id, @PersonId, @Stage, @WorkPlace, @WorkProfession, @WorkSpecifications)";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@Id", workId);
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@Stage", WorkStag);
//            dic.Add("@WorkPlace", WorkPlace);
//            dic.Add("@WorkProfession", WorkProf);
//            dic.Add("@WorkSpecifications", WorkSpec);

//            try
//            {
//                Util.AbitDB.ExecuteQuery(query, dic);
//                var res = new
//                {
//                    IsOk = true,
//                    Data = new { Id = workId.ToString("N"), Place = WorkPlace, Stag = WorkStag, Level = WorkProf, Duties = WorkSpec },
//                    ErrorMessage = ""
//                };
//                return Json(res);
//            }
//            catch
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка при сохранении данных" };
//                return Json(result);
//            }
//            //using (AbitDB db = new AbitDB())
//            //{
//            //    PersonWork pw = new PersonWork()
//            //    {
//            //        Id = Guid.NewGuid(),
//            //        PersonId = PersonId,
//            //        Stage = WorkStag,
//            //        WorkPlace = WorkPlace,
//            //        WorkProfession = WorkProf,
//            //        WorkSpecifications = WorkSpec
//            //    };

//            //    try
//            //    {
//            //        Util.ABDB.PersonWork.AddObject(pw);
//            //        Util.ABDB.SaveChanges();
//            //    }
//            //    catch
//            //    {
//            //        var result = new { IsOk = false, ErrorMessage = "Ошибка при сохранении данных" };
//            //        return Json(result);
//            //    }

//            //    var res = new 
//            //    { 
//            //        IsOk = true,
//            //        Data = new { Id = pw.Id, Place = pw.WorkPlace, Stag = pw.Stage, Level = pw.WorkProfession, Duties = pw.WorkSpecifications },
//            //        ErrorMessage = ""
//            //    };
//            //    return Json(res);
//            //}
//        }

//        public ActionResult DeleteWorkPlace(string wrkId)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(result);
//            }

//            Guid workId = new Guid();
//            if (!Guid.TryParse(wrkId, out workId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Некорректный идентификатор" };
//                return Json(result);
//            }

//            try
//            {
//                Util.AbitDB.ExecuteQuery("DELETE FROM PersonWork WHERE Id=@Id", new SortedList<string, object>() { { "@Id", workId } });
//                var res = new { IsOk = true, ErrorMessage = "" };
//                return Json(res);
//            }
//            catch
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении" };
//                return Json(result);
//            }

//            //try
//            //{
//            //    PersonWork pw = Util.ABDB.PersonWork.Where(x => x.Id == workId && x.PersonId == PersonId).DefaultIfEmpty(null).First();
//            //    if (pw == null)
//            //    {
//            //        var result = new { IsOk = false, ErrorMessage = "Запись не найдена" };
//            //        return Json(result);
//            //    }
//            //    Util.ABDB.PersonWork.DeleteObject(pw);
//            //    Util.ABDB.SaveChanges();
//            //    var res = new { IsOk = true, ErrorMessage = "" };
//            //    return Json(res);
//            //}
//            //catch
//            //{
//            //    var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении" };
//            //    return Json(result);
//            //}
//        }

//        /*
//        public ActionResult GetFileList()
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(result);
//            }

//            var files = Util.ABDB.PersonFile.Where(x => x.PersonId == PersonId).
//                Select(x => new { Id = x.Id, ServerFileName = x.ServerFileName, FileExtention = x.FileExtention, FileName = x.FileName, FileSize = x.FileSize });
//            var SFI = files.
//                Select(x => new
//                {
//                    Id = x.Id,
//                    Path = "../../Content/Files/" + x.ServerFileName + x.FileExtention,
//                    FileName = x.FileName,
//                    FileSize = x.FileSize.HasValue ? x.FileSize.Value : -1
//                }).ToList();
//            var res = new { IsOk = true, Data = SFI };
//            return Json(res);
//        }
//        */

//        public ActionResult SendMotivationMail(string info, string appId)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies((Request.Cookies), out PersonId))
//                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });

//            Guid applicationId;
//            if (!Guid.TryParse(appId, out applicationId))
//                return Json(new { IsOk = false, ErrorMessage = "Некорректный идентификатор заявления" });

//            string query = "SELECT Id FROM MotivationMail WHERE ApplicationId=@AppId";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("AppId", applicationId);
//            Guid? outMailId = (Guid?)Util.AbitDB.GetValue(query, dic);

//            try
//            {
//                dic.Clear();
//                Guid mailId = Guid.NewGuid();
//                if (outMailId.HasValue && outMailId.Value != Guid.Empty)
//                {
//                    query = "UPDATE MotivationMail SET MailText=@MailText WHERE Id=@Id";
//                    dic.Add("@MailText", info);
//                    dic.Add("@Id", outMailId.Value);
//                }
//                else
//                {
//                    query = "INSERT INTO MotivationMail(Id, ApplicationId, MailText) VALUES (@Id, @ApplicationId, @MailText)";
//                    dic.Add("@Id", mailId);
//                    dic.Add("@ApplicationId", applicationId);
//                    dic.Add("@MailText", info);
//                    outMailId = mailId;
//                }
//                Util.AbitDB.ExecuteQuery(query, dic);
//                return Json(new { IsOk = true, Id = outMailId.Value.ToString("N") });
//            }
//            catch
//            {
//                return Json(new { IsOk = false, ErrorMessage = "Ошибка при сохранении. Пожалуйста, повторите" });
//            }
//            //MotivationMail ml;
//            //if (Util.ABDB.MotivationMail.Where(x => x.ApplicationId == applicationId).Count() > 0)
//            //{
//            //    ml = Util.ABDB.MotivationMail.Where(x => x.ApplicationId == applicationId).First();
//            //    ml.MailText = info;
//            //}
//            //else
//            //{
//            //    ml = new MotivationMail()
//            //    {
//            //        Id = mailId,
//            //        ApplicationId = applicationId,
//            //        MailText = info
//            //    };
//            //    Util.ABDB.MotivationMail.AddObject(ml);
//            //}
//            //Util.ABDB.SaveChanges();
//        }

//        public ActionResult GetMotivationMail(string appId)
//        {
//            Guid personId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
//                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });

//            Guid ApplicationId;
//            if (!Guid.TryParse(appId, out ApplicationId))
//                return Json(new { IsOk = false, ErrorMessage = "Некорректный идентификатор заявления" });

//            DataTable tbl = Util.AbitDB.GetDataTable("SELECT Id, MailText FROM MotivationMail WHERE ApplicationId=@Id",
//                new SortedList<string, object>() { { "@Id", ApplicationId } });

//            if (tbl.Rows.Count == 0)
//                return Json(new { IsOk = false, Text = "" });
//            else
//                return Json(new
//                {
//                    IsOk = true,
//                    Text = tbl.Rows[0].Field<string>("MailText"),
//                    Id = tbl.Rows[0].Field<Guid>("Id").ToString("N")
//                });

//            //var apps = Util.ABDB.MotivationMail.Where(x => x.ApplicationId == ApplicationId).Select(x => new { x.Id, x.MailText }).AsEnumerable();
//            //if (apps.Count() == 0)
//            //    return Json(new { IsOk = false, Text = "" });
//            //else
//            //    return Json(new { IsOk = true, Text = apps.First().MailText, Id = apps.First().Id });
//        }

//        [HttpPost]
//        public ActionResult DeleteFile(string id)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(res);
//            }

//            Guid fileId;
//            if (!Guid.TryParse(id, out fileId))
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
//                return Json(res);
//            }
//            string attr = Util.AbitDB.GetStringValue("SELECT IsReadOnly FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
//            if (string.IsNullOrEmpty(attr))
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
//                return Json(res);
//            }
//            if (attr == "True")
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
//                return Json(res);
//            }
//            try
//            {
//                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
//            }
//            catch
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
//                return Json(res);
//            }

//            var result = new { IsOk = true, ErrorMessage = "" };
//            return Json(result);
//        }

//        public JsonResult DeleteSharedFile(string id)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//            {
//                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
//                return Json(res);
//            }

//            Guid fileId;
//            if (!Guid.TryParse(id, out fileId))
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
//                return Json(res);
//            }
//            string attr = Util.AbitDB.GetStringValue("SELECT ISNULL([IsReadOnly], 'False') FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
//            if (string.IsNullOrEmpty(attr))
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
//                return Json(res);
//            }
//            if (attr == "True")
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
//                return Json(res);
//            }
//            try
//            {
//                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
//            }
//            catch
//            {
//                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
//                return Json(res);
//            }

//            var result = new { IsOk = true, ErrorMessage = "" };
//            return Json(result);
//        }

//        public ActionResult DeleteMsg(string id)
//        {
//            if (id == "0")//system messages
//                return Json(new { IsOk = true });

//            Guid MessageId;
//            if (!Guid.TryParse(id, out MessageId))
//                return Json(new { IsOk = false, ErrorMessage = "" });

//            string query = "UPDATE PersonalMessage SET IsRead=@IsRead WHERE Id=@Id";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@IsRead", true);
//            dic.Add("@Id", MessageId);

//            try
//            {
//                Util.AbitDB.ExecuteQuery(query, dic);
//            }
//            catch (Exception e)
//            {
//                return Json(new { IsOk = false, ErrorMessage = e.Message });//
//            }

//            return Json(new { IsOk = true });
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public JsonResult GetStudyForms(string obrazProgramId)
//        {
//            int iObrazProgramId = 0;
//            int.TryParse(obrazProgramId, out iObrazProgramId);
//            string query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry WHERE ObrazProgramId=@ObrazProgramId ORDER BY 1";
//            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@ObrazProgramId", iObrazProgramId } });
//            var Data = (from DataRow rw in tbl.Rows
//                        select new
//                        {
//                            Value = rw.Field<int>("StudyFormId").ToString(),
//                            Name = rw.Field<string>("StudyFormName")
//                        });
//            return Json(new { Data = Data });
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public JsonResult GetStudyBasises(string obrazProgramId, string studyFormId)
//        {
//            int iObrazProgramId = 0;
//            int iStudyFormId = 0;
//            int.TryParse(obrazProgramId, out iObrazProgramId);
//            int.TryParse(studyFormId, out iStudyFormId);
//            string query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry WHERE ObrazProgramId=@ObrazProgramId " +
//                " AND StudyFormId=@StudyFormId ORDER BY 1";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.AddItem("@ObrazProgramId", iObrazProgramId);
//            dic.AddItem("@StudyFormId", iStudyFormId);
//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var Data = (from DataRow rw in tbl.Rows
//                        select new
//                        {
//                            Value = rw.Field<int>("StudyBasisId").ToString(),
//                            Name = rw.Field<string>("StudyBasisName")
//                        });
//            return Json(new { Data = Data });
//        }

//        [OutputCache(NoStore = true, Duration = 0)]
//        public JsonResult GetProfessions(string obrazProgramId, string studyFormId, string studyBasisId)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json(new { IsOk = false });

//            int iObrazProgramId;
//            int iStudyFormId;
//            int iStudyBasisId;
//            int.TryParse(obrazProgramId, out iObrazProgramId);
//            int.TryParse(studyFormId, out iStudyFormId);
//            int.TryParse(studyBasisId, out iStudyBasisId);

//            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramName FROM Entry WHERE ObrazProgramId=@ObrazProgramId AND StudyFormId=@StudyFormId " +
//                " AND StudyBasisId=@StudyBasisId AND CampaignYear=@CampaignYear AND SemesterId=1 ORDER BY LicenseProgramName";
//            SortedList<string, object> dic = new SortedList<string, object>();
//            dic.Add("@PersonId", PersonId);
//            dic.Add("@ObrazProgramId", iObrazProgramId);
//            dic.Add("@StudyFormId", iStudyFormId);
//            dic.Add("@StudyBasisId", iStudyBasisId);
//            dic.Add("@CampaignYear", DateTime.Now.Year);
//            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
//            var res = (from DataRow rw in tbl.Rows
//                       select new
//                       {
//                           Value = rw.Field<int>("LicenseProgramId"),
//                           Name = rw.Field<string>("LicenseProgramName")
//                       });
//            return Json(new { IsOk = true, Data = res });
//        }
//        public JsonResult CheckProfession(string obrazProgramId, string professionId, string studyFormId, string studyBasisId)
//        {
//            Guid PersonId;
//            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

//            //string query = "SELECT COUNT(Id) FROM [Application] WHERE PersonId=@PersonId AND Enabled='True'";
//            //int cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

//            int iObrazProgramId;
//            int iProfessionId;
//            int iStudyFormId;
//            int iStudyBasisId;
//            if (!int.TryParse(obrazProgramId, out iObrazProgramId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
//            if (!int.TryParse(professionId, out iProfessionId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
//            if (!int.TryParse(studyFormId, out iStudyFormId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
//            if (!int.TryParse(studyBasisId, out iStudyBasisId))
//                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

//            string query = "SELECT Id FROM Entry WHERE ObrazProgramId=@ObrazProgramId AND LicenseProgramId=@LicenseProgramId AND StudyFormId=@StudyFormId AND " +
//                " StudyBasisId=@StudyBasisId AND Id NOT IN (SELECT StudyPlanId FROM [Application] WHERE PersonId=@PersonId AND Enabled=@Enabled)";
//            //string query = "SELECT Id FROM [Application] WHERE PersonId=@PersonId AND Enabled=@Enabled";


//            DataTable tbl = Util.AbitDB.GetDataTable(query,
//                new SortedList<string, object>() 
//                { 
//                    { "@ObrazProgramId", iObrazProgramId },
//                    { "@LicenseProgramId", iProfessionId },
//                    { "@StudyFormId", iStudyFormId },
//                    { "@StudyBasisId", iStudyBasisId },
//                    { "@PersonId", PersonId },
//                    { "@Enabled", true }
//                });
//            if (tbl.Rows.Count != 0)
//                return Json(new { IsOk = true });
//            else
//                return Json(new { IsOk = false, ErrorMessage = "У абитуриента уже есть поданное заявление по заданным параметрам." });
//        }

//        #endregion
//    }
}
