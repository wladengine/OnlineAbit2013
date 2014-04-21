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
using System.Threading;

namespace OnlineAbit2013.Controllers
{
    public class ForeignAbiturientController : Controller
    {
        //
        // GET: /ForeignAbiturient/

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
                case 2: { break; }
                case 3: { return RedirectToAction("Index", "Transfer", new RouteValueDictionary() { { "step", step }}); }
                case 4: { return RedirectToAction("Index", "TransferForeign", new RouteValueDictionary() { { "step", step }}); }
                case 5: { return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", step }}); }
                case 6: { return RedirectToAction("Index", "ChangeStudyForm", new RouteValueDictionary() { { "step", step }}); }
                case 7: { return RedirectToAction("Index", "ChangeObrazProgram", new RouteValueDictionary() { { "step", step }}); ; }
                case 8: { return RedirectToAction("Index", "AG", new RouteValueDictionary() { { "step", step }}); ; }
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
                    return RedirectToAction("OpenPersonalAccount", "Abiturient");

                if (Person.RegistrationStage == 0)
                    stage = 1;
                else if (Person.RegistrationStage < stage)
                    stage = Person.RegistrationStage;

                PersonalOfficeForeign model = new PersonalOfficeForeign() { Lang = eng ? "en" : "ru", 
                    Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
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

                    DataTable tblCountr = Util.AbitDB.GetDataTable("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, Name", null);
                    model.PersonInfo.NationalityList = (from DataRow rw in tblCountr.Rows
                                                        select new SelectListItem() 
                                                        { 
                                                            Value = rw.Field<int>("Id").ToString(), 
                                                            Text = rw.Field<string>(eng ? "NameEng" : "Name") 
                                                        }).ToList();
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
                    model.ContactsInfo.AddressData = Server.HtmlDecode(PersonContacts.ForeignAddressInfo);

                    DataTable tblCountr =
                        Util.AbitDB.GetDataTable(
                            string.Format("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, {0}", eng ? "NameEng" : "Name"),
                            null);

                    model.ContactsInfo.CountryList = (from DataRow rw in tblCountr.Rows
                                                      select new SelectListItem()
                                                      {
                                                          Value = rw.Field<int>("Id").ToString(),
                                                          Text = rw.Field<string>(eng ? "NameEng" : "Name")
                                                      }).ToList();
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

                    DataTable tblQual =
                        Util.AbitDB.GetDataTable("SELECT Id, Name, NameEng FROM [Qualification]", null);
                    model.EducationInfo.QualificationList = (from DataRow rw in tblQual.Rows
                                                             select new SelectListItem()
                                                             {
                                                                 Value = rw.Field<int>("Id").ToString(),
                                                                 Text = rw.Field<string>(eng ? "NameEng" : "Name")
                                                             }).ToList();
                    model.EducationInfo.StudyFormList = Util.StudyFormAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    DataTable tblCountr =
                        Util.AbitDB.GetDataTable(
                            string.Format("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, {0}", eng ? "NameEng" : "Name"),
                            null);
                    model.EducationInfo.CountryList = (from DataRow rw in tblCountr.Rows
                                                       select new SelectListItem()
                                                       {
                                                           Value = rw.Field<int>("Id").ToString(),
                                                           Text = rw.Field<string>(eng ? "NameEng" : "Name")
                                                       }).ToList();

                    query = "SELECT Id, Name, NameEng FROM SchoolTypeAll WHERE Id IN (1, 4, 5)";
                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.EducationInfo.SchoolTypeList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>(eng ? "NameEng" : "Name")
                         }).ToList();

                    model.EducationInfo.SchoolName = Server.HtmlDecode(PersonEducationDocument.SchoolName);
                    model.EducationInfo.SchoolExitYear = Server.HtmlDecode(PersonEducationDocument.SchoolExitYear);

                    model.EducationInfo.HEExitYear = Server.HtmlDecode(PersonHighEducationInfo.ExitYear.ToString());
                    model.EducationInfo.HEEntryYear = Server.HtmlDecode(PersonHighEducationInfo.EntryYear.ToString());

                    model.EducationInfo.DiplomSeries = Server.HtmlDecode(PersonEducationDocument.Series);
                    model.EducationInfo.DiplomNumber = Server.HtmlDecode(PersonEducationDocument.Number);
                    model.EducationInfo.ProgramName = Server.HtmlDecode(PersonHighEducationInfo.ProgramName);

                    model.EducationInfo.SchoolTypeId = (PersonEducationDocument.SchoolTypeId ?? 1).ToString();
                    model.EducationInfo.PersonStudyForm = (PersonHighEducationInfo.StudyFormId ?? 1).ToString();
                    model.EducationInfo.PersonQualification = (PersonHighEducationInfo.QualificationId ?? 1).ToString();
                    model.EducationInfo.CountryEducId = (PersonEducationDocument.CountryEducId ?? 193).ToString();

                    model.EducationInfo.LanguageList = context.ForeignLanguage.Select(x => new { x.Id, Name = (eng ? x.NameEng : x.NameRus) })
                        .ToList().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    model.EducationInfo.LanguageLevelList = context.ForeignLanguageLevel.Select(x => new { x.Id, Name = (eng ? x.NameEng : x.NameRus) })
                        .ToList().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

                    model.EducationInfo.HasTRKI = PersonEducationDocument.HasTRKI ?? false;
                    model.EducationInfo.TRKICertificateNumber = PersonEducationDocument.TRKICertificateNumber;
                }
                else if (model.Stage == 5)
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
                        Person.AbiturientTypeId = 1;

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
                    int countryId = 0;
                    if (!int.TryParse(model.ContactsInfo.CountryId, out countryId))
                        countryId = 193;//Russia

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
                    PersonContacts.CountryId = countryId;

                    PersonContacts.ForeignAddressInfo = model.ContactsInfo.AddressData;

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

                    int iQualificationId = 0;
                    if (!int.TryParse(model.EducationInfo.PersonQualification, out iQualificationId))
                        iQualificationId = 0;

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
                    PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
                    PersonEducationDocument.CountryEducId = iCountryEducId;

                    PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
                    PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;

                    PersonEducationDocument.HasTRKI = model.EducationInfo.HasTRKI;
                    if (model.EducationInfo.HasTRKI)
                        PersonEducationDocument.TRKICertificateNumber = model.EducationInfo.TRKICertificateNumber;

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

                    PersonHighEducationInfo.DiplomaTheme = model.EducationInfo.DiplomTheme;
                    PersonHighEducationInfo.ProgramName = model.EducationInfo.ProgramName;
                    if (iQualificationId != 0)
                        PersonHighEducationInfo.QualificationId = iQualificationId;

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
                    PersonAddInfo.HostelAbit = model.AddInfo.HostelAbit;

                    if (Person.RegistrationStage <= 5)
                        Person.RegistrationStage = 100;

                    if (bIns)
                        context.PersonAddInfo.AddObject(PersonAddInfo);

                    context.SaveChanges();
                }

                if (model.Stage < 5)
                {
                    model.Stage++;
                    return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", model.Stage } });
                }
                else
                    return RedirectToAction("Main", "Abiturient");
            }
        }

        public ActionResult Main()
        {
            //if (Request.Url.AbsoluteUri.IndexOf("https:\\", StringComparison.OrdinalIgnoreCase) == -1 && 
            //    Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
            //    return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            //Validation
            Guid PersonID;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
                return RedirectToAction("LogOn", "Account");

            if (Util.CheckIsNew(PersonID))
                return RedirectToAction("OpenPersonalAccount");

            Util.SetThreadCultureByCookies(Request.Cookies);
            string lang = Util.GetCurrentThreadLanguage();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
                if (PersonInfo == null)
                    return RedirectToAction("Index");

                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
                switch (PersonInfo.AbiturientTypeId)
                {
                    case 1: { return RedirectToAction("Main", "Abiturient"); }
                    case 2: { break; }
                    case 3: { return RedirectToAction("Main", "Transfer"); }
                    case 4: { return RedirectToAction("Main", "TransferForeign"); }
                    case 5: { return RedirectToAction("Main", "Recover"); }
                    case 6: { return RedirectToAction("Main", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("Main", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("Main", "AG"); }
                    default: { return RedirectToAction("Main", "Abiturient"); }
                }

                int regStage = PersonInfo.RegistrationStage;
                if (regStage < 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage.ToString() } });

                SimplePerson model = new SimplePerson();
                model.Applications = new List<SimpleApplication>();
                model.Files = new List<AppendedFile>();

                string query = "SELECT Surname, Name, SecondName, RegistrationStage FROM PERSON WHERE Id=@Id";
                Dictionary<string, object> dic = new Dictionary<string, object>() { { "@Id", PersonID } };
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                if (tbl.Rows.Count != 1)
                    return RedirectToAction("Index");

                model.Name = Server.HtmlEncode(PersonInfo.Name);
                model.Surname = Server.HtmlEncode(PersonInfo.Surname);
                model.SecondName = Server.HtmlEncode(PersonInfo.SecondName);

                var Applications = context.Abiturient.Where(x => x.PersonId == PersonID);

                query = "SELECT [Application].Id, LicenseProgramName, ObrazProgramName, ProfileName, Priority, Enabled, StudyFormName, StudyBasisName FROM [Application] " +
                    "INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE PersonId=@PersonId";
                dic.Clear();
                dic.Add("@PersonId", PersonID);
                tbl = Util.AbitDB.GetDataTable(query, dic);
                foreach (var app in Applications)
                {
                    model.Applications.Add(new SimpleApplication()
                    {
                        Id = app.Id,
                        Profession = lang == "en" ? app.LicenseProgramNameEng : app.LicenseProgramName,
                        ObrazProgram = lang == "en" ? app.ObrazProgramNameEng : app.ObrazProgramName,
                        Specialization = lang == "en" ? app.ProfileNameEng : app.ProfileName,
                        Priority = app.Priority.ToString(),
                        Enabled = app.Enabled,
                        StudyBasis = lang == "en" ? app.StudyBasisNameEng : app.StudyBasisName,
                        StudyForm = lang == "en" ? app.StudyFormNameEng : app.StudyFormName
                    });
                }

                model.Messages = Util.GetNewPersonalMessages(PersonID);
                if (model.Applications.Count == 0)
                {
                    model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, Text = "Для подачи заявления нажмите кнопку <a href=\"" + Util.ServerAddress + "/Abiturient/NewApplication\">\"Подать новое заявление\"</a>" });
                }

                return View("Main", model);
            }
        }

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

            Util.SetThreadCultureByCookies(Request.Cookies);
            string lang = Util.GetCurrentThreadLanguage();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
                switch (PersonInfo.AbiturientTypeId)
                {
                    case 1: { return RedirectToAction("NewApplication", "Abiturient"); }
                    case 2: { break; }
                    case 3: { return RedirectToAction("NewApplication", "Transfer"); }
                    case 4: { return RedirectToAction("NewApplication", "TransferForeign"); }
                    case 5: { return RedirectToAction("NewApplication", "Recover"); }
                    case 6: { return RedirectToAction("NewApplication", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("NewApplication", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("NewApplication", "AG"); }
                    default: { return RedirectToAction("NewApplication", "Abiturient"); }
                }

                ApplicationModel model = new ApplicationModel();
                int? iScTypeId = (int?)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new Dictionary<string, object>() { { "@Id", PersonId } });
                if (iScTypeId.HasValue)
                {
                    if (iScTypeId.Value != 4)
                        model.EntryType = 1;//1 курс бак-спец
                    else
                        model.EntryType = 2;
                }
                else
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });

                string query = string.Format("SELECT DISTINCT StudyFormId, StudyFormName{0} AS StudyFormName FROM Entry ORDER BY 1", lang == "en" ? "Eng" : "");
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

                query = string.Format("SELECT DISTINCT StudyBasisId, StudyBasisName{0} as StudyBasisName FROM Entry ORDER BY 1", lang == "en" ? "Eng" : "");
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

            int iEntry = Util.ParseSafe(Request.Form["EntryType"]);
            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            //DateTime timeX = new DateTime(2012, 7, 13, 16, 0, 0);//13-07-2012 16:00:00
            //if (iEntry == 2 && DateTime.Now > timeX && !isForeign)
            //{
            //    if (iStudyFormId != 2)
            //        return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений в магистратуру на очное отделение окончена 13 июля в 16:00" } });
            //}

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT qEntry.Id FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id=qEntry.StudyLevelId WHERE LicenseProgramId=@LicenseProgramId " +
                " AND ObrazProgramId=@ObrazProgramId AND StudyFormId=@SFormId AND StudyBasisId=@SBasisId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced " +
                (ProfileId == Guid.Empty ? " AND ProfileId IS NULL " : " AND ProfileId=@ProfileId ") + (iFacultyId == 0 ? "" : " AND FacultyId=@FacultyId ") +
                " AND SemesterId=@SemesterId AND CampaignYear=@CampaignYear";

            Dictionary<string, object> dic = new Dictionary<string, object>();
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

            query = "SELECT DateOfClose FROM [EntryOpeningClosingDates] WHERE EntryId=@Id AND AbiturientTypeId=2";
            DateTime? DateOfClose = (DateTime?)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", EntryId } });

            if (DateOfClose.HasValue && DateTime.Now > DateOfClose)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на данное направление прекращена " + DateOfClose.Value.ToString("dd.MM.yyyy") } });

            query = "SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            var eIds =
                from DataRow rw in tbl.Rows
                select rw.Field<Guid>("EntryId");
            if (eIds.Contains(EntryId))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

            DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [Application] WHERE PersonId=@PersonId AND Enabled=@Enabled",
                new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
            int? PriorMax =
                (from DataRow rw in tblPriors.Rows
                 select rw.Field<int?>("Priority")).Max();

            Guid appId = Guid.NewGuid();
            query = "INSERT INTO [Application] (Id, PersonId, EntryId, HostelEduc, Enabled, Priority, EntryType, DateOfStart) " +
                "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @Priority, @EntryType, @DateOfStart)";
            Dictionary<string, object> prms = new Dictionary<string, object>()
            {
                { "@Id", appId },
                { "@PersonId", PersonId },
                { "@EntryId", EntryId },
                { "@HostelEduc", needHostel },
                { "@Enabled", true },
                { "@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1 },
                { "@EntryType", iEntry },
                { "@DateOfStart", DateTime.Now }
            };

            Util.AbitDB.ExecuteQuery(query, prms);

            query = "SELECT Person.Surname, Person.Name, Person.SecondName, Entry.LicenseProgramCode, Entry.LicenseProgramName, Entry.ObrazProgramName " +
                " FROM [Application] INNER JOIN Person ON Person.Id=[Application].PersonId " +
                " INNER JOIN Entry ON Application.EntryId=Entry.Id WHERE Application.Id=@AppId";
            DataTable Tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
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

            if (iEntry == 2)
            {
                byte[] pdfData = PDFUtils.GetApplicationPDFForeign(appId, Server.MapPath("~/Templates/"));
                DateTime dateTime = DateTime.Now;

                query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileExtention, FileData, FileSize, IsReadOnly, LoadDate, Comment, MimeType) " +
                    " VALUES (@Id, @PersonId, @FileName, @FileExtention, @FileData, @FileSize, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
                prms.Clear();
                prms.Add("@Id", Guid.NewGuid());
                prms.Add("@PersonId", appId);
                prms.Add("@FileName", fileInfo.Surname + " " + fileInfo.Name.FirstOrDefault() + " - Заявление [" + dateTime.ToString("dd.MM.yyyy") + "].pdf");
                prms.Add("@FileExtention", ".pdf");
                prms.Add("@FileData", pdfData);
                prms.Add("@FileSize", pdfData.Length);
                prms.Add("@IsReadOnly", true);
                prms.Add("@LoadDate", dateTime);
                prms.Add("@Comment", "Заявление на направление (" + fileInfo.ProfessionCode + ") " + fileInfo.Profession + ", образовательная программа \""
                    + fileInfo.ObrazProgram + "\", от " + dateTime.ToShortDateString());
                prms.Add("@MimeType", "[Application]/pdf");
                Util.AbitDB.ExecuteQuery(query, prms);
            }
            return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", appId.ToString("N") } });
        }

        public ActionResult AddSharedFiles()
        {
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
                    case 1: { return RedirectToAction("AddSharedFiles", "ForeignAbiturient"); }
                    case 2: { break; }
                    //case 3: { return RedirectToAction("AddSharedFiles", "Transfer"); }
                    //case 4: { return RedirectToAction("AddSharedFiles", "TransferForeign"); }
                    //case 5: { return RedirectToAction("AddSharedFiles", "Recover"); }
                    //case 6: { return RedirectToAction("AddSharedFiles", "ChangeStudyForm"); }
                    //case 7: { return RedirectToAction("AddSharedFiles", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("AddSharedFiles", "AG"); }
                    default: { return RedirectToAction("AddSharedFiles", "ForeignAbiturient"); }
                }

                Util.SetThreadCultureByCookies(Request.Cookies);

                string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

                List<AppendedFile> lst =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                            rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
                     }).ToList();

                AppendFilesModel model = new AppendFilesModel() { Files = lst };
                return View(model);
            }
        }

        public ActionResult PriorityChanger()
        {
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
                    case 1: { return RedirectToAction("PriorityChanger", "Abiturient"); }
                    case 2: { break; }
                    //case 3: { return RedirectToAction("PriorityChanger", "Transfer"); }
                    //case 4: { return RedirectToAction("PriorityChanger", "TransferForeign"); }
                    //case 5: { return RedirectToAction("PriorityChanger", "Recover"); }
                    //case 6: { return RedirectToAction("PriorityChanger", "ChangeStudyForm"); }
                    //case 7: { return RedirectToAction("PriorityChanger", "ChangeObrazProgram"); }
                    //case 8: { return RedirectToAction("PriorityChanger", "AG"); }
                    default: { return RedirectToAction("PriorityChanger", "Abiturient"); }
                }

                string query = "(SELECT [Application].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [Application] " +
                    " INNER JOIN Entry ON [Application].EntryId=Entry.Id " +
                    " WHERE PersonId=@PersonId AND Enabled=@Enabled " +
                    " UNION " +
                    " SELECT [ForeignApplication].Id, Priority, LicenseProgramName, ObrazProgramName, ProfileName FROM [ForeignApplication] " +
                    " INNER JOIN Entry ON [ForeignApplication].EntryId=Entry.Id " +
                    " WHERE PersonId=@PersonId AND Enabled=@Enabled) ORDER BY Priority ";
                Dictionary<string, object> dic = new Dictionary<string, object>()
                {
                    {"@PersonId", PersonId },
                    {"@Enabled", true }
                };
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                var apps = (from DataRow rw in tbl.Rows
                            select new SimpleApplication()
                            {
                                Id = rw.Field<Guid>("Id"),
                                Priority = rw.Field<int>("Priority").ToString(),
                                Profession = rw.Field<string>("LicenseProgramName"),
                                ObrazProgram = rw.Field<string>("ObrazProgramName"),
                                Specialization = rw.Field<string>("ProfileName")
                            }).ToList();

                MotivateMailModel mdl = new MotivateMailModel()
                {
                    Apps = apps,
                    UILanguage = Util.GetUILang(PersonId)
                };
                return View(mdl);
            }
        }

        #region Ajax

        public JsonResult GetPersonLanguages()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false });

            string query =
                "SELECT ForeignPersonLanguage.Id, ForeignLanguage.NameEng AS 'LangEng', ForeignLanguage.NameRus AS 'LangRus', ForeignLanguageLevel.NameEng AS 'LevelEng', " +
                "ForeignLanguageLevel.NameRus AS 'LevelRus' FROM ForeignPersonLanguage " +
                "INNER JOIN ForeignLanguage ON ForeignLanguage.Id=ForeignPersonLanguage.ForeignLanguageId " +
                "INNER JOIN ForeignLanguageLevel ON ForeignLanguageLevel.Id=ForeignPersonLanguage.ForeignLanguageLevelId " +
                "WHERE ForeignPersonLanguage.PersonId=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", PersonId } });
            var data =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<Guid>("Id"),
                     Language = System.Threading.Thread.CurrentThread.CurrentUICulture
                     == System.Globalization.CultureInfo.GetCultureInfo("ru-RU") ? rw.Field<string>("LangRus") : rw.Field<string>("LangEng"),
                     Level = System.Threading.Thread.CurrentThread.CurrentUICulture
                     == System.Globalization.CultureInfo.GetCultureInfo("ru-RU") ? rw.Field<string>("LevelRus") : rw.Field<string>("LevelEng")
                 });

            var res = new { IsOk = data.Count() > 0 ? true : false, Data = data };
            return Json(res);
        }
        public JsonResult GetLanguages()
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            string query = "SELECT Id, NameRus, NameEng FROM ForeignLanguage WHERE Id NOT IN (SELECT ForeignLanguageId FROM ForeignPersonLanguage WHERE PersonId=@Id)";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", PersonId } });
            var langs =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<int>("Id"),
                     Name = System.Threading.Thread.CurrentThread.CurrentUICulture
                     == System.Globalization.CultureInfo.GetCultureInfo("ru-RU") ? rw.Field<string>("NameRus") : rw.Field<string>("NameEng")
                 });

            return Json(langs);
        }
        public JsonResult AddLang(string langid, string levelid)
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            int iLanguageId = 1;
            int iLevelId = 1;

            int.TryParse(langid, out iLanguageId);
            int.TryParse(levelid, out iLevelId);

            Guid Id = Guid.NewGuid();

            string query = "INSERT INTO ForeignPersonLanguage (Id, PersonId, ForeignLanguageId, ForeignLanguageLevelId) VALUES " +
                "(@Id, @PersonId, @ForeignLanguageId, @ForeignLanguageLevelId)";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", Id);
            dic.Add("@PersonId", PersonId);
            dic.Add("@ForeignLanguageId", iLanguageId);
            dic.Add("@ForeignLanguageLevelId", iLevelId);

            //            query = @"SELECT ForeignLanguage.NameRus, ForeignLanguage.NameEng, ForeignLanguageLevel.NameRus, ForeignLanguageLevel.NameEng FROM ForeignPersonLanguage 
            //FROM ForeignPersonLanguage INNER JOIN ForeignLanguage ON ForeignLanguage.Id = ForeignPersonLanguage.ForeignLanguageId INNER JOIN ForeignLanguageLevel ON
            //ForeignLanguageLevel.Id = ForeignPersonLanguage.ForeignLanguageLevelId WHERE ForeignPersonLanguage.Id=@Id";
            //            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", Id } });

            Util.AbitDB.ExecuteQuery(query, dic);
            return Json("OK");
        }
        public JsonResult DeleteLang(string id)
        {
            Guid Id;
            if (!Guid.TryParse(id, out Id))
                return Json(new { IsOk = false });

            string query = "DELETE FROM ForeignPersonLanguage WHERE Id=@Id";
            Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", Id } });
            return Json(new { IsOk = true });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetFacs(string studyform, string studybasis, string entry)
        {
            int iStudyFormId;
            int iStudyBasisId;
            if (!int.TryParse(studyform, out iStudyFormId))
                iStudyFormId = 1;
            if (!int.TryParse(studybasis, out iStudyBasisId))
                iStudyBasisId = 1;
            int iEntryId = 1;
            if (!int.TryParse(entry, out iEntryId))
                iEntryId = 1;

            string query = string.Format("SELECT DISTINCT FacultyId, FacultyName FROM {0} WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId " +
                "AND IsSecond=@IsSecond ORDER BY FacultyId", iEntryId == 2 ? "extStudyPlan" : "extStudyPlan1K");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@IsSecond", iEntryId == 3 ? true : false);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var facs =
                from DataRow rw in tbl.Rows
                select new { Id = rw.Field<int>("FacultyId"), Name = rw.Field<string>("FacultyName") };
            return Json(facs);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetProfs(string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
        {
            

            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            Util.SetThreadCultureByCookies(Request.Cookies);
            string lang = Util.GetCurrentThreadLanguage();

            int iStudyFormId;
            int iStudyBasisId;
            int iEntryId = 1;
            int iSemesterId;
            if (!int.TryParse(studyform, out iStudyFormId))
                iStudyFormId = 1;
            if (!int.TryParse(studybasis, out iStudyBasisId))
                iStudyBasisId = 1;
            if (!int.TryParse(entry, out iEntryId))
                iEntryId = 1;
            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = string.Format("SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName{0} as LicenseProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel " +
                "AND IsReduced=@IsReduced AND [CampaignYear]=@Year AND SemesterId=@SemesterId", lang == "en" ? "Eng" : "");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);//2 == mag, 1 == 1kurs
            dic.Add("@IsSecond", bIsSecond);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var profs =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<int>("LicenseProgramId"),
                     Name = "(" + rw.Field<string>("LicenseProgramCode") + ") " + rw.Field<string>("LicenseProgramName")
                 }).OrderBy(x => x.Name);
            return Json(profs);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetObrazPrograms(string prof, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            Util.SetThreadCultureByCookies(Request.Cookies);
            string lang = Util.GetCurrentThreadLanguage();

            int iStudyFormId;
            int iStudyBasisId;
            if (!int.TryParse(studyform, out iStudyFormId))
                iStudyFormId = 1;
            if (!int.TryParse(studybasis, out iStudyBasisId))
                iStudyBasisId = 1;
            int iEntryId = 1;
            if (!int.TryParse(entry, out iEntryId))
                iEntryId = 1;
            int iProfessionId = 1;
            if (!int.TryParse(prof, out iProfessionId))
                iProfessionId = 1;

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = string.Format("SELECT DISTINCT ObrazProgramId, ObrazProgramName{0} as ObrazProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
                "AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced", lang == "en" ? "Eng" : "");
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
            dic.Add("@IsSecond", bIsSecond);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var OPs = from DataRow rw in tbl.Rows
                      select new { Id = rw.Field<int>("ObrazProgramId"), Name = rw.Field<string>("ObrazProgramName") };

            return Json(new { NoFree = OPs.Count() > 0 ? false : true, List = OPs });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetSpecializations(string prof, string obrazprogram, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0")
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            int iStudyFormId;
            int iStudyBasisId;
            if (!int.TryParse(studyform, out iStudyFormId))
                iStudyFormId = 1;
            if (!int.TryParse(studybasis, out iStudyBasisId))
                iStudyBasisId = 1;
            int iEntryId = 1;
            if (!int.TryParse(entry, out iEntryId))
                iEntryId = 1;
            int iProfessionId = 1;
            if (!int.TryParse(prof, out iProfessionId))
                iProfessionId = 1;
            int iObrazProgramId = 1;
            if (!int.TryParse(obrazprogram, out iObrazProgramId))
                iObrazProgramId = 1;

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT ProfileId, ProfileName FROM qEntry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = qEntry.StudyLevelId WHERE StudyFormId=@StudyFormId " +
                "AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@StudyLevelGroupId " +
                "AND qEntry.Id NOT IN (SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL) " +
                "AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@ObrazProgramId", iObrazProgramId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
            dic.Add("@IsSecond", bIsSecond);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);

            DataTable tblSpecs = Util.AbitDB.GetDataTable(query, dic);
            var Specs =
                from DataRow rw in tblSpecs.Rows
                select new { SpecId = rw.Field<Guid?>("ProfileId"), SpecName = rw.Field<string>("ProfileName") };

            var ret = new
            {
                NoFree = Specs.Count() == 0 ? true : false,
                List = Specs.Select(x => new { Id = x.SpecId, Name = x.SpecName })
            };
            return Json(ret);
            
        }

        #endregion
    }
}
