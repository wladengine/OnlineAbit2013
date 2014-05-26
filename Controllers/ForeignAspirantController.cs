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

using System.Net.Mail;

namespace OnlineAbit2013.Controllers
{
    public class ForeignAspirantController : Controller
    {
        //
        // GET: /ForeignAspirant/

        public ActionResult Index(string step)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (Util.CheckIsNew(PersonId))
                return RedirectToAction("OpenPersonalAccount");

            int AbitType = Util.CheckAbitType(PersonId);
            switch (AbitType)
            {
                case 1: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
                case 2: { return RedirectToAction("Index", "ForeignAbiturient", new RouteValueDictionary() { { "step", step }}); }
                case 3: { return RedirectToAction("Index", "Transfer", new RouteValueDictionary() { { "step", step }}); }
                case 4: { return RedirectToAction("Index", "TransferForeign", new RouteValueDictionary() { { "step", step }}); }
                case 5: { return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", step }}); }
                case 6: { return RedirectToAction("Index", "ChangeStudyForm", new RouteValueDictionary() { { "step", step }}); }
                case 7: { return RedirectToAction("Index", "ChangeObrazProgram", new RouteValueDictionary() { { "step", step }}); ; }
                case 8: { return RedirectToAction("Index", "AG", new RouteValueDictionary() { { "step", step }}); ; }
                case 9: { return RedirectToAction("Index", "SPO", new RouteValueDictionary() { { "step", step }}); ; }
                case 10: { return RedirectToAction("Index", "Aspirant", new RouteValueDictionary() { { "step", step }});  }
                case 11: { break; }
                default: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
            }

            int stage = 0;
            if (!int.TryParse(step, out stage))
                stage = 1;

            bool eng = false;
            if (Util.GetCurrentThreadLanguage() == "en")
                eng = true;

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                string query;
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (Person == null)//paranoia
                    return RedirectToAction("OpenPersonalAccount");

                if (Person.RegistrationStage == 0)
                    stage = 1;
                else if (Person.RegistrationStage < stage)
                    stage = Person.RegistrationStage;

                PersonalOfficeForeign model = new PersonalOfficeForeign() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
                if (model.Stage == 1)
                {
                    model.PersonInfo = new InfoPerson();

                    model.PersonInfo.Surname = Server.HtmlDecode(Person.Surname);
                    model.PersonInfo.Name = Server.HtmlDecode(Person.Name);
                    model.PersonInfo.SecondName = Server.HtmlDecode(Person.SecondName);
                    model.PersonInfo.Sex = (Person.Sex ?? false) ? "M" : "F";
                    model.PersonInfo.Nationality = Person.NationalityId.ToString();
                    model.PersonInfo.BirthPlace = Server.HtmlDecode(Person.BirthPlace);
                    model.PersonInfo.BirthDate = Person.BirthDate.HasValue ? Person.BirthDate.Value.ToString("dd.MM.yyyy") : "";

                    model.PersonInfo.NationalityList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.PersonInfo.SexList = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = LangPack.GetValue(5, model.Lang), Value = "M" }, 
                        new SelectListItem() { Text = LangPack.GetValue(6, model.Lang), Value = "F" }
                    };
                }
                else if (model.Stage == 2)
                {
                    model.PassportInfo = new PassportPerson();
                    DataTable tblPsp = Util.AbitDB.GetDataTable("SELECT Id, Name, NameEng FROM PassportType WHERE IsApprovedForeign=1", null);
                    model.PassportInfo.PassportTypeList =
                        (from DataRow rw in tblPsp.Rows
                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>(eng ? "NameEng" : "Name") }).
                        ToList();

                    model.PassportInfo.PassportType = (Person.PassportTypeId ?? 2).ToString();//Default Загран. паспорт
                    model.PassportInfo.PassportSeries = Server.HtmlDecode(Person.PassportSeries);
                    model.PassportInfo.PassportNumber = Server.HtmlDecode(Person.PassportNumber);
                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(Person.PassportAuthor);
                    model.PassportInfo.PassportDate = Person.PassportDate.HasValue ? Person.PassportDate.Value.ToString("dd.MM.yyyy") : "";
                    model.PassportInfo.PassportCode = Server.HtmlDecode(Person.PassportCode);
                    model.PassportInfo.PassportValid = Person.PassportValid.HasValue ? Person.PassportValid.Value.ToString("dd.MM.yyyy") : "";

                    model.VisaInfo = new VisaInfo();
                    DataTable tblCountr =
                        Util.AbitDB.GetDataTable(
                            string.Format("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, {0}", eng ? "NameEng" : "Name"),
                            null);
                    model.VisaInfo.CountryList = (from DataRow rw in tblCountr.Rows
                                                  select new SelectListItem()
                                                  {
                                                      Value = rw.Field<int>("Id").ToString(),
                                                      Text = rw.Field<string>(eng ? "NameEng" : "Name")
                                                  }).ToList();

                    var PersonVisaInfo = Person.PersonVisaInfo;
                    if (PersonVisaInfo == null)
                        PersonVisaInfo = new PersonVisaInfo();
                    model.VisaInfo.CountryId = PersonVisaInfo.CountryId.ToString();
                    model.VisaInfo.PostAddress = PersonVisaInfo.PostAddress;
                    model.VisaInfo.Town = PersonVisaInfo.Town;
                }
                else if (model.Stage == 3)
                {
                    model.ContactsInfo = new ContactsForeignPerson();
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                        PersonContacts = new PersonContacts();

                    model.ContactsInfo.MainPhone = Server.HtmlDecode(PersonContacts.Phone);
                    model.ContactsInfo.SecondPhone = Server.HtmlDecode(PersonContacts.Mobiles);
                    model.ContactsInfo.CountryId = PersonContacts.CountryId.ToString();

                    model.ContactsInfo.PostIndex = Server.HtmlDecode(PersonContacts.Code);
                    model.ContactsInfo.City = Server.HtmlDecode(PersonContacts.City);
                    model.ContactsInfo.Street = Server.HtmlDecode(PersonContacts.Street);
                    model.ContactsInfo.House = Server.HtmlDecode(PersonContacts.House);
                    model.ContactsInfo.Korpus = Server.HtmlDecode(PersonContacts.Korpus);
                    model.ContactsInfo.Flat = Server.HtmlDecode(PersonContacts.Flat);

                    model.ContactsInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                }
                else if (model.Stage == 4)
                {
                    model.EducationInfo = new EducationPerson();
                    var PersonEducationDocument = Person.PersonEducationDocument;
                    var PersonHighEducationInfo = Person.PersonHighEducationInfo;

                    if (PersonEducationDocument == null)
                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
                    if (PersonHighEducationInfo == null)
                        PersonHighEducationInfo = new OnlineAbit2013.PersonHighEducationInfo();

                    model.EducationInfo.QualificationList = Util.QualifaicationForAspirant.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.SchoolTypeList = Util.SchoolTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.StudyFormList = Util.StudyFormAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.LanguageList = Util.LanguagesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

                    query = "SELECT Id, Name FROM SchoolTypeAspirant";
                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.EducationInfo.SchoolTypeList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    model.EducationInfo.SchoolName = Server.HtmlDecode(PersonEducationDocument.SchoolName);
                    model.EducationInfo.SchoolExitYear = Server.HtmlDecode(PersonEducationDocument.SchoolExitYear);
                    model.EducationInfo.SchoolCity = Server.HtmlDecode(PersonEducationDocument.SchoolCity);

                    model.EducationInfo.AvgMark = PersonEducationDocument.AvgMark.HasValue ? PersonEducationDocument.AvgMark.Value.ToString() : "";
                    model.EducationInfo.IsExcellent = PersonEducationDocument.IsExcellent ?? false;
                    model.EducationInfo.EnglishMark = PersonEducationDocument.EnglishMark.ToString();

                    model.EducationInfo.HEExitYear = Server.HtmlDecode(PersonHighEducationInfo.ExitYear.ToString());
                    model.EducationInfo.HEEntryYear = Server.HtmlDecode(PersonHighEducationInfo.EntryYear.ToString());

                    model.EducationInfo.DiplomSeries = Server.HtmlDecode(PersonEducationDocument.Series);
                    model.EducationInfo.DiplomNumber = Server.HtmlDecode(PersonEducationDocument.Number);
                    model.EducationInfo.ProgramName = Server.HtmlDecode(PersonHighEducationInfo.ProgramName);
                    model.EducationInfo.DiplomTheme = Server.HtmlDecode(PersonHighEducationInfo.DiplomaTheme);

                    model.EducationInfo.SchoolTypeId = (PersonEducationDocument.SchoolTypeId ?? 6).ToString();
                    model.EducationInfo.PersonStudyForm = (PersonHighEducationInfo.StudyFormId ?? 1).ToString();
                    model.EducationInfo.PersonQualification = (PersonHighEducationInfo.QualificationId ?? 1).ToString();
                    model.EducationInfo.LanguageId = (PersonEducationDocument.LanguageId ?? 1).ToString();
                    model.EducationInfo.CountryEducId = (PersonEducationDocument.CountryEducId ?? 193).ToString();
                }
                else if (model.Stage == 5)
                {
                    model.WorkInfo = new WorkPerson();

                    query = "SELECT Id, Name FROM ScienceWorkType";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, null);

                    model.WorkInfo.ScWorks =
                        (from DataRow rw in tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();
                    //Util.ScienceWorkTypeAll.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key.ToString() }).ToList();

                    string qPSW = "SELECT PersonScienceWork.Id, ScienceWorkType.Name, PersonScienceWork.WorkInfo FROM PersonScienceWork " +
                        " INNER JOIN ScienceWorkType ON ScienceWorkType.Id=PersonScienceWork.WorkTypeId WHERE PersonScienceWork.PersonId=@Id";
                    DataTable tblPSW = Util.AbitDB.GetDataTable(qPSW, new SortedList<string, object>() { { "@Id", PersonId } });

                    model.WorkInfo.pScWorks =
                        (from DataRow rw in tblPSW.Rows
                         select new ScienceWorkInformation()
                         {
                             Id = rw.Field<Guid>("Id"),
                             ScienceWorkType = rw.Field<string>("Name"),
                             ScienceWorkInfo = rw.Field<string>("WorkInfo")
                         }).ToList();

                    string qPW = "SELECT Id, WorkPlace, Stage, WorkProfession, WorkSpecifications FROM PersonWork WHERE PersonId=@Id";
                    DataTable tblPW = Util.AbitDB.GetDataTable(qPW, new SortedList<string, object>() { { "@Id", PersonId } });

                    model.WorkInfo.pWorks =
                        (from DataRow rw in tblPW.Rows
                         select new WorkInformationModel()
                         {
                             Id = rw.Field<Guid>("Id"),
                             Place = rw.Field<string>("WorkPlace"),
                             Stag = rw.Field<string>("Stage"),
                             Level = rw.Field<string>("WorkProfession"),
                             Duties = rw.Field<string>("WorkSpecifications")
                         }).ToList();
                }
                else if (model.Stage == 6)
                {
                    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                        return RedirectToAction("LogOn", "Account");

                    var AddInfo = Person.PersonAddInfo;
                    if (AddInfo == null)
                        AddInfo = new PersonAddInfo();

                    model.AddInfo = new AdditionalInfoPerson()
                    {
                        FZ_152Agree = false,
                        ExtraInfo = Server.HtmlDecode(AddInfo.AddInfo),
                        HasPrivileges = AddInfo.HasPrivileges ?? false,
                        HostelAbit = AddInfo.HostelAbit ?? false,
                        ContactPerson = Server.HtmlDecode(AddInfo.Parents)
                    };
                }
                return View("PersonalOffice", model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NextStep(PersonalOfficeForeign model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                int iRegStage = Person.RegistrationStage;

                if (iRegStage == 0)
                    iRegStage = 1;

                if (Util.CheckPersonReadOnlyStatus(PersonId))
                {
                    if (++(model.Stage) <= 6)
                        return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", model.Stage } });
                    else
                        return RedirectToAction("Main", "Abiturient");
                }

                if (model.Stage == 1)
                {
                    DateTime bdate;
                    if (!DateTime.TryParse(model.PersonInfo.BirthDate, CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat, DateTimeStyles.None, out bdate))
                        bdate = DateTime.Now.Date;

                    if (bdate.Date > DateTime.Now.Date)
                        bdate = DateTime.Now.Date;

                    int NationalityId = 193;
                    if (!int.TryParse(model.PersonInfo.Nationality, out NationalityId))
                        NationalityId = 193;

                    Person.Surname = model.PersonInfo.Surname;
                    Person.Name = model.PersonInfo.Name;
                    Person.SecondName = model.PersonInfo.SecondName;
                    Person.BirthDate = bdate;
                    Person.BirthPlace = model.PersonInfo.BirthPlace;
                    Person.NationalityId = NationalityId;
                    Person.Sex = model.PersonInfo.Sex == "M" ? true : false;
                    Person.RegistrationStage = iRegStage < 2 ? 2 : iRegStage;

                    if (model.PersonInfo.IsEqualWithRussian)
                        Person.AbiturientTypeId = 10;

                    context.SaveChanges();
                }
                else if (model.Stage == 2)
                {
                    int iPassportType = 1;
                    if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
                        iPassportType = 1;

                    int iVisaCountryId;
                    ;

                    DateTime dtPassportDate, dtPassportValid;
                    try
                    {
                        dtPassportDate = Convert.ToDateTime(model.PassportInfo.PassportDate,
                            System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                    }
                    catch { dtPassportDate = DateTime.Now; }
                    try
                    {
                        dtPassportValid = Convert.ToDateTime(model.PassportInfo.PassportValid,
                            System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                    }
                    catch { dtPassportValid = DateTime.MinValue; }

                    if (dtPassportDate.Date > DateTime.Now.Date)
                        dtPassportDate = DateTime.Now.Date;

                    Person.PassportTypeId = iPassportType;
                    Person.PassportSeries = model.PassportInfo.PassportSeries;
                    Person.PassportNumber = model.PassportInfo.PassportNumber;
                    Person.PassportAuthor = model.PassportInfo.PassportAuthor;
                    Person.PassportDate = dtPassportDate;
                    Person.PassportCode = model.PassportInfo.PassportCode;
                    Person.PassportValid = dtPassportValid == DateTime.MinValue ? null : (DateTime?)dtPassportValid;

                    //visa
                    int.TryParse(model.VisaInfo.CountryId, out iVisaCountryId);
                    bool bIns = false;
                    var PersonVisaInfo = Person.PersonVisaInfo;
                    if (PersonVisaInfo == null)
                    {
                        PersonVisaInfo = new OnlineAbit2013.PersonVisaInfo();
                        PersonVisaInfo.PersonId = PersonId;
                        bIns = true;
                    }

                    if (iVisaCountryId > 0)//not null or something wrong
                    {
                        PersonVisaInfo.CountryId = iVisaCountryId;
                        PersonVisaInfo.PostAddress = model.VisaInfo.PostAddress;
                        PersonVisaInfo.Town = model.VisaInfo.Town;
                    }

                    if (bIns)
                        context.PersonVisaInfo.AddObject(PersonVisaInfo);

                    Person.RegistrationStage = iRegStage < 3 ? 3 : iRegStage;

                    context.SaveChanges();
                }
                else if (model.Stage == 3)
                {
                    int iCountryId = 0;
                    if (!int.TryParse(model.ContactsInfo.CountryId, out iCountryId))
                        iCountryId = 193;//Russia

                    int iRegionId = 0;
                    int? altRegionId = context.Country.Where(x => x.Id == iCountryId).Select(x => x.RegionId).FirstOrDefault();
                    if (altRegionId.HasValue && iRegionId == 0)
                    {
                        if (iCountryId != 193)
                            iRegionId = altRegionId.Value;//RegionValue
                        else
                            iRegionId = 3;//Russia
                    }
                    else
                        if (iCountryId != 193)
                            iRegionId = 11;//Далн. зарубеж.

                    bool bIns = false;
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                    {
                        PersonContacts = new PersonContacts();
                        PersonContacts.PersonId = PersonId;
                        bIns = true;
                    }

                    PersonContacts.Phone = model.ContactsInfo.MainPhone;
                    PersonContacts.Mobiles = model.ContactsInfo.SecondPhone;
                    PersonContacts.CountryId = iCountryId;
                    PersonContacts.RegionId = iRegionId;

                    PersonContacts.Code = model.ContactsInfo.PostIndex;
                    PersonContacts.City = model.ContactsInfo.City;
                    PersonContacts.Street = model.ContactsInfo.Street;
                    PersonContacts.House = model.ContactsInfo.House;
                    PersonContacts.Korpus = model.ContactsInfo.Korpus;
                    PersonContacts.Flat = model.ContactsInfo.Flat;

                    if (bIns)
                        context.PersonContacts.AddObject(PersonContacts);

                    Person.RegistrationStage = iRegStage < 4 ? 4 : iRegStage;

                    context.SaveChanges();
                }
                else if (model.Stage == 4)//образование
                {
                    int iSchoolTypeId;
                    int SchoolExitYear;
                    if (!int.TryParse(model.EducationInfo.SchoolTypeId, out iSchoolTypeId))
                        iSchoolTypeId = 1;
                    if (!int.TryParse(model.EducationInfo.SchoolExitYear, out SchoolExitYear))
                        SchoolExitYear = DateTime.Now.Year;
                    int iCountryEducId;
                    if (!int.TryParse(model.EducationInfo.CountryEducId, out iCountryEducId))
                        iCountryEducId = 1;
                    int iLanguageId;
                    if (!int.TryParse(model.EducationInfo.LanguageId, out iLanguageId))
                        iLanguageId = 1;

                    double avgBall;
                    if (!double.TryParse(model.EducationInfo.AvgMark, out avgBall))
                        avgBall = 0.0;

                    double EnglishMark;
                    if (!double.TryParse(model.EducationInfo.EnglishMark, out EnglishMark))
                        EnglishMark = 0.0;

                    int sform = 0;
                    if (!int.TryParse(model.EducationInfo.PersonStudyForm, out sform))
                        sform = 0;
                    int qualId = 0;
                    if (!int.TryParse(model.EducationInfo.PersonQualification, out qualId))
                        qualId = 0;

                    var PersonEducationDocument = Person.PersonEducationDocument;
                    var PersonHighEducationInfo = Person.PersonHighEducationInfo;

                    //-----------------PersonEducationDocument---------------------
                    bool bIns = false;
                    if (PersonEducationDocument == null)
                    {
                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
                        PersonEducationDocument.PersonId = PersonId;
                        bIns = true;
                    }

                    PersonEducationDocument.SchoolTypeId = iSchoolTypeId;
                    PersonEducationDocument.SchoolName = model.EducationInfo.SchoolName;
                    PersonEducationDocument.SchoolCity = model.EducationInfo.SchoolCity;
                    PersonEducationDocument.AvgMark = (avgBall == 0.0 ? null : (double?)avgBall);
                    PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
                    PersonEducationDocument.CountryEducId = iCountryEducId;
                    PersonEducationDocument.LanguageId = iLanguageId;
                    PersonEducationDocument.IsExcellent = model.EducationInfo.IsExcellent;
                    PersonEducationDocument.EnglishMark = EnglishMark;
                    PersonEducationDocument.StartEnglish = model.EducationInfo.StartEnglish;

                    PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
                    PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;

                    if (bIns)
                    {
                        context.PersonEducationDocument.AddObject(PersonEducationDocument);
                        bIns = false;
                    }

                    //-----------------PersonHighEducationInfo---------------------
                    if (PersonHighEducationInfo == null)
                    {
                        PersonHighEducationInfo = new PersonHighEducationInfo();
                        PersonHighEducationInfo.PersonId = PersonId;
                        bIns = true;
                    }

                    int HEEntryYear;
                    if (!int.TryParse(model.EducationInfo.HEEntryYear, out HEEntryYear))
                        HEEntryYear = 0;

                    PersonHighEducationInfo.DiplomaTheme = model.EducationInfo.DiplomTheme;
                    PersonHighEducationInfo.ProgramName = model.EducationInfo.ProgramName;
                    PersonHighEducationInfo.EntryYear = (HEEntryYear == 0 ? null : (int?)HEEntryYear);
                    if (sform != 0)
                        PersonHighEducationInfo.StudyFormId = sform;
                    if (qualId != 0)
                        PersonHighEducationInfo.QualificationId = qualId;

                    if (iSchoolTypeId == 4)
                    {
                        int iEntryYear;
                        int.TryParse(model.EducationInfo.HEEntryYear, out iEntryYear);
                        PersonHighEducationInfo.EntryYear = iEntryYear != 0 ? (int?)iEntryYear : null;
                        PersonHighEducationInfo.ExitYear = SchoolExitYear != 0 ? (int?)SchoolExitYear : null;
                    }

                    if (bIns)
                        context.PersonHighEducationInfo.AddObject(PersonHighEducationInfo);

                    Person.RegistrationStage = iRegStage < 5 ? 5 : iRegStage;
                    context.SaveChanges();
                }
                else if (model.Stage == 5)
                {
                    if (iRegStage < 6)
                    {
                        Person.RegistrationStage = 6;
                        context.SaveChanges();
                    }
                }
                else if (model.Stage == 6)
                {
                    if (!model.AddInfo.FZ_152Agree)
                    {
                        ModelState.AddModelError("AddInfo_FZ_152Agree", "Вы должны принять условия");
                        return View("PersonalOffice", model);
                    }
                    bool bIns = false;
                    var PersonAddInfo = Person.PersonAddInfo;
                    if (PersonAddInfo == null)
                    {
                        PersonAddInfo = new PersonAddInfo();
                        PersonAddInfo.PersonId = PersonId;
                        bIns = true;
                    }

                    PersonAddInfo.AddInfo = model.AddInfo.ExtraInfo;
                    PersonAddInfo.Parents = model.AddInfo.ContactPerson;
                    PersonAddInfo.HasPrivileges = model.AddInfo.HasPrivileges;
                    PersonAddInfo.HostelAbit = model.AddInfo.HostelAbit;

                    if (Person.RegistrationStage <= 6)
                        Person.RegistrationStage = 100;

                    if (bIns)
                        context.PersonAddInfo.AddObject(PersonAddInfo);

                    context.SaveChanges();
                }

                if (model.Stage < 6)
                {
                    model.Stage++;
                    return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", model.Stage } });
                }
                else
                    return RedirectToAction("Main", "Abiturient");
            }
        }

        //public ActionResult Main()
        //{
        //    //if (Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 && Util.bUseRedirection &&
        //    //    Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
        //    //    return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

        //    //Validation
        //    Guid PersonID;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
        //        return RedirectToAction("LogOn", "Account");

        //    if (Util.CheckIsNew(PersonID))
        //        return RedirectToAction("OpenPersonalAccount");

        //    using (OnlinePriemEntities context = new OnlinePriemEntities())
        //    {
        //        var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
        //        if (PersonInfo == null)
        //            return RedirectToAction("Index");

        //        //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
        //        switch (PersonInfo.AbiturientTypeId)
        //        {
        //            case 1: { return RedirectToAction("Main", "Abiturient"); }
        //            case 2: { return RedirectToAction("Main", "ForeignAbiturient"); }
        //            case 3: { return RedirectToAction("Main", "Transfer"); }
        //            case 4: { return RedirectToAction("Main", "TransferForeign"); }
        //            case 5: { return RedirectToAction("Main", "Recover"); }
        //            case 6: { return RedirectToAction("Main", "ChangeStudyForm"); }
        //            case 7: { return RedirectToAction("Main", "ChangeObrazProgram"); }
        //            case 8: { return RedirectToAction("Main", "AG"); }
        //            case 9: { return RedirectToAction("Main", "SPO"); }
        //            case 10: { return RedirectToAction("Main", "ForeignAspirant");  }
        //            case 11: { break; }
        //            default: { return RedirectToAction("Main", "Abiturient"); }
        //        }

        //        int regStage = PersonInfo.RegistrationStage;
        //        if (regStage < 100)
        //            return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage.ToString() } });

        //        SimplePerson model = new SimplePerson();
        //        model.Applications = new List<SimpleApplication>();
        //        model.Files = new List<AppendedFile>();

        //        string query = "SELECT Surname, Name, SecondName, RegistrationStage FROM PERSON WHERE Id=@Id";
        //        SortedList<string, object> dic = new SortedList<string, object>() { { "@Id", PersonID } };
        //        DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
        //        if (tbl.Rows.Count != 1)
        //            return RedirectToAction("Index");

        //        model.Name = Server.HtmlEncode(PersonInfo.Name);
        //        model.Surname = Server.HtmlEncode(PersonInfo.Surname);
        //        model.SecondName = Server.HtmlEncode(PersonInfo.SecondName);

        //        var Applications = context.Abiturient.Where(x => x.PersonId == PersonID);

        //        query = "SELECT [Application].Id, LicenseProgramName, ObrazProgramName, ProfileName, Priority, Enabled, StudyFormName, StudyBasisName FROM [Application] " +
        //            "INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE PersonId=@PersonId";
        //        dic.Clear();
        //        dic.Add("@PersonId", PersonID);
        //        tbl = Util.AbitDB.GetDataTable(query, dic);
        //        foreach (var app in Applications)
        //        {
        //            model.Applications.Add(new SimpleApplication()
        //            {
        //                Id = app.Id,
        //                Profession = app.LicenseProgramName,
        //                ObrazProgram = app.ObrazProgramName,
        //                Specialization = app.ProfileName,
        //                Priority = app.Priority.ToString(),
        //                Enabled = app.Enabled,
        //                StudyBasis = app.StudyBasisName,
        //                StudyForm = app.StudyFormName
        //            });
        //        }

        //        model.Messages = Util.GetNewPersonalMessages(PersonID);
        //        if (model.Applications.Count == 0)
        //        {
        //            model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, Text = "Для подачи заявления нажмите кнопку <a href=\"" + Util.ServerAddress + "/Abiturient/NewApplication\">\"Подать новое заявление\"</a>" });
        //        }

        //        return View("Main", model);
        //    }
        //}

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication(params string[] errors)
        {
            if (errors != null && errors.Length > 0)
            {
                foreach (string er in errors)
                    ModelState.AddModelError("", er);
            }
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
                switch (PersonInfo.AbiturientTypeId)
                {
                    case 1: { return RedirectToAction("NewApplication", "Abiturient"); }
                    case 2: { return RedirectToAction("NewApplication", "ForeignAbiturient"); }
                    case 3: { return RedirectToAction("NewApplication", "Transfer"); }
                    case 4: { return RedirectToAction("NewApplication", "TransferForeign"); }
                    case 5: { return RedirectToAction("NewApplication", "Recover"); }
                    case 6: { return RedirectToAction("NewApplication", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("NewApplication", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("NewApplication", "AG"); }
                    case 9: { return RedirectToAction("NewApplication", "SPO"); }
                    case 10: { return RedirectToAction("NewApplication", "ForeignAspirant");  }
                    case 11: { break; }
                    default: { return RedirectToAction("NewApplication", "Abiturient"); }
                }

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                ApplicationModel model = new ApplicationModel();
                //вообще, в аспирантуру нет разделения
                model.EntryType = 5;

                string query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry WHERE StudyLevelId=15 ORDER BY 1";
                DataTable tbl = Util.AbitDB.GetDataTable(query, null);
                model.StudyForms =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("StudyFormId"),
                         Text = rw.Field<string>("StudyFormName")
                     }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();

                query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry WHERE StudyLevelId=15 ORDER BY 1";
                tbl = Util.AbitDB.GetDataTable(query, null);
                model.StudyBasises =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("StudyBasisId"),
                         Text = rw.Field<string>("StudyBasisName")
                     }).AsEnumerable()
                     .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                     .ToList();

                return View("NewApplication", model);
            }
        }

        [HttpPost]
        public ActionResult NewApp()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            bool isForeign = Util.CheckIsForeign(PersonId);

            string faculty = Request.Form["lFaculty"];
            string profession = Request.Form["lProfession"];
            string obrazprogram = Request.Form["lObrazProgram"];
            string sform = Request.Form["StudyFormId"];
            string sbasis = Request.Form["StudyBasisId"];
            string isSecond = Request.Form["IsSecondHidden"];
            string isReduced = Request.Form["IsReducedHidden"];
            string isParallel = Request.Form["IsParallelHidden"];
            bool needHostel = string.IsNullOrEmpty(Request.Form["NeedHostel"]) ? false : true;

            int iStudyFormId = Util.ParseSafe(sform);
            int iStudyBasisId = Util.ParseSafe(sbasis);
            int iFacultyId = Util.ParseSafe(faculty);
            int iProfession = Util.ParseSafe(profession);
            int iObrazProgram = Util.ParseSafe(obrazprogram);
            Guid ProfileId = Guid.Empty;
            if (!string.IsNullOrEmpty(Request.Form["lSpecialization"]))
                Guid.TryParse(Request.Form["lSpecialization"], out ProfileId);

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            DateTime timeX = new DateTime(2013, 6, 20, 10, 0, 0);//20-06-2013 10:00:00

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT qEntry.Id FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id=qEntry.StudyLevelId WHERE LicenseProgramId=@LicenseProgramId " +
                " AND ObrazProgramId=@ObrazProgramId AND StudyFormId=@SFormId AND StudyBasisId=@SBasisId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced " +
                (ProfileId == Guid.Empty ? " AND ProfileId IS NULL " : " AND ProfileId=@ProfileId ") + (iFacultyId == 0 ? "" : " AND FacultyId=@FacultyId ") +
                " AND SemesterId=@SemesterId AND CampaignYear=@CampaignYear";

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@LicenseProgramId", iProfession);
            dic.Add("@ObrazProgramId", iObrazProgram);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@SBasisId", iStudyBasisId);
            dic.Add("@IsSecond", bIsSecond);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@SemesterId", 1);//only 1 semester (1kurs)
            dic.Add("@CampaignYear", Util.iPriemYear);
            if (ProfileId != Guid.Empty)
                dic.Add("@ProfileId", ProfileId);
            if (iFacultyId != 0)
                dic.Add("@FacultyId", iFacultyId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count > 1)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Неоднозначный выбор учебного плана (" + tbl.Rows.Count + ")" } });
            if (tbl.Rows.Count == 0)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Не найден учебный план" } });

            Guid EntryId = tbl.Rows[0].Field<Guid>("Id");

            query = "SELECT DateOfClose FROM [Entry] WHERE Id=@Id";
            DateTime DateOfClose = (DateTime)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", EntryId } });

            if (DateTime.Now > DateOfClose)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на данное направление прекращена " + DateOfClose.ToString("dd.MM.yyyy") } });

            query = "SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
            var eIds =
                from DataRow rw in tbl.Rows
                select rw.Field<Guid>("EntryId");
            if (eIds.Contains(EntryId))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

            DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [Application] WHERE PersonId=@PersonId AND Enabled=@Enabled",
                new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
            int? PriorMax =
                (from DataRow rw in tblPriors.Rows
                 select rw.Field<int?>("Priority")).Max();

            Guid appId = Guid.NewGuid();
            query = "INSERT INTO [Application] (Id, PersonId, EntryId, HostelEduc, Enabled, Priority, EntryType, DateOfStart) " +
                "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @Priority, @EntryType, @DateOfStart)";
            SortedList<string, object> prms = new SortedList<string, object>()
            {
                { "@Id", appId },
                { "@PersonId", PersonId },
                { "@EntryId", EntryId },
                { "@HostelEduc", needHostel },
                { "@Enabled", true },
                { "@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1 },
                { "@EntryType", 15 },
                { "@DateOfStart", DateTime.Now }
            };

            Util.AbitDB.ExecuteQuery(query, prms);

            query = "SELECT Person.Surname, Person.Name, Person.SecondName, Entry.LicenseProgramCode, Entry.LicenseProgramName, Entry.ObrazProgramName " +
                " FROM [Application] INNER JOIN Person ON Person.Id=[Application].PersonId " +
                " INNER JOIN Entry ON Application.EntryId=Entry.Id WHERE Application.Id=@AppId";
            DataTable Tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", appId } });
            var fileInfo =
                (from DataRow rw in Tbl.Rows
                 select new
                 {
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName"),
                     ProfessionCode = rw.Field<string>("LicenseProgramCode"),
                     Profession = rw.Field<string>("LicenseProgramName"),
                     ObrazProgram = rw.Field<string>("ObrazProgramName")
                 }).FirstOrDefault();

            return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", appId.ToString("N") } });
        }

        //public ActionResult PriorityChanger()
        //{
        //    Guid PersonId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
        //        return RedirectToAction("LogOn", "Account");
        //    using (OnlinePriemEntities context = new OnlinePriemEntities())
        //    {
        //        var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
        //        if (PersonInfo == null)//а что это могло бы значить???
        //            return RedirectToAction("Index");

        //        //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
        //        switch (PersonInfo.AbiturientTypeId)
        //        {
        //            case 1: { break; }
        //            case 2: { return RedirectToAction("PriorityChanger", "ForeignAbiturient"); }
        //            //case 3: { return RedirectToAction("PriorityChanger", "Transfer"); }
        //            //case 4: { return RedirectToAction("PriorityChanger", "TransferForeign"); }
        //            //case 5: { return RedirectToAction("PriorityChanger", "Recover"); }
        //            //case 6: { return RedirectToAction("PriorityChanger", "ChangeStudyForm"); }
        //            //case 7: { return RedirectToAction("PriorityChanger", "ChangeObrazProgram"); }
        //            //case 8: { return RedirectToAction("PriorityChanger", "AG"); }
        //            default: { break; }
        //        }

        //        string query = "(SELECT [Application].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [Application] " +
        //            " INNER JOIN Entry ON [Application].EntryId=Entry.Id " +
        //            " WHERE PersonId=@PersonId AND Enabled=@Enabled " +
        //            " UNION " +
        //            " SELECT [ForeignApplication].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [ForeignApplication] " +
        //            " INNER JOIN Entry ON [ForeignApplication].EntryId=Entry.Id " +
        //            " WHERE PersonId=@PersonId AND Enabled=@Enabled) ORDER BY Priority ";
        //        SortedList<string, object> dic = new SortedList<string, object>()
        //        {
        //            {"@PersonId", PersonId },
        //            {"@Enabled", true }
        //        };
        //        DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
        //        var apps = (from DataRow rw in tbl.Rows
        //                    select new SimpleApplication()
        //                    {
        //                        Id = rw.Field<Guid>("Id"),
        //                        Priority = rw.Field<int>("Priority").ToString(),
        //                        Profession = rw.Field<string>("LicenseProgramName"),
        //                        ObrazProgram = rw.Field<string>("ObrazProgramName"),
        //                        Specialization = rw.Field<string>("ProfileName")
        //                    }).ToList();

        //        MotivateMailModel mdl = new MotivateMailModel()
        //        {
        //            Apps = apps,
        //            UILanguage = Util.GetUILang(PersonId)
        //        };
        //        return View(mdl);
        //    }
        //}
    }
}
