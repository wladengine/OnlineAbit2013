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
    public class TransferForeignController : Controller
    {
        //
        // GET: /TransferForeign/

        public ActionResult Index(string step)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            int AbitType = Util.CheckAbitType(PersonId);
            switch (AbitType)
            {
                case 1: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
                case 2: { return RedirectToAction("Index", "ForeignAbiturient", new RouteValueDictionary() { { "step", step }}); }
                case 3: { return RedirectToAction("Index", "Transfer", new RouteValueDictionary() { { "step", step }});  }
                case 4: { break; }
                case 5: { return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", step }}); }
                case 6: { return RedirectToAction("Index", "ChangeStudyForm", new RouteValueDictionary() { { "step", step }}); }
                case 7: { return RedirectToAction("Index", "ChangeObrazProgram", new RouteValueDictionary() { { "step", step }}); ; }
                case 8: { return RedirectToAction("Index", "AG", new RouteValueDictionary() { { "step", step }}); ; }
                default: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
            }

            int stage = 0;
            if (!int.TryParse(step, out stage))
                stage = 1;

            string query = "";
            DataTable tbl;

            Util.SetThreadCultureByCookies(Request.Cookies);

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (Person.RegistrationStage == 0)
                    stage = 1;
                else if (Person.RegistrationStage < stage)
                    stage = Person.RegistrationStage;

                PersonalOfficeTransfer model = new PersonalOfficeTransfer() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus(PersonId) };
                if (model.Stage == 1)
                {
                    model.PersonInfo = new InfoPerson();

                    model.PersonInfo.Surname = Server.HtmlDecode(Person.Surname);
                    model.PersonInfo.Name = Server.HtmlDecode(Person.Name);
                    model.PersonInfo.SecondName = Server.HtmlDecode(Person.SecondName);
                    model.PersonInfo.Sex = (Person.Sex ?? true) ? "M" : "F";
                    model.PersonInfo.Nationality = Person.NationalityId.ToString();
                    model.PersonInfo.BirthPlace = Person.BirthPlace;
                    model.PersonInfo.BirthDate = Person.BirthDate.HasValue ? Person.BirthDate.Value.ToString("dd.MM.yyyy") : "";

                    model.PersonInfo.NationalityList = Util.GetCountryList();
                    model.PersonInfo.SexList = new List<SelectListItem>()
                    {
                        new SelectListItem() { Text = LangPack.GetValue(5, model.Lang), Value = "M" }, 
                        new SelectListItem() { Text = LangPack.GetValue(6, model.Lang), Value = "F" }
                    };
                }
                else if (model.Stage == 2)
                {
                    model.PassportInfo = new PassportPerson();
                    DataTable tblPsp = Util.AbitDB.GetDataTable("SELECT Id, Name FROM PassportType WHERE 1=@x", new Dictionary<string, object>() { { "@x", 1 } });
                    model.PassportInfo.PassportTypeList =
                        (from DataRow rw in tblPsp.Rows
                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>("Name") }).
                        ToList();

                    model.PassportInfo.PassportType = (Person.PassportTypeId ?? 1).ToString();
                    model.PassportInfo.PassportSeries = Server.HtmlDecode(Person.PassportSeries);
                    model.PassportInfo.PassportNumber = Server.HtmlDecode(Person.PassportNumber);
                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(Person.PassportAuthor);
                    model.PassportInfo.PassportDate = Person.PassportDate.HasValue ?
                        Person.PassportDate.Value.ToString("dd.MM.yyyy") : "";
                    model.PassportInfo.PassportCode = Server.HtmlDecode(Person.PassportCode);
                }
                else if (model.Stage == 3)
                {
                    model.ContactsInfo = new ContactsPerson();

                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                        PersonContacts = new OnlineAbit2013.PersonContacts();

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

                    model.ContactsInfo.CountryList = Util.GetCountryList();

                    query = "SELECT Id, Name FROM Region WHERE RegionNumber IS NOT NULL";
                    tbl = Util.AbitDB.GetDataTable(query, null);
                    model.ContactsInfo.RegionList =
                        (from DataRow rw in tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();
                    //Util.RegionsAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                }
                else if (model.Stage == 4)
                {
                    model.EducationInfo = new EducationPerson();
                    model.CurrentEducation = new CurrentEducation();
                    PersonEducationDocument EducationDocument = Person.PersonEducationDocument;
                    PersonCurrentEducation CurrentEducation = Person.PersonCurrentEducation;

                    if (EducationDocument == null)
                        EducationDocument = new PersonEducationDocument();
                    if (CurrentEducation == null)
                        CurrentEducation = new PersonCurrentEducation();

                    model.EducationInfo.SchoolTypeId = EducationDocument.SchoolTypeId.ToString();
                    model.EducationInfo.SchoolName = EducationDocument.SchoolName;
                    model.EducationInfo.SchoolExitYear = EducationDocument.SchoolExitYear;
                    model.EducationInfo.DiplomSeries = EducationDocument.Series;
                    model.EducationInfo.DiplomNumber = EducationDocument.Number;
                    model.EducationInfo.AttestatRegion = EducationDocument.AttestatRegion;
                    model.EducationInfo.CountryList = Util.GetCountryList();

                    query = "SELECT Id, Name FROM SchoolTypeAll";
                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.EducationInfo.SchoolTypeList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    model.CurrentEducation.EducationTypeList = model.EducationInfo.SchoolTypeList;

                    query = "SELECT Id, Name FROM SP_StudyLevel";
                    _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.CurrentEducation.StudyLevelList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    query = "SELECT Id, Name FROM Semester";
                    _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.CurrentEducation.SemesterList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    model.CurrentEducation.HasAccreditation = CurrentEducation.HasAccreditation ?? false;
                    model.CurrentEducation.AccreditationDate = CurrentEducation.AccreditationDate.HasValue ? CurrentEducation.AccreditationDate.Value.ToShortDateString() : "";
                    model.CurrentEducation.AccreditationNumber = CurrentEducation.AccreditationNumber;
                    model.CurrentEducation.EducationTypeId = CurrentEducation.EducTypeId.ToString();
                    model.CurrentEducation.EducationName = CurrentEducation.EducName;
                    model.CurrentEducation.SemesterId = CurrentEducation.SemesterId.ToString();
                    model.CurrentEducation.HasScholarship = CurrentEducation.HasScholarship ?? false;
                }
                else if (model.Stage == 5)
                {
                    if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                        return RedirectToAction("LogOn", "Account");

                    if (Person.RegistrationStage < 5)
                    {
                        model.AddInfo = new AdditionalInfoPerson()
                        {
                            FZ_152Agree = false,
                            ExtraInfo = "",
                            ContactPerson = "",
                            MaritalStatus = "",
                            SocialStatus = ""
                        };
                    }
                    else
                    {
                        var PersonAddInfo = Person.PersonAddInfo;
                        if (PersonAddInfo == null) PersonAddInfo = new OnlineAbit2013.PersonAddInfo();
                        model.AddInfo = new AdditionalInfoPerson()
                        {
                            FZ_152Agree = false,
                            ExtraInfo = Server.HtmlDecode(PersonAddInfo.AddInfo),
                            ContactPerson = Server.HtmlDecode(PersonAddInfo.Parents),
                            MaritalStatus = Server.HtmlDecode(PersonAddInfo.MaritalStatus),
                            SocialStatus = Server.HtmlDecode(PersonAddInfo.SocialStatus)
                        };
                    }
                }
                return View("PersonalOffice", model);
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NextStep(PersonalOfficeTransfer model)
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
                        return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", model.Stage } });
                    else
                        return RedirectToAction("Main", "Recover");
                }

                if (model.Stage == 1)
                {
                    DateTime bdate;
                    if (!DateTime.TryParse(model.PersonInfo.BirthDate, CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat, DateTimeStyles.None, out bdate))
                        bdate = DateTime.Now;

                    int NationalityId = 1;
                    if (!int.TryParse(model.PersonInfo.Nationality, out NationalityId))
                        NationalityId = 1;

                    Person.Surname = model.PersonInfo.Surname;
                    Person.Name = model.PersonInfo.Name;
                    Person.SecondName = model.PersonInfo.SecondName;
                    Person.BirthDate = bdate;
                    Person.BirthPlace = model.PersonInfo.BirthPlace;
                    Person.NationalityId = NationalityId;
                    Person.Sex = model.PersonInfo.Sex == "M" ? true : false;
                    Person.RegistrationStage = iRegStage < 2 ? 2 : iRegStage;

                    context.SaveChanges();
                }
                else if (model.Stage == 2)
                {
                    int iPassportType = 1;
                    if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
                        iPassportType = 1;

                    DateTime dtPassportDate;
                    try
                    {
                        dtPassportDate = Convert.ToDateTime(model.PassportInfo.PassportDate,
                            System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                    }
                    catch { dtPassportDate = DateTime.Now; }

                    Person.PassportTypeId = iPassportType;
                    Person.PassportSeries = model.PassportInfo.PassportSeries;
                    Person.PassportNumber = model.PassportInfo.PassportNumber;
                    Person.PassportAuthor = model.PassportInfo.PassportAuthor;
                    Person.PassportDate = dtPassportDate;
                    Person.PassportCode = model.PassportInfo.PassportCode;
                    Person.RegistrationStage = iRegStage < 3 ? 3 : iRegStage;

                    context.SaveChanges();
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
                        bIns = true;
                        PersonContacts = new OnlineAbit2013.PersonContacts();
                        PersonContacts.PersonId = PersonId;
                    }

                    PersonContacts.Phone = model.ContactsInfo.MainPhone;
                    PersonContacts.Mobiles = model.ContactsInfo.SecondPhone;
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
                    PersonCurrentEducation PersonCurrentEducation = Person.PersonCurrentEducation;
                    PersonEducationDocument PersonEducationDocument = Person.PersonEducationDocument;

                    int iEducTypeId = 1;
                    if (!int.TryParse(model.CurrentEducation.EducationTypeId, out iEducTypeId))
                        iEducTypeId = 1;//default value
                    int iSemesterId = 1;
                    if (!int.TryParse(model.CurrentEducation.SemesterId, out iSemesterId))
                        iSemesterId = 1;//default value
                    int iStudyLevelId = 16;
                    if (!int.TryParse(model.CurrentEducation.StudyLevelId, out iStudyLevelId))
                        iStudyLevelId = 16;//default value

                    DateTime? dtAccreditation;
                    DateTime tmp;
                    if (!DateTime.TryParse(model.CurrentEducation.AccreditationDate, out tmp))
                        dtAccreditation = null;
                    else
                        dtAccreditation = tmp;

                    if (PersonCurrentEducation == null)
                    {
                        PersonCurrentEducation = new PersonCurrentEducation();
                        PersonCurrentEducation.PersonId = PersonId;
                        PersonCurrentEducation.EducTypeId = iEducTypeId;
                        PersonCurrentEducation.SemesterId = iSemesterId;
                        PersonCurrentEducation.AccreditationDate = dtAccreditation;
                        PersonCurrentEducation.AccreditationNumber = model.CurrentEducation.AccreditationNumber;
                        PersonCurrentEducation.EducName = model.CurrentEducation.EducationName;
                        PersonCurrentEducation.HasAccreditation = model.CurrentEducation.HasAccreditation;
                        PersonCurrentEducation.HasScholarship = model.CurrentEducation.HasScholarship;
                        PersonCurrentEducation.StudyLevelId = iStudyLevelId;
                        context.PersonCurrentEducation.AddObject(PersonCurrentEducation);
                    }
                    else
                    {
                        PersonCurrentEducation.EducTypeId = iEducTypeId;
                        PersonCurrentEducation.SemesterId = iSemesterId;
                        PersonCurrentEducation.AccreditationDate = dtAccreditation;
                        PersonCurrentEducation.AccreditationNumber = model.CurrentEducation.AccreditationNumber;
                        PersonCurrentEducation.EducName = model.CurrentEducation.EducationName;
                        PersonCurrentEducation.HasAccreditation = model.CurrentEducation.HasAccreditation;
                        PersonCurrentEducation.HasScholarship = model.CurrentEducation.HasScholarship;
                        PersonCurrentEducation.StudyLevelId = iStudyLevelId;
                        PersonCurrentEducation.CountryId = 193;
                    }

                    int iSchoolTypeId = 1;
                    if (!int.TryParse(model.EducationInfo.SchoolTypeId, out iSchoolTypeId))
                        iSchoolTypeId = 1;
                    int iCountryEducId = 193;
                    if (!int.TryParse(model.EducationInfo.CountryEducId, out iCountryEducId))
                        iCountryEducId = 193;

                    if (PersonEducationDocument == null)
                    {
                        PersonEducationDocument = new PersonEducationDocument();
                        PersonEducationDocument.PersonId = PersonId;
                        PersonEducationDocument.SchoolTypeId = iSchoolTypeId;

                        if (iSchoolTypeId == 1)
                            PersonEducationDocument.AttestatRegion = model.EducationInfo.AttestatRegion;
                        PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
                        PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;
                        PersonEducationDocument.CountryEducId = iCountryEducId;
                        PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
                        PersonEducationDocument.SchoolName = model.EducationInfo.SchoolName;

                        PersonEducationDocument.IsEqual = model.EducationInfo.IsEqual;
                        PersonEducationDocument.EqualDocumentNumber = model.EducationInfo.EqualityDocumentNumber;

                        context.PersonEducationDocument.AddObject(PersonEducationDocument);
                    }
                    else
                    {
                        PersonEducationDocument.SchoolTypeId = iSchoolTypeId;

                        if (iSchoolTypeId == 1)
                            PersonEducationDocument.AttestatRegion = model.EducationInfo.AttestatRegion;
                        PersonEducationDocument.Series = model.EducationInfo.DiplomSeries;
                        PersonEducationDocument.Number = model.EducationInfo.DiplomNumber;
                        PersonEducationDocument.CountryEducId = iCountryEducId;
                        PersonEducationDocument.SchoolExitYear = model.EducationInfo.SchoolExitYear;
                        PersonEducationDocument.SchoolName = model.EducationInfo.SchoolName;
                    }

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

                    var ExtraPerson = context.PersonAddInfo.Where(x => x.PersonId == PersonId).FirstOrDefault();
                    if (ExtraPerson == null)
                    {
                        ExtraPerson = new PersonAddInfo();
                        ExtraPerson.PersonId = PersonId;
                        ExtraPerson.Parents = model.AddInfo.ContactPerson;
                        ExtraPerson.AddInfo = model.AddInfo.ExtraInfo;
                        ExtraPerson.MaritalStatus = model.AddInfo.MaritalStatus;
                        ExtraPerson.SocialStatus = model.AddInfo.SocialStatus;
                        context.PersonAddInfo.AddObject(ExtraPerson);
                    }
                    else
                    {
                        ExtraPerson.MaritalStatus = model.AddInfo.MaritalStatus;
                        ExtraPerson.SocialStatus = model.AddInfo.SocialStatus;
                        ExtraPerson.Parents = model.AddInfo.ContactPerson;
                        ExtraPerson.AddInfo = model.AddInfo.ExtraInfo;
                    }
                    if (Person.RegistrationStage <= 5)
                        Person.RegistrationStage = 100;

                    context.SaveChanges();
                }

                if (model.Stage < 5)
                {
                    model.Stage++;
                    return RedirectToAction("Index", "Recover", new RouteValueDictionary() { { "step", model.Stage } });
                }
                else
                    return RedirectToAction("Main", "Recover");
            }
        }

        public ActionResult Main()
        {
            //Validation
            Guid PersonID;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonID))
                return RedirectToAction("LogOn", "Account");

            if (Util.CheckIsNew(PersonID))
                return RedirectToAction("OpenPersonalAccount");

            Util.SetThreadCultureByCookies(Request.Cookies);

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
                switch (PersonInfo.AbiturientTypeId)
                {
                    case 1: { return RedirectToAction("Main", "Abiturient"); }
                    case 2: { return RedirectToAction("Main", "ForeignAbiturient"); }
                    case 3: { return RedirectToAction("Main", "Transfer"); }
                    case 4: { break; }
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
                    case 4: { break; }
                    case 5: { return RedirectToAction("NewApplication", "Recover"); }
                    case 6: { return RedirectToAction("NewApplication", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("NewApplication", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("NewApplication", "AG"); }
                    default: { return RedirectToAction("NewApplication", "Abiturient"); }
                }

                ApplicationModel model = new ApplicationModel();
                model.EntryType = 2;

                string query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry";
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

                //для иностранцев - ТОЛЬКО ПЛАТНЫЙ ПЕРЕВОД
                query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry WHERE StudyBasisId=2";
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

                query = "SELECT DISTINCT Semester.Id, Semester.Name FROM Semester INNER JOIN Entry ON Entry.SemesterId = Semester.Id WHERE Semester.Id > 1 AND Entry.DateOfStart<@Date AND Entry.DateOfClose>@Date";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Date", DateTime.Now } });
                model.SemesterList =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("Id"),
                         Text = rw.Field<string>("Name")
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
            string isReduced = Request.Form["IsReducedHidden"];
            string isParallel = Request.Form["IsParallelHidden"];
            string semesterId = Request.Form["SemesterId"];

            bool needHostel = string.IsNullOrEmpty(Request.Form["NeedHostel"]) ? false : true;

            int iStudyFormId = Util.ParseSafe(sform);
            int iStudyBasisId = Util.ParseSafe(sbasis);
            int iFacultyId = Util.ParseSafe(faculty);
            int iProfession = Util.ParseSafe(profession);
            int iSemesterId = Util.ParseSafe(semesterId);
            int iObrazProgram = Util.ParseSafe(obrazprogram);
            Guid ProfileId = Guid.Empty;
            if (!string.IsNullOrEmpty(Request.Form["lSpecialization"]))
                Guid.TryParse(Request.Form["lSpecialization"], out ProfileId);

            int iEntry = Util.ParseSafe(Request.Form["EntryType"]);
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT Entry.Id FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id=Entry.StudyLevelId WHERE LicenseProgramId=@LicenseProgramId " +
                " AND ObrazProgramId=@ObrazProgramId AND StudyFormId=@SFormId AND StudyBasisId=@SBasisId AND IsParallel=@IsParallel AND IsReduced=@IsReduced " +
                (ProfileId == Guid.Empty ? " AND ProfileId IS NULL " : " AND ProfileId=@ProfileId ") + (iFacultyId == 0 ? "" : " AND FacultyId=@FacultyId ") +
                " AND SemesterId=@SemesterId AND CampaignYear=@CampaignYear";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@LicenseProgramId", iProfession);
            dic.Add("@ObrazProgramId", iObrazProgram);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@SBasisId", iStudyBasisId);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@SemesterId", iSemesterId);
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

            //query = "SELECT Person.Surname, Person.Name, Person.SecondName, Entry.LicenseProgramCode, Entry.LicenseProgramName, Entry.ObrazProgramName " +
            //    " FROM [Application] INNER JOIN Person ON Person.Id=[Application].PersonId " +
            //    " INNER JOIN Entry ON Application.EntryId=Entry.Id WHERE Application.Id=@AppId";
            //DataTable Tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
            //var fileInfo =
            //    (from DataRow rw in Tbl.Rows
            //     select new
            //     {
            //         Surname = rw.Field<string>("Surname"),
            //         Name = rw.Field<string>("Name"),
            //         SecondName = rw.Field<string>("SecondName"),
            //         ProfessionCode = rw.Field<string>("LicenseProgramCode"),
            //         Profession = rw.Field<string>("LicenseProgramName"),
            //         ObrazProgram = rw.Field<string>("ObrazProgramName")
            //     }).FirstOrDefault();

            //if (iEntry == 2)
            //{
            //    byte[] pdfData = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"));
            //    DateTime dateTime = DateTime.Now;

            //    query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileExtention, FileData, FileSize, IsReadOnly, LoadDate, Comment, MimeType) " +
            //        " VALUES (@Id, @PersonId, @FileName, @FileExtention, @FileData, @FileSize, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
            //    prms.Clear();
            //    prms.Add("@Id", Guid.NewGuid());
            //    prms.Add("@PersonId", appId);
            //    prms.Add("@FileName", fileInfo.Surname + " " + fileInfo.Name.FirstOrDefault() + " - Заявление [" + dateTime.ToString("dd.MM.yyyy") + "].pdf");
            //    prms.Add("@FileExtention", ".pdf");
            //    prms.Add("@FileData", pdfData);
            //    prms.Add("@FileSize", pdfData.Length);
            //    prms.Add("@IsReadOnly", true);
            //    prms.Add("@LoadDate", dateTime);
            //    prms.Add("@Comment", "Заявление на направление (" + fileInfo.ProfessionCode + ") " + fileInfo.Profession + ", образовательная программа \""
            //        + fileInfo.ObrazProgram + "\", от " + dateTime.ToShortDateString());
            //    prms.Add("@MimeType", "[Application]/pdf");
            //    Util.AbitDB.ExecuteQuery(query, prms);
            //}

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
                    case 1: { return RedirectToAction("AddSharedFiles", "Abiturient"); }
                    case 2: { return RedirectToAction("AddSharedFiles", "ForeignAbiturient"); }
                    case 3: { return RedirectToAction("AddSharedFiles", "Transfer"); }
                    case 4: { break; }
                    case 5: { return RedirectToAction("AddSharedFiles", "Recover"); }
                    case 6: { return RedirectToAction("AddSharedFiles", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("AddSharedFiles", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("AddSharedFiles", "AG"); }
                    default: { return RedirectToAction("AddSharedFiles", "Abiturient"); }
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
                    case 2: { return RedirectToAction("PriorityChanger", "ForeignAbiturient"); }
                    case 3: { return RedirectToAction("PriorityChanger", "Transfer"); }
                    case 4: { break; }
                    case 5: { return RedirectToAction("PriorityChanger", "Recover"); }
                    case 6: { return RedirectToAction("PriorityChanger", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("PriorityChanger", "ChangeObrazProgram"); }
                    case 8: { return RedirectToAction("PriorityChanger", "AG"); }
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
        
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetProfs(string studyform, string studybasis, string entry, string isParallel = "0", string isReduced = "0", string semesterId = "1")
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
            int iSemesterId;
            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;

            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsParallel=@IsParallel " +
                "AND IsReduced=@IsReduced AND [CampaignYear]=@Year AND SemesterId=@SemesterId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);//2 == mag, 1 == 1kurs
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
        public ActionResult GetObrazPrograms(string prof, string studyform, string studybasis, string entry, string isParallel = "0", string isReduced = "0", string semesterId = "1")
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
            int iSemesterId;
            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;
            int iProfessionId = 1;
            if (!int.TryParse(prof, out iProfessionId))
                iProfessionId = 1;

            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
                "AND StudyLevelGroupId=@StudyLevelGroupId AND IsParallel=@IsParallel AND IsReduced=@IsReduced " +
                "AND DateOfClose>GETDATE() AND CampaignYear=@Year AND SemesterId=@SemesterId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var OPs = from DataRow rw in tbl.Rows
                      select new { Id = rw.Field<int>("ObrazProgramId"), Name = rw.Field<string>("ObrazProgramName") };

            return Json(new { NoFree = OPs.Count() > 0 ? false : true, List = OPs });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetSpecializations(string prof, string obrazprogram, string studyform, string studybasis, string entry, string isParallel = "0", string isReduced = "0", string semesterId = "1")
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
            int iSemesterId;
            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;

            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT ProfileId, ProfileName FROM Entry INNER JOIN SP_StudyLevel ON SP_StudyLevel.Id = Entry.StudyLevelId WHERE StudyFormId=@StudyFormId " +
                "AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@StudyLevelGroupId " +
                "AND Entry.Id NOT IN (SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL) " +
                "AND IsParallel=@IsParallel AND IsReduced=@IsReduced AND DateOfClose>GETDATE() AND CampaignYear=@Year AND SemesterId=@SemesterId";

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@ObrazProgramId", iObrazProgramId);
            dic.Add("@StudyLevelGroupId", iEntryId == 2 ? 2 : 1);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId);

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
