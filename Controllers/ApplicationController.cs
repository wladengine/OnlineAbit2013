using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineAbit2013.Models;
using System.Data;
using System.Web.Routing;

namespace OnlineAbit2013.Controllers
{
    public class ApplicationController : Controller
    {
        
        // GET: /Application/

        public ActionResult Index(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return RedirectToAction("LogOn", "Account");

            Guid CommitId = new Guid();
            if (!Guid.TryParse(id, out CommitId))
                return RedirectToAction("Main", "AbiturientNew");
            bool isEng = Util.GetCurrentThreadLanguageIsEng(); 
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var tblAppsMain = 
                    (from App in context.Application
                    join Entry in context.Entry on App.EntryId equals Entry.Id
                    join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                    where App.CommitId == CommitId && App.IsCommited == true && App.Enabled == true
                    select new SimpleApplication()
                    {
                        Id = App.Id,
                        Profession = isEng? (String.IsNullOrEmpty(Entry.LicenseProgramNameEng)? Entry.LicenseProgramName : Entry.LicenseProgramNameEng): Entry.LicenseProgramName,
                        ObrazProgram = isEng ? (String.IsNullOrEmpty(Entry.ObrazProgramNameEng) ? Entry.ObrazProgramName : Entry.ObrazProgramNameEng) : Entry.ObrazProgramName,
                        Specialization = isEng ? (String.IsNullOrEmpty(Entry.ProfileNameEng) ? Entry.ProfileName : Entry.ProfileNameEng) : Entry.ProfileName,
                        StudyForm = isEng ? (String.IsNullOrEmpty(Entry.StudyFormNameEng) ? Entry.StudyFormName : Entry.StudyFormNameEng) : Entry.StudyFormName,
                        StudyBasis = /*isEng ? (String.IsNullOrEmpty(Entry.StudyBasisNameEng) ? Entry.StudyBasisName : Entry.StudyBasisNameEng) : */Entry.StudyBasisName, 
                        StudyLevel = isEng ? (String.IsNullOrEmpty(Entry.StudyLevelName) ? Entry.StudyLevelName : Entry.StudyLevelName) : Entry.StudyLevelName, 
                        Priority = App.Priority,
                        IsGosLine = App.IsGosLine,
                        HasManualExams = false,
                        dateofClose = Entry.DateOfClose,
                        Enabled = App.Enabled,
                        SemesterName = (Entry.SemesterId != 1) ? Semester.Name : "",
                        StudyLevelGroupName = (isEng ? ((String.IsNullOrEmpty(Entry.StudyLevelGroupNameEng)) ? Entry.StudyLevelGroupNameRus : Entry.StudyLevelGroupNameEng) : Entry.StudyLevelGroupNameRus) +
                                    (App.SecondTypeId.HasValue ?
                                        ((App.SecondTypeId == 3) ? (isEng ? " (recovery)" : " (восстановление)") :
                                        ((App.SecondTypeId == 2) ? (isEng ? " (transfer)" : " (перевод)") :
                                        ((App.SecondTypeId == 4) ? (isEng ? " (changing form of education)" : " (смена формы обучения)") :
                                        ((App.SecondTypeId == 5) ? (isEng ? " (changing basis of education)" : " (смена основы обучения)") :
                                        ((App.SecondTypeId == 6) ? (isEng ? " (changing educational program)" : " (смена образовательной программы)") :
                                        ""))))) : "")
                    }).ToList().Union(
                    context.AG_Application.Where(x => x.CommitId == CommitId && x.IsCommited == true && x.Enabled == true).Select(x => new SimpleApplication()
                    {
                        Id = x.Id,
                        Profession = x.AG_Entry.AG_Program.Name,
                        ObrazProgram = x.AG_Entry.AG_EntryClass.Name,
                        Specialization = x.AG_Entry.AG_Profile.Name,
                        StudyForm = Resources.Common.StudyFormFullTime,
                        StudyBasis = Resources.Common.StudyBasisBudget,
                        StudyLevel = Resources.Common.AG,
                        Priority = x.Priority,
                        HasManualExams = x.ManualExamId.HasValue,
                        ManualExam = x.ManualExamId.HasValue ? x.AG_ManualExam.Name : "",
                        Enabled = x.Enabled,
                        StudyLevelGroupName = Resources.Common.AG,
                    }).ToList()).ToList();

                //if (tblAppsMain.Count() == 0)
                //    return RedirectToAction("Main", "AbiturientNew");

                string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM ApplicationFile WHERE CommitId=@CommitId";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@CommitId", CommitId } });

                List<AppendedFile> lFiles =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         IsShared = false,
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                                 (rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected) : ApprovalStatus.NotSet
                     }).ToList();

                query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", personId } });
                var lSharedFiles =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         IsShared = true,
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                                 (rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected) : ApprovalStatus.NotSet
                     }).ToList();

                var AllFiles = lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();

                bool bIsPrinted = context.ApplicationCommit.Where(x => x.Id == CommitId).Select(x => x.IsPrinted).DefaultIfEmpty(false).First();

                ExtApplicationCommitModel model = new ExtApplicationCommitModel()
                {
                    Id = CommitId,
                    Applications = tblAppsMain,
                    Files = AllFiles,
                    IsPrinted = bIsPrinted,
                    Enabled = true
                    //StudyLevelId = tblAppsMain.First().StudyLevel,
                };
                if (model.IsPrinted)
                    foreach (SimpleApplication s in tblAppsMain)
                    {
                        if (s.dateofClose != null)
                            if (s.dateofClose < DateTime.Now)
                            {
                                model.Enabled = false;
                                break;
                            }
                    }
                var AppVers = context.ApplicationCommitVersion.Where(x => x.CommitId == CommitId).Select(x => x.VersionDate).FirstOrDefault();
                if (AppVers == null)
                {
                    model.HasVersion = false;
                }
                else
                {
                    model.HasVersion = true;
                    model.VersionDate = AppVers.ToString();
                }
                return View(model);
            }
        }

        public ActionResult AppIndex(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return RedirectToAction("LogOn", "Account");

            Guid ApplicationId = new Guid();
            if (!Guid.TryParse(id, out ApplicationId))
                return RedirectToAction("Main", "Abiturient");
            bool isEng = Util.GetCurrentThreadLanguageIsEng(); 
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var ApplicationEntity = 
                    (from App in context.Application 
                     join Entry in context.Entry on App.EntryId equals Entry.Id
                     join Commission in context.Comission on Entry.ComissionId equals Commission.Id into Commission2
                     from Commission in Commission2.DefaultIfEmpty()
                     where App.Id == ApplicationId && App.IsCommited == true
                     select new 
                {
                    Id = App.Id,
                    Profession = isEng ? (String.IsNullOrEmpty(Entry.LicenseProgramNameEng) ? Entry.LicenseProgramName : Entry.LicenseProgramNameEng) : Entry.LicenseProgramName,
                    ObrazProgram = isEng ? (String.IsNullOrEmpty(Entry.ObrazProgramNameEng) ? Entry.ObrazProgramName : Entry.ObrazProgramNameEng) : Entry.ObrazProgramName,
                    Specialization = isEng ? (String.IsNullOrEmpty(Entry.ProfileNameEng) ? Entry.ProfileName : Entry.ProfileNameEng) : Entry.ProfileName,
                    StudyForm = isEng ? (String.IsNullOrEmpty(Entry.StudyFormNameEng) ? Entry.StudyFormName : Entry.StudyFormNameEng) : Entry.StudyFormName,
                    StudyBasis = /*isEng ? (String.IsNullOrEmpty(Entry.StudyBasisNameEng) ? Entry.StudyBasisName : Entry.StudyBasisNameEng) :*/ Entry.StudyBasisName,
                    StudyLevel = /*isEng ? (String.IsNullOrEmpty(Entry.StudyLevelNameEng) ? Entry.StudyLevelName : Entry.StudyLevelNameEng) : */Entry.StudyLevelName,
                    StudyLevelId = Entry.StudyLevelId,
                    Priority = App.Priority,
                    Enabled = App.Enabled,
                    ComissionAddress = Commission.Address,
                    ComissionYaCoord = Commission.YaMapCoord,
                    DateOfDisable = App.DateOfDisable,
                    IsApproved = App.IsApprovedByComission,
                    EntryTypeId = App.EntryType,
                    CommitId = App.CommitId,
                    CommitName = /*isEng ? (String.IsNullOrEmpty(Entry.StudyLevelNameEng) ? Entry.StudyLevelName : Entry.StudyLevelNameEng) :*/Entry.StudyLevelName
                }).FirstOrDefault();
                if (ApplicationEntity == null)
                {
                    ApplicationEntity = context.AG_Application.Where(x => x.Id == ApplicationId && x.IsCommited == true && x.CommitId.HasValue).Select(x => new 
                    {
                        Id = x.Id,
                        Profession = x.AG_Entry.AG_Program.Name,
                        ObrazProgram = x.AG_Entry.AG_EntryClass.Name,
                        Specialization = x.AG_Entry.AG_Profile.Name,
                        StudyForm = Resources.Common.StudyFormFullTime,
                        StudyBasis = Resources.Common.StudyBasisBudget,
                        StudyLevel = Resources.Common.AG,
                        StudyLevelId = 1,
                        Priority = x.Priority,
                        Enabled = x.Enabled,
                        ComissionAddress = "",
                        ComissionYaCoord = "",
                        DateOfDisable = x.DateOfDisable,
                        IsApproved = x.IsApprovedByComission,
                        EntryTypeId = 0,
                        CommitId = x.CommitId.Value,
                        CommitName = Resources.Common.AG
                    }).FirstOrDefault();
                }
                //var lFiles =
                //    context.ApplicationFile.Where(x => x.ApplicationId == ApplicationId).Select
                //    (
                //        x => new AppendedFile()
                //        {
                //            Id = x.Id,
                //            FileName = x.FileName,
                //            FileSize = x.FileSize,
                //            Comment = x.Comment,
                //            IsShared = false,
                //            IsApproved = x.IsApproved.HasValue ?
                //                x.IsApproved.Value ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet,
                //        }
                //    ).ToList();

                //var lSharedFiles =
                //    context.PersonFile.Where(x => x.PersonId == personId).Select
                //    (
                //        x => new AppendedFile()
                //        {
                //            Id = x.Id,
                //            FileName = x.FileName,
                //            FileSize = x.FileSize,
                //            Comment = x.Comment,
                //            IsShared = true,
                //            IsApproved = x.IsApproved.HasValue ?
                //                x.IsApproved.Value ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet
                //        }
                //    ).ToList();

                //var AllFiles = lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();

                string query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM ApplicationFile WHERE ApplicationId=@ApplicationId";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@ApplicationId", ApplicationId } });

                List<AppendedFile> lFiles =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         IsShared = false,
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                                 (rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected) : ApprovalStatus.NotSet
                     }).ToList();

                query = "SELECT Id, FileName, FileSize, Comment, IsApproved FROM PersonFile WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", personId } });
                var lSharedFiles =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         IsShared = true,
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                                 (rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected) : ApprovalStatus.NotSet
                     }).ToList();

                var AllFiles = lFiles.Union(lSharedFiles).OrderBy(x => x.IsShared).ToList();

                AbitType abt = AbitType.AG;
                switch (ApplicationEntity.StudyLevelId)
                {
                    case 1: abt = AbitType.AG; break;
                    case 8: abt = AbitType.SPO; break;
                    case 10: abt = AbitType.SPO; break;
                    case 16: abt = AbitType.FirstCourseBakSpec; break;
                    case 17: abt = AbitType.Mag; break;
                    case 18: abt = AbitType.FirstCourseBakSpec; break;
                    default: abt = AbitType.FirstCourseBakSpec; break;
                }
                int? c = (int?) Util.AbitDB.GetValue("SELECT top 1 SecondTypeId FROM Application WHERE Id=@Id AND PersonId=@PersonId", new SortedList<string, object>() { { "@PersonId", personId }, { "@Id", ApplicationId } }) ;
                if (c !=1 ){
                        abt = AbitType.FirstCourseBakSpec; 
                }
                ExtApplicationModel model = new ExtApplicationModel()
                {
                    Id = ApplicationId,
                    CommitId = ApplicationEntity.CommitId,
                    CommitName = ApplicationEntity.CommitName,
                    Files = AllFiles,
                    ComissionAddress = ApplicationEntity.ComissionAddress,
                    ComissionYaCoord = ApplicationEntity.ComissionYaCoord,
                    DateOfDisable = ApplicationEntity.DateOfDisable.HasValue ? ApplicationEntity.DateOfDisable.Value.ToString() : "",
                    Enabled = ApplicationEntity.Enabled,
                    EntryTypeId = ApplicationEntity.EntryTypeId,
                    AbiturientType = abt,
                    IsApproved = ApplicationEntity.IsApproved,
                    ObrazProgram = ApplicationEntity.ObrazProgram,
                    Priority = ApplicationEntity.Priority.ToString(),
                    Profession = ApplicationEntity.Profession,
                    Specialization = ApplicationEntity.Specialization,
                    StudyBasis = ApplicationEntity.StudyBasis,
                    StudyForm = ApplicationEntity.StudyForm,
                    UILanguage = Util.GetCurrentThreadLanguage()
                };

                return View(model);
            }
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
                return RedirectToAction("Main", "Abiturient");

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
                string query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType, [FileTypeId]) " +
                    " VALUES (@Id, @ApplicationId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType, 1)";
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

            return RedirectToAction("AppIndex", new RouteValueDictionary() { { "id", id } });
        }

        [HttpPost]
        public ActionResult AddFileInCommit()
        {
            string id = Request.Form["id"];
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid CommitId = new Guid();
            if (!Guid.TryParse(id, out CommitId))
                return RedirectToAction("Main", "Abiturient");

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
                string query = "INSERT INTO ApplicationFile (Id, CommitId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType, [FileTypeId]) " +
                    " VALUES (@Id, @CommitId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType, 1)";
                SortedList<string, object> dic = new SortedList<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@CommitId", CommitId);
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

        public ActionResult GetFile(string id)
        {
            Guid FileId = new Guid();
            if (!Guid.TryParse(id, out FileId))
                return Content("Некорректный идентификатор файла");

            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Content("Authorization required");

            DataTable tbl = Util.AbitDB.GetDataTable("SELECT FileName, FileData, MimeType, FileExtention FROM AllFiles WHERE Id=@Id",
                new SortedList<string, object>() { { "@Id", FileId } });

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

        public ActionResult GetPrint(string id)
        {
            Guid personId;
            if (!Util.CheckAuthCookies(Request.Cookies, out personId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Authentification Error"), "text/plain");

            Guid appId;
            if (!Guid.TryParse(id, out appId))
                return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Ошибка идентификатора заявления"), "text/plain");

            //string query = "SELECT COUNT(Id) FROM Application WHERE CommitId=@Id AND PersonId=@PersonId";
            //int cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", appId }, { "@PersonId", personId } });
            //if (cnt == 0)
            //{
            //    query = "SELECT COUNT(Id) FROM AG_Application WHERE CommitId=@Id AND PersonId=@PersonId";
            //    cnt = (int)Util.AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", appId }, { "@PersonId", personId } });
            //    if (cnt == 0)
            //        return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Access error"), "text/plain");
            //}

            byte[] bindata; 

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                if (context.AG_Application.Where(x => x.CommitId == appId && x.PersonId == personId).Count() > 0)
                {
                    bindata = PDFUtils.GetApplicationBlockPDF_AG(appId, Server.MapPath("~/Templates/"));
                }
                else
                {
                    var lst = context.Application.Where(x => x.CommitId == appId && x.PersonId == personId).Select(x => x.EntryType).ToList();
                    if (lst.Count == 0)
                        return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Access error"), "text/plain");
                    int? CountryEducId = context.PersonEducationDocument.Where(x => x.PersonId == personId).Select(x => x.CountryEducId).FirstOrDefault();
                    int? Secondlst = context.Application.Where(x => x.CommitId == appId && x.PersonId == personId).Select(x => x.SecondTypeId).FirstOrDefault();
                    /*if (Secondlst.Count == 0)
                        return new FileContentResult(System.Text.Encoding.ASCII.GetBytes("Access error"), "text/plain");*/

                    //дальше должно быть разделение - для 1 курса, магистров, аспирантов, переводящихся и восстанавливающихся
                    //пока что затычка из АГ
                    int EntryType = lst.First();
                     
                    switch (EntryType)
                    {
                        //бакалавриат
                        case 1: { bindata = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"), false, personId); break; }
                        //магистратура
                        case 2: { bindata = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"), true, personId); break; }
                        //СПО
                        case 3: { bindata = PDFUtils.GetApplicationPDF_SPO(appId, Server.MapPath("~/Templates/"), personId); break; }
                        //Аспирантура
                        case 4: { bindata = PDFUtils.GetApplicationPDF_Aspirant(appId, Server.MapPath("~/Templates/"), personId); break; }

                        //case 5: { bindata = PDFUtils.GetApplicationPDFRecover(appId, Server.MapPath("~/Templates/")); break; }
                        //case 6: { bindata = PDFUtils.GetApplicationPDFChangeStudyForm(appId, Server.MapPath("~/Templates/")); break; }
                        //case 7: { bindata = PDFUtils.GetApplicationPDFChangeObrazProgram(appId, Server.MapPath("~/Templates/")); break; }
                        //case 8: { bindata = PDFUtils.GetApplicationPDF_AG(appId, Server.MapPath("~/Templates/")); break; }
                        //case 9: { bindata = PDFUtils.GetApplicationPDF_SPO(appId, Server.MapPath("~/Templates/")); break; }
                        //case 10: { bindata = PDFUtils.GetApplicationPDF_Aspirant(appId, Server.MapPath("~/Templates/")); break; }
                        //case 11: { bindata = PDFUtils.GetApplicationPDF_Aspirant(appId, Server.MapPath("~/Templates/")); break; }
                        default: { bindata = PDFUtils.GetApplicationPDF(appId, Server.MapPath("~/Templates/"), false, personId); break; }
                    }
                    if (Secondlst.HasValue)
                    {
                        // восстановление
                        if (Secondlst == 3) { bindata = PDFUtils.GetApplicationPDFRecover(appId, Server.MapPath("~/Templates/"), false, personId); }
                            // перевод
                        else if (Secondlst == 2)
                        {
                            if (CountryEducId.HasValue)
                            {
                                if (CountryEducId == 193)
                                    bindata = PDFUtils.GetApplicationPDFTransfer(appId, Server.MapPath("~/Templates/"), false, personId);  
                                else
                                    bindata = PDFUtils.GetApplicationPDFTransferForeign(appId, Server.MapPath("~/Templates/"), false, personId);  
                            }
                            else
                                bindata = PDFUtils.GetApplicationPDFTransfer(appId, Server.MapPath("~/Templates/"), false, personId);  
                        } 
                             // смена формы
                        else if (Secondlst == 4) { bindata = PDFUtils.GetApplicationPDFChangeStudyBasis(appId, Server.MapPath("~/Templates/"), false, personId); }
                            // смена основы
                        else if (Secondlst == 5) { bindata = PDFUtils.GetApplicationPDFChangeStudyBasis(appId, Server.MapPath("~/Templates/"), false, personId); }
                            // смена образовательной программы
                        else if (Secondlst == 6) { bindata = PDFUtils.GetApplicationPDFChangeObrazProgram(appId, Server.MapPath("~/Templates/"), false, personId); }
                    } 
                }
            }
            
            return new FileContentResult(bindata, "application/pdf") { FileDownloadName = "Application.pdf" };
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

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                bool isAg = false;
                bool? isEnabled = context.Application.Where(x => x.Id == AppId && x.PersonId == PersonId).Select(x => (bool?)x.Enabled).FirstOrDefault();
                //(bool?)Util.AbitDB.GetValue("SELECT Enabled FROM [Application] WHERE Id=@Id AND PersonId=@PersonId",
                //new SortedList<string, object>() { { "@Id", AppId }, { "@PersonId", PersonId } });

                //var app = Util.ABDB.Application.Where(x => x.Id == AppId && x.PersonId == PersonId).FirstOrDefault();
                if (!isEnabled.HasValue)
                {
                    isEnabled = context.AG_Application.Where(x => x.Id == AppId && x.PersonId == PersonId).Select(x => (bool?)x.Enabled).FirstOrDefault();
                    if (!isEnabled.HasValue)
                    {
                        var res = new { IsOk = false, ErrorMessage = "Ошибка при поиске заявления. Попробуйте обновить страницу" };
                        return Json(res);
                    }
                    else
                        isAg = true;
                }

                if (isEnabled.HasValue && isEnabled.Value == false)
                {
                    var res = new { IsOk = false, ErrorMessage = "Заявление уже было отозвано" };
                    return Json(res);
                }

                try
                {
                    string query = string.Format("UPDATE [{0}Application] SET Enabled=@Enabled, DateOfDisable=@DateOfDisable, Priority=@Priority WHERE Id=@Id", isAg ? "AG_" : "");
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
        }

        public ActionResult DisableFull(string id)
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
            Guid CommitId;
            if (!Guid.TryParse(id, out CommitId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
                return Json(res);
            }

            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                bool isAg = false;
                bool? isEnabled = context.Application.Where(x => x.CommitId == CommitId && x.PersonId == PersonId).Select(x => (bool?)x.Enabled).FirstOrDefault();

                if (!isEnabled.HasValue)
                {
                    isEnabled = context.AG_Application.Where(x => x.CommitId == CommitId && x.PersonId == PersonId).Select(x => (bool?)x.Enabled).FirstOrDefault();
                    if (!isEnabled.HasValue)
                    {
                        var res = new { IsOk = false, ErrorMessage = "Ошибка при поиске заявления. Попробуйте обновить страницу" };
                        return Json(res);
                    }
                    else
                        isAg = true;
                }

                if (isEnabled.HasValue && isEnabled.Value == false)
                {
                    var res = new { IsOk = false, ErrorMessage = "Заявление уже было отозвано" };
                    return Json(res);
                }

                try
                {
                    bool? result = null;
                    if (!isAg)
                    {
                        result = PDFUtils.GetDisableApplicationPDF(CommitId, Server.MapPath("~/Templates/"), PersonId);
                    }
                    string query = string.Format("DELETE FROM [{0}Application] WHERE CommitId=@Id", isAg ? "AG_" : "");
                    SortedList<string, object> dic = new SortedList<string, object>();
                    dic.Add("@Id", CommitId);

                    Util.AbitDB.ExecuteQuery(query, dic);

                    var res = new { IsOk = true, Enabled = false};
                    return Json(res);
                }
                catch
                {
                    var res = new { IsOk = false, ErrorMessage = "Ошибка при поиске заявления. Попробуйте обновить страницу" };
                    return Json(res);
                }
            }
        }

        public ActionResult MotivatePost()
        {
            string id = Request.Form["id"];
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid ApplicationId = new Guid();
            if (!Guid.TryParse(id, out ApplicationId))
                return RedirectToAction("Main", "Abiturient");

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
                string query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType, [FileTypeId]) " +
                    " VALUES (@Id, @ApplicationId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType, 2)";
                SortedList<string, object> dic = new SortedList<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@ApplicationId", ApplicationId);
                dic.Add("@FileName", fileName);
                dic.Add("@FileData", fileData);
                dic.Add("@FileSize", fileSize);
                dic.Add("@FileExtention", fileext);
                dic.Add("@IsReadOnly", false);
                dic.Add("@LoadDate", DateTime.Now);
                dic.Add("@Comment", "Мотивационное письмо");
                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }

            return RedirectToAction("AppIndex", new RouteValueDictionary() { { "id", id } });
        }

        public ActionResult EssayPost()
        {
            string id = Request.Form["id"];
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            Guid ApplicationId = new Guid();
            if (!Guid.TryParse(id, out ApplicationId))
                return RedirectToAction("Main", "Abiturient");

            if (Request.Files["File"] == null || Request.Files["File"].ContentLength == 0 || string.IsNullOrEmpty(Request.Files["File"].FileName))
                return Json(Resources.ServerMessages.EmptyFileError);

            string fileName = Request.Files["File"].FileName;
            if (fileName.IndexOf('\\') > 0 && fileName.LastIndexOf('\\') < fileName.Length)
                fileName = fileName.Substring(fileName.LastIndexOf('\\') + 1);

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
                string query = "INSERT INTO ApplicationFile (Id, ApplicationId, FileName, FileData, FileSize, FileExtention, IsReadOnly, LoadDate, Comment, MimeType, [FileTypeId]) " +
                    " VALUES (@Id, @ApplicationId, @FileName, @FileData, @FileSize, @FileExtention, @IsReadOnly, @LoadDate, @Comment, @MimeType, 3)";
                SortedList<string, object> dic = new SortedList<string, object>();
                dic.Add("@Id", Guid.NewGuid());
                dic.Add("@ApplicationId", ApplicationId);
                dic.Add("@FileName", fileName);
                dic.Add("@FileData", fileData);
                dic.Add("@FileSize", fileSize);
                dic.Add("@FileExtention", fileext);
                dic.Add("@IsReadOnly", false);
                dic.Add("@LoadDate", DateTime.Now);
                dic.Add("@Comment", "Эссе");
                dic.Add("@MimeType", Util.GetMimeFromExtention(fileext));

                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch
            {
                return Json("Ошибка при записи файла");
            }

            return RedirectToAction("AppIndex", new RouteValueDictionary() { { "id", id } });
        }

        public JsonResult ChangePriority(string id, string pr)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, ErrorMessage = "Ошибка авторизации" });

            Guid ApplicationId;
            if (!Guid.TryParse(id, out ApplicationId))
                return Json(new { IsOk = false, ErrorMessage = "" });

            try
            {
                Util.AbitDB.ExecuteQuery("UPDATE Application SET Priority=@Priority WHERE Id=@Id",
                    new SortedList<string, object>() { { "@Id", ApplicationId }, { "@Priority", pr } });
                return Json(new { IsOk = true });
            }
            catch
            {
                return Json(new { IsOk = false, ErrorMessage = "Ошибка при обновлении данных" });
            }
        }

        [HttpPost]
        public ActionResult DeleteFile(string id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
            {
                var res = new { IsOk = false, ErrorMessage = "Authorization required" };
                return Json(res);
            }

            string uiLang = Util.GetUILang(PersonId);

            Guid fileId;
            if (!Guid.TryParse(id, out fileId))
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.IncorrectGUID };
                return Json(res);
            }
            string attr = Util.AbitDB.GetStringValue("SELECT IsReadOnly FROM ApplicationFile WHERE Id=@Id", new SortedList<string, object>() { { "@Id", fileId } });
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
                Util.AbitDB.ExecuteQuery("DELETE FROM ApplicationFile WHERE Id=@Id", new SortedList<string, object>() { { "@Id", fileId } });
            }
            catch
            {
                var res = new { IsOk = false, ErrorMessage = Resources.ServerMessages.ErrorWhileDeleting };
                return Json(res);
            }

            var result = new { IsOk = true, ErrorMessage = "" };
            return Json(result);
        }
    }
}
