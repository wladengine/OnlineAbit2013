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
    public class AGController : Controller
    {
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
                case 7: { return RedirectToAction("Index", "ChangeObrazProgram", new RouteValueDictionary() { { "step", step }}); }
                case 8: { break; }
                default: { return RedirectToAction("Index", "Abiturient", new RouteValueDictionary() { { "step", step }}); }
            }

            int stage = 0;
            if (!int.TryParse(step, out stage))
                stage = 1;

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                string query;
                //"SELECT Id, Surname, Name, SecondName, BirthPlace, BirthDate, Sex, NationalityId, PassportTypeId, PassportSeries, PassportNumber, PassportAuthor, " +
                //          "PassportDate, PassportCode, Phone, Mobiles, CountryId, RegionId, PostCode, City, Street, House, Korpus, Flat, PostCodeReal, CityReal, StreetReal, " +
                //          "HouseReal, KorpusReal, FlatReal, AddInfo, [Privileges], SchoolTypeId, SchoolName, SchoolAddress, SchoolExitClassId, AvgMark, " +
                //          "EducationDocumentSeries, EducationDocumentNumber, Parents, AbitHostel, RegistrationStage, CountryEducId FROM Person WHERE Id=@Id";
                
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (Person == null)//paranoia
                    return RedirectToAction("OpenPersonalAccount");

                if (Person.RegistrationStage == 0)
                    stage = 1;
                else if (Person.RegistrationStage < stage)
                    stage = Person.RegistrationStage;

                PersonalOfficeAG model = new PersonalOfficeAG() { Lang = "ru", Stage = stage != 0 ? stage : 1, Enabled = !Util.CheckPersonReadOnlyStatus_AG(PersonId) };
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
                    DataTable tblPsp = Util.AbitDB.GetDataTable("SELECT Id, Name FROM PassportType WHERE 1=@x", new Dictionary<string, object>() { { "@x", 1 } });
                    model.PassportInfo.PassportTypeList =
                        (from DataRow rw in tblPsp.Rows
                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = rw.Field<string>("Name") }).
                        ToList();

                    model.PassportInfo.PassportType = (Person.PassportTypeId ?? 1).ToString();
                    model.PassportInfo.PassportSeries = Server.HtmlDecode(Person.PassportSeries);
                    model.PassportInfo.PassportNumber = Server.HtmlDecode(Person.PassportNumber);
                    model.PassportInfo.PassportAuthor = Server.HtmlDecode(Person.PassportAuthor);
                    model.PassportInfo.PassportDate = Person.PassportDate.HasValue ? Person.PassportDate.Value.ToString("dd.MM.yyyy") : "";
                    model.PassportInfo.PassportCode = Server.HtmlDecode(Person.PassportCode);
                }
                else if (model.Stage == 3)
                {
                    model.ContactsInfo = new ContactsPerson();
                    var PersonContacts = Person.PersonContacts;
                    if (PersonContacts == null)
                        PersonContacts = new PersonContacts();

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
                }
                else if (model.Stage == 4)
                {
                    model.PersonSchoolInfo = new Models.PersonSchoolInfo();

                    var PersonEducationDocument = Person.PersonEducationDocument;
                    var PersonSchoolInfo = Person.PersonSchoolInfo;

                    if (PersonEducationDocument == null)
                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
                    if (PersonSchoolInfo == null)
                        PersonSchoolInfo = new OnlineAbit2013.PersonSchoolInfo();

                    query = "SELECT Id, Name FROM AG_SchoolType";
                    model.PersonSchoolInfo.SchoolTypeList = 
                        (from DataRow rw in Util.AbitDB.GetDataTable(query, null).Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();
                    model.PersonSchoolInfo.CountryList = Util.CountriesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    
                    
                    model.PersonSchoolInfo.SchoolExitClassList = context.SchoolExitClass.OrderByDescending(x => x.Name).Select(x => new { x.Id, x.Name }).ToList()
                        .Select(x => new SelectListItem() {
                            Value = x.Id.ToString(),
                            Text = x.Name
                         }).ToList();

                    model.PersonSchoolInfo.SchoolName = Server.HtmlDecode(PersonEducationDocument.SchoolName);
                    model.PersonSchoolInfo.SchoolAddress = Server.HtmlDecode(PersonSchoolInfo.SchoolAddress);
                    model.PersonSchoolInfo.SchoolExitClassId = (PersonSchoolInfo.SchoolExitClassId ?? 1).ToString();
                    model.PersonSchoolInfo.DiplomSeries = Server.HtmlDecode(PersonEducationDocument.Series);
                    model.PersonSchoolInfo.DiplomNumber = Server.HtmlDecode(PersonEducationDocument.Number);
                    model.PersonSchoolInfo.AvgMark = Server.HtmlDecode(PersonEducationDocument.AvgMark.HasValue ? PersonEducationDocument.AvgMark.Value.ToString() : "");

                    model.PersonSchoolInfo.SchoolTypeId = (PersonSchoolInfo.SchoolTypeId ?? 1).ToString();
                    model.PersonSchoolInfo.CountryEducId = (PersonEducationDocument.CountryEducId ?? 193).ToString();
                }
                else if (model.Stage == 5)
                {
                    model.PrivelegeInfo = new PersonPrivileges();

                    query = "SELECT Id, Name FROM AG_PrivilegeType WHERE Id NOT IN (SELECT PrivilegeTypeId FROM AG_PersonPrivilege WHERE PersonId=@PersonId)";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

                    model.PrivelegeInfo.PrivilegesList =
                        (from DataRow rw in tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    string qPSW = "SELECT AG_PersonPrivilege.Id, AG_PersonPrivilege.DocumentNumber, AG_PrivilegeType.Name FROM AG_PersonPrivilege " +
                        " INNER JOIN AG_PrivilegeType ON AG_PrivilegeType.Id=AG_PersonPrivilege.PrivilegeTypeId WHERE AG_PersonPrivilege.PersonId=@Id";
                    DataTable tblPSW = Util.AbitDB.GetDataTable(qPSW, new Dictionary<string, object>() { { "@Id", PersonId } });

                    model.PrivelegeInfo.pPrivileges =
                        (from DataRow rw in tblPSW.Rows
                         select new PrivilegeInformation()
                         {
                             Id = rw.Field<Guid>("Id"),
                             PrivilegeType = rw.Field<string>("Name"),
                             PrivilegeInfo = rw.Field<string>("DocumentNumber")
                         }).ToList();

                    query = "SELECT AG_Olympiads.Id, AG_OlympType.Name, AG_Olympiads.DocumentSeries, AG_Olympiads.DocumentNumber, AG_Olympiads.DocumentDate" +
                        " FROM AG_Olympiads INNER JOIN AG_OlympType ON AG_OlympType.Id=AG_Olympiads.OlympTypeId WHERE AG_Olympiads.PersonId=@Id";
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
                    //query = "SELECT Id, Name FROM AG_OlympType";
                    //_tbl = Util.AbitDB.GetDataTable(query, null);
                    //model.PrivelegeInfo.OlympiadsList =
                    //    (from DataRow rw in _tbl.Rows
                    //     select new SelectListItem()
                    //     {
                    //         Value = rw.Field<int>("Id").ToString(),
                    //         Text = rw.Field<string>("Name")
                    //     }).ToList();

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
        public ActionResult NextStep(PersonalOfficeAG model)
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

                if (Util.CheckPersonReadOnlyStatus_AG(PersonId))
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

                    context.SaveChanges();
                }
                else if (model.Stage == 2)
                {
                    int iPassportType = 1;
                    if (!int.TryParse(model.PassportInfo.PassportType, out iPassportType))
                        iPassportType = 1;

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
                        iRegionId = 3;//Russia

                    if (iCountryId != 193)
                    {
                        int? altRegionId = context.Country.Where(x => x.Id == iCountryId).Select(x => x.RegionId).FirstOrDefault();
                        if (altRegionId.HasValue)
                            iRegionId = altRegionId.Value;//RegionValue
                        else
                            iRegionId = 11;//Далн. зарубеж.
                    }

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
                    int SchoolExitYear;
                    if (!int.TryParse(model.PersonSchoolInfo.SchoolTypeId, out iSchoolTypeId))
                        iSchoolTypeId = 1;
                    if (!int.TryParse(model.PersonSchoolInfo.SchoolExitYear, out SchoolExitYear))
                        SchoolExitYear = DateTime.Now.Year;
                    int iCountryEducId;
                    if (!int.TryParse(model.PersonSchoolInfo.CountryEducId, out iCountryEducId))
                        iCountryEducId = 1;
                    
                    var PersonEducationDocument = Person.PersonEducationDocument;
                    var PersonSchoolInfo = Person.PersonSchoolInfo;

                    //-----------------PersonEducationDocument---------------------
                    bool bIns = false;
                    if (PersonEducationDocument == null)
                    {
                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
                        PersonEducationDocument.PersonId = PersonId;
                        bIns = true;
                    }

                    PersonEducationDocument.SchoolTypeId = 1;
                    PersonEducationDocument.SchoolName = model.PersonSchoolInfo.SchoolName;
                    PersonEducationDocument.SchoolNum = model.PersonSchoolInfo.SchoolNumber;
                    PersonEducationDocument.SchoolCity = model.PersonSchoolInfo.SchoolCity;
                    PersonEducationDocument.SchoolExitYear = model.PersonSchoolInfo.SchoolExitYear;
                    PersonEducationDocument.CountryEducId = iCountryEducId;

                    PersonEducationDocument.Series = model.PersonSchoolInfo.DiplomSeries;
                    PersonEducationDocument.Number = model.PersonSchoolInfo.DiplomNumber;

                    if (bIns)
                    {
                        context.PersonEducationDocument.AddObject(PersonEducationDocument);
                        bIns = false;
                    }

                    if (PersonSchoolInfo == null)
                    {
                        PersonSchoolInfo = new PersonSchoolInfo();
                        PersonSchoolInfo.PersonId = PersonId;
                        bIns = true;
                    }

                    int iExitClass;
                    if (!int.TryParse(model.PersonSchoolInfo.SchoolExitClassId, out iExitClass))
                        iExitClass = 1;

                    PersonSchoolInfo.SchoolAddress = model.PersonSchoolInfo.SchoolAddress;
                    PersonSchoolInfo.SchoolExitClassId = iExitClass;
                    PersonSchoolInfo.SchoolTypeId = iSchoolTypeId;

                    if (bIns)
                    {
                        context.PersonSchoolInfo.AddObject(PersonSchoolInfo);
                        bIns = false;
                    }

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

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonID).FirstOrDefault();
                if (PersonInfo == null)
                    return RedirectToAction("Index");

                //СДЕЛАТЬ ЗАХОД В НУЖНЫЙ КОНТРОЛЛЕР!!!
                switch (PersonInfo.AbiturientTypeId)
                {
                    case 1: { return RedirectToAction("Main", "Abiturient"); }
                    case 2: { return RedirectToAction("Main", "ForeignAbiturient"); }
                    case 3: { return RedirectToAction("Main", "Transfer"); }
                    case 4: { return RedirectToAction("Main", "TransferForeign"); }
                    case 5: { return RedirectToAction("Main", "Recover"); }
                    case 6: { return RedirectToAction("Main", "ChangeStudyForm"); }
                    case 7: { return RedirectToAction("Main", "ChangeObrazProgram"); }
                    case 8: { break; }
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

                query = "SELECT [AG_Application].Id, ProgramName, ObrazProgramName, ProfileName, Priority, Enabled FROM [AG_Application] " +
                    "INNER JOIN AG_qEntry ON [AG_Application].EntryId=AG_qEntry.Id WHERE PersonId=@PersonId";
                dic.Clear();
                dic.Add("@PersonId", PersonID);
                tbl = Util.AbitDB.GetDataTable(query, dic);
                foreach (DataRow rw in tbl.Rows)
                {
                    model.Applications.Add(new SimpleApplication()
                    {
                        Id = rw.Field<Guid>("Id"),
                        Profession = rw.Field<string>("ProgramName"),
                        ObrazProgram = rw.Field<string>("ObrazProgramName"),
                        Specialization = rw.Field<string>("ProfileName"),
                        Priority = rw.Field<int?>("Priority").HasValue ? rw.Field<int?>("Priority").Value.ToString() : "1",
                        Enabled = rw.Field<bool?>("Enabled") ?? true,
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

        [OutputCache(NoStore=true, Duration=0)]
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
                    case 8: { break; }
                    default: { return RedirectToAction("NewApplication", "Abiturient"); }
                }

                AG_ApplicationModel model = new AG_ApplicationModel();

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new Dictionary<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                int iAG_EntryClassId = (int)Util.AbitDB.GetValue("SELECT SchoolExitClassId FROM PersonSchoolInfo WHERE PersonId=@Id",
                    new Dictionary<string, object>() { { "@Id", PersonId } });

                string query = "SELECT DISTINCT ProgramId, ProgramName, EntryClassName FROM AG_qEntry WHERE EntryClassId=@ClassId";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@PersonId", PersonId);
                dic.Add("@ClassId", iAG_EntryClassId);
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                model.Professions = (from DataRow rw in tbl.Rows
                                     select new SelectListItem()
                                     {
                                         Value = rw.Field<int>("ProgramId").ToString(),
                                         Text = rw.Field<string>("ProgramName")
                                     }).ToList();
                model.EntryClassId = iAG_EntryClassId;
                model.EntryClassName = tbl.Rows[0].Field<string>("EntryClassName");

                return View("NewApplication", model);
            }
        }

        [HttpPost]
        public ActionResult NewApp()
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

        public ActionResult Application(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return RedirectToAction("LogOn", "Account");

            Guid appId = new Guid();
            if (!Guid.TryParse(id, out appId))
                return RedirectToAction("Main", "Abiturient");

            Dictionary<string, object> prms = new Dictionary<string, object>()
            {
                { "@PersonId", personId },
                { "@Id", appId }
            };

            DataTable tbl =
                Util.AbitDB.GetDataTable("SELECT [AG_Application].Id, ProgramName, Priority, ObrazProgramName, ProfileName, Enabled, DateOfDisable " +
                " FROM [AG_Application] INNER JOIN AG_qEntry ON [AG_Application].EntryId = AG_qEntry.Id WHERE PersonId=@PersonId AND [AG_Application].Id=@Id", prms);

            if (tbl.Rows.Count == 0)
                return RedirectToAction("Main", "Abiturient");

            DataRow rw = tbl.Rows[0];
            var app = new
            {
                Id = rw.Field<Guid>("Id"),
                Profession = rw.Field<string>("ProgramName"),
                ObrazProgram = rw.Field<string>("ObrazProgramName"),
                Specialization = rw.Field<string>("ProfileName"),
                Enabled = rw.Field<bool?>("Enabled"),
                DateOfDisable = rw.Field<DateTime?>("DateOfDisable")
            };

            //prms.Clear();
            //prms.Add("@Profession", app.Profession);
            //prms.Add("@ObrazProgram", app.ObrazProgram);
            //prms.Add("@Specialization", app.Specialization == null ? "" : app.Specialization);
            //prms.Add("@StudyBasisId", app.StudyBasisId);
            //prms.Add("@StudyFormId", app.StudyFormId);

            //tbl = Util.StudDB.GetDataTable(query, prms);

            //var exams = (from DataRow row in tbl.Rows
            //             select row.Field<string>("Exam")
            //             ).ToList();


            string query = "SELECT Id, FileName, FileSize, Comment FROM ApplicationFile WHERE ApplicationId=@AppId";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", appId } });
            var lFiles =
                (from DataRow row in tbl.Rows
                 select new AppendedFile()
                 {
                     Id = row.Field<Guid>("Id"),
                     FileName = row.Field<string>("FileName"),
                     FileSize = row.Field<int>("FileSize"),
                     Comment = row.Field<string>("Comment"),
                     IsShared = false
                 }).ToList();

            query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", personId } });
            var lSharedFiles =
                (from DataRow row in tbl.Rows
                 select new AppendedFile()
                 {
                     Id = row.Field<Guid>("Id"),
                     FileName = row.Field<string>("FileName"),
                     FileSize = row.Field<int>("FileSize"),
                     Comment = row.Field<string>("Comment"),
                     IsShared = true
                 }).ToList();

            var AllFiles = lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();

            ExtApplicationModel model = new ExtApplicationModel()
            {
                Id = app.Id,
                Profession = app.Profession,
                ObrazProgram = app.ObrazProgram,
                Specialization = app.Specialization,
                Enabled = app.Enabled.HasValue ? app.Enabled.Value : false,
                Exams = new List<string>(),
                Files = AllFiles,
                DateOfDisable = app.DateOfDisable.HasValue ? app.DateOfDisable.Value.ToString("dd.MM.yyyy HH:mm:ss") : "",
            };

            return View(model);
        }

        public ActionResult DisableApp(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации! Пожалуйста, повторите вход." };
                return Json(res);
            }

            if (PersonId == Guid.Empty)
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка авторизации! Пожалуйста, повторите вход." };
                return Json(res);
            }

            Guid AppId;
            if (!Guid.TryParse(id, out AppId))
            {
                var res = new { IsOk = false, ErrorMessage = "Некорректный идентификатор. Попробуйте обновить страницу" };
                return Json(res);
            }

            bool? isEnabled = (bool?)Util.AbitDB.GetValue("SELECT Enabled FROM [AG_Application] WHERE Id=@Id AND PersonId=@PersonId",
                new Dictionary<string, object>() { { "@Id", AppId }, { "@PersonId", PersonId } });

            //var app = Util.ABDB.Application.Where(x => x.Id == AppId && x.PersonId == PersonId).FirstOrDefault();
            if (!isEnabled.HasValue)
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка при поиске заявления. Попробуйте обновить страницу" };
                return Json(res);
            }

            if (isEnabled.HasValue && isEnabled.Value == false)
            {
                var res = new { IsOk = false, ErrorMessage = "Заявление уже было отозвано" };
                return Json(res);
            }

            try
            {
                string query = "UPDATE [AG_Application] SET Enabled=@Enabled, DateOfDisable=@DateOfDisable, Priority=@Priority WHERE Id=@Id";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", AppId);
                dic.Add("@DateOfDisable", DateTime.Now);
                dic.Add("@Priority", 0);
                dic.Add("@Enabled", false);

                Util.AbitDB.ExecuteQuery(query, dic);

                var res = new { IsOk = true, Enabled = false };
                return Json(res);
            }
            catch
            {
                var res = new { IsOk = false, ErrorMessage = "Ошибка при поиске заявления. Попробуйте обновить страницу" };
                return Json(res);
            }
        }

        public ActionResult PriorityChanger()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            int regStage = 1;
            if (!Util.CheckPersonRegStatus(PersonId, out regStage))
                return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage } });

            string query = "SELECT [AG_Application].Id, Priority, ProgramName, ObrazProgramName FROM [AG_Application] " + 
                " INNER JOIN AG_Entry ON [AG_Application].AG_EntryId=AG_Entry.Id " +
                " WHERE PersonId=@PersonId AND Enabled=@Enabled ORDER BY Priority";
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
                            Profession = rw.Field<string>("ProgramName"),
                            ObrazProgram = rw.Field<string>("ObrazProgramName")
                        }).ToList();

            MotivateMailModel mdl = new MotivateMailModel()
            {
                Apps = apps,
            };
            return View(mdl);
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
                
                string query = "UPDATE [AG_Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId";
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

        [Authorize]
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

            int regStage = 1;
            if (!Util.CheckPersonRegStatus(PersonId, out regStage))
                return RedirectToAction("Index", new RouteValueDictionary() { { "step", regStage } });

            string query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

            List<AppendedFile> lst =
                (from DataRow rw in tbl.Rows
                 select new AppendedFile() 
                 { 
                     Id = rw.Field<Guid>("Id"), 
                     FileName = rw.Field<string>("FileName"),
                     FileSize = rw.Field<int>("FileSize"),
                     Comment = rw.Field<string>("Comment")
                 }).ToList();

            AppendFilesModel model = new AppendFilesModel() { Files = lst };
            return View(model);
        }

        public ActionResult ErrorMsg()
        {
            string errMessage = Request.Form["ErrText"];
            if (!string.IsNullOrEmpty(errMessage))
            {
                string query = "INSERT INTO ErrorFeedBack(Id, Text) VALUES (@Id, @Text)";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@Text", errMessage);
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            return RedirectToAction("Main");
        }

        [HttpPost]
        public ActionResult AddSharedFile()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("Ошибка авторизации");

            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
            {
                return Json("Файл не приложен или пуст");
            }

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
                string query = "INSERT INTO PersonFile (Id, PersonId, FileName, FileData, FileSize, FileExtention, LoadDate, Comment, MimeType) " +
                    " VALUES (@Id, @PersonId, @FileName, @FileData, @FileSize, @FileExtention, @LoadDate, @Comment, @MimeType)";
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

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }

            return RedirectToAction("AddSharedFiles");
        }

        [HttpPost]
        public ActionResult AddFile()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("Ошибка авторизации");

            if (Request.Files["File"] == null)
                return Json("Файл не приложен");
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

        #region Ajax

        [OutputCache(NoStore = true, Duration = 0)]
        public JsonResult GetProfessions(string classid)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false });
            
            int iEntryClassId;
            int.TryParse(classid, out iEntryClassId);

            string query = "SELECT DISTINCT ProgramId, ProgramName FROM AG_Entry WHERE EntryClassId=@ClassId AND " + 
                " Id NOT IN (SELECT EntryId FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='False')";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@ClassId", iEntryClassId);
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var res = (from DataRow rw in tbl.Rows
                       select new
                       {
                           Id = rw.Field<int>("ProgramId"),
                           Name = rw.Field<string>("ProgramName")
                       });
            return Json(new { IsOk = true, Vals = res });
        }
        public JsonResult CheckProfession(string classid, string programid)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            string query = "SELECT COUNT(Id) FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='True'";
            int cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

            if (cnt >= 2)
                return Json(new { IsOk = false, ErrorMessage = "У абитуриента уже имеется 2 активных заявления" });

            int iClassId;
            int iProgramId;
            if (!int.TryParse(classid, out iClassId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
            if (!int.TryParse(programid, out iProgramId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            query = "SELECT Id FROM AG_Entry WHERE EntryClassId=@ClassId AND ProgramId=@ProgramId AND " +
                "Id NOT IN (SELECT EntryId FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled=@Enabled)";

            DataTable tbl = Util.AbitDB.GetDataTable(query, 
                new Dictionary<string, object>() 
                { 
                    { "@ClassId", iClassId },
                    { "@ProgramId", iProgramId },
                    { "@PersonId", PersonId },
                    { "@Enabled", true }
                });
            if (tbl.Rows.Count > 0)
                return Json(new { IsOk = true, Blocked = false });
            else
                return Json(new { IsOk = true, Blocked = true });
        }
        public JsonResult GetSpecializations(string classid, string programid)
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            int iAG_EntryClassId = 0;
            int iProgramId = 0;

            if (!int.TryParse(classid, out iAG_EntryClassId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
            if (!int.TryParse(programid, out iProgramId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            string query = "SELECT COUNT(Id) FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='True'";
            int cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@PersonId", PersonId } });

            if (cnt >= 2)
                return Json(new { IsOk = false, ErrorMessage = "У абитуриента уже имеется 2 активных заявления" });

            query = "SELECT COUNT(Id) FROM [AG_qAbiturient] WHERE PersonId=@PersonId AND ProgramId<>@ProgramId AND Enabled='True'";
            cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@PersonId", PersonId }, { "@ProgramId", iProgramId } });
            if (cnt > 0)
                return Json(new { IsOk = false, ErrorMessage = "У абитуриента уже подано заявление на другое направление" });

            query = "SELECT COUNT([AG_Application].Id) FROM [AG_Application] INNER JOIN AG_Entry ON AG_Entry.Id=[AG_Application].EntryId WHERE PersonId=@PersonId AND Enabled='True' AND ProgramId=@ProgramId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@ProgramId", iProgramId);
            cnt = (int)Util.AbitDB.GetValue(query, dic);
            if (cnt > 0)
            {
                query = @"SELECT ProgramId, ProfileId, ISNULL(HasManualExams, 'False') AS HasManualExams
from [AG_Entry]
WHERE ProgramId=@ProgramId AND EntryClassId=@EntryClassId
EXCEPT
SELECT ProgramId, ProfileId, ISNULL(HasManualExams, 'False') AS HasManualExams
FROM [AG_Application] INNER JOIN [AG_Entry] ON [AG_Entry].Id=[AG_Application].EntryId
WHERE PersonId = @PersonId AND [AG_Application].[Enabled]='True'
AND ProgramId=@ProgramId AND EntryClassId=@EntryClassId";
                dic.Add("@EntryClassId", iAG_EntryClassId);
                
                DataTable _tbl = Util.AbitDB.GetDataTable(query, dic);
                if (_tbl.Rows.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = "Заявление уже подавалось" });
            }

            query = @"SELECT ProfileId, ProfileName FROM AG_qEntry WHERE EntryClassId=@EntryClassId AND ProgramId=@ProgramId AND ProfileId IS NOT NULL";
            dic.Clear();
            dic.Add("@EntryClassId", iAG_EntryClassId);
            dic.Add("@ProgramId", iProgramId);
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count > 1)
            {
                var vals =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Id = rw.Field<int>("ProfileId"),
                         Name = rw.Field<string>("ProfileName")
                     });

                return Json(new { IsOk = true, Data = vals.ToList() });
            }
            else
            {
                query = "SELECT ISNULL(HasManualExams, 'False') AS HasManualExams FROM AG_Entry WHERE EntryClassId=@EntryClassId AND ProgramId=@ProgramId";
                dic.Clear();
                dic.Add("@EntryClassId", iAG_EntryClassId);
                dic.Add("@ProgramId", iProgramId);
                tbl = Util.AbitDB.GetDataTable(query, dic);

                bool HasProfileExams = (from DataRow rw in tbl.Rows
                            select rw.Field<bool>("HasManualExams")).FirstOrDefault();

                query = @"SELECT AG_ManualExam.Id, AG_ManualExam.Name
  FROM [AG_Entry]
  INNER JOIN AG_ManualExamInAG_Entry ON AG_ManualExamInAG_Entry.EntryId = [AG_Entry].Id
  INNER JOIN AG_ManualExam ON AG_ManualExam.Id = AG_ManualExamInAG_Entry.ExamId
  WHERE EntryClassId=@EntryClassId AND ProgramId=@ProgramId";

                tbl = Util.AbitDB.GetDataTable(query, dic);
                var Exams = (from DataRow rw in tbl.Rows
                            select new 
                            {
                                Value = rw.Field<int>("Id").ToString(),
                                Name = rw.Field<string>("Name")
                            }).DefaultIfEmpty();
                return Json(new { IsOk = true, HasProfileExams = HasProfileExams, Exams = Exams });
            }
        }
        public JsonResult CheckSpecializations(string classid, string programid, string specid)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = "Пользователь не авторизирован" });

            int iEntryClassId = 0;
            int iProgramId = 0;
            int iProfileId = 0;

            int.TryParse(classid, out iEntryClassId);
            int.TryParse(programid, out iProgramId);
            int.TryParse(specid, out iProfileId);

            string query = "SELECT COUNT(Id) FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled='True'";
            int cnt = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@PersonId" , PersonId } });
            if (cnt >= 2)
                return Json(new { IsOk = false, ErrorMessage = "У абитуриента уже имеется 2 активных заявления" });

            query = @"SELECT COUNT([AG_Application].Id) FROM [AG_Application] INNER JOIN AG_qEntry ON AG_qEntry.Id = [AG_Application].EntryId 
WHERE [AG_Application].PersonId=@PersonId AND Enabled='True' AND EntryClassId=@EntryClassId AND ProgramId=@ProgramId AND ProfileId=@ProfileId";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@EntryClassId", iEntryClassId);
            dic.Add("@ProgramId", iProgramId);
            dic.Add("@ProfileId", iProfileId);

            cnt = (int)Util.AbitDB.GetValue(query, dic);
            if (cnt > 0)
                return Json(new { IsOk = false, ErrorMessage = "Заявление уже подавалось" });
            else
            {
                query = "SELECT ISNULL(HasManualExams, 'False') AS HasManualExams FROM AG_Entry WHERE EntryClassId=@EntryClassId AND ProgramId=@ProgramId " +
                    "AND ProfileId=@ProfileId";
                return Json(new { IsOk = true });
            }
        }

        public JsonResult AddPrivelege(string id, string docNum)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });
            int iPrivelegeId;
            if (!int.TryParse(id, out iPrivelegeId))
                return Json(new { IsOk = false, ErrorMessage = "Неверная льгота" });
            if (string.IsNullOrEmpty(docNum))
                return Json(new { IsOk = false, ErrorMessage = "Не указан номер документа" });

            string query = "INSERT INTO AG_PersonPrivilege(Id, PersonId, PrivilegeTypeId, DocumentNumber) VALUES" +
                "(@Id, @PersonId, @PrivilegeTypeId, @DocumentNumber)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", Guid.NewGuid());
            dic.Add("@PersonId", PersonId);
            dic.Add("@PrivilegeTypeId", iPrivelegeId);
            dic.Add("@DocumentNumber", docNum);
            Util.AbitDB.ExecuteQuery(query, dic);
            query = "SELECT Name FROM AG_PrivilegeType WHERE Id=@Id";
            string res = Util.AbitDB.GetStringValue(query, new Dictionary<string, object>() { { "@Id", iPrivelegeId } });
            return Json(new { IsOk = true, Id = ((Guid)dic["@Id"]).ToString("N"), Type = res, Doc = Server.HtmlDecode(docNum) });
        }
        public JsonResult DeletePrivilege(string id)
        {
            Guid PersonId;
            Guid PrivId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });
            if (!Guid.TryParse(id,  out PrivId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });
            string query = "DELETE FROM AG_PersonPrivilege WHERE Id=@Id";
            Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", PrivId } });
            return Json(new { IsOk = true });
        }

        public JsonResult AddOlympiad(string id, string Series, string Number, string Date)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            int iOlympId;
            if (!int.TryParse(id, out iOlympId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            DateTime _date;
            if (!DateTime.TryParse(Date, out _date))
                _date = DateTime.Now;

            string query = "INSERT INTO AG_Olympiads(Id, PersonId, OlympTypeId, DocumentSeries, DocumentNumber, DocumentDate) VALUES " +
                "(@Id, @PersonId, @OlympTypeId, @DocumentSeries, @DocumentNumber, @DocumentDate)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", Guid.NewGuid());
            dic.Add("@PersonId", PersonId);
            dic.Add("@OlympTypeId", iOlympId);
            dic.Add("@DocumentSeries", Series);
            dic.Add("@DocumentNumber", Number);
            dic.Add("@DocumentDate", _date);

            Util.AbitDB.ExecuteQuery(query, dic);

            query = "SELECT Name FROM AG_OlympType WHERE Id=@Id";
            string type = Util.AbitDB.GetStringValue(query, new Dictionary<string, object>() { { "@Id", iOlympId } });
            
            return Json(new { IsOk = true, Id = ((Guid)dic["@Id"]).ToString("N"), Type = type, 
                Doc = Series + " " + Number + " от " + _date.ToShortDateString() });
        }
        public JsonResult DeleteOlympiad(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid OlympId;
            if (!Guid.TryParse(id, out OlympId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            string query = "DELETE FROM AG_Olympiads WHERE Id=@Id";
            Util.AbitDB.ExecuteQuery(query, new Dictionary<string, object>() { { "@Id", OlympId } });
            return Json(new { IsOk = true });
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
                return Json(new { IsOk = false, ErrorMessage="" });

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

        #endregion
    }
}
