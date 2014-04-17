﻿using System;
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
    public class AbiturientNewController : Controller
    {
        public ActionResult OpenPersonalAccount()
        {
            Request.Cookies.SetThreadCultureByCookies();
            return View("PersonStartPage");
        }
        [HttpPost]
        public ActionResult OpenPersonalAccount(OpenPersonalAccountModel model)
        {
            Guid UserId;
            Util.CheckAuthCookies(Request.Cookies, out UserId);

            string param = Request.Form["Val"];
            string val = Request.Form["val_h"];
            int res = 0;

            switch (val)
            {
                case "1": { res = 1; break; } //Поступление на 1 курс гражданам РФ
                case "2": { res = 2; break; } //Поступление на 1 курс иностранным гражданам
                case "3": { res = 3; break; } //Перевод из российского университета в СПбГУ
                case "4": { res = 4; break; } //Перевод из иностранного университета в СПбГУ
                case "5": { res = 5; break; } //Восстановление в СПбГУ
                case "6": { res = 6; break; } //Перевод с платной формы обучения на бюджетную
                case "7": { res = 7; break; } //Смена образовательной программы
                case "8": { res = 8; break; } //Поступление в Академическую Гимназию
                case "9": { res = 9; break; } //Поступление в СПО
                case "10": { res = 10; break; } //Поступление в аспирантуру гражданам РФ
                case "11": { res = 11; break; } //Поступление в аспирантуру иностранным гражданам
                default: { res = 1; break; }
            }

            //создаём запись человека в базе
            string query = "SELECT COUNT(*) FROM Person WHERE Id=@Id";
            int cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", UserId } });
            if (cnt == 0)
            {
                query = "INSERT INTO Person(Id, UserId, AbiturientTypeId) VALUES (@Id, @Id, @Type)";
                Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", UserId }, { "@Type", res} });
            }

            switch (res)
            {
                case 1: { return RedirectToAction("Index"); }
                case 2: { return RedirectToAction("Index", "ForeignAbiturient"); }
                case 3: { return RedirectToAction("Index", "Transfer"); }
                case 4: { return RedirectToAction("Index", "TransferForeign"); }
                case 5: { return RedirectToAction("Index", "Recover"); }
                case 6: { return RedirectToAction("Index", "ChangeStudyForm"); }
                case 7: { return RedirectToAction("Index", "ChangeObrazProgram"); }
                case 8: { return RedirectToAction("Index", "AG"); }
                case 9: { return RedirectToAction("Index", "SPO"); }
                case 10: { return RedirectToAction("Index", "Aspirant"); }
                case 11: { return RedirectToAction("Index", "ForeignAspirant"); }
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Abiturient/
        public ActionResult Index(string step)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (Util.CheckIsNew(PersonId))
                return RedirectToAction("OpenPersonalAccount");

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

                PersonalOffice model = new PersonalOffice() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
                //////////////////////////////////////////----------Index-----////////////////////////////////////////////////////////////////////////
                if (model.Stage == 1)
                {
                    model.PersonInfo = new InfoPerson();
                    model.ContactsInfo = new ContactsPerson();
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                        PersonContacts = new PersonContacts();
                    model.PersonInfo.Surname = Server.HtmlDecode(Person.Surname);
                    model.PersonInfo.Name = Server.HtmlDecode(Person.Name);
                    model.PersonInfo.SecondName = Server.HtmlDecode(Person.SecondName);
                    model.PersonInfo.Sex = (Person.Sex ?? false) ? "M" : "F";
                    model.PersonInfo.Nationality = Person.NationalityId.ToString();
                    model.ContactsInfo.CountryId = PersonContacts.CountryId.ToString();
                    model.PersonInfo.BirthPlace = Server.HtmlDecode(Person.BirthPlace);
                    model.PersonInfo.BirthDate = Person.BirthDate.HasValue ? Person.BirthDate.Value.ToString("dd.MM.yyyy") : "";

                    model.PersonInfo.NationalityList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.ContactsInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.PersonInfo.SexList = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = LangPack.GetValue(5, model.Lang), Value = "M" }, 
                        new SelectListItem() { Text = LangPack.GetValue(6, model.Lang), Value = "F" }
                    };
                    return View("PersonalOffice_Page1", model);
                }
                //////////////////////////////////////////----------Index-----////////////////////////////////////////////////////////////////////////
                else if (model.Stage == 2)
                {

                    model.PassportInfo = new PassportPerson();
                    model.res = Util.GetRess(PersonId);
                    string strTblPsp;
                    int defaultPsp = 1;
                    switch (model.res)
                    {
                        case 1: { strTblPsp = "SELECT Id, Name FROM PassportType "; break; }
                        case 2: { strTblPsp = "SELECT Id, Name FROM PassportType "; break; }
                        case 3: { strTblPsp = "SELECT Id, Name FROM PassportType "; break; }
                        case 4: { strTblPsp = "SELECT Id, Name FROM PassportType WHERE IsApprovedForeign=1"; defaultPsp = 2; break; }
                        default: { strTblPsp = "SELECT Id, Name FROM PassportType "; break; }
                    }
                    DataTable tblPsp = Util.AbitDB.GetDataTable(strTblPsp, null);
                    model.PassportInfo.PassportTypeList =
                        (from DataRow rw in tblPsp.Rows
                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>("Name") }).
                        ToList();
                    model.PassportInfo.PassportType = (Person.PassportTypeId ?? defaultPsp).ToString();
                    model.PassportInfo.PassportSeries = Server.HtmlDecode(Person.PassportSeries);
                    model.PassportInfo.PassportNumber = Server.HtmlDecode(Person.PassportNumber);
                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(Person.PassportAuthor);
                    model.PassportInfo.PassportDate = Person.PassportDate.HasValue ? Person.PassportDate.Value.ToString("dd.MM.yyyy") : "";
                    model.PassportInfo.PassportCode = Server.HtmlDecode(Person.PassportCode);
                    if (model.res == 4)
                    {
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
                    model.Files = Util.GetFileList(PersonId, "1");
                    model.FileTypes = Util.FileTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

                    return View("PersonalOffice_Page2", model);
                }
                //////////////////////////////////////////----------Index-----////////////////////////////////////////////////////////////////////////
                else if (model.Stage == 3)
                {
                    model.ContactsInfo = new ContactsPerson();
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                        PersonContacts = new PersonContacts();

                    model.res = Util.GetRess(PersonId);
                    model.ContactsInfo.MainPhone = Server.HtmlDecode(PersonContacts.Phone);
                    model.ContactsInfo.SecondPhone = Server.HtmlDecode(PersonContacts.Mobiles);
                    model.ContactsInfo.CountryId = PersonContacts.CountryId.ToString();
                    model.ContactsInfo.RegionId = PersonContacts.RegionId.ToString();

                    model.ContactsInfo.PostIndex = Server.HtmlDecode(PersonContacts.Code);
                    model.ContactsInfo.City = Server.HtmlDecode(PersonContacts.City);
                    model.ContactsInfo.Street = Server.HtmlDecode(PersonContacts.Street);
                    model.ContactsInfo.House = Server.HtmlDecode(PersonContacts.House);
                    model.ContactsInfo.Korpus = Server.HtmlDecode(PersonContacts.Korpus);
                    model.ContactsInfo.Flat = Server.HtmlDecode(PersonContacts.Flat);

                    model.ContactsInfo.PostIndexReal = Server.HtmlDecode(PersonContacts.CodeReal);
                    model.ContactsInfo.CityReal = Server.HtmlDecode(PersonContacts.CityReal);
                    model.ContactsInfo.StreetReal = Server.HtmlDecode(PersonContacts.StreetReal);
                    model.ContactsInfo.HouseReal = Server.HtmlDecode(PersonContacts.HouseReal);
                    model.ContactsInfo.KorpusReal = Server.HtmlDecode(PersonContacts.KorpusReal);
                    model.ContactsInfo.FlatReal = Server.HtmlDecode(PersonContacts.FlatReal);

                    model.ContactsInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

                    query = "SELECT Id, Name FROM Region WHERE RegionNumber IS NOT NULL";
                    model.ContactsInfo.RegionList =
                        (from DataRow rw in Util.AbitDB.GetDataTable(query, null).Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();
                    return View("PersonalOffice_Page3", model);
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

                    model.EducationInfo.QualificationList = Util.QualifaicationAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.SchoolTypeList = Util.SchoolTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.StudyFormList = Util.StudyFormAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.LanguageList = Util.LanguagesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();

                    query = "SELECT Id, Name FROM SchoolTypeAll";
                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.EducationInfo.SchoolTypeList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    model.EducationInfo.SchoolName = Server.HtmlDecode(PersonEducationDocument.SchoolName);
                    model.EducationInfo.SchoolNumber = Server.HtmlDecode(PersonEducationDocument.SchoolNum);
                    model.EducationInfo.SchoolExitYear = Server.HtmlDecode(PersonEducationDocument.SchoolExitYear);
                    model.EducationInfo.SchoolCity = Server.HtmlDecode(PersonEducationDocument.SchoolCity);

                    model.EducationInfo.AvgMark = PersonEducationDocument.AvgMark.HasValue ? PersonEducationDocument.AvgMark.Value.ToString() : "";
                    model.EducationInfo.IsExcellent = PersonEducationDocument.IsExcellent ?? false;
                    model.EducationInfo.StartEnglish = PersonEducationDocument.StartEnglish ?? false;
                    model.EducationInfo.EnglishMark = PersonEducationDocument.EnglishMark.ToString();

                    model.EducationInfo.HasTRKI = PersonEducationDocument.HasTRKI ?? false;
                    model.EducationInfo.TRKICertificateNumber = PersonEducationDocument.TRKICertificateNumber;
                    model.EducationInfo.IsEqual = PersonEducationDocument.IsEqual ?? false;
                    model.EducationInfo.EqualityDocumentNumber = PersonEducationDocument.EqualDocumentNumber;
                    // добавить сортировку по Name
                    model.EducationInfo.SchoolExitClassList = context.SchoolExitClass
                        .Select(x => new { x.Id, x.Name }).ToList()
                        .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

                    model.EducationInfo.VuzAdditionalTypeList = context.VuzAdditionalType.Select(x => new { x.Id, x.Name }).ToList()
                        .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

                    model.EducationInfo.RegionList = Util.RegionsAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.RegionEducId = Server.HtmlDecode(PersonEducationDocument.RegionEducId.ToString());

                    model.EducationInfo.SchoolExitClassId = Server.HtmlDecode(PersonEducationDocument.SchoolExitClassId.ToString());
                    model.EducationInfo.VuzAdditionalTypeId = Server.HtmlDecode(PersonEducationDocument.VuzAdditionalTypeId.ToString());

                    model.EducationInfo.AttestatRegion = Server.HtmlDecode(PersonEducationDocument.AttestatRegion);

                    model.EducationInfo.HEExitYear = Server.HtmlDecode(PersonHighEducationInfo.ExitYear.ToString());
                    model.EducationInfo.HEEntryYear = Server.HtmlDecode(PersonHighEducationInfo.EntryYear.ToString());

                    model.EducationInfo.DiplomSeries = Server.HtmlDecode(PersonEducationDocument.SchoolTypeId == 1 ? PersonEducationDocument.AttestatSeries : PersonEducationDocument.Series);
                    model.EducationInfo.DiplomNumber = Server.HtmlDecode(PersonEducationDocument.SchoolTypeId == 1 ? PersonEducationDocument.AttestatNumber : PersonEducationDocument.Number);
                    model.EducationInfo.ProgramName = Server.HtmlDecode(PersonHighEducationInfo.ProgramName);
                    model.EducationInfo.DiplomTheme = Server.HtmlDecode(PersonHighEducationInfo.DiplomaTheme);
                    
                    model.EducationInfo.SchoolTypeId = (PersonEducationDocument.SchoolTypeId ?? 1).ToString();
                    model.EducationInfo.PersonStudyForm = (PersonHighEducationInfo.StudyFormId ?? 1).ToString();
                    model.EducationInfo.PersonQualification = (PersonHighEducationInfo.QualificationId ?? 1).ToString();
                    model.EducationInfo.LanguageId = (PersonEducationDocument.LanguageId ?? 1).ToString();
                    model.EducationInfo.CountryEducId = (PersonEducationDocument.CountryEducId ?? 193).ToString();

                    string qEgeMarks = "SELECT EgeMark.Id, EgeCertificate.Number, EgeExam.Name, EgeMark.Value FROM Person " +
                        " INNER JOIN EgeCertificate ON EgeCertificate.PersonId = Person.Id INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
                        " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE Person.Id=@Id";
                    DataTable tblEge = Util.AbitDB.GetDataTable(qEgeMarks, new Dictionary<string, object>() { { "@Id", PersonId } });

                    model.EducationInfo.EgeMarks = new List<EgeMarkModel>();
                    model.EducationInfo.EgeMarks =
                        (from DataRow rw in tblEge.Rows
                         select new EgeMarkModel()
                         {
                             Id = rw.Field<Guid>("Id"),
                             CertificateNum = rw.Field<string>("Number"),
                             ExamName = rw.Field<string>("Name"),
                             Value = rw.Field<int?>("Value").ToString()
                         }).ToList();
                    return View("PersonalOffice_Page4", model);
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
                    DataTable tblPSW = Util.AbitDB.GetDataTable(qPSW, new Dictionary<string, object>() { { "@Id", PersonId } });

                    model.WorkInfo.pScWorks =
                        (from DataRow rw in tblPSW.Rows
                         select new ScienceWorkInformation()
                         {
                             Id = rw.Field<Guid>("Id"),
                             ScienceWorkType = rw.Field<string>("Name"),
                             ScienceWorkInfo = rw.Field<string>("WorkInfo")
                         }).ToList();

                    string qPW = "SELECT Id, WorkPlace, Stage, WorkProfession, WorkSpecifications FROM PersonWork WHERE PersonId=@Id";
                    DataTable tblPW = Util.AbitDB.GetDataTable(qPW, new Dictionary<string, object>() { { "@Id", PersonId } });

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

                    model.PrivelegeInfo = new PersonPrivileges();

                    query = "SELECT Olympiads.Id, OlympType.Name, Olympiads.DocumentSeries, Olympiads.DocumentNumber, Olympiads.DocumentDate" +
                        " FROM Olympiads INNER JOIN OlympType ON OlympType.Id=Olympiads.OlympTypeId WHERE Olympiads.PersonId=@Id";
                    DataTable _tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", PersonId } });
                    model.PrivelegeInfo.pOlympiads =
                        (from DataRow rw in _tbl.Rows
                         select new OlympiadInformation()
                         {
                             Id = rw.Field<Guid>("Id"),
                             OlympType = rw.Field<string>("Name"),
                             DocumentSeries = rw.Field<string>("DocumentSeries"),
                             DocumentNumber = rw.Field<string>("DocumentNumber"),
                             DocumentDate = rw.Field<DateTime?>("DocumentDate").HasValue ? rw.Field<DateTime>("DocumentDate") : DateTime.Now
                         }).ToList();

                    query = "SELECT Id, Name FROM OlympName";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympNameList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    query = "SELECT Id, Name FROM OlympType";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympTypeList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    query = "SELECT Id, Name FROM OlympSubject";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympSubjectList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    query = "SELECT Id, Name FROM OlympValue";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympValueList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    return View("PersonalOffice_Page5", model);
                }
                else //if (model.Stage == 6)
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
                        ContactPerson = Server.HtmlDecode(AddInfo.Parents),
                        ReturnDocumentTypeId = Server.HtmlDecode((AddInfo.ReturnDocumentTypeId ?? 1).ToString()),
                        ReturnDocumentTypeList = context.ReturnDocumentType.Select(x => new { x.Id, x.Name }).ToList().Select(x => new SelectListItem() { Value=x.Id.ToString(), Text = x.Name }).ToList()
                    };
                    return View("PersonalOffice_Page6", model);
                }
                //return View("PersonalOffice_Page", model);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NextStep(PersonalOffice model)
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
                        return RedirectToAction("Index", "AbiturientNew", new RouteValueDictionary() { { "step", model.Stage } });
                    else
                        return RedirectToAction("Main", "AbiturientNew");
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

                    int iCountryId = 0;
                    if (!int.TryParse(model.ContactsInfo.CountryId, out iCountryId))
                        iCountryId = 193;//Russia

                    bool bIns = false;
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                    {
                        PersonContacts = new PersonContacts();
                        PersonContacts.PersonId = PersonId;
                        bIns = true;
                    }

                    Person.Surname = model.PersonInfo.Surname;
                    Person.Name = model.PersonInfo.Name;
                    Person.SecondName = model.PersonInfo.SecondName;
                    Person.BirthDate = bdate;
                    Person.BirthPlace = model.PersonInfo.BirthPlace;
                    Person.NationalityId = NationalityId;
                    Person.Sex = model.PersonInfo.Sex == "M" ? true : false;
                    Person.RegistrationStage = iRegStage < 2 ? 2 : iRegStage;
                    PersonContacts.CountryId = iCountryId;
                    if (bIns)
                        context.PersonContacts.AddObject(PersonContacts);
                    context.SaveChanges();
                }

                else if (model.Stage == 2)
                {
                    int iPassportType = 1;
                    if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
                        iPassportType = 1;
                    int iVisaCountryId;

                    DateTime? dtPassportDate, dtPassportValid;
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
                    catch { dtPassportValid = null; }

                    if (dtPassportDate.Value.Date > DateTime.Now.Date)
                        dtPassportDate = DateTime.Now.Date;

                    Person.PassportTypeId = iPassportType;
                    Person.PassportSeries = model.PassportInfo.PassportSeries;
                    Person.PassportNumber = model.PassportInfo.PassportNumber;
                    Person.PassportAuthor = model.PassportInfo.PassportAuthor;
                    Person.PassportDate = dtPassportDate;
                    Person.PassportCode = model.PassportInfo.PassportCode;
                    Person.PassportValid = dtPassportValid;
                    try
                    {
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
                    }
                    catch { }

                    Person.RegistrationStage = iRegStage < 3 ? 3 : iRegStage;

                    context.SaveChanges();
                    if (Request.Form["SubmitSave"] != null)
                    {
                        return RedirectToAction("Index", "AbiturientNew", new RouteValueDictionary() { { "step", model.Stage } });
                    }
                }
                else if (model.Stage == 3)
                {
                    int iCountryId = 0;
                    if (!int.TryParse(model.ContactsInfo.CountryId, out iCountryId))
                        iCountryId = 193;//Russia

                    int iRegionId = 0;
                    if (!int.TryParse(model.ContactsInfo.RegionId, out iRegionId))
                        iRegionId = 0;//unnamed

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

                    string sCity = model.ContactsInfo.City;
                    string sStreet = model.ContactsInfo.Street;
                    string sHouse = model.ContactsInfo.House;
                    string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(model.ContactsInfo.RegionId);
                    PersonContacts.KladrCode = Util.GetKladrCodeByAddress(sRegionKladrCode, sCity, sStreet, sHouse);

                    PersonContacts.Phone = model.ContactsInfo.MainPhone;
                    PersonContacts.Mobiles = model.ContactsInfo.SecondPhone;

                    PersonContacts.RegionId = iRegionId;
                    PersonContacts.Code = model.ContactsInfo.PostIndex;
                    PersonContacts.City = sCity;
                    PersonContacts.Street = sStreet;
                    PersonContacts.House = sHouse;
                    PersonContacts.Korpus = model.ContactsInfo.Korpus;
                    PersonContacts.Flat = model.ContactsInfo.Flat;
                    PersonContacts.CodeReal = model.ContactsInfo.PostIndexReal;
                    PersonContacts.CityReal = model.ContactsInfo.CityReal;
                    PersonContacts.StreetReal = model.ContactsInfo.StreetReal;
                    PersonContacts.HouseReal = model.ContactsInfo.HouseReal;
                    PersonContacts.KorpusReal = model.ContactsInfo.KorpusReal;
                    PersonContacts.FlatReal = model.ContactsInfo.FlatReal;

                    if (bIns)
                        context.PersonContacts.AddObject(PersonContacts);

                    Person.RegistrationStage = iRegStage < 4 ? 4 : iRegStage;

                    context.SaveChanges();
                }
                else if (model.Stage == 4)//образование
                {
                    int iSchoolTypeId;
                    if (!int.TryParse(model.EducationInfo.SchoolTypeId, out iSchoolTypeId))
                        iSchoolTypeId = 1;

                    int SchoolExitYear;
                    if (!int.TryParse(model.EducationInfo.SchoolExitYear, out SchoolExitYear))
                        SchoolExitYear = DateTime.Now.Year;

                    int iCountryEducId;
                    if (!int.TryParse(model.EducationInfo.CountryEducId, out iCountryEducId))
                        iCountryEducId = 1;

                    int? iRegionEducId;
                    int iRegionEducId_tmp;
                    if (!int.TryParse(model.EducationInfo.RegionEducId, out iRegionEducId_tmp))
                        iRegionEducId = 1;
                    iRegionEducId = iRegionEducId_tmp;

                    int iLanguageId;
                    if (!int.TryParse(model.EducationInfo.LanguageId, out iLanguageId))
                        iLanguageId = 1;

                    int iVuzAddTypeId;
                    if (!int.TryParse(model.EducationInfo.VuzAdditionalTypeId, out iVuzAddTypeId))
                        iVuzAddTypeId = 1;

                    int iSchoolExitClassId;
                    if (!int.TryParse(model.EducationInfo.SchoolExitClassId, out iSchoolExitClassId))
                        iSchoolExitClassId = 1;

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
                    PersonEducationDocument.SchoolNum = model.EducationInfo.SchoolNumber;
                    PersonEducationDocument.SchoolCity = model.EducationInfo.SchoolCity;
                    PersonEducationDocument.AvgMark = (avgBall == 0.0 ? null : (double?)avgBall);
                    PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
                    PersonEducationDocument.CountryEducId = iCountryEducId;
                    PersonEducationDocument.LanguageId = iLanguageId;
                    PersonEducationDocument.IsExcellent = model.EducationInfo.IsExcellent;
                    PersonEducationDocument.EnglishMark = EnglishMark;
                    PersonEducationDocument.StartEnglish = model.EducationInfo.StartEnglish;

                    PersonEducationDocument.HasTRKI = model.EducationInfo.HasTRKI;
                    PersonEducationDocument.TRKICertificateNumber = model.EducationInfo.TRKICertificateNumber;
                    PersonEducationDocument.IsEqual = model.EducationInfo.IsEqual;
                    PersonEducationDocument.EqualDocumentNumber = model.EducationInfo.EqualityDocumentNumber;

                    if (iCountryEducId != 193)
                        iRegionEducId = context.Country.Where(x => x.Id == iCountryEducId).Select(x => x.RegionId).FirstOrDefault();

                    PersonEducationDocument.RegionEducId = iRegionEducId;
                    PersonEducationDocument.VuzAdditionalTypeId = iVuzAddTypeId;
                    PersonEducationDocument.SchoolExitClassId = iSchoolExitClassId;

                    if (iSchoolTypeId == 1)//Pure School
                    {
                        PersonEducationDocument.AttestatRegion = model.EducationInfo.AttestatRegion;
                        PersonEducationDocument.AttestatSeries = model.EducationInfo.DiplomSeries;
                        PersonEducationDocument.AttestatNumber = model.EducationInfo.DiplomNumber;
                    }
                    else
                    {
                        PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
                        PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;
                    }

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

                    int iReturnDocumentTypeId;
                    if (!int.TryParse(model.AddInfo.ReturnDocumentTypeId, out iReturnDocumentTypeId))
                        iReturnDocumentTypeId = 1;

                    PersonAddInfo.AddInfo = model.AddInfo.ExtraInfo;
                    PersonAddInfo.Parents = model.AddInfo.ContactPerson;
                    PersonAddInfo.HasPrivileges = model.AddInfo.HasPrivileges;
                    PersonAddInfo.HostelAbit = model.AddInfo.HostelAbit;
                    PersonAddInfo.ReturnDocumentTypeId = iReturnDocumentTypeId;

                    if (Person.RegistrationStage <= 6)
                        Person.RegistrationStage = 100;

                    if (bIns)
                        context.PersonAddInfo.AddObject(PersonAddInfo);

                    context.SaveChanges();
                }

                if (model.Stage < 6)
                {
                    model.Stage++;
                    return RedirectToAction("Index", "AbiturientNew", new RouteValueDictionary() { { "step", model.Stage } });
                }
                else
                    return RedirectToAction("Main", "AbiturientNew");
            }
        }

        public ActionResult Main()
        {
            if (Request.Url.AbsoluteUri.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == -1 && Util.bUseRedirection &&
                Request.Url.AbsoluteUri.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) == -1)
                return Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));

            //Validation
            Guid PersonID;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
                return RedirectToAction("LogOn", "Account");

            if (Util.CheckIsNew(PersonID))
            {
                if (Util.CreateNew(PersonID))
                    return RedirectToAction("Index");
            }

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
                if (PersonInfo == null)
                    return RedirectToAction("Index");

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
                        Profession = app.LicenseProgramName,
                        ObrazProgram = app.ObrazProgramName,
                        Specialization = app.ProfileName,
                        Priority = app.Priority.HasValue ? app.Priority.Value.ToString() : "1",
                        Enabled = app.Enabled ?? true,
                        StudyBasis = app.StudyBasisName,
                        StudyForm = app.StudyFormName
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
        public ActionResult NewApplication_AG(params string[] errors)
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
                  
                AG_ApplicationModel model = new AG_ApplicationModel();

                //

                

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new Dictionary<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                /*int iAG_EntryClassId = (int)Util.AbitDB.GetValue("SELECT SchoolExitClassId FROM PersonSchoolInfo WHERE PersonId=@Id",
                    new Dictionary<string, object>() { { "@Id", PersonId } });*/
                
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new Dictionary<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId == 4)
                {
                    model.Enabled = false; 
                }
                else
                {
                    // ссылка на объект  и пр., когда SchoolExitClassId = null
                    DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new Dictionary<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                    {
                        model.Enabled = false;
                    }
                    else
                    {
                        int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                        if (iAG_EntryClassValue > 9)//В АГ могут поступать только 7-8-9 классники
                        {
                            model.Enabled = false;
                        }
                        else
                        {
                            model.Enabled = true;
                            string query = "SELECT DISTINCT ProgramId, ProgramName, EntryClassName FROM AG_qEntry WHERE EntryClassId=@ClassId";
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("@PersonId", PersonId);
                            dic.Add("@ClassId", iAG_EntryClassId);
                            tbl = Util.AbitDB.GetDataTable(query, dic);
                            model.Professions = (from DataRow rw in tbl.Rows
                                                 select new SelectListItem()
                                                 {
                                                     Value = rw.Field<int>("ProgramId").ToString(),
                                                     Text = rw.Field<string>("ProgramName")
                                                 }).ToList();
                            model.EntryClassId = iAG_EntryClassId;
                            model.EntryClassName = tbl.Rows[0].Field<string>("EntryClassName");
                            //пока что так
                            model.MaxBlocks = iAG_EntryClassValue == 9 ? 2 : 1;
                        }
                    }
                }
                return View("NewApplication_AG", model);
            }
        }

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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new Dictionary<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

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

                string query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry ORDER BY 1";
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

                query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry ORDER BY 1";
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
        public ActionResult NewApplicationSelect()
        { 
            string val = Request.Form["val_h"]; 
            switch (val)
            {
                case "1": { return RedirectToAction("page404", "AbiturientNew"); } //Поступление на 1 курс гражданам РФ
                case "2": { return RedirectToAction("page404", "AbiturientNew"); } //Поступление на 1 курс иностранным гражданам
                case "3": { return RedirectToAction("page404", "AbiturientNew"); } //Перевод из российского университета в СПбГУ
                case "4": { return RedirectToAction("page404", "AbiturientNew"); } //Перевод из иностранного университета в СПбГУ
                case "5": { return RedirectToAction("page404", "AbiturientNew"); } //Восстановление в СПбГУ
                case "6": { return RedirectToAction("page404", "AbiturientNew"); } //Перевод с платной формы обучения на бюджетную
                case "7": { return RedirectToAction("page404", "AbiturientNew"); } //Смена образовательной программы
                case "8": { return RedirectToAction("NewApplication_AG", "AbiturientNew"); } //Поступление в Академическую Гимназию
                case "9": { return RedirectToAction("page404", "AbiturientNew"); } //Поступление в СПО
                case "10": { return RedirectToAction("page404", "AbiturientNew"); } //Поступление в аспирантуру гражданам РФ
                case "11": { return RedirectToAction("page404", "AbiturientNew"); } //Поступление в аспирантуру иностранным гражданам
                default: { return RedirectToAction("page404", "AbiturientNew"); }
            }  
        }

        public ActionResult page404(params string[] errors)
        {
            return View("page404"); 
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

            DateTime timeX = new DateTime(2013, 6, 20, 10, 0, 0);//20-06-2013 10:00:00
            if (iEntry != 2 && DateTime.Now < timeX )
            {
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на 1 курс начнётся 20 июня в 10:00 МСК" } });
            }

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

            query = "SELECT DateOfClose FROM [Entry] WHERE Id=@Id";
            DateTime DateOfClose = (DateTime)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", EntryId } });

            if (DateTime.Now > DateOfClose)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на данное направление прекращена " + DateOfClose.ToString("dd.MM.yyyy") } });

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
                byte[] pdfData = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
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

        [HttpPost]
        public ActionResult NewAppAG()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string profession = Request.Form["Professions"];
            string Entryclass = Request.Form["EntryClassId"];
            bool needHostel = string.IsNullOrEmpty(Request.Form["NeedHostel"]) ? false : true;
            string profileid = Request.Form["Profile"];
            string manualExam = Request.Form["Exam"];

            int iEntryClassId = Util.ParseSafe(Entryclass);
            int iProfession = Util.ParseSafe(profession);
            int iProfileId = Util.ParseSafe(profileid);
            int iManualExamId = Util.ParseSafe(manualExam);

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT Id, DateOfStartEntry, DateOfStopEntry FROM AG_Entry WHERE ProgramId=@ProgramId AND EntryClassId=@EntryClassId ";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@ProgramId", iProfession);
            dic.Add("@EntryClassId", iEntryClassId);
            if (iProfileId != 0)
            {
                query += " AND ProfileId=@ProfileId ";
                dic.Add("@ProfileId", iProfileId);
            }
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count > 1)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Неоднозначный выбор учебного плана (" + tbl.Rows.Count + ")" } });
            if (tbl.Rows.Count == 0)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Не найден учебный план" } });

            Guid EntryId = tbl.Rows[0].Field<Guid>("Id");
            DateTime? timeOfStart = tbl.Rows[0].Field<DateTime?>("DateOfStartEntry");
            DateTime? timeOfStop = tbl.Rows[0].Field<DateTime?>("DateOfStopEntry");

            if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Приём заявлений на данное направление ещё не открыт." } });

            if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Приём заявлений на данное направление закрыт." } });

            query = "SELECT EntryId FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            var eIds =
                from DataRow rw in tbl.Rows
                select rw.Field<Guid>("EntryId");
            if (eIds.Contains(EntryId))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

            DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled=@Enabled",
                new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
            int? PriorMax =
                (from DataRow rw in tblPriors.Rows
                 select rw.Field<int?>("Priority")).Max();

            Guid appId = Guid.NewGuid();
            query = "INSERT INTO [AG_Application] (Id, PersonId, EntryId, HostelEduc, Enabled, DateOfStart, ManualExamId, Priority) " +
                "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @DateOfStart, @ManualExamId, @Priority)";
            Dictionary<string, object> prms = new Dictionary<string, object>();
            prms.Add("@Id", appId);
            prms.Add("@PersonId", PersonId);
            prms.Add("@EntryId", EntryId);
            prms.Add("@HostelEduc", needHostel);
            prms.Add("@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1);
            prms.Add("@Enabled", true);
            prms.Add("@DateOfStart", DateTime.Now);
            prms.AddItem("@ManualExamId", iManualExamId == 0 ? null : (int?)iManualExamId);

            Util.AbitDB.ExecuteQuery(query, prms);

            //query = "SELECT Person.Surname, Person.Name, Person.SecondName, AG_Entry.ProgramName, AG_Entry.ObrazProgramName " +
            //    " FROM [AG_Application] INNER JOIN Person ON Person.Id=[AG_Application].PersonId " +
            //    " INNER JOIN AG_qAG_Entry ON AG_Application.AG_EntryId=AG_qAG_Entry.Id WHERE AG_Application.Id=@AppId";
            //DataTable Tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
            //var fileInfo =
            //    (from DataRow rw in Tbl.Rows
            //     select new
            //     {
            //         Surname = rw.Field<string>("Surname"),
            //         Name = rw.Field<string>("Name"),
            //         SecondName = rw.Field<string>("SecondName"),
            //         Profession = rw.Field<string>("ProgramName"),
            //         ObrazProgram = rw.Field<string>("ObrazProgramName")
            //     }).FirstOrDefault();

            //byte[] pdfData = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
            //DateTime dateTime = DateTime.Now;

            //query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileExtention, FileData, FileSize, IsReadOnly, LoadDate, Comment, MimeType) " +
            //    " VALUES (@Id, @PersonId, @FileName, @FileExtention, @FileData, @FileSize, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
            //prms.Clear();
            //prms.Add("@Id", Guid.NewGuid());
            //prms.Add("@PersonId", appId);
            //prms.Add("@FileName", fileInfo.Surname + " " + fileInfo.Name.FirstOrDefault() + " - Заявление [" + dateTime.ToString("dd.MM.yyyy HH.mm.ss") + "].pdf");
            //prms.Add("@FileExtention", ".pdf");
            //prms.Add("@FileData", pdfData);
            //prms.Add("@FileSize", pdfData.Length);
            //prms.Add("@IsReadOnly", true);
            //prms.Add("@LoadDate", dateTime);
            //prms.Add("@Comment", "Заявление на направление " + fileInfo.Profession + ", " + fileInfo.ObrazProgram + " образовательная программа, " 
            //    + " от " + dateTime.ToShortDateString());
            //prms.Add("@MimeType", "[AG_Application]/pdf");
            //Util.AbitDB.ExecuteQuery(query, prms);
            return RedirectToAction("Main");
        }

        public ActionResult MotivateMail()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            string query = "SELECT Id, Priority, Profession, ObrazProgram, Specialization FROM extApplicationAll WHERE PersonId=@PersonId AND Enabled=@Enabled";
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
                            Profession = rw.Field<string>("Profession"),
                            ObrazProgram = rw.Field<string>("ObrazProgram"),
                            Specialization = rw.Field<string>("Specialization")
                        }).ToList();

            MotivateMailModel mdl = new MotivateMailModel()
            {
                Apps = apps
            };
            return View(mdl);
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

        [HttpPost]
        public ActionResult ChangePriority(MotivateMailModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            int prior = 0;
            string[] allKeys = Request.Form.AllKeys;
            foreach (string key in allKeys)
            {
                Guid appId;
                if (!Guid.TryParse(key, out appId))
                    continue;

                string query = "UPDATE [Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.AddItem("@Priority", ++prior);
                dic.AddItem("@Id", appId);
                dic.AddItem("@PersonId", PersonId);

                try
                {
                    Util.AbitDB.ExecuteQuery(query, dic);
                }
                catch { }
            }
            return RedirectToAction("PriorityChanger");
        }

        public ActionResult AddFiles()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            string query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

            List<AppendedFile> lst =
                (from DataRow rw in tbl.Rows
                 select new AppendedFile() { Id = rw.Field<Guid>("Id"), FileName = rw.Field<string>("FileName"), FileSize = rw.Field<int>("FileSize"), Comment = rw.Field<string>("Comment") })
                .ToList();

            AppendFilesModel model = new AppendFilesModel() { Files = lst };
            return View(model);
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

                Util.SetThreadCultureByCookies(Request.Cookies);
                AppendFilesModel model = new AppendFilesModel();
                model.Files = Util.GetFileList(PersonId);
                model.FileTypes = Util.FileTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult AddSharedFile()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("Ошибка авторизации");

            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
                return Json(Resources.ServerMessages.EmptyFileError);

            string fileName = Request.Files["File"].FileName;

            int lastSlashPos = 0;
            lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
            if (lastSlashPos > 0)
                fileName = fileName.Substring(lastSlashPos + 1);
            int PersonFileTypeId = Convert.ToInt32(Request.Form["FileTypeId"]);

            string fileComment = Request.Form["Comment"];
            int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
            byte[] fileData = new byte[fileSize];
            //читаем данные из ПОСТа
            Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
            string fileext = "";
            try
            {
                fileext = fileName.Substring(fileName.LastIndexOf('.'));
            }
            catch
            {
                fileext = "";
            }
            //////////////////////////////////////////////// 
            /*  if (Util.GetMimeFromExtention(fileext).StartsWith("image"))
              {

                  try
                  {
                      System.Drawing.Image image;
                      using (MemoryStream inStream = new MemoryStream())
                      {
                          inStream.Write(fileData, 0, fileSize);
                          image = System.Drawing.Bitmap.FromStream(inStream);
                      }
                      int lg = image.Size.Width;
                      int hg = image.Size.Height;
                      if ((lg < 600) || (hg < 600))
                          return Json(Resources.ServerMessages.SizeFileError);
                  }
                  catch
                  { 
                  } 
              }
              */

            ////////////////////////////////////////////////
            try
            {
                string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, LoadDate, Comment, MimeType, PersonFileTypeId) " +
                    " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @LoadDate, @Comment, @MimeType, @PersonFileTypeId)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@PersonId", PersonId);
                dic.Add("@FileName", fileName);
                dic.Add("@FileData", fileData);
                dic.Add("@FileSize", fileSize);
                dic.Add("@FileExtention", fileext);
                dic.Add("@LoadDate", DateTime.Now);
                dic.Add("@Comment", fileComment);
                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));
                dic.Add("@PersonFileTypeId", PersonFileTypeId);
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }
            if (Request.Form["Stage"] != null)
            {
                string stage = Convert.ToString(Request.Form["Stage"]);
                return RedirectToAction("Index", "AbiturientNew", new RouteValueDictionary() { { "step", stage } });
            }
            return RedirectToAction("AddSharedFiles");
        }

        [HttpPost]
        public ActionResult AddFile()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("Ошибка авторизации");

            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
                return Json("Файл не приложен или пуст");

            string fileName = Request.Files["File"].FileName;
            int lastSlashPos = 0;
            lastSlashPos = fileName.LastIndexOfAny(new char[] { '\\', '/' });
            if (lastSlashPos > 0)
                fileName = fileName.Substring(lastSlashPos);
            string fileComment = Request.Form["Comment"];
            int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
            byte[] fileData = new byte[fileSize];
            //читаем данные из ПОСТа
            Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
            string fileext = "";
            try
            {
                fileext = fileName.Substring(fileName.LastIndexOf('.'));
            }
            catch
            {
                fileext = "";
            }

            try
            {
                string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType) " +
                    " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@PersonId", PersonId);
                dic.Add("@FileName", fileName);
                dic.Add("@FileData", fileData);
                dic.Add("@FileSize", fileSize);
                dic.Add("@FileExtention", fileext);
                dic.Add("@IsReadOnly", false);
                dic.Add("@LoadDate", DateTime.Now);
                dic.Add("@Comment", fileComment);
                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }

            return RedirectToAction("AddFiles");
        }

        public ActionResult GetFile(string id)
        {
            Guid FileId = new Guid();
            if (!Guid.TryParse(id, out FileId))
                return Content("Некорректный идентификатор файла");

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Content("Authorization required");

            DataTable tbl = Util.AbitDB.GetDataTable("SELECT FileName, FileData, MimeType, FileExtention FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id",
                new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", FileId } });

            if (tbl.Rows.Count == 0)
                return Content("Файл не найден");

            string fileName = tbl.Rows[0].Field<string>("FileName");
            string contentType = tbl.Rows[0].Field<string>("MimeType");
            byte[] content = tbl.Rows[0].Field<byte[]>("FileData");
            string ext = tbl.Rows[0].Field<string>("FileExtention");


            if (string.IsNullOrEmpty(contentType))
            {
                if (string.IsNullOrEmpty(ext))
                    contentType = "application/octet-stream";
                else
                    contentType = Util.GetMimeFromExtention(ext);
            }
            bool openMenu = true;
            if (ext.IndexOf("jpg", StringComparison.OrdinalIgnoreCase) != -1)
                openMenu = false;
            if (ext.IndexOf("jpeg", StringComparison.OrdinalIgnoreCase) != -1)
                openMenu = false;
            if (ext.IndexOf("gif", StringComparison.OrdinalIgnoreCase) != -1)
                openMenu = false;
            if (ext.IndexOf("png", StringComparison.OrdinalIgnoreCase) != -1)
                openMenu = false;

            //var file = Util.ABDB.PersonFile.Where(x => x.PersonId == PersonId && x.Id == FileId).
            //    Select(x => new { RealName = x.FileName, x.FileData }).FirstOrDefault();

            try
            {
                if (openMenu)
                    return File(content, contentType, fileName);
                else
                    return File(content, contentType);
            }
            catch
            {
                return Content("Ошибка при чтении файла");
            }
        }

        public ActionResult GetMotivationMailPDF(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return Content("Authorization required");
            string fontspath = Server.MapPath("~/Templates/times.ttf");
            return File(PDFUtils.GetMotivateMail(id, fontspath), "application/pdf", "MotivateEdit.pdf");
        }

        public ActionResult FilesList(string id)
        {
            Guid PersonId;
            Guid ApplicationId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Content(Resources.ServerMessages.AuthorizationRequired);
            if (!Guid.TryParse(id, out ApplicationId))
                return Content(Resources.ServerMessages.IncorrectGUID);
            string fontspath = Server.MapPath("~/Templates/times.ttf");
            return File(PDFUtils.GetFilesList(PersonId, ApplicationId, fontspath), "application/pdf", "FilesList.pdf");
        }

        public ActionResult MotivatePost()
        {
            string appId = Request.Form["AppId"];
            string mailId = Request.Form["MailId"];
            string mailText = Request.Form["MailText"];

            Guid Id;
            Guid.TryParse(mailId, out Id);
            Guid ApplicationId;
            Guid.TryParse(appId, out ApplicationId);

            string query = "";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (Id == Guid.Empty && ApplicationId == Guid.Empty)
                return RedirectToAction("Main");
            else if (Id == Guid.Empty)
            {
                query = "INSERT INTO MotivateMail (Id, ApplicationId, MailText) VALUES (@Id, @ApplicationId, @MailText)";
                dic.Add("@Id", Guid.NewGuid());
                dic.AddItem("@ApplicationId", ApplicationId);
                dic.AddItem("@MailText", mailText);
            }
            else
            {
                query = "UPDATE MotivateMail SET MailText=@MailText WHERE Id=@Id";
                dic.Add("@Id", Guid.NewGuid());
                dic.AddItem("@MailText", mailText);
            }

            Util.AbitDB.ExecuteQuery(query, dic);

            if (ApplicationId != Guid.Empty)
                return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", appId } });
            else
                return RedirectToAction("Main");
        }

        public ActionResult CheckEqualWithRussia(string email)
        {
            EqualWithRussiaModel model = new EqualWithRussiaModel();
            model.Email = email;
            return View(model);
        }

        public ActionResult NewApplicationRectorScholarship()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            else
            {
                NewApplicationRectorScholarshipModel mdl = new NewApplicationRectorScholarshipModel();
                mdl.Files = GetRectorScholarshipFileList(PersonId);
                return View(mdl);
            }
        }

        private List<AppendedFile> GetRectorScholarshipFileList(Guid PersonId)
        {
            string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM RectorScholarshipApplicationFile WHERE RectorScholarshipApplicationId=@AppId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", PersonId } });
            var lFiles =
                (from DataRow row in tbl.Rows
                 select new AppendedFile()
                 {
                     Id = row.Field<Guid>("Id"),
                     FileName = row.Field<string>("FileName"),
                     FileSize = row.Field<int>("FileSize"),
                     Comment = row.Field<string>("Comment"),
                     IsShared = false,
                     IsApproved = row.Field<bool?>("IsApproved").HasValue ?
                        row.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
                 }).ToList();

            query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            var lSharedFiles =
                (from DataRow row in tbl.Rows
                 select new AppendedFile()
                 {
                     Id = row.Field<Guid>("Id"),
                     FileName = row.Field<string>("FileName"),
                     FileSize = row.Field<int>("FileSize"),
                     Comment = row.Field<string>("Comment"),
                     IsShared = true,
                     IsApproved = row.Field<bool?>("IsApproved").HasValue ?
                        row.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
                 }).ToList();

            return lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();
        }

        [HttpPost]
        public ActionResult NewApplicationRectorScholarshipAddFile()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
                return Json(Resources.ServerMessages.EmptyFileError);

            string fileName = Request.Files["File"].FileName;
            string fileComment = Request.Form["Comment"];
            int fileSize = Convert.ToInt32(Request.Files["File"].InputStream.Length);
            byte[] fileData = new byte[fileSize];
            //читаем данные из ПОСТа
            Request.Files["File"].InputStream.Read(fileData, 0, fileSize);
            string fileext = "";
            try
            {
                fileext = fileName.Substring(fileName.LastIndexOf('.'));
            }
            catch
            {
                fileext = "";
            }

            try
            {
                string query = "INSERT INTO RectorScholarshipApplicationFile (Id, RectorScholarshipApplicationId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType, [FileTypeId]) " +
                    " VALUES (@Id, @ApplicationId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType, 1)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@ApplicationId", PersonId);
                dic.Add("@FileName", fileName);
                dic.Add("@FileData", fileData);
                dic.Add("@FileSize", fileSize);
                dic.Add("@FileExtention", fileext);
                dic.Add("@IsReadOnly", false);
                dic.Add("@LoadDate", DateTime.Now);
                dic.Add("@Comment", fileComment);
                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }

            return RedirectToAction("NewApplicationRectorScholarship");
        }

        public ActionResult NewAppRectorScholarship()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                if (context.RectorScholarshipApplication.Where(x => x.PersonId == PersonId).Count() > 0)
                {
                    NewApplicationRectorScholarshipModel mdl = new NewApplicationRectorScholarshipModel();
                    mdl.Message = "Заявление уже подавалось";
                    mdl.Files = GetRectorScholarshipFileList(PersonId);
                    return View("NewApplicationRectorScholarship", mdl);
                }

                RectorScholarshipApplication app = new RectorScholarshipApplication();
                app.PersonId = PersonId;
                app.Id = Guid.NewGuid();

                context.RectorScholarshipApplication.AddObject(app);
                context.SaveChanges();
                return View("NewApplicationRectorScholarshipSuccess");
            }
        }

        public ActionResult CheckEqualWithRussia2(EqualWithRussiaModel model)
        {
            return View("CheckEqualWithRussia", model);
        }

        [HttpPost]
        public ActionResult SetEqualWithRussia(EqualWithRussiaModel model)
        {

            string email = model.Email;
            string remixPwd = Util.MD5Str(model.Password);

            model.Errors = "";

            string query = "SELECT Id FROM [User] WHERE Password=@Password AND Email=@Email";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Password", remixPwd);
            dic.Add("@Email", email);
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

            if (tbl.Rows.Count == 0)
            {
                model.Errors = "Неверно введён пароль";
                return CheckEqualWithRussia2(model);
            }
            if (tbl.Rows.Count == 1)
            {
                query = "UPDATE Person SET AbiturientTypeId=1 WHERE Id=@Id";
                Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", tbl.Rows[0].Field<Guid?>("Id") } });
                return View("SetEqualWithRussia_Success");
            }

            return CheckEqualWithRussia2(model);
        }

        public ActionResult EqualWithRussiaSendEmail()
        {
            string query = @"
SELECT [User].Email
  FROM [OnlinePriem2012].[dbo].[Person]
  INNER JOIN [OnlinePriem2012].[dbo].[User] ON [User].Id = Person.Id
  INNER JOIN [OnlinePriem2012].[dbo].Country ON Country.Id = Person.NationalityId
  WHERE Country.PriemDictionaryId IN (5,7,8,9,10,11,12,13,14,15) AND Person.AbiturientTypeId = 2";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (DataRow rw in tbl.Rows)
            {
                string email = rw.Field<string>("Email");
                string body = string.Format(Util.GetMailBody(Server.MapPath("~/Templates/EmailBodyEqualWithRussia.eml")),
                        Util.ServerAddress + Url.Action("CheckEqualWithRussia", "Abiturient", new RouteValueDictionary() { { "email", email } }));
                try
                {
                    MailMessage msg = new MailMessage();
                    msg.To.Add(email);
                    msg.Body = body;
                    msg.Subject = "Приёмная комиссия СПбГУ - участие в равном конкурсе с гражданами РФ";
                    SmtpClient client = new SmtpClient();
                    client.Send(msg);
                    query = "INSERT INTO User_SentEmails([From], [Email], [Text]) VALUES (@From, @Email, @Text)";
                    dic.Clear();
                    dic.Add("@From", "no-reply@spb.edu");
                    dic.Add("@Email", email);
                    dic.Add("@Text", msg.Body);
                    Util.AbitDB.ExecuteQuery(query, dic);
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception exc)
                {
                    try
                    {
                        query = "INSERT INTO User_SentEmails([From], [Email], [Text], [FailStatus]) VALUES (@From, @Email, @Text, @FailStatus)";
                        dic.Clear();
                        dic.Add("@From", "no-reply@spb.edu");
                        dic.Add("@Email", email);
                        dic.Add("@Text", body);
                        dic.Add("@FailStatus", exc.Message);
                        Util.AbitDB.ExecuteQuery(query, dic);
                    }
                    catch { }//вдруг база сломалась, тогда всё, не залогировать никак
                }
            }
            return View("Main");
        }

        #region Ajax

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

        public ActionResult GetLicenseProgramList(string slId)
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            int iStudyLevelId;
            if (!int.TryParse(slId, out iStudyLevelId))
                iStudyLevelId = 1;

            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM Entry WHERE StudyLevelId=@StudyLevelId ORDER BY LicenseProgramCode, LicenseProgramName";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyLevelId", iStudyLevelId);

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
        public ActionResult GetProfs(string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

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

            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel " +
                "AND IsReduced=@IsReduced AND [CampaignYear]=@Year AND SemesterId=@SemesterId";
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

            if (profs.Count() == 0)
            {
                return Json(new { NoFree = true });
            }

            return Json(profs);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetObrazPrograms(string prof, string studyform, string studybasis, string entry, string isSecond = "0", string isParallel = "0", string isReduced = "0", string semesterId = "1")
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

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
                "AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced AND DateOfClose>GETDATE() ";
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
                "AND IsSecond=@IsSecond AND IsParallel=@IsParallel AND IsReduced=@IsReduced  AND DateOfClose>GETDATE() ";

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

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetAbitCertsAndExams()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("", JsonRequestBehavior.AllowGet);

            string query = "SELECT DISTINCT Number FROM EgeCertificate WHERE PersonId=@PersonId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
            List<string> certs = (from DataRow rw in tbl.Rows
                                  select rw.Field<string>("Number")).ToList();

            query = "SELECT EgeExam.Id, EgeExam.Name FROM EgeCertificate INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
                " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE EgeCertificate.PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

            List<KeyValuePair<int, string>> exams =
                Util.EgeExamsAll.Except(
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).
                 ToDictionary(x => x.Id, y => y.Name)).ToList();

            var result = new { Certs = certs, Exams = exams };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult AddMark(string certNumber, string examName, string examValue, string Is2014, string IsInUniversity, string IsSecondWave)
        {

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(result);
            }

            int iExamId = 0;
            if (!int.TryParse(examName, out iExamId))
                iExamId = 0;

            int iExamValue = 0;
            if (!int.TryParse(examValue, out iExamValue))
                iExamValue = 0;

            bool bIs2014 = false;
            if (bool.TryParse(Is2014, out bIs2014))
                bIs2014 = false;

            bool bIsInUniversity = false;
            if (bool.TryParse(IsInUniversity, out bIsInUniversity))
                bIsInUniversity = false;

            bool bIsSecondWave = false;
            if (bool.TryParse(IsSecondWave, out bIsSecondWave))
                bIsSecondWave = false;

            string query;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Guid EgeCertificateId = Guid.Empty;
            
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var certs = context.EgeCertificate.Where(x => x.Number == certNumber || (x.Is2014 == true && x.PersonId == PersonId))
                    .Select(x => new { x.Id, x.PersonId, x.Is2014, x.Number }).ToList();

                //номер должен быть уникальным
                if (certs.Count() > 1)
                    return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });//Это косяк, двух не может быть!!!
                if (certs.Count() == 1)
                {
                    if (certs[0].PersonId != PersonId)
                        return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });
                    else
                        EgeCertificateId = certs[0].Id;
                }

                //query = "SELECT EgeMark.Value FROM EgeMark INNER JOIN EgeCertificate ON EgeCertificate.Id=EgeMark.EgeCertificateId " +
                //    " WHERE EgeCertificate.PersonId=@PersonId AND EgeMark.EgeExamId=@ExamId";
                //dic.Clear();
                //dic.Add("@PersonId", PersonId);
                //dic.Add("@ExamId", iExamId);
                //string MarkVal = Util.AbitDB.GetStringValue(query, dic);

                string MarkVal = context.EgeMark.Where(x => x.EgeCertificate.PersonId == PersonId && x.EgeExamId == iExamId)
                    .Select(x => new { x.IsInUniversity, x.IsSecondWave, x.Value })
                    .ToList()
                    .Select(x => x.IsSecondWave ? "Сдаю во второй волне" : (x.IsInUniversity ? "Сдаю в СПбГУ" : (x.Value ?? 0).ToString()))
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(MarkVal) && MarkVal != "0")
                    return Json(new { IsOk = false, ErrorMessage = "Оценка по данному предмету уже введена" });

                try
                {
                    if (EgeCertificateId == Guid.Empty)
                    {
                        EgeCertificateId = Guid.NewGuid();
                        context.EgeCertificate.AddObject(new EgeCertificate()
                        {
                            Id = EgeCertificateId,
                            Is2014 = bIs2014,
                            Number = certNumber,
                            PersonId = PersonId
                        });
                        context.SaveChanges();
                    }

                    Guid MarkId = Guid.NewGuid();
                    context.EgeMark.AddObject(new EgeMark()
                    {
                        Id = MarkId,
                        EgeCertificateId = EgeCertificateId,
                        EgeExamId = iExamId,
                        Value = iExamValue,
                        IsInUniversity = bIsInUniversity,
                        IsSecondWave = bIsSecondWave
                    });

                    string exName = Util.EgeExamsAll.ContainsKey(iExamId) ? Util.EgeExamsAll[iExamId] : "";
                    string exValue = "";
                    if (bIsSecondWave)
                        exValue = "Сдаю во второй волне";
                    else if (bIsInUniversity)
                        exValue = "Сдаю в СПбГУ";
                    else
                        exValue = iExamValue.ToString();
                    
                    var res = new
                    {
                        IsOk = true,
                        Data = new
                        {
                            Id = MarkId.ToString(),
                            CertificateNumber = certNumber,
                            ExamName = exName,
                            ExamMark = iExamValue.ToString()
                        },
                        ErrorMessage = ""
                    };
                    return Json(res);
                }

                catch
                {
                    return Json(new { IsOk = false, ErrorMessage = "Ошибка при сохранении оценки." });
                }
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult DeleteEgeMark()
        {
            string mId = Request.Params["mId"];
            Guid id;
            if (!Util.CheckAuthCookies(Request.Cookies, out id))
            {
                var result = new { IsOk = false, ErrorMsg = "Ошибка авторизации" };
                return Json(result);
            }

            Guid markId;
            if (!Guid.TryParse(mId, out markId))
            {
                var result = new { IsOk = false, ErrorMsg = "Некорректный идентификатор" };
                return Json(result);
            }

            try
            {
                Util.AbitDB.ExecuteQuery("DELETE FROM EgeMark WHERE Id=@Id", new Dictionary<string, object>() { { "@Id", markId } });
                var res = new { IsOk = true, ErrorMsg = "" };
                return Json(res);
            }
            catch
            {
                var result = new { IsOk = false, ErrorMsg = "Ошибка при обновлении" };
                return Json(result);
            }

            //using (AbitDB db = new AbitDB())
            //{

            //    EgeMark Mark = db.EgeMark.Where(x => x.Id == markId).DefaultIfEmpty(null).First();

            //    if (Mark != null)
            //        db.EgeMark.DeleteObject(Mark);

            //    try
            //    {
            //        db.SaveChanges(System.Data.Objects.SaveOptions.None);
            //    }
            //    catch
            //    {
            //        var result = new { IsOk = false, ErrorMsg = "Ошибка при обновлении" };
            //        return Json(result);
            //    }

            //    var res = new { IsOk = true, ErrorMsg = "" };
            //    return Json(res);
            //}
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult UpdateScienceWorks(string ScWorkInfo, string ScWorkType)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var result = new { IsOk = false, ErrorMsg = "Ошибка авторизации" };
                return Json(result);
            }

            int iScWorkType = 1;
            if (!(int.TryParse(ScWorkType, out iScWorkType)))
                iScWorkType = 1;

            Guid wrkId = Guid.NewGuid();

            string query = "INSERT INTO PersonScienceWork (Id, PersonId, WorkTypeId, WorkInfo) VALUES (@Id, @PersonId, @WorkTypeId, @WorkInfo)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", wrkId);
            dic.Add("@PersonId", PersonId);
            dic.Add("@WorkTypeId", iScWorkType);
            dic.Add("@WorkInfo", ScWorkInfo);
            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
                string scType = Util.ScienceWorkTypeAll[iScWorkType];
                string scInfo = HttpUtility.HtmlEncode(ScWorkInfo);
                var res = new { IsOk = true, Data = new { Id = wrkId.ToString("N"), Type = scType, Info = scInfo }, ErrorMsg = "" };
                return Json(res);
            }
            catch
            {
                var result = new { IsOk = false, ErrorMsg = "Ошибка при сохранении данных" };
                return Json(result);
            }
            //using (AbitDB db = new AbitDB())
            //{
            //    PersonScienceWork psw = new PersonScienceWork()
            //    {
            //        Id = Guid.NewGuid(),
            //        PersonId = PersonId,
            //        WorkTypeId = iScWorkType,
            //        WorkInfo = ScWorkInfo
            //    };

            //    try
            //    {
            //        db.PersonScienceWork.AddObject(psw);
            //        db.SaveChanges(System.Data.Objects.SaveOptions.None);
            //    }
            //    catch
            //    {
            //        var result = new { IsOk = false, ErrorMsg = "Ошибка при сохранении данных" };
            //        return Json(result);
            //    }
            //    string scType = Util.ScienceWorkTypeAll[iScWorkType];
            //    string scInfo = HttpUtility.HtmlEncode(ScWorkInfo);
            //    var res = new { IsOk = true, Data = new { Id = psw.Id, Type = scType, Info = scInfo }, ErrorMsg = "" };
            //    return Json(res);
            //}
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult DeleteScienceWorks(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(result);
            }

            Guid wrkId = Guid.Empty;
            if (!Guid.TryParse(id, out wrkId))
            {
                var result = new { IsOk = false, ErrorMessage = "Некорректный идентификатор" };
                return Json(result);
            }
            try
            {
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonScienceWork WHERE Id=@Id", new Dictionary<string, object>() { { "@Id", wrkId } });
                var res = new { IsOk = true, ErrorMessage = "" };
                return Json(res);
            }
            catch
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении данных" };
                return Json(result);
            }
            //using (AbitDB db = new AbitDB())
            //{
            //    PersonScienceWork psw = 
            //        db.PersonScienceWork.Where(x => x.Id == wrkId && x.PersonId == PersonId).DefaultIfEmpty(null).First();

            //    if (psw == null)
            //    {
            //        var result = new { IsOk = false, ErrorMessage = "Запись не найдена" };
            //        return Json(result);
            //    }

            //    try
            //    {
            //        db.PersonScienceWork.DeleteObject(psw);
            //        db.SaveChanges();
            //    }
            //    catch
            //    {
            //        var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении данных" };
            //        return Json(result);
            //    }

            //    var res = new { IsOk = true, ErrorMessage = "" };
            //    return Json(res);
            //}
        }

        public JsonResult LoadVuzNames(string schoolType)
        {
            int iSchoolType;
            int.TryParse(schoolType, out iSchoolType);
            string query = @"SELECT SchoolName, count(SchoolName) as cnt 
FROM EducationDocument 
WHERE SchoolName IS NOT NULL AND SchoolTypeId=@SchTypeId 
group by SchoolName
Order by cnt desc";
            DataTable tbl = Util.StudDB.GetDataTable(query, new Dictionary<string, object>() { { "@SchTypeId", iSchoolType } });
            List<string> vals =
                (from DataRow rw in tbl.Rows
                 select rw.Field<string>("SchoolName")).ToList();
            return Json(new { IsOk = true, Values = vals });
        }

        public ActionResult AddWorkPlace(string WorkStag, string WorkPlace, string WorkProf, string WorkSpec)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(result);
            }

            Guid workId = Guid.NewGuid();
            string query = "INSERT INTO PersonWork(Id, PersonId, Stage, WorkPlace, WorkProfession, WorkSpecifications) " +
                " VALUES (@Id, @PersonId, @Stage, @WorkPlace, @WorkProfession, @WorkSpecifications)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", workId);
            dic.Add("@PersonId", PersonId);
            dic.Add("@Stage", WorkStag);
            dic.Add("@WorkPlace", WorkPlace);
            dic.Add("@WorkProfession", WorkProf);
            dic.Add("@WorkSpecifications", WorkSpec);

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
                var res = new
                {
                    IsOk = true,
                    Data = new { Id = workId.ToString("N"), Place = WorkPlace, Stag = WorkStag, Level = WorkProf, Duties = WorkSpec },
                    ErrorMessage = ""
                };
                return Json(res);
            }
            catch
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка при сохранении данных" };
                return Json(result);
            }
            //using (AbitDB db = new AbitDB())
            //{
            //    PersonWork pw = new PersonWork()
            //    {
            //        Id = Guid.NewGuid(),
            //        PersonId = PersonId,
            //        Stage = WorkStag,
            //        WorkPlace = WorkPlace,
            //        WorkProfession = WorkProf,
            //        WorkSpecifications = WorkSpec
            //    };

            //    try
            //    {
            //        Util.ABDB.PersonWork.AddObject(pw);
            //        Util.ABDB.SaveChanges();
            //    }
            //    catch
            //    {
            //        var result = new { IsOk = false, ErrorMessage = "Ошибка при сохранении данных" };
            //        return Json(result);
            //    }

            //    var res = new 
            //    { 
            //        IsOk = true,
            //        Data = new { Id = pw.Id, Place = pw.WorkPlace, Stag = pw.Stage, Level = pw.WorkProfession, Duties = pw.WorkSpecifications },
            //        ErrorMessage = ""
            //    };
            //    return Json(res);
            //}
        }

        public ActionResult DeleteWorkPlace(string wrkId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(result);
            }

            Guid workId = new Guid();
            if (!Guid.TryParse(wrkId, out workId))
            {
                var result = new { IsOk = false, ErrorMessage = "Некорректный идентификатор" };
                return Json(result);
            }

            try
            {
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonWork WHERE Id=@Id", new Dictionary<string, object>() { { "@Id", workId } });
                var res = new { IsOk = true, ErrorMessage = "" };
                return Json(res);
            }
            catch
            {
                var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении" };
                return Json(result);
            }

            //try
            //{
            //    PersonWork pw = Util.ABDB.PersonWork.Where(x => x.Id == workId && x.PersonId == PersonId).DefaultIfEmpty(null).First();
            //    if (pw == null)
            //    {
            //        var result = new { IsOk = false, ErrorMessage = "Запись не найдена" };
            //        return Json(result);
            //    }
            //    Util.ABDB.PersonWork.DeleteObject(pw);
            //    Util.ABDB.SaveChanges();
            //    var res = new { IsOk = true, ErrorMessage = "" };
            //    return Json(res);
            //}
            //catch
            //{
            //    var result = new { IsOk = false, ErrorMessage = "Ошибка при обновлении" };
            //    return Json(result);
            //}
        }

        public ActionResult SendMotivationMail(string info, string appId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies((Request.Cookies), out PersonId))
                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });

            Guid applicationId;
            if (!Guid.TryParse(appId, out applicationId))
                return Json(new { IsOk = false, ErrorMessage = "Некорректный идентификатор заявления" });

            string query = "SELECT Id FROM MotivationMail WHERE ApplicationId=@AppId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("AppId", applicationId);
            Guid? outMailId = (Guid?)Util.AbitDB.GetValue(query, dic);

            try
            {
                dic.Clear();
                Guid mailId = Guid.NewGuid();
                if (outMailId.HasValue && outMailId.Value != Guid.Empty)
                {
                    query = "UPDATE MotivationMail SET MailText=@MailText WHERE Id=@Id";
                    dic.Add("@MailText", info);
                    dic.Add("@Id", outMailId.Value);
                }
                else
                {
                    query = "INSERT INTO MotivationMail(Id, ApplicationId, MailText) VALUES (@Id, @ApplicationId, @MailText)";
                    dic.Add("@Id", mailId);
                    dic.Add("@ApplicationId", applicationId);
                    dic.Add("@MailText", info);
                    outMailId = mailId;
                }
                Util.AbitDB.ExecuteQuery(query, dic);
                return Json(new { IsOk = true, Id = outMailId.Value.ToString("N") });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при сохранении. Пожалуйста, повторите" });
            }
            //MotivationMail ml;
            //if (Util.ABDB.MotivationMail.Where(x => x.ApplicationId == applicationId).Count() > 0)
            //{
            //    ml = Util.ABDB.MotivationMail.Where(x => x.ApplicationId == applicationId).First();
            //    ml.MailText = info;
            //}
            //else
            //{
            //    ml = new MotivationMail()
            //    {
            //        Id = mailId,
            //        ApplicationId = applicationId,
            //        MailText = info
            //    };
            //    Util.ABDB.MotivationMail.AddObject(ml);
            //}
            //Util.ABDB.SaveChanges();
        }

        public ActionResult GetMotivationMail(string appId)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });

            Guid ApplicationId;
            if (!Guid.TryParse(appId, out ApplicationId))
                return Json(new { IsOk = false, ErrorMessage = "Некорректный идентификатор заявления" });

            DataTable tbl = Util.AbitDB.GetDataTable("SELECT Id, MailText FROM MotivationMail WHERE ApplicationId=@Id",
                new Dictionary<string, object>() { { "@Id", ApplicationId } });

            if (tbl.Rows.Count == 0)
                return Json(new { IsOk = false, Text = "" });
            else
                return Json(new
                {
                    IsOk = true,
                    Text = tbl.Rows[0].Field<string>("MailText"),
                    Id = tbl.Rows[0].Field<Guid>("Id").ToString("N")
                });

            //var apps = Util.ABDB.MotivationMail.Where(x => x.ApplicationId == ApplicationId).Select(x => new { x.Id, x.MailText }).AsEnumerable();
            //if (apps.Count() == 0)
            //    return Json(new { IsOk = false, Text = "" });
            //else
            //    return Json(new { IsOk = true, Text = apps.First().MailText, Id = apps.First().Id });
        }

        [HttpPost]
        public ActionResult DeleteFile(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(res);
            }

            Guid fileId;
            if (!Guid.TryParse(id, out fileId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
                return Json(res);
            }
            string attr = Util.AbitDB.GetStringValue("SELECT IsReadOnly FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
            if (string.IsNullOrEmpty(attr))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
                return Json(res);
            }
            if (attr == "True")
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
                return Json(res);
            }
            try
            {
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
            }
            catch
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
                return Json(res);
            }

            var result = new { IsOk = true, ErrorMessage = "" };
            return Json(result);
        }

        public JsonResult DeleteSharedFile(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации" };
                return Json(res);
            }

            Guid fileId;
            if (!Guid.TryParse(id, out fileId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
                return Json(res);
            }
            string attr = Util.AbitDB.GetStringValue("SELECT ISNULL([IsReadOnly], 'False') FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
            if (string.IsNullOrEmpty(attr))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.FileNotFound };
                return Json(res);
            }
            if (attr == "True")
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ReadOnlyFile };
                return Json(res);
            }
            try
            {
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
            }
            catch
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
                return Json(res);
            }

            var result = new { IsOk = true, ErrorMessage = "" };
            return Json(result);
        }

        public ActionResult DeleteMsg(string id)
        {
            if (id == "0")//system messages
                return Json(new { IsOk = true });

            Guid MessageId;
            if (!Guid.TryParse(id, out MessageId))
                return Json(new { IsOk = false, ErrorMessage = "" });

            string query = "UPDATE PersonalMessage SET IsRead=@IsRead WHERE Id=@Id";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@IsRead", true);
            dic.Add("@Id", MessageId);

            try
            {
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch (Exception e)
            {
                return Json(new { IsOk = false, ErrorMessage = e.Message });//
            }

            return Json(new { IsOk = true });
        }

        public JsonResult GetCityNames(string regionId)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var towns = Util.GetCityListByRegion(sRegionKladrCode);

                return Json(new { IsOk = true, List = towns.Select(x => x.Value).Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetStreetNames(string regionId, string cityName)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var streets = Util.GetStreetListByRegion(sRegionKladrCode, cityName);

                return Json(new { IsOk = true, List = streets.Select(x => x.Value).Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }
        public JsonResult GetHouseNames(string regionId, string cityName, string streetName)
        {
            try
            {
                string sRegionKladrCode = Util.GetRegionKladrCodeByRegionId(regionId);
                var streets = Util.GetHouseListByStreet(sRegionKladrCode, cityName, streetName);

                return Json(new { IsOk = true, List = streets.Distinct().ToList() });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при выполнении запроса. Попробуйте обновить страницу" });
            }
        }

        public JsonResult GetOlympNameList(string OlympTypeId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            int iOlympTypeId;
            if (!int.TryParse(OlympTypeId, out iOlympTypeId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var OlData = context.OlympBook.Where(x => x.OlympTypeId == iOlympTypeId).Select(x => new { Id = x.OlympNameId, Name = x.OlympName.Name }).Distinct().ToList();

                return Json(new
                {
                    IsOk = true,
                    List = OlData
                });
            }
        }
        public JsonResult GetOlympSubjectList(string OlympTypeId, string OlympNameId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            int iOlympTypeId;
            if (!int.TryParse(OlympTypeId, out iOlympTypeId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iOlympNameId;
            if (!int.TryParse(OlympNameId, out iOlympNameId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var OlData = context.OlympBook.Where(x => x.OlympTypeId == iOlympTypeId && x.OlympNameId == iOlympNameId)
                    .Select(x => new { Id = x.OlympSubjectId, Name = x.OlympSubject.Name }).Distinct().ToList();

                return Json(new
                {
                    IsOk = true,
                    List = OlData
                });
            }
        }

        public JsonResult AddOlympiad(string OlympTypeId, string OlympNameId, string OlympSubjectId, string OlympValueId, string Series, string Number, string Date)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            int iOlympTypeId;
            if (!int.TryParse(OlympTypeId, out iOlympTypeId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iOlympNameId;
            if (!int.TryParse(OlympNameId, out iOlympNameId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iOlympSubjectId;
            if (!int.TryParse(OlympSubjectId, out iOlympSubjectId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            int iOlympValueId;
            if (!int.TryParse(OlympValueId, out iOlympValueId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            DateTime _date;
            if (!DateTime.TryParse(Date, out _date))
                _date = DateTime.Now;

            Guid Id = Guid.NewGuid();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                context.Olympiads.AddObject(new Olympiads()
                {
                    Id = Id,
                    OlympNameId = iOlympNameId,
                    OlympSubjectId = iOlympSubjectId,
                    OlympTypeId = iOlympTypeId,
                    OlympValueId = iOlympValueId,
                    DocumentSeries = Series,
                    DocumentNumber = Number,
                    DocumentDate = _date,
                    PersonId = PersonId
                });
                context.SaveChanges();

                var Ol = context.Olympiads.Where(x => x.Id == Id).Select(x => new
                {
                    OlympName = x.OlympName.Name,
                    OlympSubject = x.OlympSubject.Name,
                    OlympType = x.OlympType.Name,
                    OlympValue = x.OlympValue.Name
                }).FirstOrDefault();

                return Json(new
                {
                    IsOk = true,
                    Id = Id.ToString("N"),
                    Type = Ol.OlympType,
                    Name = Ol.OlympName,
                    Subject = Ol.OlympSubject,
                    Value = Ol.OlympValue,
                    Doc = Series + " " + Number + " от " + _date.ToShortDateString()
                });
            }
        }
        public JsonResult DeleteOlympiad(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid OlympId;
            if (!Guid.TryParse(id, out OlympId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            string query = "DELETE FROM Olympiads WHERE Id=@Id";
            Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", OlympId } });
            return Json(new { IsOk = true });
        }

        [HttpPost]
        public JsonResult CheckApplication_AG(string profession, string Entryclass, string profileid, string manualExam, string NeedHostel)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.AG_PriemClosed });

                bool needHostel = string.IsNullOrEmpty(NeedHostel) ? false : true;

                int iEntryClassId = Util.ParseSafe(Entryclass);
                int iProfession = Util.ParseSafe(profession);
                int iProfileId = Util.ParseSafe(profileid);
                int iManualExamId = Util.ParseSafe(manualExam);

                //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
                var EntryList =
                    (from Ent in context.AG_Entry
                     join EntInEntGroup in context.AG_EntryInEntryGroup on Ent.Id equals EntInEntGroup.EntryId
                     where Ent.ProgramId == iProfession && Ent.EntryClassId == iEntryClassId && (iProfileId != 0 ? Ent.ProfileId == iProfileId : true)
                     select new
                     {
                         EntryId = Ent.Id,
                         EntInEntGroup.EntryGroupId,
                         Ent.DateOfStartEntry,
                         Ent.DateOfStopEntry
                     }).ToList();
                if (EntryList.Count > 1)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_2MoreEntry + " (" + EntryList.Count.ToString() + ")" });
                if (EntryList.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoEntry });

                Guid EntryId = EntryList.First().EntryId;
                DateTime? timeOfStart = EntryList.First().DateOfStartEntry;
                DateTime? timeOfStop = EntryList.First().DateOfStopEntry;

                //проверка на группы
                var EntryGroupList =
                    (from Entr in context.AG_Entry
                     join EntrInEntryGroup in context.AG_EntryInEntryGroup on Entr.Id equals EntrInEntryGroup.EntryId
                     join Abit in context.AG_Application on Entr.Id equals Abit.EntryId
                     where Abit.PersonId == PersonId && Abit.Enabled == true
                     select EntrInEntryGroup.EntryGroupId);

                var AllNeededEntries =
                    (from Entr in context.AG_Entry
                     join EntrInEntryGroup in context.AG_EntryInEntryGroup on Entr.Id equals EntrInEntryGroup.EntryId
                     where EntryGroupList.Contains(EntrInEntryGroup.EntryGroupId)
                     select Entr.Id).ToList();

                var FreeEntries = AllNeededEntries.Except(
                    context.AG_Application.Where(x => x.PersonId == PersonId).Select(x => x.EntryId).ToList()).ToList();

                if (FreeEntries.Count == 0)
                    return Json(new { IsOk = true, FreeEntries = false });
                else
                {
                    if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NotOpenedEntry });

                    if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_ClosedEntry });

                    var eIds =
                        (from App in context.AG_Application
                         where App.PersonId == PersonId && App.Enabled == true
                         select App.EntryId).ToList();

                    if (eIds.Contains(EntryId))
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_HasApplicationOnEntry });
                }

                return Json(new { IsOk = true, FreeEntries = true });
            }
        }
        
        [HttpPost]
        public JsonResult AddApplication_AG(string profession, string Entryclass, string profileid, string manualExam, string NeedHostel)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.AG_PriemClosed });

                bool needHostel = string.IsNullOrEmpty(NeedHostel) ? false : true;

                int iEntryClassId = Util.ParseSafe(Entryclass);
                int iProfession = Util.ParseSafe(profession);
                int iProfileId = Util.ParseSafe(profileid);
                int iManualExamId = Util.ParseSafe(manualExam);

                //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
                string query = "SELECT Id, DateOfStartEntry, DateOfStopEntry FROM AG_Entry WHERE ProgramId=@ProgramId AND EntryClassId=@EntryClassId ";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@ProgramId", iProfession);
                dic.Add("@EntryClassId", iEntryClassId);
                if (iProfileId != 0)
                {
                    query += " AND ProfileId=@ProfileId ";
                    dic.Add("@ProfileId", iProfileId);
                }
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                if (tbl.Rows.Count > 1)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_2MoreEntry + " (" + tbl.Rows.Count + ")" });
                if (tbl.Rows.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoEntry });

                Guid EntryId = tbl.Rows[0].Field<Guid>("Id");
                DateTime? timeOfStart = tbl.Rows[0].Field<DateTime?>("DateOfStartEntry");
                DateTime? timeOfStop = tbl.Rows[0].Field<DateTime?>("DateOfStopEntry");

                if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NotOpenedEntry });

                if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_ClosedEntry });

                query = "SELECT EntryId FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });
                var eIds =
                    from DataRow rw in tbl.Rows
                    select rw.Field<Guid>("EntryId");
                if (eIds.Contains(EntryId))
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_HasApplicationOnEntry });

                DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled=@Enabled",
                    new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
                int? PriorMax =
                    (from DataRow rw in tblPriors.Rows
                     select rw.Field<int?>("Priority")).Max();

                Guid appId = Guid.NewGuid();
                query = "INSERT INTO [AG_Application] (Id, PersonId, EntryId, HostelEduc, Enabled, DateOfStart, ManualExamId, Priority) " +
                    "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @DateOfStart, @ManualExamId, @Priority)";
                Dictionary<string, object> prms = new Dictionary<string, object>();
                prms.Add("@Id", appId);
                prms.Add("@PersonId", PersonId);
                prms.Add("@EntryId", EntryId);
                prms.Add("@HostelEduc", needHostel);
                prms.Add("@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1);
                prms.Add("@Enabled", true);
                prms.Add("@DateOfStart", DateTime.Now);
                prms.AddItem("@ManualExamId", iManualExamId == 0 ? null : (int?)iManualExamId);

                Util.AbitDB.ExecuteQuery(query, prms);

                return Json(new { IsOk = true });
            }
        }

        #endregion
    }
}
