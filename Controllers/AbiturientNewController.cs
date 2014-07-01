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
    public class AbiturientNewController : Controller
    {
        int maxBlockMag = 50;
        int maxBlock1kurs = 7;
        int maxBlockSPO = 6;
        int maxBlockAspirant = 6;
        int maxBlockRecover = 1;
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
            int cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", UserId } });
            if (cnt == 0)
            {
                query = "INSERT INTO Person(Id, UserId, AbiturientTypeId) VALUES (@Id, @Id, @Type)";
                Util.AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Id", UserId }, { "@Type", res} });
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
            
            bool isEng = Util.GetCurrentThreadLanguageIsEng(); 

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
                model.ConstInfo = new Constants();
                model.ConstInfo = Util.getConstInfo();
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
                    model.PersonInfo.NationalityList = Util.GetCountryList();
                    model.ContactsInfo.CountryList   = Util.GetCountryList();
                    model.PersonInfo.HasRussianNationality = Person.HasRussianNationality;
                    model.PersonInfo.SexList = new List<SelectListItem>()
                    {
                        new SelectListItem () {Text = isEng ? "Male" : "Мужской", Value = "M" },
                        new SelectListItem () {Text = isEng ? "Female" : "Женский", Value = "F" }
                    };

                    return View("PersonalOffice_Page1", model);
                }
                //////////////////////////////////////////----------Index-----////////////////////////////////////////////////////////////////////////
                else if (model.Stage == 2)
                {
                    model.PersonInfo = new InfoPerson();
                    model.PassportInfo = new PassportPerson();
                    model.res = Util.GetRess(PersonId);
                    string strTblPsp;
                    int defaultPsp = 1;
                    switch (model.res)
                    {
                        case 1: { strTblPsp = "SELECT Id, Name, NameEng FROM PassportType "; break; }
                        case 2: { strTblPsp = "SELECT Id, Name, NameEng FROM PassportType "; break; }
                        case 3: { strTblPsp = "SELECT Id, Name, NameEng FROM PassportType "; break; }
                        case 4: { strTblPsp = "SELECT Id, Name, NameEng FROM PassportType WHERE IsApprovedForeign=1"; defaultPsp = 2; break; }
                        default: { strTblPsp = "SELECT Id, Name, NameEng FROM PassportType "; break; }
                    }
                    DataTable tblPsp = Util.AbitDB.GetDataTable(strTblPsp, null);
                    model.PassportInfo.PassportTypeList =
                        (from DataRow rw in tblPsp.Rows
                         select new SelectListItem() { Value = rw.Field<int>("Id").ToString(), Text = isEng? rw.Field<string>("NameEng") : rw.Field<string>("Name") }).
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
                                string.Format("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, {0}", isEng ? "NameEng" : "Name"),
                                null);
                        model.VisaInfo.CountryList = (from DataRow rw in tblCountr.Rows
                                                      select new SelectListItem()
                                                      {
                                                          Value = rw.Field<int>("Id").ToString(),
                                                          Text = rw.Field<string>(isEng ? "NameEng" : "Name")
                                                      }).ToList();

                        var PersonVisaInfo = Person.PersonVisaInfo;
                        if (PersonVisaInfo == null)
                            PersonVisaInfo = new PersonVisaInfo();
                        model.VisaInfo.CountryId = PersonVisaInfo.CountryId.ToString();
                        model.VisaInfo.PostAddress = PersonVisaInfo.PostAddress;
                        model.VisaInfo.Town = PersonVisaInfo.Town;
                    }
                    model.PersonInfo.SNILS = Person.SNILS ?? "";
                    model.Files = Util.GetFileList(PersonId, "1");
                    model.FileTypes = Util.GetPersonFileTypeList();

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

                    model.ContactsInfo.CountryList = Util.GetCountryList();

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
               /////////////////////////////////////////////////////////////////////////////////////////////
                else if (model.Stage == 4)
                {
                    model.EducationInfo = new EducationPerson();
                    var PersonEducationDocument = Person.PersonEducationDocument;
                    var PersonHighEducationInfo = Person.PersonHighEducationInfo;

                    if (PersonEducationDocument == null)
                        PersonEducationDocument = new OnlineAbit2013.PersonEducationDocument();
                    if (PersonHighEducationInfo == null)
                        PersonHighEducationInfo = new OnlineAbit2013.PersonHighEducationInfo();

                    model.EducationInfo.QualificationList = Util.GetQualificationList(isEng);
                    model.EducationInfo.SchoolTypeList = Util.SchoolTypesAll.Select(x => new SelectListItem() { Value = x.Key.ToString(), Text = x.Value }).ToList();
                    model.EducationInfo.StudyFormList = Util.GetStudyFormList();
                    model.EducationInfo.StudyBasisList = Util.GetStudyBasisList();
                    model.EducationInfo.LanguageList = Util.GetLanguageList();
                    model.EducationInfo.CountryList = Util.GetCountryList();

                    query = "SELECT Id, Name, NameEng FROM SchoolTypeAll";
                    DataTable _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.EducationInfo.SchoolTypeList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = (isEng ?
                                        (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                                        : rw.Field<string>("Name"))
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
                    model.EducationInfo.SchoolExitClassList = context.SchoolExitClass.OrderBy(x => x.IntValue)
                        .Select(x => new { x.Id, x.Name }).ToList()
                        .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

                    model.EducationInfo.VuzAdditionalTypeList = isEng? context.VuzAdditionalType.Select(x => new { x.Id, x.NameEng }).ToList()
                        .Select(x => new SelectListItem() { Text = x.NameEng, Value = x.Id.ToString() }).ToList(): 
                    context.VuzAdditionalType.Select(x => new { x.Id, x.Name }).ToList()
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

                    string qEgeMarks = "SELECT EgeMark.Id, EgeCertificate.Number, EgeExam.Name, EgeMark.Value, EgeMark.IsSecondWave, EgeMark.IsInUniversity FROM Person " +
                        " INNER JOIN EgeCertificate ON EgeCertificate.PersonId = Person.Id INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
                        " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE Person.Id=@Id";
                    DataTable tblEge = Util.AbitDB.GetDataTable(qEgeMarks, new SortedList<string, object>() { { "@Id", PersonId } });

                    model.EducationInfo.EgeMarks = new List<EgeMarkModel>();
                    model.EducationInfo.EgeMarks =
                        (from DataRow rw in tblEge.Rows
                         select new EgeMarkModel()
                         {
                             Id = rw.Field<Guid>("Id"),
                             CertificateNum = rw.Field<string>("Number"),
                             ExamName = rw.Field<string>("Name"),
                             Value = rw.Field<bool>("IsSecondWave") ? ("Сдаю во второй волне") : (rw.Field<bool>("IsInUniversity") ? "Сдаю в СПбГУ" : rw.Field<int?>("Value").ToString())
                         }).ToList();
                    ////////////////////////////////////////////////
                    model.CurrentEducation = new CurrentEducation();
                    PersonCurrentEducation CurrentEducation = Person.PersonCurrentEducation;
                    if (CurrentEducation == null)
                        CurrentEducation = new PersonCurrentEducation();
                    model.CurrentEducation.EducationTypeList = model.EducationInfo.SchoolTypeList;

                    model.CurrentEducation.SemesterList = Util.GetSemestrList() ;

                    query = "SELECT Id, Name FROM SP_StudyLevel";
                    _tblT = Util.AbitDB.GetDataTable(query, null);
                    model.CurrentEducation.StudyLevelList =
                        (from DataRow rw in _tblT.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    model.CurrentEducation.StudyLevelId = CurrentEducation.StudyLevelId.ToString();
                    model.CurrentEducation.StudyFormId = CurrentEducation.StudyFormId ?? 1;
                    model.CurrentEducation.SemesterId = CurrentEducation.SemesterId.ToString();
                    model.CurrentEducation.StudyBasisId = CurrentEducation.StudyBasisId ?? 1;
                    model.CurrentEducation.HiddenLicenseProgramId = CurrentEducation.LicenseProgramId.ToString();
                    model.CurrentEducation.HiddenObrazProgramId = CurrentEducation.ObrazProgramId.ToString();
                    model.CurrentEducation.ProfileName = CurrentEducation.ProfileName;

                    model.CurrentEducation.HasAccreditation = CurrentEducation.HasAccreditation ?? false;
                    model.CurrentEducation.AccreditationDate = CurrentEducation.AccreditationDate.HasValue ? CurrentEducation.AccreditationDate.Value.ToShortDateString() : "";
                    model.CurrentEducation.AccreditationNumber = CurrentEducation.AccreditationNumber;
                    model.CurrentEducation.EducationTypeId = CurrentEducation.EducTypeId.ToString();
                    model.CurrentEducation.EducationName = CurrentEducation.EducName;
                    model.CurrentEducation.HasScholarship = CurrentEducation.HasScholarship ?? false;
                    /////
                    model.ChangeStudyFormReason = new PersonChangeStudyFormReason();
                    PersonChangeStudyFormReason ChangeStudyFormReason = Person.PersonChangeStudyFormReason;
                    if (ChangeStudyFormReason == null)
                        ChangeStudyFormReason = new PersonChangeStudyFormReason();
                    model.ChangeStudyFormReason.Reason = ChangeStudyFormReason.Reason;
                    /////////////////////////////////////////////////
                    model.DisorderInfo = new DisorderedSPBUEducation();
                    if (Person.PersonDisorderInfo != null)
                    {
                        model.DisorderInfo.YearOfDisorder = Person.PersonDisorderInfo.YearOfDisorder;
                        model.DisorderInfo.EducationProgramName = Person.PersonDisorderInfo.EducationProgramName;
                        model.DisorderInfo.IsForIGA = Person.PersonDisorderInfo.IsForIGA;
                    }
                    else
                    {
                        model.DisorderInfo.YearOfDisorder = "";
                        model.DisorderInfo.EducationProgramName = "";
                        model.DisorderInfo.IsForIGA = false;
                    }
                    /////////////////////////////////////////////////
                    return View("PersonalOffice_Page4", model);
                }
                else if (model.Stage == 5)
                {
                    model.WorkInfo = new WorkPerson();

                    query = "SELECT Id, Name, NameEng FROM ScienceWorkType";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, null);

                    model.WorkInfo.ScWorks =
                        (from DataRow rw in tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>(isEng ? "NameEng" : "Name")
                         }).ToList();
                    //Util.ScienceWorkTypeAll.Select(x => new SelectListItem() { Text = x.Value, Value = x.Key.ToString() }).ToList();

                    string qPSW = "SELECT PersonScienceWork.Id, ScienceWorkType.Name, ScienceWorkType.NameEng, PersonScienceWork.WorkInfo FROM PersonScienceWork " +
                        " INNER JOIN ScienceWorkType ON ScienceWorkType.Id=PersonScienceWork.WorkTypeId WHERE PersonScienceWork.PersonId=@Id";
                    DataTable tblPSW = Util.AbitDB.GetDataTable(qPSW, new SortedList<string, object>() { { "@Id", PersonId } });

                    model.WorkInfo.pScWorks =
                        (from DataRow rw in tblPSW.Rows
                         select new ScienceWorkInformation()
                         {
                             Id = rw.Field<Guid>("Id"),
                             ScienceWorkType = rw.Field<string>(isEng? "NameEng":"Name"),
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

                    model.PrivelegeInfo = new PersonPrivileges();
                    model.PrivelegeInfo.pOlympiads = context.Olympiads.Where(x => x.PersonId == PersonId)
                        .Select(x => new OlympiadInformation()
                        {
                            Id = x.Id,
                            OlympType = x.OlympType.Name,
                            OlympName = x.OlympName.Name,
                            OlympSubject = x.OlympSubject.Name,
                            OlympValue = x.OlympValue.Name,
                            DocumentSeries = x.DocumentSeries,
                            DocumentNumber = x.DocumentNumber,
                            DocumentDate = x.DocumentDate.HasValue ? x.DocumentDate.Value : DateTime.Now
                        }).ToList();

                    query = "SELECT Id, Name FROM OlympName";
                    DataTable _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympNameList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = rw.Field<string>("Name")
                         }).ToList();

                    query = "SELECT Id, Name, NameEng FROM OlympType";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympTypeList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(), 
                             Text =  (isEng ?
                                        (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng")) 
                                        : rw.Field<string>("Name"))
                         }).ToList();

                    query = "SELECT Id, Name, NameEng FROM OlympSubject";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympSubjectList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = (isEng ?
                                        (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                                        : rw.Field<string>("Name"))
                         }).ToList();

                    query = "SELECT Id, Name, NameEng FROM OlympValue";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.OlympValueList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = (isEng ?
                                        (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                                        : rw.Field<string>("Name"))
                         }).ToList();

                    query = "SELECT Id, Name, NameEng FROM SportQualification";
                    _tbl = Util.AbitDB.GetDataTable(query, null);
                    model.PrivelegeInfo.SportQualificationList =
                        (from DataRow rw in _tbl.Rows
                         select new SelectListItem()
                         {
                             Value = rw.Field<int>("Id").ToString(),
                             Text = (isEng ?
                                        (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                                        : rw.Field<string>("Name"))
                         }).ToList();

                    PersonSportQualification PersonSportQualification = Person.PersonSportQualification;
                    if (PersonSportQualification == null)
                        PersonSportQualification = new OnlineAbit2013.PersonSportQualification();
                    else
                    {
                        model.PrivelegeInfo.SportQualificationId = (PersonSportQualification.SportQualificationId ?? 0).ToString();
                        model.PrivelegeInfo.SportQualificationLevel = PersonSportQualification.SportQualificationLevel ?? "";
                        model.PrivelegeInfo.OtherSportQualification = PersonSportQualification.OtherSportQualification ?? "";
                    }
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
                        HostelEduc = AddInfo.HostelEduc,
                        ContactPerson = Server.HtmlDecode(AddInfo.Parents),
                        ReturnDocumentTypeId = Server.HtmlDecode((AddInfo.ReturnDocumentTypeId ?? 1).ToString()),
                        ReturnDocumentTypeList = isEng ? 
                            context.ReturnDocumentType.Select(x => new { x.Id, x.NameEng }).ToList().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.NameEng }).ToList() : 
                            context.ReturnDocumentType.Select(x => new { x.Id, x.Name }).ToList().Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList() 
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

                    if (iCountryId != 193)
                    {
                        PersonContacts.RegionId = context.Country.Where(x => x.Id == iCountryId).Select(x => x.RegionId).DefaultIfEmpty(1).First();
                    }

                    Person.HasRussianNationality = (NationalityId==193)? true :model.PersonInfo.HasRussianNationality;

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

                    Person.SNILS = model.PersonInfo.SNILS;

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
                    PersonEducationDocument.CountryEducId = ((iVuzAddTypeId == 2) || (iVuzAddTypeId ==3)) ? 193 : iCountryEducId;
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

                    PersonEducationDocument.RegionEducId = ((iVuzAddTypeId == 2) || (iVuzAddTypeId == 3)) ? 1 : iRegionEducId;
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
                    {
                        context.PersonHighEducationInfo.AddObject(PersonHighEducationInfo);
                        bIns = false;
                    }

                    //-----------------PersonCurrentEducation---------------------
                    if (iVuzAddTypeId != 1)
                    {
                        bIns = false;
                        PersonCurrentEducation PersonCurrentEducation = Person.PersonCurrentEducation;
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
                        int iLicenseProgramId = 1;
                        if (!int.TryParse(model.CurrentEducation.HiddenLicenseProgramId, out iLicenseProgramId))
                            iLicenseProgramId = 1;//default value
                        int iObrazProgramId = 1;
                        if (!int.TryParse(model.CurrentEducation.HiddenObrazProgramId, out iObrazProgramId))
                            iObrazProgramId = 1;//default value

                        if (PersonCurrentEducation == null)
                        {
                            PersonCurrentEducation = new PersonCurrentEducation();
                            PersonCurrentEducation.PersonId = PersonId;
                            bIns = true;
                        }
                        PersonCurrentEducation.EducTypeId = iEducTypeId;
                        PersonCurrentEducation.SemesterId = iSemesterId;
                        PersonCurrentEducation.AccreditationDate = dtAccreditation;
                        PersonCurrentEducation.AccreditationNumber = model.CurrentEducation.AccreditationNumber;

                        PersonCurrentEducation.EducName = model.CurrentEducation.EducationName;
                        PersonCurrentEducation.HasAccreditation = model.CurrentEducation.HasAccreditation;
                        PersonCurrentEducation.HasScholarship = model.CurrentEducation.HasScholarship;
                        PersonCurrentEducation.StudyLevelId = iStudyLevelId;
                        PersonCurrentEducation.StudyFormId = model.CurrentEducation.StudyFormId;
                        PersonCurrentEducation.StudyBasisId = model.CurrentEducation.StudyBasisId;

                        PersonCurrentEducation.LicenseProgramId = iLicenseProgramId;
                        PersonCurrentEducation.ObrazProgramId = iObrazProgramId;

                        PersonCurrentEducation.CountryId = ((iVuzAddTypeId == 2) || (iVuzAddTypeId == 3)) ? 193 : iCountryEducId;
                        PersonCurrentEducation.ProfileName = model.CurrentEducation.ProfileName;


                        if (bIns)
                        {
                            context.PersonCurrentEducation.AddObject(PersonCurrentEducation);
                            bIns = false;
                        }
                        
                        if (iVuzAddTypeId == 2)
                        {
                            bIns = false;
                            PersonChangeStudyFormReason ChangeStudyFormReason = Person.PersonChangeStudyFormReason;
                            if (ChangeStudyFormReason == null)
                            {
                                ChangeStudyFormReason = new PersonChangeStudyFormReason();
                                ChangeStudyFormReason.PersonId = PersonId;
                                bIns = true;
                            }

                            ChangeStudyFormReason.Reason = model.ChangeStudyFormReason.Reason;
                            if (bIns)
                            {
                                context.PersonChangeStudyFormReason.AddObject(ChangeStudyFormReason);
                                bIns = false;
                            }
                        } 
                    }
                    //-----------------PersonDisorderInfo---------------------
                    if (iVuzAddTypeId == 3)
                    {
                        bIns = false;
                        PersonDisorderInfo PersonDisorderEducation = Person.PersonDisorderInfo;
                        if (PersonDisorderEducation == null)
                        {
                            PersonDisorderEducation = new PersonDisorderInfo();
                            PersonDisorderEducation.PersonId = PersonId;
                            bIns = true;
                        }
                        PersonDisorderEducation.IsForIGA = model.DisorderInfo.IsForIGA;
                        PersonDisorderEducation.YearOfDisorder = model.DisorderInfo.YearOfDisorder;
                        PersonDisorderEducation.EducationProgramName = model.DisorderInfo.EducationProgramName;
                        if (bIns)
                        {
                            context.PersonDisorderInfo.AddObject(PersonDisorderEducation);
                        }
                    }
                    //--------------------------------------
                    Person.RegistrationStage = iRegStage < 5 ? 5 : iRegStage;
                    context.SaveChanges();
                }
                else if (model.Stage == 5)
                {
                    bool bIns = false;
                    var PersonSportQualification = Person.PersonSportQualification;
                    if (PersonSportQualification == null)
                    {
                        PersonSportQualification = new PersonSportQualification();
                        bIns = true;
                        PersonSportQualification.PersonId = PersonId;
                    }

                    int iSportQualificationId = 0;
                    int.TryParse(model.PrivelegeInfo.SportQualificationId, out iSportQualificationId);

                    PersonSportQualification.OtherSportQualification = model.PrivelegeInfo.OtherSportQualification;
                    PersonSportQualification.SportQualificationId = iSportQualificationId;
                    PersonSportQualification.SportQualificationLevel = model.PrivelegeInfo.SportQualificationLevel;

                    if (bIns)
                        context.PersonSportQualification.AddObject(PersonSportQualification);

                    if (iRegStage < 6)
                        Person.RegistrationStage = 6;
                    
                    context.SaveChanges();
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
                    PersonAddInfo.HostelEduc = model.AddInfo.HostelEduc;
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

                bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

                SimplePerson model = new SimplePerson();
                model.Applications = new List<SimpleApplicationPackage>();
                model.Files = new List<AppendedFile>();

                string query = "SELECT Surname, Name, SecondName, RegistrationStage FROM PERSON WHERE Id=@Id";
                SortedList<string, object> dic = new SortedList<string, object>() { { "@Id", PersonID } };
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
                if (tbl.Rows.Count != 1)
                    return RedirectToAction("Index");

                model.Name = Server.HtmlEncode(PersonInfo.Name);
                model.Surname = Server.HtmlEncode(PersonInfo.Surname);
                model.SecondName = Server.HtmlEncode(PersonInfo.SecondName);

                var Applications = context.Abiturient.Where(x => x.PersonId == PersonID && x.Enabled == true && x.IsCommited == true)
                    .Select(x => new { x.CommitId, x.StudyLevelGroupNameRus, x.StudyLevelGroupNameEng, x.EntryType, x.SecondTypeId }).Distinct();
                foreach (var app in Applications)
                {
                    model.Applications.Add(new SimpleApplicationPackage()
                    {
                        Id = app.CommitId,
                        StudyLevel = bIsEng ? app.StudyLevelGroupNameEng : app.StudyLevelGroupNameRus +
                                    (app.SecondTypeId.HasValue ?
                                        ((app.SecondTypeId == 3) ? (bIsEng ? " (recovery)" : " (восстановление)") : 
                                        ((app.SecondTypeId == 2) ? (bIsEng ? " (transfer)" : " (перевод)") : 
                                        ((app.SecondTypeId == 4) ? (bIsEng ? " (changing form of education)" : " (смена формы обучения)") :
                                        ((app.SecondTypeId == 5) ? (bIsEng ? " (changing basis of education)" : " (смена основы обучения)") :
                                        ((app.SecondTypeId == 6) ? (bIsEng ? " (changing educational program)" : " (смена образовательной программы)") : 
                                        ""))))) : "")
                    });
                }

                Applications = context.AG_Application.Where(x => x.PersonId == PersonID && x.Enabled == true && x.IsCommited == true && x.CommitId.HasValue)
                    .Select(x => new { CommitId = x.CommitId.Value, StudyLevelGroupNameRus = "", StudyLevelGroupNameEng = "", EntryType = 0, SecondTypeId = (int?)0 }).Distinct();

                foreach (var app in Applications)
                {
                    model.Applications.Add(new SimpleApplicationPackage()
                    {
                        Id = app.CommitId,
                        StudyLevel = "АГ"
                    });
                }

                string temp_str;
                if (bIsEng)
                    temp_str = "To submit an application select the link <a href=\"" + Util.ServerAddress + "/AbiturientNew/NewApplication\">\"New application\"</a>";
                else
                    temp_str = "Для подачи заявления нажмите кнопку <a href=\"" + Util.ServerAddress + "/AbiturientNew/NewApplication\">\"Подать новое заявление\"</a>";
                model.Messages = Util.GetNewPersonalMessages(PersonID);
                if (model.Applications.Count == 0)
                {
                    model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, Text = temp_str });
                }

                //model.Messages.Add(new PersonalMessage() { Id = "0", Type = MessageType.TipMessage, Text = "Уважаемые абитуриенты! Не забудьте проверить свой выбор \"Нуждаюсь в общежитии на время обучения\" на <a href=\"https://cabinet.spbu.ru/AbiturientNew?step=6\">последней странице анкеты</a>" });

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
                model.Applications = new List<AG_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");
                model.Enabled = true;
                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });
                  
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 1)
                {
                    model.Enabled = false;
                    model.HasError = true;
                    model.ErrorMessage = "Невозможно подать заявление в Академическую Гимназию (не соответствует уровень образования)";
                }
                else
                {
                    DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        model.ErrorMessage = "Невозможно подать заявление в Академическую Гимназию (не соответствует уровень образования)";
                    }
                    else
                    {
                        int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                        if (iAG_EntryClassValue > 9)//В АГ могут поступать только 7-8-9 классники
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            model.ErrorMessage = "Невозможно подать заявление в Академическую Гимназию (не соответствует уровень образования)";
                        }
                        else
                        {
                            model.Enabled = true;
                            string query = "SELECT DISTINCT ProgramId, ProgramName, EntryClassName FROM AG_qEntry WHERE EntryClassId=@ClassId";
                            SortedList<string, object> dic = new SortedList<string, object>();
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

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_Mag(params string[] errors)
        {
            if (errors != null && errors.Length > 0)
            {
                foreach (string er in errors)
                    ModelState.AddModelError("", er);
            }
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");
 
                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил школу 
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление в магистратуру (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                }
                else
                {
                    // остается 4 - закончил вуз (где проверка на то, что действительно закончил?) 
                    model.MaxBlocks = maxBlockMag;

                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        {
                            model.Enabled = false; 
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в магистратуру (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в магистратуру (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                    }

                }
                
                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                    
                return View("NewApplication_Mag", model);
            }
        }
 
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_1kurs(params string[] errors)
         {
             if (errors != null && errors.Length > 0)
             {
                 foreach (string er in errors)
                     ModelState.AddModelError("", er);
             }
             Guid PersonId;
             if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                 return RedirectToAction("LogOn", "Account");
             bool bIsEng = Util.GetCurrentThreadLanguageIsEng();
             using (OnlinePriemEntities context = new OnlinePriemEntities())
             {
                 var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                 if (PersonInfo == null)//а что это могло бы значить???
                     return RedirectToAction("Index");

                 int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                 if (c != 100)
                     return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                 Mag_ApplicationModel model = new Mag_ApplicationModel();
                 model.Applications = new List<Mag_ApplicationSipleEntity>();
                 model.CommitId = Guid.NewGuid().ToString("N");

                 DataTable tbl;
                 model.Enabled = true;
                 int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                 if (iAG_SchoolTypeId == 1)
                 {
                     // ссылка на объект  и пр., когда SchoolExitClassId = null
                     tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
                        INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                     if (tbl.Rows.Count == 0)
                     {
                         model.Enabled = false;
                         model.HasError = true;
                         if (!bIsEng)
                             model.ErrorMessage = "Невозможно подать заявление на первый курс (не соответствует уровень образования)";
                         else
                             model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                     }
                     else
                     {
                         int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                         int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                         if (iAG_EntryClassValue < 11)
                         {
                             model.Enabled = false;
                             model.HasError = true;
                             if (!bIsEng)
                                 model.ErrorMessage = "Невозможно подать заявление на первый курс (не соответствует уровень образования)";
                             else
                                 model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                         }
                         else
                         {
                             model.Enabled = true;
                         }
                     }
                 }
                 else 
                 {
                     int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                     if (VuzAddType.HasValue)
                     {
                         if ((int)VuzAddType != 1)
                         {
                             model.Enabled = false;
                             model.HasError = true;
                             if (!bIsEng)
                                 model.ErrorMessage = "Невозможно подать заявление на первый курс (смените тип поступления в Анкете)";
                             else
                                 model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                         }
                     }
                     else
                     {
                         model.Enabled = false;
                         model.HasError = true;
                         if (!bIsEng)
                             model.ErrorMessage = "Невозможно подать заявление на первый курс (смените тип поступления в Анкете)";
                         else
                             model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                     }
                 }
                 model.MaxBlocks = maxBlock1kurs; 
                 model.StudyFormList = Util.GetStudyFormList();
                 model.StudyBasisList = Util.GetStudyBasisList();
                 return View("NewApplication_1kurs", model);
             }
         }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_SPO(params string[] errors)
        {
            if (errors != null && errors.Length > 0)
            {
                foreach (string er in errors)
                    ModelState.AddModelError("", er);
            }
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                DataTable tbl;
                model.Enabled = true;
                int iSchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                  new SortedList<string, object>() { { "@Id", PersonId } });
                if (iSchoolTypeId == 1)
                {
                    // ссылка на объект и пр., когда SchoolExitClassId = null
                    tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
                        INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Подача заявления в СПО доступна только для людей, уже закончивших 9 классов школы";
                        else
                            model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    }
                    else
                    {
                        int iEntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iEntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                        if (iEntryClassValue < 9)
                        {
                            model.HasError = true;
                            model.Enabled = false;
                            if (!bIsEng)
                                model.ErrorMessage = "Подача заявления в СПО доступна только для людей, уже закончивших 9 классов школы";
                            else
                                model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                        }
                    }
                }
                else 
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                    }
                }
                model.MaxBlocks = maxBlockSPO;
                string query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry WHERE StudyLevelGroupId = 3 ORDER BY 1";
                tbl = Util.AbitDB.GetDataTable(query, null);
                model.StudyFormList =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("StudyFormId"),
                         Text = rw.Field<string>("StudyFormName")
                     }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();

                query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry WHERE StudyLevelGroupId = 3 ORDER BY 1";
                tbl = Util.AbitDB.GetDataTable(query, null);
                model.StudyBasisList =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Value = rw.Field<int>("StudyBasisId"),
                         Text = rw.Field<string>("StudyBasisName")
                     }).AsEnumerable()
                     .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                     .ToList();
                return View("NewApplication_SPO", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_Aspirant(params string[] errors)
        {
            if (errors != null && errors.Length > 0)
            {
                foreach (string er in errors)
                    ModelState.AddModelError("", er);
            }
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");
            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление в аспирантуру (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                }
                else
                {
                    int? iQualificationId = (int?)Util.AbitDB.GetValue("SELECT QualificationId FROM PersonHighEducationInfo WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (iQualificationId.HasValue)
                    {
                        if ((int)iQualificationId != 1)
                        {
                            model.MaxBlocks = maxBlockAspirant;
                            int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (VuzAddType.HasValue)
                            {
                                if ((int)VuzAddType != 1)
                                {
                                    model.Enabled = false;
                                    model.HasError = true;
                                    if (!bIsEng)
                                        model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                                    else
                                        model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                                }
                            }
                            else
                            {
                                model.Enabled = false;
                                model.HasError = true;
                                if (!bIsEng)
                                    model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                                else
                                    model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            }
                        }
                        else
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в аспирантуру (не соответствует уровень образования)";
                            else
                                model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в аспирантуру (не соответствует уровень образования)";
                        else
                            model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    }
                }

                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                  
                return View("NewApplication_Aspirant", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_Recover(params string[] errors)
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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType == 3)
                            model.MaxBlocks = 1;
                        else
                            model.Enabled = false;
                    }
                    else
                        model.Enabled = false;
                }
                
                model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                model.SemestrList = Util.GetSemestrList();

                return View("NewApplication_Recover", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_Transfer(params string[] errors)
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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType == 4)
                            model.MaxBlocks = 1;
                        else
                            model.Enabled = false;
                    }
                    else
                        model.Enabled = false;
                }

                model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                model.SemestrList = Util.GetSemestrList();

                return View("NewApplication_Transfer", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_ChangeStudyForm(params string[] errors)
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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType == 2)
                            model.MaxBlocks = 1;
                        else
                            model.Enabled = false;
                    }
                    else
                        model.Enabled = false;
                }

                model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                model.SemestrList = Util.GetSemestrList();

                return View("NewApplication_ChangeStudyForm", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_ChangeObrazProgram(params string[] errors)
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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                model.CommitId = Guid.NewGuid().ToString("N");

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType == 2)
                            model.MaxBlocks = 1;
                        else
                            model.Enabled = false;
                    }
                    else
                        model.Enabled = false;
                }

                model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                model.StudyFormList = Util.GetStudyFormList();
                model.StudyBasisList = Util.GetStudyBasisList();
                model.SemestrList = Util.GetSemestrList();
                model.SemestrId = (int?)Util.AbitDB.GetValue("SELECT S.NextSemesterId FROM PersonCurrentEducation P INNER JOIN Semester S ON S.Id = P.SemesterId WHERE PersonId=@PersonId", new SortedList<string, object>() { { "@PersonId", PersonId } }) ?? 3;
                for (int i = 0; i < model.SemestrList.Count ; i++)
                {
                    if (model.SemestrList[i].Value == model.SemestrId.ToString())
                        model.SemestrList[i].Selected = true;
                }
                return View("NewApplication_ChangeObrazProgram", model);
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult NewApplication_ChangeStudyBasis(params string[] errors)
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

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                Mag_ApplicationModel model = new Mag_ApplicationModel();
                model.Applications = new List<Mag_ApplicationSipleEntity>();
                Guid gComm = Guid.NewGuid();
                model.CommitId = gComm.ToString("N");

                bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

                model.Enabled = true;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType == 2)
                            model.MaxBlocks = 1;
                        else
                            model.Enabled = false;
                    }
                    else
                        model.Enabled = false;

                    if (context.Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.SecondTypeId == 5).Count() > 0)
                    {
                        return RedirectToAction("Main", "AbiturientNew");
                    }
                }
                c = (int?)Util.AbitDB.GetValue("SELECT LicenseProgramId FROM PersonCurrentEducation WHERE PersonId=@PersonId ", new SortedList<string, object>() { { "@PersonId", PersonId } });
                if (!c.HasValue)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });
                c = (int?)Util.AbitDB.GetValue("SELECT ObrazProgramId FROM PersonCurrentEducation WHERE PersonId=@PersonId ", new SortedList<string, object>() { { "@PersonId", PersonId } });
                if (!c.HasValue)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });

                var EntryList =
                    (from Ent in context.Entry
                     join SPStudyLevel in context.SP_StudyLevel on Ent.StudyLevelId equals SPStudyLevel.Id
                     join PersonCurrentEduc in context.PersonCurrentEducation on PersonId equals PersonCurrentEduc.PersonId
                     join Semester in context.Semester on Ent.SemesterId equals Semester.Id
                     where  PersonCurrentEduc.LicenseProgramId == Ent.LicenseProgramId &&
                            Ent.ObrazProgramId == PersonCurrentEduc.ObrazProgramId &&
                            Ent.StudyFormId == PersonCurrentEduc.StudyFormId &&
                            Ent.StudyBasisId == PersonCurrentEduc.StudyBasisId &&
                            Ent.CampaignYear == Util.iPriemYear &&
                            Ent.StudyLevelId == PersonCurrentEduc.StudyLevelId &&
                            Ent.IsParallel == false &&
                            Ent.IsReduced == false &&
                            Ent.IsSecond == false && 
                            Ent.SemesterId == PersonCurrentEduc.SemesterId 
                     select new
                     {
                         EntryId = Ent.Id,
                         EntryTypeId = Ent.StudyLevelGroupId 
                     }).FirstOrDefault();

                if (EntryList == null)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });

                Guid appId = Guid.NewGuid();
                context.Application.AddObject(new Application()
                {
                    Id = appId,
                    PersonId = PersonId,
                    EntryId = EntryList.EntryId,
                    HostelEduc = false,
                    Priority = 1,
                    Enabled = true,
                    EntryType = EntryList.EntryTypeId,
                    DateOfStart = DateTime.Now,
                    CommitId = gComm,
                    IsGosLine = false,
                    IsCommited = true,
                    SecondTypeId = 5
                });
                context.SaveChanges(); 
                return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", gComm } });
            }
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ChangeApplication(string Id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");
                Guid gComm = Guid.Parse(Id);
                bool isPrinted = (bool)Util.AbitDB.GetValue("SELECT IsPrinted FROM ApplicationCommit WHERE Id=@Id ", new SortedList<string, object>() { { "@Id", gComm } });
                if (isPrinted) return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", Guid.Parse(Id) } });
                
                int? c = (int?)Util.AbitDB.GetValue("SELECT top 1 SecondTypeId FROM Application WHERE CommitId=@Id AND PersonId=@PersonId", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", Guid.Parse(Id) } });
                if (c != null)
                {
                    // перевод
                    if ((c == 2) || (c == 4) || (c == 5) || (c == 6))
                    {
                        Mag_ApplicationModel model = new Mag_ApplicationModel();
                        model.Applications = new List<Mag_ApplicationSipleEntity>();
                        model.CommitId = Id;
                        model.Enabled = true;
                        int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                           new SortedList<string, object>() { { "@Id", PersonId } });
                        if (iAG_SchoolTypeId != 4)
                        {
                            model.Enabled = false;
                        }
                        else
                        {
                            int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (VuzAddType.HasValue)
                            {
                                if ((int)VuzAddType == 2)
                                    model.MaxBlocks = 1;
                                else
                                    model.Enabled = false;
                            }
                            else
                                model.Enabled = false;
                        }
                        model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                        model.StudyFormList = Util.GetStudyFormList();
                        model.StudyBasisList = Util.GetStudyBasisList();
                        model.SemestrList = Util.GetSemestrList();
                        Guid CommitId = Guid.Parse(Id);
                        var CommId = context.Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == CommitId && x.SecondTypeId==c).Select(x => x.CommitId);
                        if (CommId.Count() > 0)
                        {
                            model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                        }
                        if (c == 2)
                            return View("NewApplication_Transfer", model);
                        else if (c==4)
                            return View("NewApplication_ChangeStudyForm", model);
                        else if (c == 5)
                            return View("NewApplication_ChangeStudyBasis", model);
                        else if (c == 6)
                            return View("NewApplication_ChangeObrazProgram", model);
                    }
                        // восстановление
                    else if (c == 3) {
                        Mag_ApplicationModel model = new Mag_ApplicationModel();
                        model.Applications = new List<Mag_ApplicationSipleEntity>();
                        model.CommitId = Id;
                        model.Enabled = true;
                        int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                           new SortedList<string, object>() { { "@Id", PersonId } });
                        if (iAG_SchoolTypeId != 4)
                        {
                            // окончил не вуз
                            model.Enabled = false;
                        }
                        else
                        {
                            int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (VuzAddType.HasValue)
                            {
                                if ((int)VuzAddType == 3)
                                    model.MaxBlocks = 1;
                                else
                                    model.Enabled = false;
                            }
                            else
                                model.Enabled = false;
                        }
                        model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                        model.StudyFormList = Util.GetStudyFormList();
                        model.StudyBasisList = Util.GetStudyBasisList();
                        model.SemestrList = Util.GetSemestrList();
                        Guid CommitId = Guid.Parse(Id);
                        var CommId = context.Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == CommitId && x.SecondTypeId == c).Select(x => x.CommitId);
                        if (CommId.Count() > 0)
                        {
                            model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                        }
                        return View("NewApplication_Recover", model);
                    }  
                }

                c = (int?)Util.AbitDB.GetValue("SELECT top 1 EntryType FROM Application WHERE CommitId=@Id AND PersonId=@PersonId", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", Guid.Parse(Id) } });
                if (c != null)
                {
                    Mag_ApplicationModel model = new Mag_ApplicationModel();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.CommitId = Id;

                    DataTable tbl;
                    model.Enabled = true;
                    int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                        new SortedList<string, object>() { { "@Id", PersonId } });
                    if (c == 2)
                    {
                        if (iAG_SchoolTypeId != 4)
                        { 
                            model.Enabled = false;
                        } 
                    }
                    else if (c == 1)
                    {
                        if (iAG_SchoolTypeId == 1)
                        { 
                            tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
                             INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (tbl.Rows.Count == 0)
                            {
                                model.Enabled = false;
                            }
                            else
                            {
                                int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                                int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                                if (iAG_EntryClassValue < 11)
                                {
                                    model.Enabled = false;
                                } 
                            }
                        }
                    }
                    else if (c == 3)
                    {
                        if (iAG_SchoolTypeId == 4)
                        {
                            int? iQualificationId = (int?)Util.AbitDB.GetValue("SELECT QualificationId FROM PersonHighEducationInfo WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (iQualificationId.HasValue)
                            {
                                if ((int)iQualificationId == 1)
                                    model.Enabled = false; 
                            }
                            else
                                model.Enabled = false; 
                        }
                    }


                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    Guid CommitId = Guid.Parse(Id);

                    var CommId = context.Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == CommitId).Select(x => x.CommitId);
                    if (CommId.Count() > 0)
                    { 
                        model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    }

                    if (c == 2)
                    {
                        model.MaxBlocks = maxBlockMag;
                        return View("NewApplication_Mag", model); 
                    }
                    else if (c == 1)
                    {
                        model.MaxBlocks = maxBlock1kurs;
                        return View("NewApplication_1kurs", model); 
                    }
                    else if (c == 3)
                    {
                        model.MaxBlocks = maxBlockSPO;
                        return View("NewApplication_SPO", model);
                    }
                    else if (c == 4)
                    {
                        model.MaxBlocks = maxBlockAspirant;
                        return View("NewApplication_Aspirant", model);
                    } 

                }
                else
                {
                    c = (int?)Util.AbitDB.GetValue("SELECT count(Id) FROM AG_Application WHERE CommitId=@Id AND PersonId=@PersonId", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", Guid.Parse(Id) } });
                    if (c != 0)
                    {
                        AG_ApplicationModel model = new AG_ApplicationModel();
                        model.Applications = new List<AG_ApplicationSipleEntity>();
                        model.CommitId = Id;
                        c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                        if (c != 100)
                            return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                        /*int iAG_EntryClassId = (int)Util.AbitDB.GetValue("SELECT SchoolExitClassId FROM PersonSchoolInfo WHERE PersonId=@Id",
                            new SortedList<string, object>() { { "@Id", PersonId } });*/

                        int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                           new SortedList<string, object>() { { "@Id", PersonId } });
                        if (iAG_SchoolTypeId == 4)
                        {
                            model.Enabled = false;
                        }
                        else
                        {
                            // ссылка на объект  и пр., когда SchoolExitClassId = null
                            DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
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
                                    var CommId = context.AG_Application.Where(x => x.PersonId == PersonId && x.IsCommited == true).Select(x => x.CommitId);
                                    if (CommId.Count() > 0 )
                                    {
                                        Guid CommitId = CommId.First().Value;
                                        model.CommitId = CommitId.ToString("N");

                                        var AppList = context.AG_Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == CommitId).OrderBy(x => x.Priority)
                                            .Select(x => new
                                            {
                                                x.Id,
                                                x.AG_Entry.ProgramId,
                                                ProgramName = x.AG_Entry.AG_Program.Name,
                                                x.AG_Entry.ProfileId,
                                                ProfileName = x.AG_Entry.AG_Profile.Name,
                                                x.ManualExamId,
                                                x.AG_Entry.HasManualExams,
                                                x.AG_Entry.EntryClassId
                                            });
                                        foreach (var App in AppList)
                                        {
                                            var Ent = new AG_ApplicationSipleEntity()
                                            {
                                                Id = App.Id,
                                                ProgramId = App.ProgramId,
                                                ProgramName = App.ProgramName,
                                                ProfileId = App.ProfileId,
                                                ProfileName = App.ProfileName,
                                            };

                                            var ProgramList = context.AG_Entry.Where(x => x.EntryClassId == App.EntryClassId)
                                                .Select(x => new { x.AG_Program.Id, x.AG_Program.Name }).Distinct().ToList()
                                                .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id == App.ProgramId }).ToList();
                                            Ent.ProgramList = ProgramList;

                                            var ProfileList = context.AG_Entry.Where(x => x.EntryClassId == App.EntryClassId && x.ProgramId == App.ProgramId)
                                                .Select(x => new { x.AG_Profile.Id, x.AG_Profile.Name }).Distinct().ToList()
                                                .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id == App.ProfileId }).ToList();
                                            if (ProfileList.Count > 1)
                                                Ent.ProfileList = ProfileList;

                                            if (App.HasManualExams)
                                            {
                                                var ManualExamsList = context.AG_Entry.Where(x => x.EntryClassId == App.EntryClassId && x.ProgramId == App.ProgramId && x.ProfileId == App.ProfileId)
                                                    .Select(x => new { x.AG_Profile.Id, x.AG_Profile.Name }).Distinct().ToList()
                                                    .Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name, Selected = x.Id == App.ProfileId }).ToList();

                                                if (App.ManualExamId.HasValue)
                                                {
                                                    Ent.ManualExamId = App.ManualExamId.Value;
                                                    Ent.ManualExamList = ManualExamsList;
                                                }
                                            }

                                            model.Applications.Add(Ent);
                                        }
                                    }
                                    model.Enabled = true;
                                    string query = "SELECT DISTINCT ProgramId, ProgramName, EntryClassName FROM AG_qEntry WHERE EntryClassId=@ClassId";
                                    SortedList<string, object> dic = new SortedList<string, object>();
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
                return RedirectToAction("Index");
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
            bool bisEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                int? c = (int?)Util.AbitDB.GetValue("SELECT RegistrationStage FROM Person WHERE Id=@Id AND RegistrationStage=100", new SortedList<string, object>() { { "@Id", PersonId } });
                if (c != 100)
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", (c ?? 6).ToString() } });

                ApplicationModel model = new ApplicationModel();
                model.IsForeign = Util.GetRess(PersonId) == 4;
                int? iScTypeId = (int?)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                if (iScTypeId.HasValue)
                {
                    string ScTypeName = Util.AbitDB.GetStringValue("SELECT Name FROM SchoolTypeAll WHERE Id=@Id", new SortedList<string, object>() { { "@Id", iScTypeId ?? 1 } });
                    string ScTypeNameEng = Util.AbitDB.GetStringValue("SELECT NameEng FROM SchoolTypeAll WHERE Id=@Id", new SortedList<string, object>() { { "@Id", iScTypeId ?? 1 } });
                     
                    model.SchoolType = bisEng? (String.IsNullOrEmpty(ScTypeNameEng)?ScTypeName:ScTypeNameEng): ScTypeName;
                    if (iScTypeId.Value != 4)
                    {
                        model.EntryType = 1;//1 курс бак-спец, АГ, СПО
                        if (iScTypeId.Value != 1)
                        {
                            model.ExitClassId = 11;
                        }
                        else
                        {
                            int? iScExClassId = (int?)Util.AbitDB.GetValue("SELECT SchoolExitClass.IntValue FROM PersonEducationDocument INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                            if (iScExClassId.HasValue)
                            {
                                model.ExitClassId = (int)iScExClassId;
                            }
                        }
                    }
                    else
                    {
                        model.EntryType = 2;
                        int? iScExClassId = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                        if (iScExClassId.HasValue)
                        {
                            model.VuzAddType = (int)iScExClassId;
                        }
                        else
                            return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });
                        int? iQualificationId = (int?)Util.AbitDB.GetValue("SELECT QualificationId FROM PersonHighEducationInfo WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                        if (iQualificationId.HasValue)
                        {
                            model.ExitClassId = (int)iQualificationId;
                        }
                        else
                            return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });
                    }
                }
                else
                    return RedirectToAction("Index", new RouteValueDictionary() { { "step", "4" } });
                model.StudyForms = Util.GetStudyFormList(); 
                model.StudyBasises = Util.GetStudyBasisList();
                bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

                //выборка активных заявлений
                model.Applications = new List<SimpleApplicationPackage>();
                var Applications = context.Abiturient.Where(x => x.PersonId == PersonId && x.Enabled == true && x.IsCommited == true)
                    .Select(x => new { x.CommitId, x.StudyLevelGroupNameEng, x.StudyLevelGroupNameRus, x.EntryType, x.SecondTypeId }).Distinct();
                foreach (var app in Applications)
                {
                    model.Applications.Add(new SimpleApplicationPackage()
                    {
                        Id = app.CommitId,
                        PriemType = app.EntryType.ToString(),
                        StudyLevel = bIsEng ? app.StudyLevelGroupNameEng : app.StudyLevelGroupNameRus +
                                     (app.SecondTypeId.HasValue ?
                                        ((app.SecondTypeId == 3) ? (bIsEng ? " (recovery)" : " (восстановление)") :
                                        ((app.SecondTypeId == 2) ? (bIsEng ? " (transfer)" : " (перевод)") :
                                        ((app.SecondTypeId == 4) ? (bIsEng ? " (changing form of education)" : " (смена формы обучения)") :
                                        ((app.SecondTypeId == 5) ? (bIsEng ? " (changing basis of education)" : " (смена основы обучения)") :
                                        ((app.SecondTypeId == 6) ? (bIsEng ? " (changing educational program)" : " (смена образовательной программы)") :
                                        ""))))) : "")
                    });
                }

                Applications = context.AG_Application.Where(x => x.PersonId == PersonId && x.Enabled == true && x.IsCommited == true && x.CommitId.HasValue)
                    .Select(x => new { CommitId = x.CommitId.Value, StudyLevelGroupNameEng = "", StudyLevelGroupNameRus = "", EntryType = 0, SecondTypeId=(int?)0 }).Distinct();

                foreach (var app in Applications)
                {
                    model.Applications.Add(new SimpleApplicationPackage()
                    {
                        Id = app.CommitId,
                        PriemType = "",
                        StudyLevel = "АГ"
                    });
                }
                if (model.VuzAddType == 2)
                {
                    int? qw = (int?)Util.AbitDB.GetValue("SELECT LicenseProgramId FROM PersonCurrentEducation WHERE PersonId=@PersonId ", new SortedList<string, object>() { { "@PersonId", PersonId } });
                    if (qw.HasValue)
                    {
                        model.LicenseProgramName = Util.AbitDB.GetStringValue("select top 1 LicenseProgramName from Entry where LicenseProgramId=@Id", new SortedList<string, object>() { { "@Id", qw } });
                    }
                    qw = (int?)Util.AbitDB.GetValue("SELECT ObrazProgramId FROM PersonCurrentEducation WHERE PersonId=@PersonId ", new SortedList<string, object>() { { "@PersonId", PersonId } });
                    if (qw.HasValue)
                    {
                        model.ObrazProgramName = Util.AbitDB.GetStringValue("select top 1 ObrazProgramName from Entry  where ObrazProgramId=@Id", new SortedList<string, object>() { { "@Id", qw } });
                    }
                    
                    var EntryList =
                        (from Ent in context.Entry
                         join SPStudyLevel in context.SP_StudyLevel on Ent.StudyLevelId equals SPStudyLevel.Id
                         join PersonCurrentEduc in context.PersonCurrentEducation on PersonId equals PersonCurrentEduc.PersonId
                         join Semester in context.Semester on Ent.SemesterId equals Semester.Id
                         where PersonCurrentEduc.LicenseProgramId == Ent.LicenseProgramId &&
                                Ent.ObrazProgramId == PersonCurrentEduc.ObrazProgramId &&
                                Ent.StudyFormId == PersonCurrentEduc.StudyFormId &&
                                Ent.StudyBasisId == PersonCurrentEduc.StudyBasisId &&
                                Ent.CampaignYear == Util.iPriemYear &&
                                Ent.StudyLevelId == PersonCurrentEduc.StudyLevelId &&
                                Ent.IsParallel == false &&
                                Ent.IsReduced == false &&
                                Ent.IsSecond == false &&
                                Ent.SemesterId == PersonCurrentEduc.SemesterId
                         select new
                         {
                             EntryId = Ent.Id 
                         }).FirstOrDefault();

                    if (EntryList == null)
                    {
                        model.Message = "Данные некорректны (не найден учебный план). Вернитесь в анкету и проверьте правильность данных";
                    } 
                }
                return View("NewApplication", model);
            }
        }
     
        [HttpPost]
        public ActionResult NewApplicationSelect()
        { 
            string val = Request.Form["val_h"]; 
            switch (val)
            {
                case "1": { return RedirectToAction("NewApplication_1kurs", "AbiturientNew"); } //Поступление на 1 курс гражданам РФ
                case "2": { return RedirectToAction("NewApplication_Mag", "AbiturientNew"); } //Поступление в магистратуру
                case "3": { return RedirectToAction("NewApplication_ChangeStudyBasis", "AbiturientNew"); } //Перевод ОСНОВА
                case "4": { return RedirectToAction("NewApplication_Transfer", "AbiturientNew"); } //Перевод  в СПбГУ
                case "5": { return RedirectToAction("NewApplication_Recover", "AbiturientNew"); } //Восстановление в СПбГУ
                case "6": { return RedirectToAction("NewApplication_ChangeStudyForm", "AbiturientNew"); } //Перевод ФОРМА
                case "7": { return RedirectToAction("NewApplication_ChangeObrazProgram", "AbiturientNew"); } //Смена образовательной программы
                case "8": { return RedirectToAction("NewApplication_AG", "AbiturientNew"); } //Поступление в Академическую Гимназию
                case "9": { return RedirectToAction("NewApplication_SPO", "AbiturientNew"); } //Поступление в СПО
                case "10": { return RedirectToAction("NewApplication_Aspirant", "AbiturientNew"); } //Поступление в аспирантуру гражданам РФ
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

            DateTime timeX = new DateTime(2014, 6, 20, 10, 0, 0);//20-06-2013 10:00:00
            if (iEntry != 2 && DateTime.Now < timeX )
            {
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Подача заявлений на 1 курс начнётся 20 июня в 10:00 МСК" } });
            }

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
                { "@EntryType", iEntry },
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

        [HttpPost]
        public ActionResult NewApp_AG()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            {
                if (!bIsEng)
                    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });
                else
                    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Entry is closed" } });
            }

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID); 

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int type = 0;
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                   new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 1)
                    type = 1;                
                else
                {
                    DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                        type = 1;  
                    else
                    {
                        int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                        if (iAG_EntryClassValue > 9)//В АГ могут поступать только 7-8-9 классники
                            type = 1; 
                    }
                }
                if (context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true).Count() > 0)
                    type = 2;
                if (context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId).Count() == 0)
                    type = 3;

                if (type == 1)
                {
                    if (!bIsEng)
                        return RedirectToAction("NewApplication_AG",
                            new RouteValueDictionary() { { "errors", "Невозможно подать заявление в Академическую Гимназию (не соответствует уровень образования)" } });
                    else
                        return RedirectToAction("NewApplication_AG",
                       new RouteValueDictionary() { { "errors", "Change your previous education degree in Questionnaire Data." } });
                } 
                else if (type == 2)
                {
                    if (!bIsEng)
                        return RedirectToAction("NewApplication_AG",
                            new RouteValueDictionary() { { "errors", "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные." } });
                    else
                        return RedirectToAction("NewApplication_AG",
                            new RouteValueDictionary() { { "errors", "To submit new application you should cancel your active application." } });
                }
                else if (type == 3)
                {
                    if (!bIsEng)
                        return RedirectToAction("NewApplication_AG",
                           new RouteValueDictionary() { { "errors", "Невозможно подать пустое заявление." } });
                    else
                        return RedirectToAction("NewApplication_AG",
                           new RouteValueDictionary() { { "errors", "You can not submit empty application." } });
                }
                else
                {
                    var Ids = context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId).Select(x => x.Id).ToList();
                    foreach (var AppId in Ids)
                    {
                        var App = context.AG_Application.Where(x => x.Id == AppId).FirstOrDefault();
                        App.IsCommited = true;
                    }
                    context.SaveChanges();

                    //всё, что вне коммита - удаляем
                    Ids = context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == false).Select(x => x.Id).ToList();
                    foreach (var AppId in Ids)
                    {
                        var App = context.AG_Application.Where(x => x.Id == AppId).FirstOrDefault();
                        context.AG_Application.DeleteObject(App);
                    }
                    context.SaveChanges();
                }
            }

            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_Mag(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление в магистратуру (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_Mag", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        { 
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в магистратуру (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                            return View("NewApplication_Mag", model); 
                        }
                    }
                    else
                    {   model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в магистратуру (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                        return View("NewApplication_Mag", model);
                    } 
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.C_Entry.StudyLevelId == 17).Count() > 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockMag;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";  
                    return View("NewApplication_Mag", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count()==0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockMag;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";   
                    return View("NewApplication_Mag", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            } 
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_Asp(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true; 
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление в аспирантуру (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_Aspirant", model);
                }
                else
                {
                    int? iQualificationId = (int?)Util.AbitDB.GetValue("SELECT QualificationId FROM PersonHighEducationInfo WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (iQualificationId.HasValue)
                    {
                        if ((int)iQualificationId == 1)
                        {
                            model.Enabled = false;
                            model.HasError = true; 
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в аспирантуру (не соответствует уровень образования)";
                            else
                                model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                            return View("NewApplication_Aspirant", model);
                        } 
                    }
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        {
                            model.Enabled = false;
                            model.HasError = true; 
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в аспирантуру (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                            return View("NewApplication_Aspirant", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true; 
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в аспирантуру (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                        return View("NewApplication_Aspirant", model);
                    } 
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.C_Entry.StudyLevelId == 15).Count() > 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockAspirant;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";  
                    return View("NewApplication_Aspirant", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockAspirant;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application."; 
                    return View("NewApplication_Aspirant", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_1kurs(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId == 1)
                {
                    DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
                        INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                    {
                        model.Enabled = false;
                        model.HasError = true; 
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на первый курс (не соответствует уровень образования)";
                        else
                            model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                        return View("NewApplication_1kurs", model);
                    }
                    else
                    {
                        int iAG_EntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iAG_EntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");
                        if (iAG_EntryClassValue < 11)
                        {
                            model.Enabled = false;
                            model.HasError = true; 
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на первый курс (не соответствует уровень образования)";
                            else
                                model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                            return View("NewApplication_1kurs", model);
                        }
                    }
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        {
                            model.Enabled = false;
                            model.HasError = true; 
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на первый курс (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                            return View("NewApplication_1kurs", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true; 
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на первый курс (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                        return View("NewApplication_1kurs", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && (x.C_Entry.StudyLevelId == 16 || x.C_Entry.StudyLevelId == 18)).Count() > 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlock1kurs;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";  
                    return View("NewApplication_1kurs", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlock1kurs;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application."; 
                    return View("NewApplication_1kurs", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_SPO(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iSchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                 new SortedList<string, object>() { { "@Id", PersonId } });
                if (iSchoolTypeId == 1)
                {
                    // ссылка на объект и пр., когда SchoolExitClassId = null
                    DataTable tbl = Util.AbitDB.GetDataTable(@"SELECT SchoolExitClass.IntValue AS SchoolExitClassValue, PersonEducationDocument.SchoolExitClassId FROM PersonEducationDocument 
                        INNER JOIN SchoolExitClass ON SchoolExitClass.Id = PersonEducationDocument.SchoolExitClassId WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (tbl.Rows.Count == 0)
                    {
                        model.Enabled = false;
                        model.HasError = true; 
                        if (!bIsEng)
                            model.ErrorMessage = "Подача заявления в СПО доступна только для людей, уже закончивших 9 классов школы";
                        else
                            model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                        return View("NewApplication_SPO", model);
                    }
                    else
                    {
                        int iEntryClassId = (int)tbl.Rows[0].Field<int>("SchoolExitClassId");
                        int iEntryClassValue = (int)tbl.Rows[0].Field<int>("SchoolExitClassValue");

                        if (iEntryClassValue < 9)
                        {
                            model.HasError = true;
                            model.Enabled = false;
                            if (!bIsEng)
                                model.ErrorMessage = "Подача заявления в СПО доступна только для людей, уже закончивших 9 классов школы";
                            else
                                model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                            return View("NewApplication_SPO", model);
                        }
                    }
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 1)
                        {
                            model.Enabled = false;
                            model.HasError = true; 
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                            return View("NewApplication_SPO", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true; 
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление в СПО (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data"; 
                        return View("NewApplication_SPO", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && (x.C_Entry.StudyLevelId == 10 || x.C_Entry.StudyLevelId == 8)).Count() > 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockSPO;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";  
                    return View("NewApplication_SPO", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockSPO;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application."; 
                    return View("NewApplication_SPO", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }

        [HttpPost]
        public ActionResult NewApp_Recover(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление на восстановление (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_Recover", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 3)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на восстановление (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            return View("NewApplication_Recover", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на восстановление (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        return View("NewApplication_Recover", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.SecondTypeId == 3).Count() > 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";
                    return View("NewApplication_Recover", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";
                    return View("NewApplication_Recover", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }

        [HttpPost]
        public ActionResult NewApp_Transfer(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление на перевод (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_Transfer", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 4)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            model.Applications = new List<Mag_ApplicationSipleEntity>();
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            return View("NewApplication_Transfer", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        model.Applications = new List<Mag_ApplicationSipleEntity>();
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        return View("NewApplication_Transfer", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.SecondTypeId == 2).Count() > 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";
                    return View("NewApplication_Transfer", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";
                    return View("NewApplication_Transfer", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_ChangeStudyForm(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление на перевод (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_ChangeStudyForm", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 2)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            return View("NewApplication_ChangeStudyForm", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        return View("NewApplication_ChangeStudyForm", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.SecondTypeId == 4).Count() > 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";
                    return View("NewApplication_ChangeStudyForm", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";
                    return View("NewApplication_ChangeStudyForm", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_ChangeObrazProgram(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление на перевод (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_ChangeObrazProgram", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 2)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            return View("NewApplication_ChangeObrazProgram", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        return View("NewApplication_ChangeObrazProgram", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.SecondTypeId == 6).Count() > 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";
                    return View("NewApplication_ChangeObrazProgram", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";
                    return View("NewApplication_ChangeObrazProgram", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }
        [HttpPost]
        public ActionResult NewApp_ChangeStudyBasis(Mag_ApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
            //    return RedirectToAction("NewApplication_AG", new RouteValueDictionary() { { "errors", "Приём документов в АГ СПбГУ ЗАКРЫТ" } });

            string sCommitId = Request.Form["CommitId"];
            Guid CommitId;
            if (!Guid.TryParse(sCommitId, out CommitId))
                return Json(Resources.ServerMessages.IncorrectGUID);

            bool bIsEng = Util.GetCurrentThreadLanguageIsEng();

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                int iAG_SchoolTypeId = (int)Util.AbitDB.GetValue("SELECT SchoolTypeId FROM PersonEducationDocument WHERE PersonId=@Id",
                       new SortedList<string, object>() { { "@Id", PersonId } });
                if (iAG_SchoolTypeId != 4)
                {
                    // окончил не вуз
                    model.Enabled = false;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать заявление на перевод (не соответствует уровень образования)";
                    else
                        model.ErrorMessage = "Change your previous education degree in Questionnaire Data";
                    return View("NewApplication_ChangeStudyBasis", model);
                }
                else
                {
                    int? VuzAddType = (int?)Util.AbitDB.GetValue("SELECT VuzAdditionalTypeId FROM PersonEducationDocument WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                    if (VuzAddType.HasValue)
                    {
                        if ((int)VuzAddType != 2)
                        {
                            model.Enabled = false;
                            model.HasError = true;
                            if (!bIsEng)
                                model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                            else
                                model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                            return View("NewApplication_ChangeStudyBasis", model);
                        }
                    }
                    else
                    {
                        model.Enabled = false;
                        model.HasError = true;
                        if (!bIsEng)
                            model.ErrorMessage = "Невозможно подать заявление на перевод (смените тип поступления в Анкете)";
                        else
                            model.ErrorMessage = "Change your Entry Type in Questionnaire Data";
                        return View("NewApplication_ChangeStudyBasis", model);
                    }
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == true && x.SecondTypeId == 5).Count() > 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.Applications = Util.GetApplicationListInCommit(CommitId, PersonId);
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Уже существует активное заявление. Для создания нового заявления необходимо удалить уже созданные.";
                    else
                        model.ErrorMessage = "To submit new application you should cancel your active application.";
                    return View("NewApplication_ChangeStudyBasis", model);
                }
                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).Count() == 0)
                {
                    model.SemestrList = Util.GetSemestrList();
                    model.StudyFormList = Util.GetStudyFormList();
                    model.StudyBasisList = Util.GetStudyBasisList();
                    model.StudyLevelGroupList = Util.GetStudyLevelGroupList();
                    model.Applications = new List<Mag_ApplicationSipleEntity>();
                    model.MaxBlocks = maxBlockRecover;
                    model.HasError = true;
                    model.Enabled = true;
                    if (!bIsEng)
                        model.ErrorMessage = "Невозможно подать пустое заявление";
                    else
                        model.ErrorMessage = "You can not submit empty application.";
                    return View("NewApplication_ChangeStudyBasis", model);
                }
                Util.CommitApplication(CommitId, PersonId, context);
            }
            return RedirectToAction("PriorityChanger", new RouteValueDictionary() { { "CommitId", CommitId.ToString() } });
        }

        public ActionResult PriorityChanger(string CommitId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid gCommitId;
            if (!Guid.TryParse(CommitId, out gCommitId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            Guid VersionId = Guid.NewGuid();
            bool bisEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                var apps =
                    (from App in context.Application
                     join Entry in context.Entry on App.EntryId equals Entry.Id
                     join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                     where App.PersonId == PersonId && App.CommitId == gCommitId && App.IsCommited == true && App.Enabled == true
                     select new SimpleApplication()
                     {
                         Id = App.Id,
                         Priority = App.Priority,
                         StudyForm = (bisEng ? ((String.IsNullOrEmpty(Entry.StudyFormNameEng)) ? Entry.StudyFormName : Entry.StudyFormNameEng) : Entry.StudyFormName),
                         StudyBasis = (/*bisEng ? ((String.IsNullOrEmpty(Entry.StudyBasisNameEng)) ? Entry.StudyBasisName : Entry.StudyBasisNameEng) :*/ Entry.StudyBasisName),
                         Profession = Entry.LicenseProgramCode + " " + (bisEng ? ((String.IsNullOrEmpty(Entry.LicenseProgramNameEng))?Entry.LicenseProgramName:Entry.LicenseProgramNameEng) : Entry.LicenseProgramName),
                         ObrazProgram = Entry.ObrazProgramCrypt + " " + (bisEng ? ((String.IsNullOrEmpty(Entry.ObrazProgramNameEng)) ? Entry.ObrazProgramName : Entry.ObrazProgramNameEng) : Entry.ObrazProgramName),
                         Specialization = (bisEng ? ((String.IsNullOrEmpty(Entry.ProfileNameEng)) ? Entry.ProfileName : Entry.ProfileNameEng) : Entry.ProfileName),
                         HasManualExams = false,
                         HasSeparateObrazPrograms = context.ObrazProgramInEntry.Where(x => x.EntryId == App.EntryId).Count() > 0,
                         ObrazProgramInEntryId = context.ObrazProgramInEntry.Where(x => x.EntryId == App.EntryId).Count() == 1 ? context.ObrazProgramInEntry.Where(x => x.EntryId == App.EntryId).Select(x => x.Id).FirstOrDefault() : Guid.Empty,
                         EntryId = App.EntryId,
                         IsGosLine = App.IsGosLine,
                         SemesterName = (Entry.SemesterId!=1)?Semester.Name:"",
                         SecondTypeName = "",
                         StudyLevelGroupName = (bisEng ? ((String.IsNullOrEmpty(Entry.StudyLevelGroupNameEng)) ? Entry.StudyLevelGroupNameRus : Entry.StudyLevelGroupNameEng) : Entry.StudyLevelGroupNameRus) +
                                    (App.SecondTypeId.HasValue ?
                                        ((App.SecondTypeId == 3) ? (bisEng ? " (recovery)" : " (восстановление)") :
                                        ((App.SecondTypeId == 2) ? (bisEng ? " (transfer)" : " (перевод)") :
                                        ((App.SecondTypeId == 4) ? (bisEng ? " (changing form of education)" : " (смена формы обучения)") :
                                        ((App.SecondTypeId == 5) ? (bisEng ? " (changing basis of education)" : " (смена основы обучения)") :
                                        ((App.SecondTypeId == 6) ? (bisEng ? " (changing educational program)" : " (смена образовательной программы)") :
                                        ""))))) : "")
                     }).ToList().Union(
                     (from AG_App in context.AG_Application
                      where AG_App.PersonId == PersonId && AG_App.CommitId == gCommitId && AG_App.IsCommited == true && AG_App.Enabled == true
                      select new SimpleApplication()
                      {
                          Id = AG_App.Id,
                          Priority = AG_App.Priority,
                          Profession = AG_App.AG_Entry.AG_Program.Name,
                          ObrazProgram = AG_App.AG_Entry.AG_EntryClass.Name,
                          Specialization = AG_App.AG_Entry.AG_Profile.Name,
                          HasManualExams = AG_App.ManualExamId.HasValue,
                          ManualExam = AG_App.ManualExamId.HasValue ? AG_App.AG_ManualExam.Name : "",
                          StudyForm = Resources.Common.StudyFormFullTime,
                          StudyBasis = Resources.Common.StudyBasisBudget,
                          HasSeparateObrazPrograms = false,
                          IsGosLine = false,
                          SecondTypeName = "",
                          StudyLevelGroupName = Resources.Common.AG
                      }).ToList()).OrderBy(x => x.Priority).ToList();

                MotivateMailModel mdl = new MotivateMailModel()
                {
                    Apps = apps,
                    UILanguage = Util.GetUILang(PersonId),
                    VersionId = VersionId.ToString("N")
                };
                return View(mdl);
            }
        }
        public ActionResult PriorityChangerApplication(string AppId, string V)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid gAppId;
            if (!Guid.TryParse(AppId, out gAppId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            Guid gVersionId;
            if (!Guid.TryParse(V, out gVersionId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            bool isEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                var appl =
                    (from App in context.Application
                     join Entry in context.Entry on App.EntryId equals Entry.Id
                     join OPIE in context.ObrazProgramInEntry on App.EntryId equals OPIE.EntryId

                     where App.PersonId == PersonId && App.IsCommited == true && App.Enabled == true && App.Id == gAppId
                     select new
                     {
                         Id = App.Id,
                         CommitId = App.CommitId,
                         CommitName = isEng ? Entry.StudyLevelGroupNameEng : Entry.StudyLevelGroupNameRus,
                         App.EntryId,
                     }).FirstOrDefault();

                var appPriors = (from AppDetails in context.ApplicationDetails
                                 where AppDetails.ApplicationId == gAppId
                                 select new
                                 {
                                     AppDetails.ObrazProgramInEntryId,
                                     AppDetails.ObrazProgramInEntryPriority,
                                 }).ToList();

                var Ops = 
                    (from OPInEntry in context.ObrazProgramInEntry
                     where OPInEntry.EntryId == appl.EntryId
                     select new StandartObrazProgramInEntryRow()
                     {
                         Id =  OPInEntry.Id,
                         Name = isEng ? OPInEntry.SP_ObrazProgram.NameEng : OPInEntry.SP_ObrazProgram.Name,
                         Priority = OPInEntry.DefaultPriorityValue,
                         DefaultPriority = OPInEntry.DefaultPriorityValue
                     }).ToList();
                     

                int ind = 0;
                foreach (var op in Ops)
                {
                    if (appPriors.Where(x => x.ObrazProgramInEntryId == op.Id).Count() > 0)
                        Ops[ind].Priority = appPriors.Where(x => x.ObrazProgramInEntryId == op.Id).First().ObrazProgramInEntryPriority;
                    ind++;
                }

                var RetVal = Ops.OrderBy(x => x.Priority)
                    .Select(x => new KeyValuePair<Guid, ObrazProgramInEntrySmallEntity>(x.Id, new ObrazProgramInEntrySmallEntity()
                    {
                        Name = x.Name,
                        HasProfileInObrazProgramInEntry = (context.ProfileInObrazProgramInEntry.Where(z => z.ObrazProgramInEntryId == x.Id).Count() > 1)
                    })).Distinct().ToList();

                if (Ops.Count == 1)
                    return RedirectToAction("PriorityChangerProfile", new RouteValueDictionary() { { "AppId", AppId }, { "OPIE", RetVal.First().Key.ToString("N") } });

                else //Ops.Count > 1
                {
                    PriorityChangerApplicationModel mdl = new PriorityChangerApplicationModel()
                    {
                        ApplicationId = gAppId,
                        CommitId = appl.CommitId,
                        CommitName = appl.CommitName,
                        lstObrazPrograms = RetVal,
                        ApplicationVersionId = gVersionId
                    };
                    return View(mdl);
                }
            }
        }
        public ActionResult PriorityChangerProfile(string AppId, string OPIE, string V)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid gAppId;
            if (!Guid.TryParse(AppId, out gAppId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            Guid gObrazProgramInEntryId;
            if (!Guid.TryParse(OPIE, out gObrazProgramInEntryId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            Guid gVersionId;
            if (!Guid.TryParse(V, out gVersionId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            bool isEng = Util.GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return RedirectToAction("Index");

                var appl =
                    (from App in context.Application
                     join Entry in context.Entry on App.EntryId equals Entry.Id
                     where App.PersonId == PersonId && App.IsCommited == true && App.Enabled == true && App.Id == gAppId
                     select new
                     {
                         Id = App.Id,
                         CommitId = App.CommitId,
                         CommitName = isEng ? Entry.StudyLevelGroupNameEng : Entry.StudyLevelGroupNameRus,
                         App.EntryId,
                     }).FirstOrDefault();

                var appPriors =
                    (from AppDetails in context.ApplicationDetails
                     where AppDetails.ApplicationId == gAppId
                     select new
                     {
                         AppDetails.ProfileInObrazProgramInEntryId,
                         AppDetails.ProfileInObrazProgramInEntryPriority,
                     }).ToList();

                var Ops =
                    (from ProfInOPInEntry in context.ProfileInObrazProgramInEntry
                     where ProfInOPInEntry.ObrazProgramInEntryId == gObrazProgramInEntryId
                     select new StandartObrazProgramInEntryRow()
                     {
                         Id = ProfInOPInEntry.Id,
                         Name = isEng ? ProfInOPInEntry.SP_Profile.NameEng : ProfInOPInEntry.SP_Profile.Name,
                         Priority = ProfInOPInEntry.DefaultPriorityValue ?? 0,
                         DefaultPriority = ProfInOPInEntry.DefaultPriorityValue ?? 0
                     }).ToList();

                int ind = 0;
                foreach (var op in Ops)
                {
                    if (appPriors.Where(x => x.ProfileInObrazProgramInEntryId == op.Id).Count() > 0)
                        Ops[ind].Priority = appPriors.Where(x => x.ProfileInObrazProgramInEntryId == op.Id).First().ProfileInObrazProgramInEntryPriority ?? 0;
                    ind++;
                }

                string ObrazProgramName = context.ObrazProgramInEntry.Where(x => x.Id == gObrazProgramInEntryId).Select(x => isEng ? x.SP_ObrazProgram.NameEng : x.SP_ObrazProgram.Name).FirstOrDefault();
                var RetVal = Ops.OrderBy(x => x.Priority).Select(x => new KeyValuePair<Guid, string>(x.Id, x.Name)).Distinct().ToList();

                PriorityChangerProfileModel mdl = new PriorityChangerProfileModel()
                {
                    ApplicationId = gAppId,
                    CommitId = appl.CommitId,
                    CommitName = appl.CommitName,
                    lstProfiles = RetVal,
                    ApplicationVersionId = gVersionId,
                    ObrazProgramName = ObrazProgramName,
                    ObrazProgramInEntryId = gObrazProgramInEntryId
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

            Guid gCommId;
            if (!Guid.TryParse(model.CommitId, out gCommId))
                return Json(Resources.ServerMessages.IncorrectGUID, JsonRequestBehavior.AllowGet);

            //создаём новую версию изменений
            SortedList<string, object> slParams = new SortedList<string, object>();
            slParams.Add("CommitId", gCommId);
            slParams.Add("VersionDate", DateTime.Now);
            string val = Util.AbitDB.InsertRecordReturnValue("ApplicationCommitVersion", slParams);
            int iCommitVersionId = 0;
            int.TryParse(val, out iCommitVersionId);

            int prior = 0;
            string[] allKeys = Request.Form.AllKeys;
            foreach (string key in allKeys)
            {
                Guid appId;
                if (!Guid.TryParse(key, out appId))
                    continue;

                string query = "UPDATE [Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId AND CommitId=@CommitId;" +
                    " INSERT INTO [ApplicationCommitVersonDetails] (ApplicationCommitVersionId, ApplicationId, Priority) VALUES (@ApplicationCommitVersionId, @Id, @Priority)";
                SortedList<string, object> dic = new SortedList<string, object>();
                dic.AddItem("@Priority", ++prior);
                dic.AddItem("@Id", appId);
                dic.AddItem("@PersonId", PersonId);
                dic.AddItem("@CommitId", gCommId);
                dic.AddItem("@ApplicationCommitVersionId", iCommitVersionId);
                
                try
                {
                    Util.AbitDB.ExecuteQuery(query, dic);
                }
                catch { }

                query = "UPDATE [AG_Application] SET Priority=@Priority WHERE Id=@Id AND PersonId=@PersonId AND CommitId=@CommitId";
                try { 
                    Util.AbitDB.ExecuteQuery(query, dic); 
                }
                catch { }
            }
            return RedirectToAction("Index", "Application", new RouteValueDictionary() { { "id", model.CommitId } });
        }
        [HttpPost]
        public ActionResult PriorityChangeApplication(PriorityChangerApplicationModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid gCommId = model.CommitId;

            int prior = 0;
            string[] allKeys = Request.Form.AllKeys;
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                if (context.ApplicationVersion.Where(x => x.Id == model.ApplicationVersionId).Count() == 0)
                    context.ApplicationVersion.AddObject(new ApplicationVersion() { Id = model.ApplicationVersionId, ApplicationId = model.ApplicationId, VersionDate = DateTime.Now });

                var s = context.ApplicationDetails.Where(x => x.ApplicationId == model.ApplicationId).Select(x => new { x.Id, x.ObrazProgramInEntryId, x.ProfileInObrazProgramInEntryId }).ToList();
                foreach (string key in allKeys)
                {
                    Guid ObrazProgramInEntryId;
                    if (!Guid.TryParse(key, out ObrazProgramInEntryId))
                        continue;

                    prior++;

                    var ProfInOpInEnt = context.ProfileInObrazProgramInEntry.Where(x => x.ObrazProgramInEntryId == ObrazProgramInEntryId)
                        .Select(x => new { x.Id, x.DefaultPriorityValue }).ToList();

                    var versDetails = s.Where(x => x.ObrazProgramInEntryId == ObrazProgramInEntryId).ToList();

                    if (versDetails.Count == 0) //ещё ничего не создано
                    {
                        if (ProfInOpInEnt.Count > 0)
                        {
                            foreach (var p in ProfInOpInEnt)
                            {
                                context.ApplicationDetails.AddObject(new ApplicationDetails()
                                {
                                    Id = Guid.NewGuid(),
                                    ApplicationId = model.ApplicationId,
                                    ObrazProgramInEntryId = ObrazProgramInEntryId,
                                    ObrazProgramInEntryPriority = prior,
                                    ProfileInObrazProgramInEntryId = p.Id,
                                    ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue
                                });
                                //вставляем в логи
                                if (context.ApplicationVersionDetails.Where(x => x.ApplicationVersionId == model.ApplicationVersionId && ObrazProgramInEntryId == ObrazProgramInEntryId && x.ObrazProgramInEntryPriority == prior).Count() == 0)
                                {
                                    context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                                    {
                                        ApplicationVersionId = model.ApplicationVersionId,
                                        ObrazProgramInEntryId = ObrazProgramInEntryId,
                                        ObrazProgramInEntryPriority = prior,
                                        ProfileInObrazProgramInEntryId = p.Id,
                                        ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue
                                    });
                                }
                            }
                        }
                        else
                        {
                            context.ApplicationDetails.AddObject(new ApplicationDetails()
                            {
                                Id = Guid.NewGuid(),
                                ApplicationId = model.ApplicationId,
                                ObrazProgramInEntryId = ObrazProgramInEntryId,
                                ObrazProgramInEntryPriority = prior,
                            });
                            //вставляем в логи
                            context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                            {
                                ApplicationVersionId = model.ApplicationVersionId,
                                ObrazProgramInEntryId = ObrazProgramInEntryId,
                                ObrazProgramInEntryPriority = prior,
                            });
                        }
                    }
                    else //уже что-то есть - нужно лишь обновить и дополнить, если требуется
                    {
                        if (ProfInOpInEnt.Count > 0)
                        {
                            foreach (var p in ProfInOpInEnt) //пробегаем по всем профилям
                            {
                                if (versDetails.Where(x => x.ProfileInObrazProgramInEntryId == p.Id).Count() == 0) //если нет для профиля записи - создать
                                {
                                    context.ApplicationDetails.AddObject(new ApplicationDetails()
                                    {
                                        Id = Guid.NewGuid(),
                                        ApplicationId = model.ApplicationId,
                                        ObrazProgramInEntryId = ObrazProgramInEntryId,
                                        ObrazProgramInEntryPriority = prior,
                                        ProfileInObrazProgramInEntryId = p.Id,
                                        ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue // со стандартным приоритетом
                                    });
                                    //вставляем в логи
                                    context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                                    {
                                        ApplicationVersionId = model.ApplicationVersionId,
                                        ObrazProgramInEntryId = ObrazProgramInEntryId,
                                        ObrazProgramInEntryPriority = prior,
                                        ProfileInObrazProgramInEntryId = p.Id,
                                        ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue
                                    });
                                }
                                else
                                {
                                    var avd = context.ApplicationDetails
                                        .Where(x => x.ProfileInObrazProgramInEntryId == p.Id && x.ObrazProgramInEntryId == ObrazProgramInEntryId && x.ApplicationId == model.ApplicationId)
                                        .FirstOrDefault();
                                    if (avd == null)
                                    {
                                        context.ApplicationDetails.AddObject(new ApplicationDetails()
                                        {
                                            Id = Guid.NewGuid(),
                                            ApplicationId = model.ApplicationId,
                                            ObrazProgramInEntryId = ObrazProgramInEntryId,
                                            ObrazProgramInEntryPriority = prior,
                                            ProfileInObrazProgramInEntryId = p.Id,
                                            ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue // со стандартным приоритетом
                                        });
                                        //вставляем в логи
                                        context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                                        {
                                            ApplicationVersionId = model.ApplicationVersionId,
                                            ObrazProgramInEntryId = ObrazProgramInEntryId,
                                            ObrazProgramInEntryPriority = prior,
                                            ProfileInObrazProgramInEntryId = p.Id,
                                            ProfileInObrazProgramInEntryPriority = p.DefaultPriorityValue
                                        });
                                    }
                                    else // если есть - обновить только приоритет
                                    {
                                        avd.ObrazProgramInEntryPriority = prior;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var avd = context.ApplicationDetails
                                        .Where(x => x.ObrazProgramInEntryId == ObrazProgramInEntryId && x.ApplicationId == model.ApplicationId)
                                        .FirstOrDefault();
                            if (avd == null)
                            {
                                context.ApplicationDetails.AddObject(new ApplicationDetails()
                                {
                                    Id = model.ApplicationVersionId,
                                    ApplicationId = model.ApplicationId,
                                    ObrazProgramInEntryId = ObrazProgramInEntryId,
                                    ObrazProgramInEntryPriority = prior,
                                });
                                //вставляем в логи
                                context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                                {
                                    ApplicationVersionId = model.ApplicationVersionId,
                                    ObrazProgramInEntryId = ObrazProgramInEntryId,
                                    ObrazProgramInEntryPriority = prior,
                                });
                            }
                            else
                            {
                                avd.ObrazProgramInEntryPriority = prior;
                            }
                        }
                    }

                    context.SaveChanges();
                }
            }
            return RedirectToAction("PriorityChangerApplication", new RouteValueDictionary() { { "AppId", model.ApplicationId.ToString("N") }, { "V", model.ApplicationVersionId.ToString("N") } });
        }
        [HttpPost]
        public ActionResult PriorityChangeProfile(PriorityChangerProfileModel model)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid gCommId = model.CommitId;

            int prior = 0;
            string[] allKeys = Request.Form.AllKeys;
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                if (context.ApplicationVersion.Where(x => x.Id == model.ApplicationVersionId).Count() == 0)
                    context.ApplicationVersion.AddObject(new ApplicationVersion() { Id = model.ApplicationVersionId, ApplicationId = model.ApplicationId, VersionDate = DateTime.Now });

                var s = context.ApplicationDetails.Where(x => x.ApplicationId == model.ApplicationId).Select(x => new { x.Id, x.ProfileInObrazProgramInEntryId, x.ProfileInObrazProgramInEntryPriority }).ToList();
                foreach (string key in allKeys)
                {
                    Guid ProfileInObrazProgramInEntryId;
                    if (!Guid.TryParse(key, out ProfileInObrazProgramInEntryId))
                        continue;

                    prior++;

                    var p = context.ProfileInObrazProgramInEntry.Where(x => x.Id == ProfileInObrazProgramInEntryId)
                        .Select(x => new { x.Id, x.ObrazProgramInEntryId, DefaultPriorityValue = x.ObrazProgramInEntry.DefaultPriorityValue }).First();

                    var details = s.Where(x => x.ProfileInObrazProgramInEntryId == ProfileInObrazProgramInEntryId);
                    if (details.Count() == 0) //если не внесено, то внести
                    {
                        context.ApplicationDetails.AddObject(new ApplicationDetails()
                        {
                            Id = Guid.NewGuid(),
                            ApplicationId = model.ApplicationId,
                            ObrazProgramInEntryId = p.ObrazProgramInEntryId,
                            ObrazProgramInEntryPriority = p.DefaultPriorityValue,
                            ProfileInObrazProgramInEntryId = p.Id,
                            ProfileInObrazProgramInEntryPriority = prior
                        });
                        //вставляем в логи
                        if (context.ApplicationVersionDetails.Where(x => x.ApplicationVersionId == model.ApplicationVersionId && x.ProfileInObrazProgramInEntryId == ProfileInObrazProgramInEntryId && x.ProfileInObrazProgramInEntryPriority == prior).Count() == 0)
                        {
                            context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                            {
                                ApplicationVersionId = model.ApplicationVersionId,
                                ProfileInObrazProgramInEntryId = p.Id,
                                ProfileInObrazProgramInEntryPriority = prior,
                                ObrazProgramInEntryId = p.ObrazProgramInEntryId,
                                ObrazProgramInEntryPriority = p.DefaultPriorityValue
                            });
                        }
                    }
                    else //уже что-то есть - нужно лишь обновить
                    {
                        var appDet = context.ApplicationDetails.Where(x => x.ApplicationId == model.ApplicationId && x.ProfileInObrazProgramInEntryId == ProfileInObrazProgramInEntryId).FirstOrDefault();
                        if (appDet == null)
                        {
                            context.ApplicationDetails.AddObject(new ApplicationDetails()
                            {
                                Id = Guid.NewGuid(),
                                ApplicationId = model.ApplicationId,
                                ObrazProgramInEntryId = p.ObrazProgramInEntryId,
                                ObrazProgramInEntryPriority = p.DefaultPriorityValue,
                                ProfileInObrazProgramInEntryId = p.Id,
                                ProfileInObrazProgramInEntryPriority = prior,
                            });
                        }
                        else
                        {
                            appDet.ProfileInObrazProgramInEntryPriority = prior;
                        }
                        //вставляем в логи
                        context.ApplicationVersionDetails.AddObject(new ApplicationVersionDetails()
                        {
                            ApplicationVersionId = model.ApplicationVersionId,
                            ObrazProgramInEntryId = appDet.ObrazProgramInEntryId,
                            ObrazProgramInEntryPriority = appDet.ObrazProgramInEntryPriority,
                            ProfileInObrazProgramInEntryId = ProfileInObrazProgramInEntryId,
                            ProfileInObrazProgramInEntryPriority = prior,
                        });
                    }

                    context.SaveChanges();
                }
            }
            return RedirectToAction("PriorityChangerProfile", 
                new RouteValueDictionary() { { "AppId", model.ApplicationId.ToString("N") }, { "OPIE", model.ObrazProgramInEntryId.ToString("N") }, { "V", model.ApplicationVersionId.ToString("N") } });
        }

        public ActionResult AddFiles()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            string query = "SELECT Id, FileName, FileSize, Comment FROM PersonFile WHERE PersonId=@PersonId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

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
                model.FileTypes = Util.GetPersonFileTypeList();
                return View(model);
            }
        }
        [HttpPost]
        public ActionResult GetFileList()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("Ошибка авторизации");
            List<AppendedFile> lstFiles = Util.GetFileList(PersonId);

            return Json(new { IsOk = lstFiles.Count() > 0 ? true : false, Data = lstFiles });
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
                SortedList<string, object> dic = new SortedList<string, object>();
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
                SortedList<string, object> dic = new SortedList<string, object>();
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
                new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", FileId } });

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

        //public ActionResult GetMotivationMailPDF(string id)
        //{
        //    Guid personId;
        //    if (!Util.CheckAuthCookies(Request.Cookies, out personId))
        //        return Content("Authorization required");
        //    string fontspath = Server.MapPath("~/Templates/times.ttf");
        //    return File(PDFUtils.GetMotivateMail(id, fontspath), "application/pdf", "MotivateEdit.pdf");
        //}

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
            SortedList<string, object> dic = new SortedList<string, object>();
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
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", PersonId } });
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
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
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
                SortedList<string, object> dic = new SortedList<string, object>();
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
                Util.AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Id", tbl.Rows[0].Field<Guid?>("Id") } });
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
            SortedList<string, object> dic = new SortedList<string, object>();
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

            int iStudyLevelId = 0;
            if (iEntryId == 8 || iEntryId == 10)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 3;
            }
            if (iEntryId == 16)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 1;
            }
            if (iEntryId == 18)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 1;
            }
            if (iEntryId == 17)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 2;
            }
            if (iEntryId == 15)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 4;
            }

            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;

            bool bIsSecond = isSecond == "1" ? true : false;
            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT LicenseProgramId, LicenseProgramCode, LicenseProgramName, LicenseProgramNameEng FROM Entry " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND StudyLevelGroupId=@StudyLevelGroupId AND IsSecond=@IsSecond AND IsParallel=@IsParallel " +
                "AND IsReduced=@IsReduced AND [CampaignYear]=@Year AND SemesterId=@SemesterId";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@StudyLevelGroupId", iEntryId);//2 == mag, 1 == 1kurs, 3 - SPO, 4 - аспирант
            if (iStudyLevelId != 0)
            {
                query += " AND StudyLevelId=@StudyLevelId";
                dic.Add("@StudyLevelId", iStudyLevelId);//Id=8 - 9kl, Id=10 - 11 kl
            }
            dic.Add("@IsSecond", bIsSecond);
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId);

            bool isEng = Util.GetCurrentThreadLanguageIsEng();

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var profs =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<int>("LicenseProgramId"),
                     Name = "(" + rw.Field<string>("LicenseProgramCode") + ") " + 
                        (isEng ?
                          (string.IsNullOrEmpty(rw.Field<string>("LicenseProgramNameEng")) ? rw.Field<string>("LicenseProgramName") : rw.Field<string>("LicenseProgramNameEng")) 
                          : rw.Field<string>("LicenseProgramName"))
                 }).OrderBy(x => x.Name);

            if (profs.Count() == 0)
            {
                return Json(new { NoFree = true });
            }

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

            int iStudyLevelId = 0;
            if (iEntryId == 8 || iEntryId == 10)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 3;
            }

            int iSemesterId;
            if (!int.TryParse(semesterId, out iSemesterId))
                iSemesterId = 1;
            int iProfessionId = 1;
            if (!int.TryParse(prof, out iProfessionId))
                iProfessionId = 1;

            bool bIsReduced = isReduced == "1" ? true : false;
            bool bIsParallel = isParallel == "1" ? true : false;

            string query = "SELECT DISTINCT ObrazProgramId, ObrazProgramName, ObrazProgramNameEng FROM Entry " +
                "WHERE StudyFormId=@StudyFormId AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId " +
                "AND StudyLevelGroupId=@StudyLevelGroupId AND IsParallel=@IsParallel AND IsReduced=@IsReduced " +
                "AND CampaignYear=@Year AND SemesterId=@SemesterId";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@StudyLevelGroupId", iEntryId);
            if (iStudyLevelId != 0)
            {
                query += " AND StudyLevelId=@StudyLevelId";
                dic.Add("@StudyLevelId", iStudyLevelId);//Id=8 - 9kl, Id=10 - 11 kl
            }
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId);

            bool isEng = Util.GetCurrentThreadLanguageIsEng();

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var OPs = from DataRow rw in tbl.Rows
                      select new
                      {
                          Id = rw.Field<int>("ObrazProgramId"),
                          Name = isEng ?
                            (string.IsNullOrEmpty(rw.Field<string>("ObrazProgramNameEng")) ? rw.Field<string>("ObrazProgramName") : rw.Field<string>("ObrazProgramNameEng"))
                            : rw.Field<string>("ObrazProgramName")
                      };

            return Json(new { NoFree = OPs.Count() > 0 ? false : true, List = OPs });
        }

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetSpecializations(string prof, string obrazprogram, string studyform, string studybasis, string entry, string CommitId, string isParallel = "0", string isReduced = "0", string semesterId = "1")
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

            int iStudyLevelId = 0;
            if (iEntryId == 8 || iEntryId == 10)
            {
                iStudyLevelId = iEntryId;
                iEntryId = 3;
            }

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
            //bool bIsGosLine = isgosline == "1" ? true : false;

            string query = "SELECT DISTINCT ProfileId, ProfileName FROM Entry WHERE StudyFormId=@StudyFormId " +
                "AND StudyBasisId=@StudyBasisId AND LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyLevelGroupId=@StudyLevelGroupId " +
                //"AND Entry.Id NOT IN (SELECT EntryId FROM [Application] WHERE PersonId=@PersonId AND IsCommited='True' AND EntryId IS NOT NULL and CommitId=@CommitId and IsDeleted=0 and IsGosLine<>@IsGosLine) " +
                "AND IsParallel=@IsParallel AND IsReduced=@IsReduced "+ 
                "AND CampaignYear=@Year AND SemesterId=@SemesterId ";

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@StudyFormId", iStudyFormId);
            dic.Add("@StudyBasisId", iStudyBasisId);
            dic.Add("@LicenseProgramId", iProfessionId);
            dic.Add("@ObrazProgramId", iObrazProgramId);
            dic.Add("@StudyLevelGroupId", iEntryId);
           // dic.Add("@IsGosLine", bIsGosLine);
            if (iStudyLevelId != 0)
            {
                query += " AND StudyLevelId=@StudyLevelId";
                dic.Add("@StudyLevelId", iStudyLevelId);//Id=8 - 9kl, Id=10 - 11 kl
            }
            dic.Add("@IsParallel", bIsParallel);
            dic.Add("@IsReduced", bIsReduced);
            dic.Add("@Year", Util.iPriemYear);
            dic.Add("@SemesterId", iSemesterId); 
            dic.Add("@CommitId", Guid.Parse(CommitId));

            DataTable tblSpecs = Util.AbitDB.GetDataTable(query, dic);
            var Specs =
                from DataRow rw in tblSpecs.Rows
                select new { SpecId = rw.Field<Guid?>("ProfileId"), SpecName = rw.Field<string>("ProfileName") };

            var ret = new
            {
                NoFree = Specs.Count() == 0 ? true : false,
                List = Specs.Select(x => new { Id = x.SpecId, Name = x.SpecName }).ToList()
            };

            int GosLine = Util.IsGosLine(PersonId);

            return Json(new { ret, GosLine });
        } 

        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetAbitCertsAndExams()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json("", JsonRequestBehavior.AllowGet);

            string query = "SELECT DISTINCT Number FROM EgeCertificate WHERE PersonId=@PersonId";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
            List<string> certs = (from DataRow rw in tbl.Rows
                                  select rw.Field<string>("Number")).ToList();

            query = "SELECT EgeExam.Id, EgeExam.Name FROM EgeCertificate INNER JOIN EgeMark ON EgeMark.EgeCertificateId=EgeCertificate.Id " +
                " INNER JOIN EgeExam ON EgeExam.Id=EgeMark.EgeExamId WHERE EgeCertificate.PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

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

            bool bIs2014 = (Is2014 == "true");
            /*if (bool.TryParse(Is2014, out bIs2014))
                bIs2014 = false;*/

            bool bIsInUniversity = (IsInUniversity == "true");  
           /* if (bool.TryParse(IsInUniversity, out bIsInUniversity))
                bIsInUniversity = false;*/

            bool bIsSecondWave = (IsSecondWave == "true");
            /*if (bool.TryParse(IsSecondWave, out bIsSecondWave))
                bIsSecondWave = false;*/
             
            SortedList<string, object> dic = new SortedList<string, object>();
            Guid EgeCertificateId = Guid.Empty;
            
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                
                if (!String.IsNullOrEmpty(certNumber))
                {
                    var certs = context.EgeCertificate.Where(x => x.Number == certNumber).Select(x => new { x.Id, x.PersonId, x.Is2014, x.Number }).ToList();
                    if (certs.Count() > 1)
                        return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });//Это косяк, двух не может быть!!!
                    if (certs.Count() == 1)
                    {
                        if (certs[0].PersonId != PersonId)
                            return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });
                        else
                            EgeCertificateId = certs[0].Id;
                    }
                }
                else
                {
                    var certs = context.EgeCertificate.Where(x => x.Is2014 == true && x.PersonId == PersonId).Select(x => new { x.Id, x.PersonId, x.Is2014, x.Number }).ToList();
                    if (certs.Count() > 1)
                        return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });//Это косяк, двух не может быть!!!
                    if (certs.Count() == 1)
                    {
                        if (certs[0].PersonId != PersonId)
                            return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });
                        else
                            EgeCertificateId = certs[0].Id;
                    }
                }
                 
                //номер должен быть уникальным
               /*if (certs.Count() > 1)
                    return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });//Это косяк, двух не может быть!!!
                if (certs.Count() == 1)
                {
                    if (certs[0].PersonId != PersonId)
                        return Json(new { IsOk = false, ErrorMessage = "Данный сертификат в базе данных принадлежит другому лицу" });
                    else
                        EgeCertificateId = certs[0].Id;
                }*/

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
                    context.SaveChanges();
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
                            ExamMark = exValue//iExamValue.ToString()
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
                Util.AbitDB.ExecuteQuery("DELETE FROM EgeMark WHERE Id=@Id", new SortedList<string, object>() { { "@Id", markId } });
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonScienceWork WHERE Id=@Id", new SortedList<string, object>() { { "@Id", wrkId } });
                DataTable tbl = Util.AbitDB.GetDataTable("SELECT count(Id) as cnt FROM PersonScienceWork WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                var res = new { IsOk = true, Count = tbl.Rows[0].Field<int>("cnt"), ErrorMessage = "" }; 
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
            DataTable tbl = Util.StudDB.GetDataTable(query, new SortedList<string, object>() { { "@SchTypeId", iSchoolType } });
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonWork WHERE Id=@Id", new SortedList<string, object>() { { "@Id", workId } });
                DataTable tbl = Util.AbitDB.GetDataTable("SELECT count(Id) as cnt FROM PersonWork WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
                var res = new { IsOk = true, Count = tbl.Rows[0].Field<int>("cnt"), ErrorMessage = "" };  
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
                new SortedList<string, object>() { { "@Id", ApplicationId } });

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
            string attr = Util.AbitDB.GetStringValue("SELECT IsReadOnly FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
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
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
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
            string attr = Util.AbitDB.GetStringValue("SELECT ISNULL([IsReadOnly], 'False') FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
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
                Util.AbitDB.ExecuteQuery("DELETE FROM PersonFile WHERE PersonId=@PersonId AND Id=@Id", new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Id", fileId } });
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
            SortedList<string, object> dic = new SortedList<string, object>();
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
            DateTime? dtDate;
            if (!DateTime.TryParse(Date, out _date))
                dtDate = null;
            else
                dtDate = _date;


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
                    DocumentDate = dtDate,
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
                    Doc = Series + " " + Number + " от " + (dtDate.HasValue ? dtDate.Value.ToShortDateString() : "-")
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
            Util.AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Id", OlympId } });
            DataTable tbl = Util.AbitDB.GetDataTable("SELECT count(Id) as cnt FROM Olympiads WHERE PersonId=@Id", new SortedList<string, object>() { { "@Id", PersonId } });
            return Json(new { IsOk = true, Count = tbl.Rows[0].Field<int>("cnt")});
        }

        [HttpPost]
        public JsonResult CheckApplication_AG(string profession, string Entryclass, string profileid, string manualExam, string NeedHostel, string CommitId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

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
                     where Abit.PersonId == PersonId && Abit.Enabled == true && Abit.CommitId == gCommId
                     select EntrInEntryGroup.EntryGroupId);

                var AllNeededEntries =
                    (from Entr in context.AG_Entry
                     join EntrInEntryGroup in context.AG_EntryInEntryGroup on Entr.Id equals EntrInEntryGroup.EntryId
                     where EntryGroupList.Contains(EntrInEntryGroup.EntryGroupId)
                     select Entr.Id).ToList();

                var FreeEntries = AllNeededEntries.Except(
                    context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId == gCommId).Select(x => x.EntryId).ToList()).ToList();

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
                         where App.PersonId == PersonId && App.Enabled == true && App.CommitId == gCommId
                         select App.EntryId).ToList();

                    if (eIds.Contains(EntryId))
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_HasApplicationOnEntry });
                }

                return Json(new { IsOk = true, FreeEntries = true });
            }
        }

        [HttpPost]
        public JsonResult AddApplication_AG(string Entryclass, string profession, string profileid, string manualExam, string NeedHostel, string CommitId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.AG_PriemClosed });

                bool needHostel = string.Equals(NeedHostel, "false") ? false : true;

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
                         Ent.DateOfStopEntry,
                         ProgramName = Ent.AG_Program.Name,
                         ProfileName = Ent.AG_Profile.Name,
                     }).ToList();

                if (EntryList.Count > 1)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_2MoreEntry + " (" + EntryList.Count + ")" });
                if (EntryList.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoEntry });

                Guid EntryId = EntryList.First().EntryId;
                DateTime? timeOfStart = EntryList.First().DateOfStartEntry;
                DateTime? timeOfStop = EntryList.First().DateOfStopEntry;
                string Profession = EntryList.First().ProgramName;
                string Specialization = EntryList.First().ProfileName;

                if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NotOpenedEntry });

                if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_ClosedEntry });

                //проверка на группы
                var EntryGroupList =
                    (from Entr in context.AG_Entry
                     join EntrInEntryGroup in context.AG_EntryInEntryGroup on Entr.Id equals EntrInEntryGroup.EntryId
                     join Abit in context.AG_Application on Entr.Id equals Abit.EntryId
                     where Abit.PersonId == PersonId && Abit.Enabled == true && Abit.CommitId == gCommId
                     select EntrInEntryGroup.EntryGroupId);

                bool isNoEntries = EntryGroupList.Count() == 0;
                var AllNeededEntries =
                    (from Entr in context.AG_Entry
                     join EntrInEntryGroup in context.AG_EntryInEntryGroup on Entr.Id equals EntrInEntryGroup.EntryId
                     where EntryGroupList.Contains(EntrInEntryGroup.EntryGroupId) || isNoEntries
                     select Entr.Id).ToList();

                var FreeEntries = AllNeededEntries.Except(
                    context.AG_Application.Where(x => x.PersonId == PersonId && x.CommitId == gCommId).Select(x => x.EntryId).ToList()).ToList();

                if (FreeEntries.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoFreeEntries });
                else
                {
                    if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NotOpenedEntry });

                    if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_ClosedEntry });

                    var eIds =
                        (from App in context.AG_Application
                         where App.PersonId == PersonId && App.Enabled == true && App.CommitId == gCommId
                         select App.EntryId).ToList();

                    if (eIds.Contains(EntryId))
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_HasApplicationOnEntry });
                }

                int? PriorMax = context.AG_Application.Where(x => x.PersonId == PersonId && x.Enabled == true && x.CommitId == gCommId).Select(x => x.Priority).DefaultIfEmpty(0).Max();
                // если в коммите уже есть закоммиченные заявления, то добавляемое тоже считаем закоммиченным
                bool isCommited = context.AG_Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == gCommId).Count() > 0;
                Guid appId = Guid.NewGuid();
                context.AG_Application.AddObject(new AG_Application()
                {
                    Id = appId,
                    PersonId = PersonId,
                    EntryId = EntryId,
                    HostelEduc = needHostel,
                    Priority = PriorMax.HasValue ? PriorMax.Value + 1 : 1,
                    Enabled = true,
                    DateOfStart = DateTime.Now,
                    ManualExamId = iManualExamId == 0 ? null : (int?)iManualExamId,
                    CommitId = gCommId,
                    IsCommited = isCommited
                });
                context.SaveChanges();

                string ManualExamName = context.AG_ManualExam.Where(x => x.Id == iManualExamId).Select(x => x.Name).FirstOrDefault();

                return Json(new { IsOk = true, Profession = Profession, Specialization = Specialization, ManualExam = ManualExamName, Id = appId.ToString("N") });
            }
        }

        [HttpPost]
        public JsonResult DeleteApplication_AG(string id, string CommitId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                Guid ApplicationId;
                if (!Guid.TryParse(id, out ApplicationId))
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                var App = context.AG_Application.Where(x => x.Id == ApplicationId).FirstOrDefault();
                if (App == null)
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                //if (App.IsCommited)
                //    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_FailDeleteApp_IsCommited });
                try
                {
                    context.AG_Application.DeleteObject(App);
                    context.SaveChanges();
                }
                catch
                {
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_DeleteApp_Fail });
                }

                return Json(new { IsOk = true });
            }
        }

        [HttpPost]
        public JsonResult AddApplication_Mag(string priority, string studyform, string studybasis, string entry, string isSecond, string isReduced, string isParallel, string profession, string obrazprogram, string specialization, string NeedHostel, string IsGosLine, string CommitId, string semesterId="1", string secondtype="1", string reason="")
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                /*if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.AG_PriemClosed });*/

                bool needHostel = string.Equals(NeedHostel, "false") ? false : true;

                int iStudyFormId = Util.ParseSafe(studyform);
                int iStudyBasisId = Util.ParseSafe(studybasis);
                int EntryTypeId = Util.ParseSafe(entry);
                if (EntryTypeId == 8 || EntryTypeId == 10)
                {
                    EntryTypeId = 3;
                }
                int iPriority = Util.ParseSafe(priority);
                 
                int iProfession = Util.ParseSafe(profession);
                int iObrazProgram = Util.ParseSafe(obrazprogram);

                int iParallel = Util.ParseSafe(isParallel);
                int iReduced = Util.ParseSafe(isReduced);
                int iSecond = Util.ParseSafe(isSecond);
                int iGosLine = Util.ParseSafe(IsGosLine);

                int iSemesterId;
                if (!int.TryParse(semesterId, out iSemesterId))
                    iSemesterId = 1;

                int iSecondType;
                if (!int.TryParse(secondtype, out iSecondType))
                    iSecondType = 1;

                bool bIsParallel = iParallel == 1;
                bool bIsReduced = iReduced == 1;
                bool bIsSecond = iSecond == 1;
                bool bIsGosLine = iGosLine == 1;
                
                Guid gSpecialization = Guid.Empty;
                if ((specialization != null) && (specialization != "") && (specialization != "null"))
                    gSpecialization = Guid.Parse(specialization);

                bool bisEng = Util.GetCurrentThreadLanguageIsEng();

                //------------------Проверка на дублирование заявлений---------------------------------------------------------------------

                var EntryList =
                     (from Ent in context.Entry
                      join SPStudyLevel in context.SP_StudyLevel on Ent.StudyLevelId equals SPStudyLevel.Id
                      join Semester in context.Semester on Ent.SemesterId equals Semester.Id
                      where Ent.StudyFormId == iStudyFormId &&
                            Ent.StudyBasisId == iStudyBasisId &&
                            Ent.LicenseProgramId == iProfession &&
                            Ent.ObrazProgramId == iObrazProgram &&
                            Ent.CampaignYear == Util.iPriemYear &&
                            SPStudyLevel.StudyLevelGroupId == EntryTypeId &&
                            Ent.IsParallel == bIsParallel &&
                            Ent.IsReduced == bIsReduced &&
                            Ent.IsSecond == bIsSecond &&
                           (gSpecialization == Guid.Empty ? true : Ent.ProfileId == gSpecialization) &&
                            Ent.SemesterId == iSemesterId
                      select new
                      {
                          EntryId = Ent.Id,
                          Ent.DateOfStart,
                          Ent.DateOfClose, 
                          Ent.DateOfStart_Foreign,
                          Ent.DateOfClose_Foreign,
                          Ent.DateOfStart_GosLine,
                          Ent.DateOfClose_GosLine,
                          Ent.FacultyId,
                          Ent.FacultyName,
                          SemestrName = Semester.Name,
                          StudyFormName = bisEng ? (String.IsNullOrEmpty(Ent.StudyFormNameEng) ? Ent.StudyFormName : Ent.StudyFormNameEng) : Ent.StudyFormName,
                          StudyBasisName = /*bisEng ? (String.IsNullOrEmpty(Ent.StudyBasisNameEng) ? Ent.StudyBasisName : Ent.StudyBasisNameEng) :*/ Ent.StudyBasisName,
                          Profession = bisEng ? (String.IsNullOrEmpty(Ent.LicenseProgramNameEng) ? Ent.LicenseProgramName : Ent.LicenseProgramNameEng) : Ent.LicenseProgramName,
                          ObrazProgram = bisEng ? (String.IsNullOrEmpty(Ent.ObrazProgramNameEng) ? Ent.ObrazProgramName : Ent.ObrazProgramNameEng) : Ent.ObrazProgramName,
                          Specialization = bisEng ? (String.IsNullOrEmpty(Ent.ProfileNameEng) ? Ent.ProfileName : Ent.ProfileNameEng) : Ent.ProfileName,
                      }).ToList();
                 
                if (EntryList.Count > 1)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_2MoreEntry + " (" + EntryList.Count + ")" });
                if (EntryList.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoEntry });

                Guid EntryId = EntryList.First().EntryId;

                if (context.Application.Where(x => x.PersonId == PersonId && x.CommitId == gCommId && x.EntryId == EntryId && x.IsDeleted == false && x.IsGosLine == bIsGosLine).Count() > 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.ErrorHasApplication }); 

                DateTime? timeOfStart; 
                DateTime? timeOfStop;

                string StudyFormName = EntryList.First().StudyFormName;
                string StudyBasisName = EntryList.First().StudyBasisName;
                string Profession = EntryList.First().Profession;
                string ObrazProgram = EntryList.First().ObrazProgram;
                string Specialization = EntryList.First().Specialization;
                string faculty = EntryList.First().FacultyName;
                string SemesterName = EntryList.First().SemestrName;
                
                  
                int res = Util.GetRess(PersonId);

                if (bIsGosLine == true)
                {
                    // рф-рф
                    timeOfStart = EntryList.First().DateOfStart_GosLine;
                    timeOfStop = EntryList.First().DateOfClose_GosLine;
                }
                else
                {
                    if ((iStudyBasisId == 2) && ((res == 2) || (res == 4)))
                    {
                        timeOfStart = EntryList.First().DateOfStart_Foreign;
                        timeOfStop = EntryList.First().DateOfClose_Foreign;
                    }
                    else
                    {
                        timeOfStart = EntryList.First().DateOfStart;
                        timeOfStop = EntryList.First().DateOfClose;
                    }
                }
                  
                if (timeOfStart.HasValue && timeOfStart > DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NotOpenedEntry });

                if (timeOfStop.HasValue && timeOfStop < DateTime.Now)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_ClosedEntry });

                //проверка на 3 направления
                if (EntryTypeId == 1)
                {
                    var cnt = context.Abiturient.Where(x => x.CommitId == gCommId && x.LicenseProgramId != iProfession && !x.IsDeleted).Select(x => x.LicenseProgramId).Distinct().Count();
                    if (cnt > 2)
                        return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_3MorePrograms });
                }

                int count = context.Application.Where(x => x.PersonId == PersonId && x.Enabled == true && x.CommitId == gCommId && x.Priority == iPriority).Select(x => x.Id).Count();
                if (count == 0)
                {
                }
                else
                {
                    int? PriorMax = context.Application.Where(x => x.PersonId == PersonId && x.Enabled == true && x.CommitId == gCommId).Select(x => x.Priority).DefaultIfEmpty(0).Max();
                    iPriority = PriorMax ?? 1;
                }
                // если в коммите уже есть закоммиченные заявления, то добавляемое тоже считаем закоммиченным

                bool isCommited = context.Application.Where(x => x.PersonId == PersonId && x.IsCommited == true && x.CommitId == gCommId).Count() > 0;
                Guid appId = Guid.NewGuid();
                context.Application.AddObject(new Application()
                {
                    Id = appId,
                    PersonId = PersonId,
                    EntryId = EntryId,
                    HostelEduc = needHostel,
                    Priority = iPriority,
                    Enabled = true,
                    EntryType = EntryTypeId,
                    DateOfStart = DateTime.Now,
                    CommitId = gCommId,
                    IsGosLine = bIsGosLine,
                    IsCommited = isCommited,
                    SecondTypeId = iSecondType
                });
                context.SaveChanges();

                if (!String.IsNullOrEmpty(reason))
                {
                    Guid gCommitId = Guid.Parse(CommitId);
                    string query = "Select COUNT(PersonId) from PersonChangeStudyFormReason where PersonId=@PersonId ";
                    count = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId }, { "@CommitId", gCommitId } });
                    if (count > 0)
                    {
                        query = "Update PersonChangeStudyFormReason set Reason = @Reason, ApplicationId = @AppId, CommitId=@CommitId WHERE PersonId=@PersonId ";
                        Util.AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@PersonId", PersonId }, { "@AppId", appId }, { "@CommitId", gCommitId }, { "@Reason", reason } });
                    }
                    else
                    {
                        query = "Insert into PersonChangeStudyFormReason (PersonId, ApplicationId, CommitId, Reason) VALUES (@PersonId, @AppId, @CommitId, @Reason)";
                        Util.AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@PersonId", PersonId }, { "@AppId", appId }, { "@CommitId", gCommitId }, { "@Reason", reason } });
                    }
                }


                var Applications = context.Abiturient.Where(x => x.PersonId == PersonId && x.CommitId == gCommId && x.IsCommited == isCommited)
                    .Select(x => new { x.StudyLevelGroupNameRus, x.StudyLevelGroupNameEng, x.SecondTypeId}).FirstOrDefault();
                string LevelGroupName = bisEng ? Applications.StudyLevelGroupNameEng : Applications.StudyLevelGroupNameRus +
                                    (Applications.SecondTypeId.HasValue ?
                                        ((Applications.SecondTypeId == 3) ? (bisEng ? " (recovery)" : " (восстановление)") :
                                        ((Applications.SecondTypeId == 2) ? (bisEng ? " (transfer)" : " (перевод)") :
                                        ((Applications.SecondTypeId == 4) ? (bisEng ? " (changing form of education)" : " (смена формы обучения)") :
                                        ((Applications.SecondTypeId == 5) ? (bisEng ? " (changing basis of education)" : " (смена основы обучения)") :
                                        ((Applications.SecondTypeId == 6) ? (bisEng ? " (changing educational program)" : " (смена образовательной программы)") :
                                        ""))))) : "");

                return Json(new { IsOk = true, StudyLevelGroupName = LevelGroupName, StudyFormName = StudyFormName, StudyBasisName = StudyBasisName, Profession = Profession, Specialization = Specialization, ObrazProgram = ObrazProgram, Id = appId.ToString("N"), Faculty = faculty, isgosline = IsGosLine, semesterId = SemesterName, Reason = reason });
            }
        }

        [HttpPost]
        public JsonResult DeleteApplication_Mag(string id, string CommitId)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                Guid ApplicationId;
                if (!Guid.TryParse(id, out ApplicationId))
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                var App = context.Application.Where(x => x.Id == ApplicationId).FirstOrDefault();
                if (App == null)
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                //if (App.IsCommited)
                //    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_FailDeleteApp_IsCommited });
                try
                {
                    App.IsDeleted = true;
                    context.SaveChanges();
                }
                catch
                {
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_DeleteApp_Fail });
                }

                return Json(new { IsOk = true });
            }
        }

        [HttpPost]
        public JsonResult CheckApplication_Mag(string studyform, string studybasis, string entry, string isSecond, string isReduced, string isParallel, string profession, string obrazprogram, string specialization, string NeedHostel, string CommitId, string semesterId="1")
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            Guid gCommId;
            if (!Guid.TryParse(CommitId, out gCommId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var PersonInfo = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (PersonInfo == null)//а что это могло бы значить???
                    return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID });

                //if (DateTime.Now >= new DateTime(2014, 6, 23, 0, 0, 0))
                //    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.AG_PriemClosed });

                bool needHostel = string.IsNullOrEmpty(NeedHostel) ? false : true;

                int iStudyFormId = Util.ParseSafe(studyform);
                int iStudyBasisId = Util.ParseSafe(studybasis);
                int EntryTypeId = Util.ParseSafe(entry);
                if (EntryTypeId == 8 || EntryTypeId == 10)
                {
                    EntryTypeId = 3;
                }
                int iProfession = Util.ParseSafe(profession);
                int iObrazProgram = Util.ParseSafe(obrazprogram);
                int iParallel = Util.ParseSafe(isParallel);
                int iReduced = Util.ParseSafe(isReduced);
                int iSecond = Util.ParseSafe(isSecond);
                int iSemesterId;
                if (!int.TryParse(semesterId, out iSemesterId))
                    iSemesterId = 1;

                bool bIsParallel = iParallel == 1;
                bool bIsReduced = iReduced == 1;
                bool bIsSecond = iSecond == 1; 

                Guid gSpecialization = Guid.Empty;
                if ((specialization != null) && (specialization != "") && (specialization != "null"))
                    gSpecialization = Guid.Parse(specialization);

                //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
                var EntryList =
                      (from Ent in context.Entry
                       join SPStudyLevel in context.SP_StudyLevel on Ent.StudyLevelId equals SPStudyLevel.Id
                       where Ent.StudyFormId == iStudyFormId &&
                             Ent.StudyBasisId == iStudyBasisId &&
                             Ent.LicenseProgramId == iProfession &&
                             Ent.ObrazProgramId == iObrazProgram &&
                             Ent.CampaignYear == Util.iPriemYear &&
                             SPStudyLevel.StudyLevelGroupId == EntryTypeId &&
                             Ent.IsParallel == bIsParallel &&
                             Ent.IsReduced == bIsReduced &&
                             Ent.IsSecond == bIsSecond &&
                            (gSpecialization == Guid.Empty ? true : Ent.ProfileId == gSpecialization) &&
                             Ent.SemesterId == iSemesterId
                       select new
                       {
                           EntryId = Ent.Id,
                           Ent.DateOfStart,
                           Ent.DateOfClose,
                           StudyFormName = Ent.StudyFormName,
                           StudyBasisName = Ent.StudyBasisName,
                           Profession = Ent.LicenseProgramName,
                           ObrazProgram = Ent.ObrazProgramName,
                           Specialization = Ent.ProfileName
                       }).ToList();

                if (EntryList.Count > 1)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_2MoreEntry + " (" + EntryList.Count.ToString() + ")" });
                if (EntryList.Count == 0)
                    return Json(new { IsOk = false, ErrorMessage = Resources.NewApplication.NewApp_NoEntry });

                Guid EntryId = EntryList.First().EntryId;
                DateTime? timeOfStart = EntryList.First().DateOfStart;
                DateTime? timeOfStop = EntryList.First().DateOfClose; 

                return Json(new { IsOk = true, FreeEntries = true });
            }
        }

        public JsonResult GetStudyLevels_SPO()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired });

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var Person = context.Person.Where(x => x.Id == PersonId).FirstOrDefault();
                if (Person == null)
                    return Json(new { IsOk = false });

                if (Person.PersonEducationDocument == null)
                    return Json(new { IsOk = false, ErrorMessage = "Нет сведений об образовании!" });

                PersonEducationDocument PersonEducationDocument = Person.PersonEducationDocument;
                if (PersonEducationDocument.SchoolTypeId == 1)
                {
                    if (PersonEducationDocument.SchoolExitClassId.HasValue)
                    {
                        //школьники 11 класса могут поступать всюду
                        //школьники 9-10 классов могут поступать только на 9 класс
                        //остальные школьники в пролёте
                        if (PersonEducationDocument.SchoolExitClass.IntValue < 9)
                        {
                            return Json(new { IsOk = false, ErrorMessage = "Для " + PersonEducationDocument.SchoolExitClass.IntValue + " класса доступен только приём в АГ" });
                        }
                        else if (PersonEducationDocument.SchoolExitClass.IntValue < 11)
                        {
                            var lst = context.SP_StudyLevel.Where(x => x.Id == 10).Select(x => new { x.Id, x.Name }).ToList();
                            return Json(new { IsOk = true, List = lst });
                        }
                        else
                        {
                            var lst = context.SP_StudyLevel.Where(x => x.StudyLevelGroupId == 3).Select(x => new { x.Id, x.Name }).ToList();
                            return Json(new { IsOk = true, List = lst });
                        }
                    }
                    else
                        return Json(new { IsOk = false, ErrorMessage = "Нет данных об оконченном классе!" });
                }
                else
                {
                    var lst = context.SP_StudyLevel.Where(x => x.StudyLevelGroupId == 3).Select(x => new { x.Id, x.Name }).ToList();
                    return Json(new { IsOk = true, List = lst });
                }

            }
        }
        #endregion

        public ActionResult HeartBeat()
        {
            try
            {
                string query = "SELECT Id FROM [dbo].[_HeartBeat] WHERE Id=@Id";
                Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", Guid.NewGuid() } });
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message + (ex.InnerException == null ? "" : "\nINNER EXCEPTION: " + ex.InnerException), JsonRequestBehavior.AllowGet);
            }
        }
    }
}
