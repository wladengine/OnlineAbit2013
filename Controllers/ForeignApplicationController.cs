using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data;
using OnlineAbit2013.Models;

namespace OnlineAbit2013.Controllers
{
    public class ForeignApplicationController : Controller
    {
        public ActionResult Index(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return RedirectToAction("LogOn", "Account");

            Guid appId = new Guid();
            if (!Guid.TryParse(id, out appId))
                return RedirectToAction("Main", "Abiturient");

            SortedList<string, object> prms = new SortedList<string, object>()
            {
                { "@PersonId", personId },
                { "@Id", appId }
            };

            DataTable tbl =
                Util.AbitDB.GetDataTable("SELECT [ForeignApplication].Id, LicenseProgramName, Priority, ObrazProgramName, ProfileName, [Entry].StudyFormId, " +
                " [Entry].StudyBasisId, Enabled, DateOfDisable FROM [ForeignApplication] INNER JOIN Entry ON [ForeignApplication].EntryId = Entry.Id " +
                " WHERE PersonId=@PersonId AND [ForeignApplication].Id=@Id", prms);

            if (tbl.Rows.Count == 0)
                return RedirectToAction("Main", "Abiturient");

            DataRow rw = tbl.Rows[0];
            var app = new
            {
                Id = rw.Field<Guid>("Id"),
                Profession = rw.Field<string>("LicenseProgramName"),
                Priority = rw.Field<int>("Priority"),
                ObrazProgram = rw.Field<string>("ObrazProgramName"),
                Specialization = rw.Field<string>("ProfileName"),
                StudyFormId = rw.Field<int>("StudyFormId"),
                StudyBasisId = rw.Field<int>("StudyBasisId"),
                Enabled = rw.Field<bool?>("Enabled"),
                DateOfDisable = rw.Field<DateTime?>("DateOfDisable")
            };

            int sfId = app.StudyFormId;
            int sbId = app.StudyBasisId;

            string query = "SELECT DISTINCT Exam FROM AbitMark INNER JOIN Student ON Student.Id = AbitMark.AbiturientId WHERE " +
                "Profession=@Profession AND ObrazProgram=@ObrazProgram AND Specialization=@Specialization AND StudyBasisId=@StudyBasisId AND StudyFormId=@StudyFormId";

            prms.Clear();
            prms.Add("@Profession", app.Profession);
            prms.Add("@ObrazProgram", app.ObrazProgram);
            prms.Add("@Specialization", app.Specialization == null ? "" : app.Specialization);
            prms.Add("@StudyBasisId", app.StudyBasisId);
            prms.Add("@StudyFormId", app.StudyFormId);

            tbl = Util.StudDB.GetDataTable(query, prms);

            var exams = (from DataRow row in tbl.Rows
                         select row.Field<string>("Exam")
                         ).ToList();


            query = "SELECT Id, FileName, FileSize, Comment FROM ApplicationFile WHERE ApplicationId=@AppId";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", appId } });
            var lFiles =
                (from DataRow row in tbl.Rows
                 select new AppendedFile()
                 {
                     Id = row.Field<Guid>("Id"),
                     FileName = row.Field<string>("FileName"),
                     FileSize = row.Field<int>("FileSize"),
                     Comment = row.Field<string>("Comment")
                 }).ToList();

            query = "SELECT Id, MailText FROM MotivationMail WHERE ApplicationId=@AppId";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", app.Id } });

            string motivMail = "";
            Guid motivId = Guid.Empty;
            if (tbl.Rows.Count > 0)
            {
                motivMail = tbl.Rows[0].Field<string>("MailText");
                motivId = tbl.Rows[0].Field<Guid>("Id");
            }


            ExtApplicationModel model = new ExtApplicationModel()
            {
                Id = app.Id,
                Priority = app.Priority.ToString(),
                Profession = app.Profession,
                ObrazProgram = app.ObrazProgram,
                Specialization = app.Specialization,
                StudyForm = Util.StudyFormAll[sfId] == null ? "" : Util.StudyFormAll[sfId],
                StudyBasis = Util.StudyBasisAll[sbId] == null ? "" : Util.StudyBasisAll[sbId],
                Enabled = app.Enabled.HasValue ? app.Enabled.Value : false,
                Exams = exams,
                Files = lFiles,
                DateOfDisable = app.DateOfDisable.HasValue ? app.DateOfDisable.Value.ToString("dd.MM.yyyy HH:mm:ss") : "",
                MotivateEditText = motivMail,
                MotivateEditId = motivId
            };

            return View(model);
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
                return RedirectToAction("LogOnForeign", "Account");

            if (!Util.CheckIsForeign(PersonId))
                return RedirectToAction("NewApplication", "Abiturient");

            DataTable tbl = Util.AbitDB.GetDataTable("SELECT StudyLevelId, ObtainingLevelId FROM ForeignPerson WHERE Id=@Id",
                new SortedList<string, object>() { { "@Id", PersonId } });

            if (tbl.Rows.Count == 0)
                return RedirectToAction("PersonalOffice", "ForeignAbiturient");


            int iStudyLevelId = tbl.Rows[0].Field<int?>("StudyLevelId").HasValue ?
                tbl.Rows[0].Field<int?>("StudyLevelId").Value : 1;
            int iObtainingLevelId = tbl.Rows[0].Field<int?>("ObtainingLevelId").HasValue ?
                tbl.Rows[0].Field<int?>("ObtainingLevelId").Value : 1;

            ForeignNewApplicationModel model = new ForeignNewApplicationModel();
            if (iObtainingLevelId == 2 || iObtainingLevelId == 4)//бак-спец без ограничений
                model.EntryType = 1;
            else if (iObtainingLevelId == 3)//маг (не младше бакалавра)
                model.EntryType = 2;

            string query = "SELECT StudyLevel.Id, StudyLevel.NameEng, StudyLevel.NameRus FROM ForeignPerson INNER JOIN StudyLevel " +
                " ON ForeignPerson.ObtainingLevelId=StudyLevel.Id WHERE ForeignPerson.Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
            var StLev =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Name = rw.Field<string>("NameRus"),
                     NameEng = rw.Field<string>("NameEng")
                 }).ToList().FirstOrDefault();

            if (StLev != null)
            {
                model.ObtainingLevel =
                    System.Threading.Thread.CurrentThread.CurrentUICulture.Name ==
                    System.Globalization.CultureInfo.GetCultureInfo("ru-RU").Name ?
                    StLev.Name : StLev.NameEng;
            }

            query = "SELECT DISTINCT StudyFormId, StudyFormName FROM Entry";
            tbl = Util.AbitDB.GetDataTable(query, null);
            model.StudyForms =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("StudyFormId"),
                     Text = rw.Field<string>("StudyFormName")
                 }).AsEnumerable()
                .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                .ToList();

            query = "SELECT DISTINCT StudyBasisId, StudyBasisName FROM Entry";
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

        [HttpPost]
        public ActionResult NewApp()
        {
            //Guid PersonId;
            //if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            //    return RedirectToAction("LogOnForeign", "Account");

            //List<string> errList = new List<string>();

            //int iFacultyId;
            //int iProfessionId;
            //int iObrazProgramId;
            //int iProfileId;
            //bool bHostelEduc;
            //if (!int.TryParse(Request.Form["Faculty"], out iFacultyId))
            //    errList.Add("Faculty is not selected");
            //if (!int.TryParse(Request.Form["Profession"], out iProfessionId))
            //    errList.Add("Profession is not selected");
            //if (!int.TryParse(Request.Form["ObrazProgram"], out iObrazProgramId))
            //    errList.Add("Study program is not selected");
            //int.TryParse(Request.Form["Profile"], out iProfileId);
            //bool.TryParse(Request.Form["HostelEduc"], out bHostelEduc);

            //if (errList.Count > 0)
            //    return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", errList.ToArray() } });

            //string query = 
            //    "SELECT SP_Entry.Id FROM SP_Entry WHERE StudyBasisId='2' AND FacultyId=@FacultyId AND ProfessionId=@ProfesionId AND ObrazProgramId=@ObrazProgramId {0} " + 
            //    "EXCEPT (SELECT EntryId FROM [ForeignApplication] WHERE PersonId=@PersonId AND Enabled='True')";
            //SortedList<string, object> dic = new SortedList<string, object>();
            //if (iProfileId != 0)
            //{
            //    query = string.Format(query, "AND ProfileId=@ProfileId");
            //    dic.Add("@ProfileId", iProfileId);
            //}

            //dic.AddItem("@FacultyId", iFacultyId);
            //dic.AddItem("@ProfesionId", iProfessionId);
            //dic.AddItem("@ObrazProgramId", iObrazProgramId);
            //dic.AddItem("@PersonId", PersonId);

            //DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            //if (tbl.Rows.Count == 0)
            //{
            //    errList.Add("No study plans founded. Please, try again");
            //    return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", errList.ToArray() } });
            //}

            //int iPriority = 0;
            //query = "SELECT MAX(Priority) FROM [ForeignApplication] WHERE PersonId=@PersonId AND Enabled='True'";
            //string maxval = Util.AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
            //if (string.IsNullOrEmpty(maxval))
            //    iPriority = 0;
            //else
            //    if (!int.TryParse(maxval, out iPriority))
            //        iPriority = 0;

            //Guid EntryId = tbl.Rows[0].Field<Guid>("Id");


            //query = "INSERT INTO [ForeignApplication] (Id, PersonId, Priority, EntryId, Enabled, HostelEduc) " +
            //    " VALUES (@Id, @PersonId, @Priority, @EntryId, @Enabled, @HostelEduc)";

            //dic.Clear();
            //dic.Add("@Id", Guid.NewGuid());
            //dic.Add("@PersonId", PersonId);
            //dic.Add("@Priority", iPriority++);
            //dic.Add("@EntryId", EntryId);
            //dic.Add("@Enabled", true);
            //dic.Add("@HostelEduc", bHostelEduc);
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            string faculty = Request.Form["lFaculty"];
            string profession = Request.Form["lProfession"];
            string obrazprogram = Request.Form["lObrazProgram"];
            string sform = Request.Form["StudyFormId"];
            string sbasis = Request.Form["StudyBasisId"];
            bool needHostel = string.IsNullOrEmpty(Request.Form["HostelEduc"]) ? false : true;

            int iStudyFormId = Util.ParseSafe(sform);
            int iStudyBasisId = Util.ParseSafe(sbasis);
            int iFacultyId = Util.ParseSafe(faculty);
            int iProfession = Util.ParseSafe(profession);
            int iObrazProgram = Util.ParseSafe(obrazprogram);
            Guid ProfileId = Guid.Empty;
            if (!string.IsNullOrEmpty(Request.Form["lSpecialization"]))
                Guid.TryParse(Request.Form["lSpecialization"], out ProfileId);

            int iEntry = Util.ParseSafe(Request.Form["EntryType"]);

            //------------------Проверка на дублирование заявлений---------------------------------------------------------------------
            string query = "SELECT Id FROM Entry WHERE LicenseProgramId=@LicenseProgramId AND ObrazProgramId=@ObrazProgramId AND StudyFormId=@SFormId AND StudyBasisId=@SBasisId  AND IsSecond='False' AND IsReduced='False' AND IsParallel='False'" +
                (ProfileId == Guid.Empty ? "" : " AND ProfileId=@ProfileId ") + (iFacultyId == 0 ? "" : " AND FacultyId=@FacultyId ");

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@LicenseProgramId", iProfession);
            dic.Add("@ObrazProgramId", iObrazProgram);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@SBasisId", iStudyBasisId);
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

            query = "SELECT EntryId FROM [ForeignApplication] WHERE PersonId=@PersonId AND Enabled='True' AND EntryId IS NOT NULL";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
            var eIds =
                from DataRow rw in tbl.Rows
                select rw.Field<Guid>("EntryId");
            if (eIds.Contains(EntryId))
                return RedirectToAction("NewApplication", new RouteValueDictionary() { { "errors", "Заявление на данную программу уже подано" } });

            DataTable tblPriors = Util.AbitDB.GetDataTable("SELECT Priority FROM [ForeignApplication] WHERE PersonId=@PersonId AND Enabled=@Enabled",
                new SortedList<string, object>() { { "@PersonId", PersonId }, { "@Enabled", true } });
            int? PriorMax =
                (from DataRow rw in tblPriors.Rows
                 select rw.Field<int?>("Priority")).Max();

            Guid appId = Guid.NewGuid();
            query = "INSERT INTO [ForeignApplication] (Id, PersonId, EntryId, HostelEduc, Enabled, Priority, EntryType) " +
                "VALUES (@Id, @PersonId, @EntryId, @HostelEduc, @Enabled, @Priority, @EntryType)";
            SortedList<string, object> prms = new SortedList<string, object>()
            {
                { "@Id", appId },
                { "@PersonId", PersonId },
                { "@EntryId", EntryId },
                { "@HostelEduc", needHostel },
                { "@Enabled", true },
                { "@Priority", PriorMax.HasValue ? PriorMax.Value + 1 : 1 },
                { "@EntryType", iEntry }
            };

            Util.AbitDB.ExecuteQuery(query, prms);

            return RedirectToAction("Main", "ForeignAbiturient");
        }

        [HttpPost]
        public ActionResult AddFile()
        {
            string id = Request.Form["id"];
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid ApplicationId = new Guid();
            if (!Guid.TryParse(id, out ApplicationId))
                return RedirectToAction("Main", "ForeignAbiturient");

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
                string query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType) " +
                    " VALUES (@Id, @ApplicationId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType)";
                SortedList<string, object> dic = new SortedList<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@ApplicationId", ApplicationId);
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

            return RedirectToAction("Index", new RouteValueDictionary() { { "id", id } });
        }

        public ActionResult Disable(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired };
                return Json(res);
            }

            if (PersonId == Guid.Empty)
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.AuthorizationRequired };
                return Json(res);
            }

            Guid AppId;
            if (!Guid.TryParse(id, out AppId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
                return Json(res);
            }

            bool? isEnabled = (bool?)Util.AbitDB.GetValue("SELECT Enabled FROM [ForeignApplication] WHERE Id=@Id AND PersonId=@PersonId",
                new SortedList<string, object>() { { "@Id", AppId }, { "@PersonId", PersonId } });

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
                string query = "UPDATE [ForeignApplication] SET Enabled=@Enabled, DateOfDisable=@DateOfDisable, Priority=@Priority WHERE Id=@Id";
                SortedList<string, object> dic = new SortedList<string, object>();
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

        public FileContentResult GetPrint(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Authentification Error"), "text/plain");

            Guid appId;
            if (!Guid.TryParse(id, out appId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Ошибка идентификатора заявления"), "text/plain");

            string query = "SELECT COUNT(Id) FROM Application WHERE Id=@Id AND PersonId=@PersonId";
            int cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", appId }, { "@PersonId", personId } });
            if (cnt == 0)
            {
                query = "SELECT COUNT(Id) FROM ForeignApplication WHERE Id=@Id AND PersonId=@PersonId";
                cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", appId }, { "@PersonId", personId } });
                if (cnt == 0)
                    return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Access error"), "text/plain");
            }

            //using (System.IO.FileStream fs = new System.IO.FileStream(Util.FilesPath + "Templates/Application_2011.pdf", System.IO.FileMode.Open))
            //{
            //    byte[] bindata = new byte[fs.Length];
            //    fs.Read(bindata, 0, bindata.Length);
            //    return new FileContentResult(bindata, "application/octet-stream") { FileDownloadName = "Application.pdf" };
            //}

            byte[] bindata = PDFUtils.GetApplicationPDFForeign(appId, Server.MapPath("~/Templates/"));
            return new FileContentResult(bindata, "application/pdf") { FileDownloadName = "Application.pdf" };
        }

        #region Ajax

        public JsonResult GetFaculties(string sfid, string entid)
        {
            int iStudyFormId;
            if (!int.TryParse(sfid, out iStudyFormId))
                iStudyFormId = 1;//Full-Time

            int iEntryId;
            if (!int.TryParse(entid, out iEntryId))
                iEntryId = 1;//bak

            string query = "SELECT DISTINCT SP_Entry.FacultyId, SP_Faculty.Name FROM SP_Entry INNER JOIN SP_Faculty ON SP_Faculty.Id=SP_Entry.Id " +
                " WHERE SP_Entry.StudyFormId=@StudyFormId";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@StudyFormId", iStudyFormId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var facs =
                from DataRow rw in tbl.Rows
                select new
                {
                    Id = rw.Field<int>("FacultyId"),
                    Name = rw.Field<string>("Name")
                };

            return Json(facs);
        }
        public JsonResult GetProfessions(string sfid, string facid, string entid)
        {
            int iStudyFormId;
            int iEntryId;
            int iFacId;

            int.TryParse(sfid, out iStudyFormId);
            int.TryParse(entid, out iEntryId);
            int.TryParse(facid, out iFacId);

            int StudyLevelId;
            if (iEntryId == 1)//bak
                StudyLevelId = 16;
            else if (iEntryId == 2)//mag
                StudyLevelId = 17;
            else if (iEntryId == 3)//spec
                StudyLevelId = 18;
            else//coolhackers must suck
                StudyLevelId = 0;

            string query = "SELECT DISTINCT SP_Entry.LicenseProgramId, " +
                " (CASE WHEN (SP_LicenseProgram.NameEng IS NULL THEN SP_LicenseProgram.Name ELSE SP_LicenseProgram.NameEng END) AS Name " +
                " FROM SP_Entry INNER JOIN SP_LicenseProgram ON SP_LicenseProgram.Id=SP_Entry.LicenseProgramId " +
                " WHERE SP_Entry.FacultyId=@FacId AND SP_Entry.StudyFormId=@SFormId AND SP_LicenseProgram.StudyLevelId=@SLevelId ";

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@FacId", iFacId);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@SLevelId", StudyLevelId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var profs =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<int>("LicenseProgramId"),
                     Name = rw.Field<string>("Name")
                 });

            return Json(profs);
        }
        public JsonResult GetObrazPrograms(string sfid, string facid, string profid, string entid)
        {
            int iStudyFormId;
            int iEntryId;
            int iFacId;
            int iProfId;

            //TryParse = 0 if fail
            int.TryParse(sfid, out iStudyFormId);
            int.TryParse(entid, out iEntryId);
            int.TryParse(facid, out iFacId);
            int.TryParse(profid, out iProfId);

            int StudyLevelId;
            if (iEntryId == 1)//bak
                StudyLevelId = 16;
            else if (iEntryId == 2)//mag
                StudyLevelId = 17;
            else if (iEntryId == 3)//spec
                StudyLevelId = 18;
            else//coolhackers must suck
                StudyLevelId = 0;

            string query = "SELECT DISTINCT SP_Entry.ObrazProgramId, " +
                " (CASE WHEN (SP_ObrazProgram.NameEng IS NULL THEN SP_ObrazProgram.Name ELSE SP_ObrazProgram.NameEng END) AS Name " +
                " FROM SP_Entry INNER JOIN SP_ObrazProgram ON SP_ObrazProgram.Id=SP_Entry.ObrazProgramId " +
                " INNER JOIN SP_LicenseProgram ON SP_LicenseProgram.Id=SP_Entry.LicenseProgramId " +
                " WHERE SP_Entry.FacultyId=@FacId AND SP_Entry.StudyFormId=@SFormId AND SP_LicenseProgram.StudyLevelId=@SLevelId " +
                " AND SP_Entry.LicenseProgramId=@LProgramId";

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@FacId", iFacId);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@LProgramId", iProfId);
            dic.Add("@SLevelId", StudyLevelId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var ops =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<int>("ObrazProgramId"),
                     Name = rw.Field<string>("Name")
                 });

            return Json(ops);
        }
        public JsonResult GetProfiles(string sfid, string facid, string profid, string obrazprogram, string entid)
        {
            Guid PersonId;
            Util.CheckAuthCookies(Request.Cookies, out PersonId);

            int iStudyFormId;
            int iEntryId;
            int iFacId;
            int iProfId;
            int iObrazProgramId;

            //TryParse = 0 if fail
            int.TryParse(sfid, out iStudyFormId);
            int.TryParse(entid, out iEntryId);
            int.TryParse(facid, out iFacId);
            int.TryParse(profid, out iProfId);
            int.TryParse(obrazprogram, out iObrazProgramId);

            int StudyLevelId;
            if (iEntryId == 1)//bak
                StudyLevelId = 16;
            else if (iEntryId == 2)//mag
                StudyLevelId = 17;
            else if (iEntryId == 3)//spec
                StudyLevelId = 18;
            else//coolhackers must suck
                StudyLevelId = 0;

            string query = "SELECT SP_Entry.Id, SP_Entry.ProfileId, " +
                " (CASE WHEN (SP_Profile.NameEng IS NULL OR SP_Profile.NameEng = '') THEN SP_Profile.Name ELSE SP_Profile.NameEng END) AS Name " +
                " FROM SP_Entry INNER JOIN SP_Profile ON SP_Profile.Id=SP_Entry.ProfileId " +
                " INNER JOIN SP_LicenseProgram ON SP_LicenseProgram.Id=SP_Entry.LicenseProgramId " +
                " WHERE SP_Entry.FacultyId=@FacId AND SP_Entry.StudyFormId=@SFormId AND SP_LicenseProgram.StudyLevelId=@SLevelId " +
                " AND SP_Entry.LicenseProgramId=@LProgramId AND SP_Entry.ObrazProgramId=@OProgramId";

            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@FacId", iFacId);
            dic.Add("@SFormId", iStudyFormId);
            dic.Add("@LProgramId", iProfId);
            dic.Add("@OProgramId", iObrazProgramId);
            dic.Add("@SLevelId", StudyLevelId);

            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            var EntryAndProfiles =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Id = rw.Field<Guid>("Id"),
                     ProfileId = rw.Field<Guid>("ProfileId"),
                     Name = rw.Field<string>("Name")
                 });

            query = "SELECT EntryId FROM [ForeignApplication] WHERE PersonId=@PersonId";
            dic.Clear();
            dic.Add("@PersonId", PersonId);
            tbl = Util.AbitDB.GetDataTable(query, dic);

            var existEntry =
                (from DataRow rw in tbl.Rows
                 select rw.Field<Guid>("EntryId")
                 );

            if (EntryAndProfiles.Count() == 0)
                return Json(new { IsOk = true, HasProfiles = false });

            var notAviableProfiles = EntryAndProfiles.Select(x => x.Id).Distinct().Except(existEntry);
            if (notAviableProfiles.Count() == 0)
                return Json(new { IsOk = false, HasProfiles = true });

            var profiles =
                EntryAndProfiles.Where(x => notAviableProfiles.Contains(x.Id)).
                Select(x => new { Id = x.ProfileId, Name = x.Name }).
                Distinct();

            return Json(new { IsOk = true, HasProfiles = true, Profiles = profiles });
        }

        #endregion
    }
}
