using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Data;

using iTextSharp.text;
using iTextSharp.text.pdf;
using OnlineAbit2013.Models;

namespace OnlineAbit2013.Controllers
{
    public class ShortAppcationDetails
    {
        public Guid ApplicationId { get; set; }
        public int? CurrVersion { get; set; }
        public DateTime? CurrDate { get; set; }
        public int ObrazProgramInEntryPriority { get; set; }
        public string ObrazProgramName { get; set; }
        public int? ProfileInObrazProgramInEntryPriority { get; set; }
        public string ProfileName { get; set; }
    }
    public class ShortAppcation
    {
        public Guid ApplicationId { get; set; }
        public int Priority { get; set; }
        public string LicenseProgramName { get; set; }
        public string ObrazProgramName { get; set; }
        public string ProfileName { get; set; }

        public bool HasInnerPriorities { get; set; }
        public int InnerPrioritiesNum { get; set; }

        public int StudyFormId { get; set; }
        public int StudyBasisId { get; set; }
    }

    public static class PDFUtils
    {
        /// <summary>
        /// PDF Список файлов
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="fontPath"></param>
        /// <returns></returns>
        public static byte[] GetFilesList(Guid personId, Guid applicationId, string fontPath)
        {
            MemoryStream ms = new MemoryStream();

            string query = "SELECT Surname, Name, SecondName FROM Person WHERE Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", personId } });

            if (tbl.Rows.Count == 0)
                return new byte[1] { 0x00 };

            var person =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName")
                 }).FirstOrDefault();

            string FIO = person.Surname + " " + person.Name + " " + person.SecondName;
            FIO = FIO.Trim();

            using (Document doc = new Document())
            {
                BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                iTextSharp.text.Font font12 = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                iTextSharp.text.Font font16 = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.NORMAL);

                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();

                Paragraph p = new Paragraph("ПРИЕМНАЯ КОМИССИЯ СПБГУ", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                p = new Paragraph("Опись \n поданных документов", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                p = new Paragraph(FIO + "\n\n", font16);
                p.Alignment = Element.ALIGN_CENTER;
                doc.Add(p);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 10f, 40f, 15f, 15f, 20f });
                //table.SetWidthPercentage(new float[] { 10f, 40f, 15f, 15f, 20f }, doc.PageSize);

                PdfPCell cell = new PdfPCell(new Phrase("№ п/п", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Наименование документа (имя файла)", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Копия / подлинник", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Дата подачи (загрузки)", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Подпись сотрудника ПК, принявшего документ при личной подаче", font12));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                int cnt = 0;
                query = "SELECT FileName, Comment, LoadDate FROM PersonFile WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", personId } });
                var PersonFile =
                    from DataRow rw in tbl.Rows
                    select new
                    {
                        Comment = rw.Field<string>("Comment"),
                        FileName = rw.Field<string>("FileName"),
                        LoadDate = rw.Field<DateTime?>("LoadDate").HasValue ? rw.Field<DateTime>("LoadDate").ToShortDateString() : "нет"
                    };
                query = "SELECT FileName, Comment, LoadDate FROM ApplicationFile WHERE ApplicationId=@AppId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@AppId", applicationId } });
                var ApplicationFile =
                    from DataRow rw in tbl.Rows
                    select new
                    {
                        Comment = rw.Field<string>("Comment"),
                        FileName = rw.Field<string>("FileName"),
                        LoadDate = rw.Field<DateTime?>("LoadDate").HasValue ? rw.Field<DateTime>("LoadDate").ToShortDateString() : "нет"
                    };

                var AllFiles = ApplicationFile.Union(PersonFile);

                foreach (var file in AllFiles)
                {
                    ++cnt;
                    cell = new PdfPCell(new Phrase(cnt.ToString(), font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    table.AddCell(new Phrase(string.Format("{0} ({1})", file.Comment, file.FileName), font12));

                    cell = new PdfPCell(new Phrase("Копия", font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(file.LoadDate, font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    table.AddCell("");
                }

                for (int j = 0; j < 3; j++)
                {
                    ++cnt;
                    cell = new PdfPCell(new Phrase(cnt.ToString(), font12));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                    for (int z = 0; z < 4; z++)
                    {
                        cell = new PdfPCell(new Phrase("", font12));
                        table.AddCell(cell);
                    }
                }
                doc.Add(table);


                p = new Paragraph("Создано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), font12);
                p.Alignment = Element.ALIGN_RIGHT;
                doc.Add(p);

                doc.Close();
            }

            return ms.ToArray();
        }

        //1курс-магистратура ОСНОВНОЙ (AbitTypeId = 1)
        public static byte[] GetApplicationPDF(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address
                                }).OrderBy(x => x.Priority).ToList();

                var abitProfileList = (from x in context.Application
                                       join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                       where x.CommitId == appId
                                       select new ShortAppcationDetails()
                                       {
                                           ApplicationId = x.Id,
                                           CurrVersion = Ad.CurrVersion,
                                           CurrDate = Ad.CurrDate,
                                           ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                           ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                           ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                           ProfileName = Ad.ProfileName
                                       }).ToList();

                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.SchoolName,
                                  x.PersonEducationDocument.SchoolNum,
                                  AddInfo = x.PersonAddInfo.AddInfo,
                                  Parents = x.PersonAddInfo.Parents,
                                  x.PersonEducationDocument.StartEnglish,
                                  x.PersonEducationDocument.EnglishMark,
                                  x.PersonEducationDocument.IsEqual,
                                  x.PersonEducationDocument.EqualDocumentNumber,
                                  CountryEduc = x.PersonEducationDocument.CountryEduc != null ? x.PersonEducationDocument.CountryEduc.Name : "",
                                  x.PersonEducationDocument.CountryEducId,
                                  Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                  x.PersonEducationDocument.SchoolTypeId,
                                  EducationDocumentSeries = x.PersonEducationDocument.Series,
                                  EducationDocumentNumber = x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.AttestatRegion,
                                  x.PersonEducationDocument.AttestatSeries,
                                  x.PersonEducationDocument.AttestatNumber,
                                  Language = x.PersonEducationDocument.Language.Name,
                                  HasPrivileges = x.PersonAddInfo.HasPrivileges ?? false,
                                  x.PersonEducationDocument.HasTRKI,
                                  x.PersonEducationDocument.TRKICertificateNumber,
                                  x.PersonAddInfo.ReturnDocumentTypeId,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.HasRussianNationality
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName;

                if (isMag)//mag
                    dotName = "ApplicationMag_page3.pdf";
                else
                    dotName = "Application_page3.pdf";

                byte[] templateBytes;

                List<byte[]> lstFiles = new List<byte[]>();
                List<byte[]> lstAppendixes = new List<byte[]>();
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                var Version = context.ApplicationCommitVersion.Where(x => x.CommitId == appId).Select(x => new { x.VersionDate, x.Id }).ToList().LastOrDefault();
                string sVersion = "";
                if (Version != null)
                    sVersion = "Версия №" + Version.Id + (Version.VersionDate.HasValue ? " от " + Version.VersionDate.Value.ToString("dd.MM.yyyy HH:mm") : "");
                string FIO = ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim();

                List<ShortAppcation> lstApps = abitList
                    .Select(x => new ShortAppcation()
                    {
                        ApplicationId = x.Id,
                        LicenseProgramName = x.ProfessionCode + " " + x.Profession,
                        ObrazProgramName = x.ObrazProgram,
                        ProfileName = x.Specialization,
                        Priority = x.Priority,
                        StudyBasisId = x.StudyBasisId,
                        StudyFormId = x.StudyFormId,
                        HasInnerPriorities = abitProfileList.Where(y => y.ApplicationId == x.Id).Count() > 0,
                    }).ToList();
                int incrmtr = 1;
                for (int u = 0; u < lstApps.Count; u++)
                {
                    if (lstApps[u].HasInnerPriorities) //если есть профили
                    {
                        lstApps[u].InnerPrioritiesNum = incrmtr; //то пишем об этом
                        //и сразу же создаём приложение с описанием - потом приложим

                        if (isMag) //для магов всё просто
                        {
                            lstAppendixes.Add(GetApplicationPDF_ProfileAppendix_Mag(abitProfileList.Where(x => x.ApplicationId == lstApps[u].ApplicationId).ToList(), lstApps[u].LicenseProgramName, FIO, dirPath, incrmtr));
                            incrmtr++;
                        }
                        else //для перваков всё запутаннее
                        {   //сначала надо проверить, нет ли внутреннего разбиения по программам
                            //если есть, то для каждой программы сделать своё приложение, а затем уже для тех программ, где есть внутри профили доложить приложений с профилями
                            var profs = abitProfileList.Where(x => x.ApplicationId == lstApps[u].ApplicationId).ToList();
                            var OP = profs.Select(x => x.ObrazProgramName).Distinct().ToList();
                            if (OP.Count > 1)
                            {
                                lstAppendixes.Add(GetApplicationPDF_OPAppendix_1kurs(profs, lstApps[u].LicenseProgramName, FIO, dirPath, incrmtr));
                                incrmtr++;
                            }
                            foreach (var OP_name in OP)
                            {
                                var lstProfs = abitProfileList.Where(x => x.ApplicationId == lstApps[u].ApplicationId && x.ObrazProgramName == OP_name).ToList();
                                if (lstProfs.Count > 1)
                                {
                                    lstAppendixes.Add(GetApplicationPDF_ProfileAppendix_1kurs(lstProfs, lstApps[u].LicenseProgramName, FIO, dirPath, incrmtr));
                                    incrmtr++;
                                }
                            }
                        }
                    }
                }

                List<ShortAppcation> lstAppsFirst = new List<ShortAppcation>();
                for (int u = 0; u < 3; u++)
                {
                    if (lstApps.Count > u)
                        lstAppsFirst.Add(lstApps[u]);
                }


                int multiplyer = isMag ? 2 : 1;
                string code = ((multiplyer * 100000) + abitList.First().CommitIntNumber).ToString();

                //добавляем первый файл
                lstFiles.Add(GetApplicationPDF_FirstPage(lstAppsFirst, lstApps, dirPath, isMag ? "ApplicationMag_page1.pdf" : "Application_page1.pdf", FIO, sVersion, code, isMag));
                acrFlds.SetField("Version", sVersion);

                //остальные - по 4 на новую страницу
                int appcount = 3;
                while (appcount < lstApps.Count)
                {
                    lstAppsFirst = new List<ShortAppcation>();
                    for (int u = 0; u < 4; u++)
                    {
                        if (lstApps.Count > appcount)
                            lstAppsFirst.Add(lstApps[appcount]);
                        else
                            break;
                        appcount++;
                    }

                    lstFiles.Add(GetApplicationPDF_NextPage(lstAppsFirst, lstApps, dirPath, "ApplicationMag_page2.pdf", FIO));
                }


                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (abitList.Where(x => x.IsGosLine).Count() > 0)
                    acrFlds.SetField("IsGosLine", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                if (person.BirthDate.HasValue)
                {
                    acrFlds.SetField("BirthDateYear", person.BirthDate.Value.Year.ToString("D2"));
                    acrFlds.SetField("BirthDateMonth", person.BirthDate.Value.Month.ToString("D2"));
                    acrFlds.SetField("BirthDateDay", person.BirthDate.Value.Day.ToString());
                }
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);

                //dd.MM.yyyy :12.05.2000
                string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);
                if (person.HasRussianNationality)
                    acrFlds.SetField("HasRussianNationalityYes", "1");
                else
                    acrFlds.SetField("HasRussianNationalityNo", "1");

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);

                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish ?? false)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Phone", person.Phone);
                acrFlds.SetField("Email", email);
                acrFlds.SetField("Mobiles", person.Mobiles);

                acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
                string SchoolNameNum = person.SchoolName;
                if (person.SchoolTypeId == 1)
                {
                    if (!String.IsNullOrEmpty(person.SchoolNum))
                    {
                        if (person.SchoolNum.StartsWith("№"))
                            SchoolNameNum += " (" + person.SchoolNum + ")";
                        else if (person.SchoolNum.StartsWith("#"))
                            SchoolNameNum += " (" + person.SchoolNum + ")";
                        else
                            SchoolNameNum += " (№" + person.SchoolNum + ")";
                    }
                }
                splitStr = GetSplittedStrings(SchoolNameNum ?? "", 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("School" + i, splitStr[i - 1]);

                //только у магистров
                acrFlds.SetField("HEProfession", person.ProgramName ?? "");
                acrFlds.SetField("Qualification", person.Qualification ?? "");

                acrFlds.SetField("Original", "0");
                acrFlds.SetField("Copy", "0");
                acrFlds.SetField("CountryEduc", person.CountryEduc ?? "");
                acrFlds.SetField("Language", person.Language ?? "");

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
                for (int i = 1; i <= 3; i++)
                {
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);
                    acrFlds.SetField("ExtraParents" + i.ToString(), splitStr[i - 1]);
                }

                string Attestat = person.SchoolTypeId == 1 ? ("аттестат серия " + (person.AttestatRegion + " " ?? "") + (person.AttestatSeries ?? "") + " №" + (person.AttestatNumber ?? "")) :
                        ("диплом серия " + (person.EducationDocumentSeries ?? "") + " №" + (person.EducationDocumentNumber ?? ""));
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                if ((person.IsEqual ?? false) && person.CountryEducId.HasValue && person.CountryEducId.Value != 193)
                {
                    acrFlds.SetField("IsEqual", "1");
                    acrFlds.SetField("EqualSertificateNumber", person.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }

                if (person.HasPrivileges)
                    acrFlds.SetField("HasPrivileges", "1");

                acrFlds.SetField("ReturnDocumentType" + person.ReturnDocumentTypeId, "1");
                // имею ли образование этого уровня:
                // если закончил школу, спо, нпо и прочее (кроме ВУЗа) + (если выбран ВУЗ + Квалификация 
                if ((person.SchoolTypeId != 4) || (isMag && person.SchoolTypeId == 4 && (person.Qualification).ToLower().IndexOf("магист") < 0))
                    acrFlds.SetField("NoEduc", "1");
                else
                {
                    acrFlds.SetField("HasEduc", "1");
                    acrFlds.SetField("HighEducation", person.SchoolName);
                }

                if (!isMag)
                {
                    //EGE
                    query = "SELECT ExamName, MarkValue, Number FROM EgeMarksAll WHERE PersonId=@PersonId";
                    tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
                    var exams =
                        from DataRow rw in tbl.Rows
                        select new
                        {
                            ExamName = rw.Field<string>("ExamName"),
                            MarkValue = rw.Field<int?>("MarkValue"),
                            Number = rw.Field<string>("Number")
                        };
                    int egeCnt = 1;
                    foreach (var ex in exams)
                    {
                        acrFlds.SetField("TableName" + egeCnt, ex.ExamName);
                        acrFlds.SetField("TableValue" + egeCnt, ex.MarkValue.ToString());
                        acrFlds.SetField("TableNumber" + egeCnt, ex.Number);

                        if (egeCnt == 4)
                            break;
                        egeCnt++;
                    }


                    //VSEROS
                    var OlympVseros = context.Olympiads.Where(x => x.PersonId == PersonId && x.OlympType.IsVseross)
                        .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                    egeCnt = 1;
                    foreach (var ex in OlympVseros)
                    {
                        acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                        acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                        acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                        if (egeCnt == 2)
                            break;
                        egeCnt++;
                    }

                    //OTHEROLYMPS
                    var OlympNoVseros = context.Olympiads.Where(x => x.PersonId == PersonId && !x.OlympType.IsVseross)
                        .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                    egeCnt = 1;
                    foreach (var ex in OlympNoVseros)
                    {
                        acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                        acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                        acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                        if (egeCnt == 2)
                            break;
                        egeCnt++;
                    }



                    if (!string.IsNullOrEmpty(person.SchoolName))
                        acrFlds.SetField("chbSchoolFinished", "1");

                }

                query = "SELECT WorkPlace, WorkProfession, Stage FROM PersonWork WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
                var work =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         WorkPlace = rw.Field<string>("WorkPlace"),
                         WorkProfession = rw.Field<string>("WorkProfession"),
                         Stage = rw.Field<string>("Stage")
                     }).FirstOrDefault();
                if (work != null)
                {
                    acrFlds.SetField("HasStag", "1");
                    acrFlds.SetField("WorkPlace", work.WorkPlace + ", " + work.WorkProfession);
                    acrFlds.SetField("Stag", work.Stage);
                }
                else
                    acrFlds.SetField("NoStag", "1");

                int comInd = 1;
                foreach (var comission in abitList.Select(x => x.ComissionAddress).Distinct().ToList())
                {
                    acrFlds.SetField("Comission" + comInd++, comission);
                }

                //ApplicationCommit.IsPrinted
                var Commt = context.ApplicationCommit.Where(x => x.Id == appId).FirstOrDefault();
                if (Commt != null)
                    Commt.IsPrinted = true;

                context.SaveChanges();

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                lstFiles.Add(ms.ToArray());

                return MergePdfFiles(lstFiles.Union(lstAppendixes).ToList());
            }
        }

        public static byte[] GetApplicationPDF_ProfileAppendix_Mag(List<ShortAppcationDetails> lst, string LicenseProgramName, string FIO, string dirPath, int Num)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = "PriorityProfiles_Mag2014.pdf";

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Num", Num.ToString());
            acrFlds.SetField("FIO", FIO);

            acrFlds.SetField("ObrazProgramHead", lst.First().ObrazProgramName);
            acrFlds.SetField("LicenseProgram", LicenseProgramName);
            acrFlds.SetField("ObrazProgram", lst.First().ObrazProgramName);
            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.ProfileInObrazProgramInEntryPriority))
                acrFlds.SetField("Profile" + rwind++, p.ProfileName);

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetApplicationPDF_OPAppendix_1kurs(List<ShortAppcationDetails> lst, string LicenseProgramName, string FIO, string dirPath, int Num)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = "PriorityOP2014.pdf";

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Num", Num.ToString());
            acrFlds.SetField("FIO", FIO);

            acrFlds.SetField("LicenseProgram", LicenseProgramName);
            int rwind = 1;
            foreach (var p in lst.Select(x => new { x.ObrazProgramName, x.ObrazProgramInEntryPriority }).Distinct().OrderBy(x => x.ObrazProgramInEntryPriority))
            {
                acrFlds.SetField("ObrazProgram" + rwind++, p.ObrazProgramName);
            }
            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetApplicationPDF_ProfileAppendix_1kurs(List<ShortAppcationDetails> lst, string LicenseProgramName, string FIO, string dirPath, int Num)
        {
            MemoryStream ms = new MemoryStream();
            string dotName = "PriorityProfiles2014.pdf";

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Num", Num.ToString());
            acrFlds.SetField("FIO", FIO);

            acrFlds.SetField("ObrazProgramHead", lst.First().ObrazProgramName);
            acrFlds.SetField("LicenseProgram", LicenseProgramName);
            acrFlds.SetField("ObrazProgram", lst.First().ObrazProgramName);
            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.ProfileInObrazProgramInEntryPriority))
                acrFlds.SetField("Profile" + rwind++, p.ProfileName);

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF_FirstPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string FIO, string Version, string code, bool isMag)
        {
            MemoryStream ms = new MemoryStream();

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);

            //добавляем штрихкод
            Barcode128 barcode = new Barcode128();
            barcode.Code = code;
            PdfContentByte cb = pdfStm.GetOverContent(1);
            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            if (isMag)
                img.SetAbsolutePosition(420, 720);
            else
                img.SetAbsolutePosition(440, 740);
            cb.AddImage(img);

            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Version", Version);
            acrFlds.SetField("FIO", FIO);

            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Priority" + rwind, p.Priority.ToString());
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.HasInnerPriorities ? "Приложение к заявлению № " + p.InnerPrioritiesNum : p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");

                if (lstFullSource.Where(x => x.LicenseProgramName == p.LicenseProgramName && x.ObrazProgramName == p.ObrazProgramName && x.ProfileName == p.ProfileName && x.StudyFormId == p.StudyFormId).Count() > 1)
                    acrFlds.SetField("IsPriority" + rwind, "1");

                rwind++;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetApplicationPDF_NextPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string FIO)
        {
            MemoryStream ms = new MemoryStream();

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;
            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Priority" + rwind, p.Priority.ToString());
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.HasInnerPriorities ? "Приложение к заявлению № " + p.InnerPrioritiesNum : p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");

                if (lstFullSource.Where(x => x.LicenseProgramName == p.LicenseProgramName && x.ObrazProgramName == p.ObrazProgramName && x.ProfileName == p.ProfileName && x.StudyFormId == p.StudyFormId).Count() > 1)
                    acrFlds.SetField("IsPriority" + rwind, "1");

                rwind++;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        //1курс-магистратура иностранцы (AbitTypeId = 2)
        //PREFIX 3000000
        public static byte[] GetApplicationPDFForeign(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            try
            {
                using (OnlinePriemEntities context = new OnlinePriemEntities())
                {
                    var abit = (from x in context.Application
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.Id == appId
                                select new
                                {
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    x.HostelEduc
                                }).FirstOrDefault();

                    string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", abit.PersonId } });
                    string email = tbl.Rows[0].Field<string>("Email");

                    query = "SELECT LanguageNameRus, LevelNameRus FROM extForeignPersonLanguage WHERE PersonId=@PersonId";
                    DataTable tblLangs = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", abit.PersonId } });
                    string Language = string.Join(",", (from DataRow rw in tblLangs.Rows select rw.Field<string>("LanguageNameRus") + " - " + rw.Field<string>("LevelNameRus") + ","));

                    var person = (from x in context.Person
                                  where x.Id == abit.PersonId
                                  select new
                                  {
                                      x.Surname,
                                      x.Name,
                                      x.SecondName,
                                      x.PersonAddInfo.HostelAbit,
                                      x.BirthDate,
                                      BirthPlace = x.BirthPlace ?? "",
                                      Sex = x.Sex ?? false,
                                      Nationality = x.Nationality.Name,
                                      Country = x.PersonContacts.Country.Name,
                                      PassportType = x.PassportType.Name,
                                      x.PassportSeries,
                                      x.PassportNumber,
                                      x.PassportAuthor,
                                      x.PassportDate,
                                      Address = x.PersonContacts.ForeignAddressInfo,
                                      x.PersonContacts.Phone,
                                      x.PersonContacts.Mobiles,
                                      x.PersonEducationDocument.SchoolExitYear,
                                      x.PersonEducationDocument.SchoolName,
                                      Language = Language,
                                      AddInfo = x.PersonAddInfo.AddInfo,
                                      Parents = x.PersonAddInfo.Parents,
                                      x.PersonEducationDocument.IsEqual,
                                      x.PersonEducationDocument.EqualDocumentNumber,
                                      CountryEduc = x.PersonEducationDocument.CountryEduc != null ? x.PersonEducationDocument.CountryEduc.Name : "",
                                      Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                      x.PersonEducationDocument.SchoolTypeId,
                                      EducationDocumentSeries = x.PersonEducationDocument.Series,
                                      EducationDocumentNumber = x.PersonEducationDocument.Number,
                                      x.PersonEducationDocument.AttestatRegion,
                                      x.PersonEducationDocument.AttestatSeries,
                                      x.PersonEducationDocument.AttestatNumber
                                  }).FirstOrDefault();


                    MemoryStream ms = new MemoryStream();
                    string dotName;

                    if (abit.EntryType == 2)//mag
                        dotName = "MagApplicationForeign.pdf";
                    else
                        dotName = "ApplicationForeign.pdf";

                    byte[] templateBytes;
                    using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                    {
                        templateBytes = new byte[fs.Length];
                        fs.Read(templateBytes, 0, templateBytes.Length);
                    }

                    PdfReader pdfRd = new PdfReader(templateBytes);
                    PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                    pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                    AcroFields acrFlds = pdfStm.AcroFields;
                    string code = (3000000 + abit.Barcode).ToString();

                    //добавляем штрихкод
                    Barcode128 barcode = new Barcode128();
                    barcode.Code = code;
                    PdfContentByte cb = pdfStm.GetOverContent(1);
                    iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                    if (abit.EntryType == 2)
                        img.SetAbsolutePosition(420, 720);
                    else
                        img.SetAbsolutePosition(440, 740);
                    cb.AddImage(img);

                    acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                    acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
                    //if (abit.EntryType != 2)
                    acrFlds.SetField("Specialization", abit.Specialization);
                    acrFlds.SetField("Faculty", abit.Faculty);
                    acrFlds.SetField("ObrazProgram", abit.ObrazProgram);
                    acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                    acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");

                    if (abit.HostelEduc)
                        acrFlds.SetField("HostelEducYes", "1");
                    else
                        acrFlds.SetField("HostelEducNo", "1");

                    if (person.HostelAbit ?? false)
                        acrFlds.SetField("HostelAbitYes", "1");
                    else
                        acrFlds.SetField("HostelAbitNo", "1");

                    if (person.Sex)
                        acrFlds.SetField("Male", "1");
                    else
                        acrFlds.SetField("Female", "1");

                    acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                    acrFlds.SetField("BirthPlace", person.BirthPlace);
                    acrFlds.SetField("Nationality", person.Nationality);

                    acrFlds.SetField("PassportSeries", person.PassportSeries);
                    acrFlds.SetField("PassportNumber", person.PassportNumber);
                    acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                    acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                    string[] splitStr = PDFUtils.GetSplittedStrings(person.Address, 30, 70, 2);
                    for (int i = 1; i <= 2; i++)
                        acrFlds.SetField("Address" + i.ToString(), splitStr[i - 1]);

                    string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");
                    splitStr = PDFUtils.GetSplittedStrings(phones, 30, 70, 2);
                    for (int i = 1; i <= 2; i++)
                        acrFlds.SetField("Phone" + i.ToString(), splitStr[i - 1]);

                    splitStr = PDFUtils.GetSplittedStrings(person.Parents, 40, 70, 3);
                    for (int i = 1; i <= 3; i++)
                        acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);

                    acrFlds.SetField("ExitYear", person.SchoolExitYear);
                    acrFlds.SetField("School", person.SchoolName ?? "");
                    //acrFlds.SetField("Original", "0");
                    //acrFlds.SetField("Copy", "0");

                    acrFlds.SetField("Attestat", person.SchoolTypeId == 1 ?
                        person.AttestatRegion + " " + person.AttestatSeries + " " + person.AttestatNumber :
                        person.EducationDocumentSeries + " " + person.EducationDocumentNumber);

                    acrFlds.SetField("Language", person.Language ?? "");
                    acrFlds.SetField("CountryEduc", person.CountryEduc ?? "");
                    acrFlds.SetField("Extra", person.AddInfo ?? "");

                    if (person.IsEqual ?? false)
                    {
                        acrFlds.SetField("HasEqual", "1");
                        acrFlds.SetField("EqualityDocument", person.EqualDocumentNumber);
                    }
                    else
                        acrFlds.SetField("NoEqual", "1");

                    acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");

                    if (abit.EntryType != 2)//no mag application
                    {
                        if (abit.StudyLevelId == 16)
                            acrFlds.SetField("chbBak", "1");
                        else
                            acrFlds.SetField("chbSpec", "1");

                        if (person.SchoolTypeId != 4)
                            acrFlds.SetField("NoHE", "1");
                        else
                        {
                            acrFlds.SetField("HasHE", "1");
                            acrFlds.SetField("HEName", person.SchoolName);
                        }
                    }
                    else
                        acrFlds.SetField("Qualification", "1");

                    pdfStm.FormFlattening = true;
                    pdfStm.Close();
                    pdfRd.Close();

                    return ms.ToArray();
                }
            }
            catch
            {
                return System.Text.ASCIIEncoding.UTF8.GetBytes("Еrror");
            }
        }
        //перевод (AbitTypeId = 3)
        public static byte[] GetApplicationPDFTransfer(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address,
                                    SemesterName = Semester.Name,
                                    EducYear = Semester.EducYear
                                }).OrderBy(x => x.Priority).FirstOrDefault();

                /* var abitProfileList = (from x in context.Application
                                        join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                        where x.CommitId == appId
                                        select new ShortAppcationDetails()
                                        {
                                            ApplicationId = x.Id,
                                            CurrVersion = Ad.CurrVersion,
                                            CurrDate = Ad.CurrDate,
                                            ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                            ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                            ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                            ProfileName = Ad.ProfileName
                                        }).ToList();
                 */
                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              join PersonCurrentEducation in context.PersonCurrentEducation on PersonId equals PersonCurrentEducation.PersonId
                              join Semester in context.Semester on PersonCurrentEducation.SemesterId equals Semester.Id
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.PersonEducationDocument.SchoolName,
                                  x.PersonCurrentEducation.HasAccreditation,
                                  x.PersonCurrentEducation.AccreditationDate,
                                  x.PersonCurrentEducation.AccreditationNumber,
                                  x.PersonCurrentEducation.HasScholarship,
                                  x.PersonAddInfo.Parents,
                                  x.PersonAddInfo.AddInfo,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.Series,
                                  x.PersonCurrentEducation.StudyLevelId,

                                  Semester.EducYear
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationTransfer.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;
                string code = (3000000 + abitList.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abitList.ProfessionCode + " " + abitList.Profession);
                acrFlds.SetField("Specialization", abitList.Specialization);
                acrFlds.SetField("Faculty", abitList.Faculty);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                acrFlds.SetField("StudyForm" + abitList.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abitList.StudyBasisId, "1");

                acrFlds.SetField("Course", abitList.EducYear.ToString());
                acrFlds.SetField("Semester", abitList.SemesterName);

                switch (abitList.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("chbBak", "1"); break; }
                    case 17: { acrFlds.SetField("chbMag", "1"); break; }
                    case 18: { acrFlds.SetField("chbSpec", "1"); break; }
                }

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                      string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                      person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                      person.Flat == string.Empty ? "" : "кв. " + person.Flat);
                string[] splitStr, strSplit;
                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                strSplit = GetSplittedStrings(person.Parents, 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Parents" + i.ToString(), strSplit[i - 1]);

                acrFlds.SetField("CurrentEducationName", person.SchoolName);
                acrFlds.SetField((person.HasAccreditation ?? false) ? "HasAccred" : "NoAccred", "1");
                string AccredInfo = (person.AccreditationNumber ?? "") +
                    (person.AccreditationDate.HasValue ? " от " + person.AccreditationDate.Value.ToShortDateString() : "");
                acrFlds.SetField("EducationAccreditationNumber", (person.HasAccreditation ?? false) ? AccredInfo : "");
                if (abitList.SemesterName != null)
                    acrFlds.SetField("CurrentCourse", person.EducYear.ToString());

                switch (person.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("CurrentBak", "1"); break; }
                    case 17: { acrFlds.SetField("CurrentMag", "1"); break; }
                    case 18: { acrFlds.SetField("CurrentSpec", "1"); break; }
                }

                acrFlds.SetField("ExitYear", "");//person.SchoolExitYear ?? "");
                acrFlds.SetField("School", "");//person.SchoolName ?? "");
                acrFlds.SetField("EducationDocument", ""); //(person.Series ?? "") + " " + (person.Number ?? ""));
                if (person.HasScholarship ?? false)
                    acrFlds.SetField("HasScholarship", "1");
                else
                    acrFlds.SetField("NoScholarship", "1");

                //acrFlds.SetField("Extra", person.AddInfo ?? "");
                //acrFlds.SetField("Copy", "1");

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //перевод иностранцев (AbitTypeId = 4)
        public static byte[] GetApplicationPDFTransferForeign(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address,
                                    SemesterName = Semester.Name,
                                    EducYear = Semester.EducYear
                                }).OrderBy(x => x.Priority).FirstOrDefault();

                /* var abitProfileList = (from x in context.Application
                                        join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                        where x.CommitId == appId
                                        select new ShortAppcationDetails()
                                        {
                                            ApplicationId = x.Id,
                                            CurrVersion = Ad.CurrVersion,
                                            CurrDate = Ad.CurrDate,
                                            ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                            ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                            ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                            ProfileName = Ad.ProfileName
                                        }).ToList();
                 */
                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              join PersonCurrentEducation in context.PersonCurrentEducation on PersonId equals PersonCurrentEducation.PersonId
                              join Semester in context.Semester on PersonCurrentEducation.SemesterId equals Semester.Id
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.PersonEducationDocument.SchoolName,
                                  x.PersonCurrentEducation.HasAccreditation,
                                  x.PersonCurrentEducation.AccreditationDate,
                                  x.PersonCurrentEducation.AccreditationNumber,
                                  x.PersonCurrentEducation.HasScholarship,
                                  x.PersonAddInfo.Parents,
                                  x.PersonAddInfo.AddInfo,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.Series,
                                  x.PersonCurrentEducation.StudyLevelId,
                                  SemesterName = Semester.Name,
                                  Semester.EducYear,
                                  x.PersonEducationDocument.CountryEducId,
                                  x.PersonEducationDocument.EqualDocumentNumber,
                                  x.PersonEducationDocument.IsEqual
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationTransferForeign.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;
                string code = (4000000 + abitList.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abitList.ProfessionCode + " " + abitList.Profession);
                acrFlds.SetField("Specialization", abitList.Specialization);
                acrFlds.SetField("Faculty", abitList.Faculty);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                acrFlds.SetField("StudyForm" + abitList.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abitList.StudyBasisId, "1");

                acrFlds.SetField("Course", abitList.EducYear.ToString());
                acrFlds.SetField("Semester", abitList.SemesterName);

                switch (abitList.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("chbBak", "1"); break; }
                    case 17: { acrFlds.SetField("chbMag", "1"); break; }
                    case 18: { acrFlds.SetField("chbSpec", "1"); break; }
                }

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                      string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                      person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                      person.Flat == string.Empty ? "" : "кв. " + person.Flat);
                string[] splitStr, strSplit;
                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                strSplit = GetSplittedStrings(phones, 70, 70, 2);
                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                strSplit = GetSplittedStrings(person.Parents, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), strSplit[i - 1]);

                /*strSplit = GetSplittedStrings(person.SocialStatus, 50, 70, 2);*/
                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("SocialStatus" + i.ToString(), "");
                acrFlds.SetField("MaritalStatus", "");

                acrFlds.SetField("CurrentEducationName", person.SchoolName);
                acrFlds.SetField((person.HasAccreditation ?? false) ? "HasAccred" : "NoAccred", "1");
                string AccredInfo = (person.AccreditationNumber ?? "") +
                    (person.AccreditationDate.HasValue ? person.AccreditationDate.Value.ToShortDateString() : "");
                acrFlds.SetField("EducationAccreditationNumber", (person.HasAccreditation ?? false) ? AccredInfo : "");
                if (abitList.SemesterName != null)
                    acrFlds.SetField("CurrentSemester", person.EducYear.ToString() + " курс, " + person.SemesterName);

                switch (person.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("CurrentBak", "1"); break; }
                    case 17: { acrFlds.SetField("CurrentMag", "1"); break; }
                    case 18: { acrFlds.SetField("CurrentSpec", "1"); break; }
                }

                acrFlds.SetField("ExitYear", "");//person.SchoolExitYear ?? "");
                acrFlds.SetField("School", "");//person.SchoolName ?? "");
                acrFlds.SetField("SchoolName", "");//person.SchoolName ?? "");
                acrFlds.SetField("EducationDocument", "");//(person.Series ?? "") + (person.Number ?? ""));
                acrFlds.SetField("CountryEduc", "");//person.CountryEducId.HasValue ? person.CountryEduc.Name : "");
                /*
                if (person.HasScholarship ?? false)
                    acrFlds.SetField("HasScholarship", "1");
                else
                    acrFlds.SetField("NoScholarship", "1");*/
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                strSplit = GetSplittedStrings(person.Parents, 30, 70, 3);
                for (int i = 1; i < 4; i++)
                    acrFlds.SetField("Parents", strSplit[i - 1]);

                if ((person.IsEqual ?? false) && person.CountryEducId.HasValue && person.CountryEducId.Value != 193)
                {
                    acrFlds.SetField("HasEqual", "1");
                    acrFlds.SetField("EqualNumber", person.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }
                //acrFlds.SetField("Copy", "1");

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //восстановление (AbitTypeId = 5)
        public static byte[] GetApplicationPDFRecover(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address,
                                    SemesterName = Semester.Name,
                                    EducYear = Semester.EducYear
                                }).OrderBy(x => x.Priority).FirstOrDefault();

                /* var abitProfileList = (from x in context.Application
                                        join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                        where x.CommitId == appId
                                        select new ShortAppcationDetails()
                                        {
                                            ApplicationId = x.Id,
                                            CurrVersion = Ad.CurrVersion,
                                            CurrDate = Ad.CurrDate,
                                            ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                            ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                            ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                            ProfileName = Ad.ProfileName
                                        }).ToList();
                 */
                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              join PersonDisorderInfo in context.PersonDisorderInfo on PersonId equals PersonDisorderInfo.PersonId
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.SchoolName,
                                  x.PersonEducationDocument.StartEnglish,
                                  x.PersonEducationDocument.EnglishMark,
                                  x.PersonEducationDocument.IsEqual,
                                  x.PersonEducationDocument.EqualDocumentNumber,
                                  CountryEduc = x.PersonEducationDocument.CountryEduc != null ? x.PersonEducationDocument.CountryEduc.Name : "",
                                  x.PersonEducationDocument.CountryEducId,
                                  Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                  x.PersonEducationDocument.SchoolTypeId,
                                  EducationDocumentSeries = x.PersonEducationDocument.Series,
                                  EducationDocumentNumber = x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.AttestatRegion,
                                  x.PersonEducationDocument.AttestatSeries,
                                  x.PersonEducationDocument.AttestatNumber,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.PersonDisorderInfo.YearOfDisorder,
                                  x.PersonDisorderInfo.EducationProgramName
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationRecover.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;
                string code = (5000000 + abitList.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abitList.ProfessionCode + " " + abitList.Profession);
                acrFlds.SetField("Specialization", abitList.Specialization);
                acrFlds.SetField("Faculty", abitList.Faculty);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                acrFlds.SetField("StudyForm" + abitList.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abitList.StudyBasisId, "1");

                acrFlds.SetField("Course", abitList.EducYear.ToString());
                acrFlds.SetField("Semester", abitList.SemesterName);

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);
                string[] splitStr, strSplit;
                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                acrFlds.SetField("DisorderYear", person.YearOfDisorder);

                strSplit = GetSplittedStrings(person.EducationProgramName ?? "", 80, 80, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("DisorderProgram" + i.ToString(), strSplit[i - 1]);

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //перевод на бюджет (AbitTypeId = 6)
        public static byte[] GetApplicationPDFChangeStudyBasis(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address,
                                    SemesterName = Semester.Name,
                                    EducYear = Semester.EducYear
                                }).OrderBy(x => x.Priority).FirstOrDefault();

                /* var abitProfileList = (from x in context.Application
                                        join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                        where x.CommitId == appId
                                        select new ShortAppcationDetails()
                                        {
                                            ApplicationId = x.Id,
                                            CurrVersion = Ad.CurrVersion,
                                            CurrDate = Ad.CurrDate,
                                            ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                            ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                            ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                            ProfileName = Ad.ProfileName
                                        }).ToList();
                 */
                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              //join PersonChangeStudyFormReason in context.PersonChangeStudyFormReason on PersonId equals PersonChangeStudyFormReason.PersonId
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.PersonChangeStudyFormReason.Reason,
                                  x.PersonCurrentEducation.ProfileName
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationChangeStudyBasis.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;
                string code = (6000000 + abitList.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                if (abitList.EntryType == 2)
                    img.SetAbsolutePosition(420, 720);
                else
                    img.SetAbsolutePosition(440, 740);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abitList.ProfessionCode + " " + abitList.Profession);
                acrFlds.SetField("Specialization", person.ProfileName);
                acrFlds.SetField("Faculty", abitList.Faculty);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                acrFlds.SetField("StudyForm" + abitList.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abitList.StudyBasisId, "1");
                acrFlds.SetField("Course", abitList.EducYear.ToString());
                acrFlds.SetField("Semester", abitList.SemesterName);
                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");

                string Reason = "";
                if (!String.IsNullOrEmpty(person.Reason))
                    Reason = person.Reason;

                string[] ss = GetSplittedStrings(Reason, 95, 95, 3);
                for (int i = 1; i <= 3; i++)
                {
                    acrFlds.SetField("Reason" + i, ss[i - 1]);
                }

                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);
                string[] splitStr, strSplit;
                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //смена обр. программы (AbitTypeId = 7)
        public static byte[] GetApplicationPDFChangeObrazProgram(Guid appId, string dirPath, bool isMag, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address,
                                    SemesterName = Semester.Name,
                                    EducYear = Semester.EducYear
                                }).OrderBy(x => x.Priority).FirstOrDefault();

                /* var abitProfileList = (from x in context.Application
                                        join Ad in context.extApplicationDetails on x.Id equals Ad.ApplicationId
                                        where x.CommitId == appId
                                        select new ShortAppcationDetails()
                                        {
                                            ApplicationId = x.Id,
                                            CurrVersion = Ad.CurrVersion,
                                            CurrDate = Ad.CurrDate,
                                            ObrazProgramInEntryPriority = Ad.ObrazProgramInEntryPriority,
                                            ObrazProgramName = ((Ad.ObrazProgramCrypt + " ") ?? "") + Ad.ObrazProgramName,
                                            ProfileInObrazProgramInEntryPriority = Ad.ProfileInObrazProgramInEntryPriority,
                                            ProfileName = Ad.ProfileName
                                        }).ToList();
                 */
                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              join Sem in context.Semester on x.PersonCurrentEducation.SemesterId equals Sem.Id into Sem2
                              from Sem in Sem2.DefaultIfEmpty()


                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.PersonCurrentEducation.LicenseProgramId,
                                  x.PersonCurrentEducation.SemesterId,
                                  Sem.EducYear
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationChangeObrazProgram.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;
                string code = (7000000 + abitList.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                if (abitList.EntryType == 2)
                    img.SetAbsolutePosition(420, 720);
                else
                    img.SetAbsolutePosition(440, 740);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abitList.ProfessionCode + " " + abitList.Profession);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                //if (abit.EntryType != 2)
                acrFlds.SetField("Specialization", abitList.Specialization);
                acrFlds.SetField("Faculty", abitList.Faculty);
                acrFlds.SetField("ObrazProgram", abitList.ObrazProgram);
                acrFlds.SetField("StudyForm" + abitList.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abitList.StudyBasisId, "1");

                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);

                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                acrFlds.SetField("Nationality", person.Nationality);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);
                string[] splitStr, strSplit;
                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 70, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                acrFlds.SetField("CurrentProfession", person.LicenseProgramId.HasValue ?
                    context.Entry.Where(x => x.LicenseProgramId == person.LicenseProgramId)
                    .Select(x => x.LicenseProgramName).FirstOrDefault() : "");
                acrFlds.SetField("CurrentCourse", person.SemesterId.HasValue ? (person.EducYear.HasValue ? person.EducYear.ToString() : "-") : "-");
                //acrFlds.SetField("Original", "0");
                //acrFlds.SetField("Copy", "0");
                //acrFlds.SetField("Language", PersonEducationDocument.Language.Name ?? "");
                //acrFlds.SetField("Extra", PersonAddInfo.AddInfo ?? "");


                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }

        public static byte[] GetApplicationPDF_AG(Guid appId, string dirPath)
        {
            string query = "SELECT TOP 1 PersonId, Barcode, ProgramNameRod, ObrazProgramNum, ObrazProgramId, ProfileName, EntryClassNum, HostelEduc, AG_ManualExam.Name AS ManualExam" +
                " FROM [AG_Application] INNER JOIN AG_qEntry ON [AG_Application].EntryId=AG_qEntry.Id LEFT JOIN AG_ManualExam ON AG_ManualExam.Id = [AG_Application].ManualExamId" +
                " WHERE [AG_Application].Id=@Id ORDER BY [AG_Application].Priority";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", appId } });

            var abit =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     PersonId = rw.Field<Guid?>("PersonId"),
                     Barcode = rw.Field<int>("Barcode"),
                     Profession = rw.Field<string>("ProgramNameRod"),
                     ObrazProgram = rw.Field<string>("ObrazProgramNum"),
                     ObrazProgramId = rw.Field<int>("ObrazProgramId"),
                     Specialization = rw.Field<string>("ProfileName"),
                     ClassNum = rw.Field<string>("EntryClassNum"),
                     ManualExam = rw.Field<string>("ManualExam")
                 }).FirstOrDefault();

            query = "SELECT Surname, Name, SecondName, PersonAddInfo.HostelEduc FROM Person INNER JOIN PersonAddInfo ON PersonAddInfo.Id = Person.Id PersonAddInfo.HostelEduc WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", abit.PersonId } });

            var person =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName"),
                     HostelEduc = rw.Field<bool>("HostelEduc"),
                 }).FirstOrDefault();

            query = "SELECT PrintName, DocumentNumber FROM AG_AllPriveleges WHERE PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", abit.PersonId } });
            var privileges =
                (from DataRow rw in tbl.Rows
                 select rw.Field<string>("PrintName") + " " + rw.Field<string>("DocumentNumber"));

            MemoryStream ms = new MemoryStream();
            string dotName = "ApplicationAG_2013.pdf";

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
            AcroFields acrFlds = pdfStm.AcroFields;
            string code = (800000 + abit.Barcode).ToString();

            //добавляем штрихкод
            Barcode128 barcode = new Barcode128();
            barcode.Code = code;
            PdfContentByte cb = pdfStm.GetOverContent(1);
            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            img.SetAbsolutePosition(70, 750);
            cb.AddImage(img);

            acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
            acrFlds.SetField("ProgramName_1", abit.Profession);
            acrFlds.SetField("ObrazProgramNum", abit.ObrazProgram);
            acrFlds.SetField("ClassNum", abit.ClassNum);
            acrFlds.SetField("Date", DateTime.Now.ToShortDateString());
            acrFlds.SetField("ManualExam_1", abit.ManualExam ?? "нет");
            if (person.HostelEduc)
                acrFlds.SetField("HostelEducYes", "1");
            else
                acrFlds.SetField("HostelEducNo", "1");

            if (abit.ObrazProgramId == 1)
            {
                acrFlds.SetField("is9Class", "______________");
            }
            else
            {
                acrFlds.SetField("is11Class_1", "______________");
                acrFlds.SetField("is11Class_2", "______");
            }

            query = "SELECT ProgramNameRod, ProfileName, EntryClassNum, HostelEduc, AG_ManualExam.Name AS ManualExam" +
                " FROM [AG_Application] INNER JOIN AG_qEntry ON [AG_Application].EntryId=AG_qEntry.Id LEFT JOIN AG_ManualExam ON AG_ManualExam.Id = [AG_Application].ManualExamId" +
                " WHERE [AG_Application].Id<>@Id AND [AG_Application].PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", appId }, { "@PersonId", abit.PersonId } });
            if (tbl.Rows.Count > 0)
            {
                var otherapp =
                     (from DataRow rw in tbl.Rows
                      select new
                      {
                          Specialization = rw.Field<string>("ProfileName"),
                          ManualExam = rw.Field<string>("ManualExam")
                      }).FirstOrDefault();

                acrFlds.SetField("Specialization_2", otherapp.Specialization);
                acrFlds.SetField("ManualExam_2", otherapp.ManualExam ?? "нет");

                acrFlds.SetField("Specialization_Priority", abit.Specialization);
            }

            string AllPrivileges = privileges.DefaultIfEmpty().Aggregate((x, next) => x + "; " + next) ?? "";
            int index = 0, startindex = 0;
            for (int i = 1; i <= 6; i++)
            {
                if (AllPrivileges.Length > startindex && startindex >= 0)
                {
                    int rowLength = 100;//длина строки, разная у первых строк
                    if (i > 1) //длина остальных строк одинакова
                        rowLength = 100;
                    index = startindex + rowLength;
                    if (index < AllPrivileges.Length)
                    {
                        index = AllPrivileges.IndexOf(" ", index);
                        string val = index > 0 ?
                            AllPrivileges.Substring(startindex, index - startindex)
                            : AllPrivileges.Substring(startindex);
                        acrFlds.SetField("AddDocs" + i.ToString(), val);
                    }
                    else
                        acrFlds.SetField("AddDocs" + i.ToString(),
                            AllPrivileges.Substring(startindex));
                }
                startindex = index;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF_SPO(Guid appId, string dirPath, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,

                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine
                                }).ToList();

                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.SchoolName,
                                  AddInfo = x.PersonAddInfo.AddInfo,
                                  Parents = x.PersonAddInfo.Parents,
                                  x.PersonEducationDocument.StartEnglish,
                                  x.PersonEducationDocument.EnglishMark,
                                  x.PersonEducationDocument.IsEqual,
                                  x.PersonEducationDocument.EqualDocumentNumber,
                                  CountryEduc = x.PersonEducationDocument.CountryEduc != null ? x.PersonEducationDocument.CountryEduc.Name : "",
                                  x.PersonEducationDocument.CountryEducId,
                                  Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                  x.PersonEducationDocument.SchoolTypeId,
                                  EducationDocumentSeries = x.PersonEducationDocument.Series,
                                  EducationDocumentNumber = x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.AttestatRegion,
                                  x.PersonEducationDocument.AttestatSeries,
                                  x.PersonEducationDocument.AttestatNumber,
                                  Language = x.PersonEducationDocument.Language.Name,
                                  HasPrivileges = x.PersonAddInfo.HasPrivileges ?? false,
                                  x.PersonEducationDocument.HasTRKI,
                                  x.PersonEducationDocument.TRKICertificateNumber,
                                  x.PersonAddInfo.ReturnDocumentTypeId,
                                  SportQualificationName = x.PersonSportQualification.SportQualification1.Name,
                                  x.PersonSportQualification.SportQualificationId,
                                  x.PersonSportQualification.SportQualificationLevel,
                                  x.PersonSportQualification.OtherSportQualification,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.HasRussianNationality
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationSPO_2014.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());


                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (abitList.Where(x => x.IsGosLine).Count() > 0)
                    acrFlds.SetField("IsGosLine", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                if (person.BirthDate.HasValue)
                {
                    acrFlds.SetField("BirthDateYear", person.BirthDate.Value.Year.ToString("D2"));
                    acrFlds.SetField("BirthDateMonth", person.BirthDate.Value.Month.ToString("D2"));
                    acrFlds.SetField("BirthDateDay", person.BirthDate.Value.Day.ToString());
                }
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);

                //dd.MM.yyyy :12.05.2000
                string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);

                if (person.HasRussianNationality)
                    acrFlds.SetField("HasRussianNationalityYes", "1");
                else
                    acrFlds.SetField("HasRussianNationalityNo", "1");

                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish ?? false)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Phone", person.Phone);
                acrFlds.SetField("Email", email);
                acrFlds.SetField("Mobiles", person.Mobiles);

                acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
                splitStr = GetSplittedStrings(person.SchoolName ?? "", 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("School" + i, splitStr[i - 1]);

                //только у магистров
                acrFlds.SetField("HEProfession", person.ProgramName ?? "");
                acrFlds.SetField("Qualification", person.Qualification ?? "");

                acrFlds.SetField("Original", "0");
                acrFlds.SetField("Copy", "0");
                acrFlds.SetField("CountryEduc", person.CountryEduc ?? "");
                acrFlds.SetField("Language", person.Language ?? "");

                //SportQualification
                if (person.SportQualificationId == 0)
                    acrFlds.SetField("SportQualification", "нет");
                else if (person.SportQualificationId == 44)
                    acrFlds.SetField("SportQualification", person.OtherSportQualification);
                else
                    acrFlds.SetField("SportQualification", person.SportQualificationName + "; " + person.SportQualificationLevel);

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);

                string Attestat = person.SchoolTypeId == 1 ? ("аттестат серия " + (person.AttestatRegion + " " ?? "") + (person.AttestatSeries ?? "") + " №" + (person.AttestatNumber ?? "")) :
                        ("диплом серия " + (person.EducationDocumentSeries ?? "") + " №" + (person.EducationDocumentNumber ?? ""));
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                if ((person.IsEqual ?? false) && person.CountryEducId.HasValue && person.CountryEducId.Value != 193)
                {
                    acrFlds.SetField("IsEqual", "1");
                    acrFlds.SetField("EqualSertificateNumber", person.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }

                if (person.HasPrivileges)
                    acrFlds.SetField("HasPrivileges", "1");

                acrFlds.SetField("ReturnDocumentType" + person.ReturnDocumentTypeId, "1");
                if ((person.SchoolTypeId != 2) && (person.SchoolTypeId != 5))//SSUZ & SPO
                    acrFlds.SetField("NoEduc", "1");
                else
                {
                    acrFlds.SetField("HasEduc", "1");
                    acrFlds.SetField("HighEducation", person.SchoolName);
                }

                //VSEROS
                var OlympVseros = context.Olympiads.Where(x => x.PersonId == PersonId && x.OlympType.IsVseross)
                    .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                int egeCnt = 1;
                foreach (var ex in OlympVseros)
                {
                    acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                    acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                //OTHEROLYMPS
                var OlympNoVseros = context.Olympiads.Where(x => x.PersonId == PersonId && !x.OlympType.IsVseross)
                    .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                egeCnt = 1;
                foreach (var ex in OlympNoVseros)
                {
                    acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                    acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                query = "SELECT WorkPlace, WorkProfession, Stage FROM PersonWork WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
                var work =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         WorkPlace = rw.Field<string>("WorkPlace"),
                         WorkProfession = rw.Field<string>("WorkProfession"),
                         Stage = rw.Field<string>("Stage")
                     }).FirstOrDefault();
                if (work != null)
                {
                    acrFlds.SetField("HasStag", "1");
                    acrFlds.SetField("WorkPlace", work.WorkPlace + ", " + work.WorkProfession);
                    acrFlds.SetField("Stag", work.Stage);
                }
                else
                    acrFlds.SetField("NoStag", "1");

                var Comm = context.ApplicationCommit.Where(x => x.Id == appId).FirstOrDefault();
                if (Comm != null)
                {
                    int multiplyer = 3;
                    string code = ((multiplyer * 100000) + Comm.IntNumber).ToString();

                    //добавляем штрихкод
                    Barcode128 barcode = new Barcode128();
                    barcode.Code = code;
                    PdfContentByte cb = pdfStm.GetOverContent(1);
                    iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                    img.SetAbsolutePosition(440, 740);
                    cb.AddImage(img);
                }

                int rwInd = 1;
                foreach (var abit in abitList.OrderBy(x => x.Priority))
                {
                    acrFlds.SetField("Profession" + rwInd, abit.ProfessionCode + " " + abit.Profession);
                    acrFlds.SetField("Specialization" + rwInd, abit.Specialization);
                    acrFlds.SetField("ObrazProgram" + rwInd, abit.ObrazProgram);
                    acrFlds.SetField("Priority" + rwInd, abit.Priority.ToString());

                    acrFlds.SetField("StudyForm" + abit.StudyFormId + rwInd, "1");
                    acrFlds.SetField("StudyBasis" + abit.StudyBasisId + rwInd, "1");

                    if (abitList.Where(x => x.Profession == abit.Profession && x.ObrazProgram == abit.ObrazProgram && x.Specialization == abit.Specialization && x.StudyFormId == abit.StudyFormId).Count() > 1)
                        acrFlds.SetField("IsPriority" + rwInd, "1");
                    rwInd++;
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }

        public static byte[] GetApplicationPDF_Aspirant(Guid appId, string dirPath, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = (Entry.StudyLevelId == 17 ? 2 : 1),
                                    Entry.StudyLevelId,

                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine
                                }).ToList();

                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName,
                                  x.Barcode,
                                  x.PersonAddInfo.HostelAbit,
                                  x.BirthDate,
                                  BirthPlace = x.BirthPlace ?? "",
                                  Sex = x.Sex ?? false,
                                  Nationality = x.Nationality.Name,
                                  Country = x.PersonContacts.Country.Name,
                                  PassportType = x.PassportType.Name,
                                  x.PassportSeries,
                                  x.PassportNumber,
                                  x.PassportAuthor,
                                  x.PassportDate,
                                  x.PersonContacts.City,
                                  Region = x.PersonContacts.Region.Name,
                                  x.PersonHighEducationInfo.ProgramName,
                                  x.PersonContacts.Code,
                                  x.PersonContacts.Street,
                                  x.PersonContacts.House,
                                  x.PersonContacts.Korpus,
                                  x.PersonContacts.Flat,
                                  x.PersonContacts.Phone,
                                  x.PersonContacts.Mobiles,
                                  x.PersonEducationDocument.SchoolExitYear,
                                  x.PersonEducationDocument.SchoolName,
                                  AddInfo = x.PersonAddInfo.AddInfo,
                                  Parents = x.PersonAddInfo.Parents,
                                  x.PersonEducationDocument.StartEnglish,
                                  x.PersonEducationDocument.EnglishMark,
                                  x.PersonEducationDocument.IsEqual,
                                  x.PersonEducationDocument.EqualDocumentNumber,
                                  CountryEduc = x.PersonEducationDocument.CountryEduc != null ? x.PersonEducationDocument.CountryEduc.Name : "",
                                  x.PersonEducationDocument.CountryEducId,
                                  Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                  x.PersonEducationDocument.SchoolTypeId,
                                  EducationDocumentSeries = x.PersonEducationDocument.Series,
                                  EducationDocumentNumber = x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.AttestatRegion,
                                  x.PersonEducationDocument.AttestatSeries,
                                  x.PersonEducationDocument.AttestatNumber,
                                  Language = x.PersonEducationDocument.Language.Name,
                                  HasPrivileges = x.PersonAddInfo.HasPrivileges ?? false,
                                  x.PersonEducationDocument.HasTRKI,
                                  x.PersonEducationDocument.TRKICertificateNumber,
                                  x.PersonAddInfo.ReturnDocumentTypeId,
                                  SportQualificationName = x.PersonSportQualification.SportQualification1.Name,
                                  x.PersonAddInfo.HostelEduc,
                                  x.PersonContacts.Country.IsRussia,
                                  x.HasRussianNationality
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationAsp_2014.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());


                if (person.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (abitList.Where(x => x.IsGosLine).Count() > 0)
                    acrFlds.SetField("IsGosLine", "1");

                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");

                if (person.BirthDate.HasValue)
                {
                    acrFlds.SetField("BirthDateYear", person.BirthDate.Value.Year.ToString("D2"));
                    acrFlds.SetField("BirthDateMonth", person.BirthDate.Value.Month.ToString("D2"));
                    acrFlds.SetField("BirthDateDay", person.BirthDate.Value.Day.ToString());
                }
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);

                //dd.MM.yyyy :12.05.2000
                string[] splitStr = GetSplittedStrings(person.PassportAuthor + " " + person.PassportDate.Value.ToString("dd.MM.yyyy"), 60, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("PassportAuthor" + i, splitStr[i - 1]);

                string Address = string.Format("{0} {1}{2},", (person.Code) ?? "", (person.IsRussia ? (person.Region + ", ") ?? "" : person.Country + ", "), (person.City + ", ") ?? "") +
                    string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом " + person.House,
                    person.Korpus == string.Empty ? "" : "корп. " + person.Korpus,
                    person.Flat == string.Empty ? "" : "кв. " + person.Flat);

                if (person.HasRussianNationality)
                    acrFlds.SetField("HasRussianNationalityYes", "1");
                else
                    acrFlds.SetField("HasRussianNationalityNo", "1");

                splitStr = GetSplittedStrings(Address, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Address" + i, splitStr[i - 1]);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish ?? false)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Phone", person.Phone);
                acrFlds.SetField("Email", email);
                acrFlds.SetField("Mobiles", person.Mobiles);

                acrFlds.SetField("Language", person.Language);

                acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
                splitStr = GetSplittedStrings(person.SchoolName ?? "", 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("School" + i, splitStr[i - 1]);

                //только у магистров
                acrFlds.SetField("HEProfession", person.ProgramName ?? "");
                acrFlds.SetField("Qualification", person.Qualification ?? "");

                acrFlds.SetField("Original", "0");
                acrFlds.SetField("Copy", "0");
                acrFlds.SetField("CountryEduc", person.CountryEduc ?? "");
                acrFlds.SetField("Language", person.Language ?? "");

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 70, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i - 1]);

                string Attestat = person.SchoolTypeId == 1 ? ("аттестат серия " + (person.AttestatRegion + " " ?? "") + (person.AttestatSeries ?? "") + " №" + (person.AttestatNumber ?? "")) :
                        ("диплом серия " + (person.EducationDocumentSeries ?? "") + " №" + (person.EducationDocumentNumber ?? ""));
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                if ((person.IsEqual ?? false) && person.CountryEducId.HasValue && person.CountryEducId.Value != 193)
                {
                    acrFlds.SetField("IsEqual", "1");
                    acrFlds.SetField("EqualSertificateNumber", person.EqualDocumentNumber);
                }
                else
                {
                    acrFlds.SetField("NoEqual", "1");
                }

                if (person.HasPrivileges)
                    acrFlds.SetField("HasPrivileges", "1");

                acrFlds.SetField("ReturnDocumentType" + person.ReturnDocumentTypeId, "1");
                if ((person.SchoolTypeId != 2) && (person.SchoolTypeId != 5))//SSUZ & SPO
                    acrFlds.SetField("NoEduc", "1");
                else
                {
                    acrFlds.SetField("HasEduc", "1");
                    acrFlds.SetField("HighEducation", person.SchoolName);
                }

                //VSEROS
                var OlympVseros = context.Olympiads.Where(x => x.PersonId == PersonId && x.OlympType.IsVseross)
                    .Select(x => new { x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                int egeCnt = 1;
                foreach (var ex in OlympVseros)
                {
                    acrFlds.SetField("OlympVserosName" + egeCnt, ex.Name);
                    acrFlds.SetField("OlympVserosYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympVserosDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                //OTHEROLYMPS
                var OlympNoVseros = context.Olympiads.Where(x => x.PersonId == PersonId && !x.OlympType.IsVseross)
                    .Select(x => new { x.OlympName.Name, OlympSubject = x.OlympSubject.Name, x.DocumentDate, x.DocumentSeries, x.DocumentNumber }).ToList();
                egeCnt = 1;
                foreach (var ex in OlympNoVseros)
                {
                    acrFlds.SetField("OlympName" + egeCnt, ex.Name + " (" + ex.OlympSubject + ")");
                    acrFlds.SetField("OlympYear" + egeCnt, ex.DocumentDate.HasValue ? ex.DocumentDate.Value.Year.ToString() : "");
                    acrFlds.SetField("OlympDiplom" + egeCnt, (ex.DocumentSeries + " " ?? "") + (ex.DocumentNumber ?? ""));

                    if (egeCnt == 2)
                        break;
                    egeCnt++;
                }

                query = "SELECT WorkPlace, WorkProfession, Stage FROM PersonWork WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
                var work =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         WorkPlace = rw.Field<string>("WorkPlace"),
                         WorkProfession = rw.Field<string>("WorkProfession"),
                         Stage = rw.Field<string>("Stage")
                     }).FirstOrDefault();
                if (work != null)
                {
                    acrFlds.SetField("HasStag", "1");
                    acrFlds.SetField("WorkPlace", work.WorkPlace + ", " + work.WorkProfession);
                    acrFlds.SetField("Stag", work.Stage);
                }
                else
                    acrFlds.SetField("NoStag", "1");

                var Comm = context.ApplicationCommit.Where(x => x.Id == appId).FirstOrDefault();
                if (Comm != null)
                {
                    int multiplyer = 3;
                    string code = ((multiplyer * 100000) + Comm.IntNumber).ToString();

                    //добавляем штрихкод
                    Barcode128 barcode = new Barcode128();
                    barcode.Code = code;
                    PdfContentByte cb = pdfStm.GetOverContent(1);
                    iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                    img.SetAbsolutePosition(440, 740);
                    cb.AddImage(img);
                }

                int rwInd = 1;
                foreach (var abit in abitList.OrderBy(x => x.Priority))
                {
                    acrFlds.SetField("Profession" + rwInd, abit.ProfessionCode + " " + abit.Profession);
                    acrFlds.SetField("Specialization" + rwInd, abit.Specialization);
                    acrFlds.SetField("ObrazProgram" + rwInd, abit.ObrazProgram);
                    acrFlds.SetField("Priority" + rwInd, abit.Priority.ToString());

                    acrFlds.SetField("StudyForm" + abit.StudyFormId + rwInd, "1");
                    acrFlds.SetField("StudyBasis" + abit.StudyBasisId + rwInd, "1");

                    if (abitList.Where(x => x.Profession == abit.Profession && x.ObrazProgram == abit.ObrazProgram && x.Specialization == abit.Specialization && x.StudyFormId == abit.StudyFormId).Count() > 1)
                        acrFlds.SetField("IsPriority" + rwInd, "1");
                    rwInd++;
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }

        public static byte[] GetApplicationBlockPDF_AG(Guid commitId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList =
                    (from App in context.AG_Application
                     orderby App.Priority
                     where App.CommitId == commitId && App.IsCommited == true && App.Enabled == true
                     select new
                     {
                         PersonId = App.PersonId,
                         Barcode = App.Barcode,
                         Profession = App.AG_Entry.AG_Program.Name,
                         ObrazProgram = App.AG_Entry.AG_ObrazProgram.Num,
                         ObrazProgramName = App.AG_Entry.AG_Profile.Name,
                         Specialization = App.AG_Entry.AG_Profile.Name,
                         ClassNum = App.AG_Entry.AG_EntryClass.Num,
                         ManualExam = App.ManualExamId.HasValue ? App.AG_ManualExam.Name : "нет"
                     });

                Guid PersonId = abitList.FirstOrDefault().PersonId;
                var person =
                    context.Person.Where(x => x.Id == PersonId).Select(x => new
                    {
                        x.Surname,
                        x.Name,
                        x.SecondName,
                        x.Sex,
                        x.PersonAddInfo.HasPrivileges,
                        x.PersonAddInfo.HostelEduc
                    }).FirstOrDefault();

                //query = "SELECT PrintName, DocumentNumber FROM AG_AllPriveleges WHERE PersonId=@PersonId";
                //tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@PersonId", abit.PersonId } });
                var Olympiads =
                    from Ol in context.Olympiads
                    where Ol.PersonId == PersonId
                    select new
                    {
                        Ol.DocumentSeries,
                        Ol.DocumentNumber,
                        Ol.DocumentDate,
                        OlympName = Ol.OlympName.Name,
                        OlympSubject = Ol.OlympSubject.Name
                    };
                //(from DataRow rw in tbl.Rows
                // select rw.Field<string>("PrintName") + " " + rw.Field<string>("DocumentNumber"));

                MemoryStream ms = new MemoryStream();
                string dotName = "ApplicationAG.pdf";

                byte[] templateBytes;
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("ObrazProgramYear", abitList.FirstOrDefault().ObrazProgram);
                acrFlds.SetField("EntryClass", abitList.FirstOrDefault().ClassNum);
                if (person.HostelEduc)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                int inc = 0;
                bool hasSecondApp = abitList.Count() > 1;
                foreach (var abit in abitList)
                {
                    string code = (800000 + abit.Barcode).ToString();
                    inc++;
                    string i = inc.ToString();
                    if (hasSecondApp)
                    {
                        acrFlds.SetField("chbIsPriority" + i, "1");
                        hasSecondApp = false;
                    }
                    //добавляем штрихкод
                    //Barcode128 barcode = new Barcode128();
                    //barcode.Code = code;
                    //PdfContentByte cb = pdfStm.GetOverContent(1);
                    //iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
                    //img.SetAbsolutePosition(70, 750);
                    //cb.AddImage(img);

                    acrFlds.SetField("RegNum" + i, code);
                    acrFlds.SetField("Profession" + i, abit.Profession);
                    acrFlds.SetField("ObrazProgram" + i, abit.ObrazProgramName);
                    acrFlds.SetField("ManualExam" + i, abit.ManualExam);

                    //string AllPrivileges = privileges.DefaultIfEmpty().Aggregate((x, next) => x + "; " + next) ?? "";
                    //int index = 0, startindex = 0;
                    //for (int i = 1; i <= 6; i++)
                    //{
                    //    if (AllPrivileges.Length > startindex && startindex >= 0)
                    //    {
                    //        int rowLength = 100;//длина строки, разная у первых строк
                    //        if (i > 1) //длина остальных строк одинакова
                    //            rowLength = 100;
                    //        index = startindex + rowLength;
                    //        if (index < AllPrivileges.Length)
                    //        {
                    //            index = AllPrivileges.IndexOf(" ", index);
                    //            string val = index > 0 ?
                    //                AllPrivileges.Substring(startindex, index - startindex)
                    //                : AllPrivileges.Substring(startindex);
                    //            acrFlds.SetField("AddDocs" + i.ToString(), val);
                    //        }
                    //        else
                    //            acrFlds.SetField("AddDocs" + i.ToString(),
                    //                AllPrivileges.Substring(startindex));
                    //    }
                    //    startindex = index;
                    //}
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }

        public static string[] GetSplittedStrings(string sourceStr, int firstStrLen, int strLen, int numOfStrings)
        {
            sourceStr = sourceStr ?? "";
            string[] retStr = new string[numOfStrings];
            int index = 0, startindex = 0;
            for (int i = 0; i < numOfStrings; i++)
            {
                if (sourceStr.Length > startindex && startindex >= 0)
                {
                    int rowLength = firstStrLen;//длина первой строки
                    if (i > 1) //длина остальных строк одинакова
                        rowLength = strLen;
                    index = startindex + rowLength;
                    if (index < sourceStr.Length)
                    {
                        index = sourceStr.IndexOf(" ", index);
                        string val = index > 0 ? sourceStr.Substring(startindex, index - startindex) : sourceStr.Substring(startindex);
                        retStr[i] = val;
                    }
                    else
                        retStr[i] = sourceStr.Substring(startindex);
                }
                startindex = index;
            }

            return retStr;
        }
        public static byte[] MergePdfFiles(List<byte[]> lstFilesBinary)
        {
            MemoryStream ms = new MemoryStream();
            Document document = new Document(PageSize.A4);
            PdfWriter writer = PdfWriter.GetInstance(document, ms);

            document.Open();

            foreach (byte[] doc in lstFilesBinary)
            {
                PdfReader reader = new PdfReader(doc);
                int n = reader.NumberOfPages;
                //writer.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);

                PdfContentByte cb = writer.DirectContent;
                PdfImportedPage page;

                for (int i = 0; i < n; i++)
                {
                    document.NewPage();
                    page = writer.GetImportedPage(reader, i + 1);
                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                }
            }

            document.Close();
            return ms.ToArray();
        }

        public static bool GetDisableApplicationPDF(Guid appId, string dirPath, Guid PersonId)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.CommitId == appId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = Entry.FacultyName,
                                    Profession = Entry.LicenseProgramName,
                                    ProfessionCode = Entry.LicenseProgramCode,
                                    ObrazProgram = Entry.ObrazProgramCrypt + " " + Entry.ObrazProgramName,
                                    Specialization = Entry.ProfileName,
                                    Entry.StudyFormId,
                                    Entry.StudyFormName,
                                    Entry.StudyBasisId,
                                    EntryType = Entry.StudyLevelGroupId,
                                    StudyLevelGroupName = Entry.StudyLevelGroupNameRus,
                                    Entry.StudyLevelId,
                                    CommitIntNumber = Commit.IntNumber,
                                    x.Priority,
                                    x.IsGosLine,
                                    Entry.ComissionId,
                                    ComissionAddress = Entry.Address, 
                                }).OrderBy(x => x.Priority).ToList();


                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");
                var person = (from x in context.Person
                              where x.Id == PersonId
                              select new
                              {
                                  x.Surname,
                                  x.Name,
                                  x.SecondName
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName = "DisableApplication2014_page3.pdf";

                byte[] templateBytes;

                List<byte[]> lstFiles = new List<byte[]>();
                using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
                {
                    templateBytes = new byte[fs.Length];
                    fs.Read(templateBytes, 0, templateBytes.Length);
                }

                PdfReader pdfRd = new PdfReader(templateBytes);
                PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
                //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);
                AcroFields acrFlds = pdfStm.AcroFields;

                var Version = context.ApplicationCommitVersion.Where(x => x.CommitId == appId).Select(x => new { x.VersionDate, x.Id }).ToList().LastOrDefault();
                string sVersion = "";
                if (Version != null)
                    sVersion = "Отказ от заявления №" + Version.Id + (Version.VersionDate.HasValue ? " от " + Version.VersionDate.Value.ToString("dd.MM.yyyy HH:mm") : "");
                string FIO = ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim();

                List<ShortAppcation> lstApps = abitList
                    .Select(x => new ShortAppcation()
                    {
                        Priority = x.Priority,
                        ApplicationId = x.Id,
                        LicenseProgramName = x.ProfessionCode + " " + x.Profession,
                        ObrazProgramName = x.ObrazProgram,
                        ProfileName = x.Specialization,
                        StudyBasisId = x.StudyBasisId,
                        StudyFormId = x.StudyFormId,
                    }).ToList();

                List<ShortAppcation> lstAppsFirst = new List<ShortAppcation>();
                for (int u = 0; u < 4; u++)
                {
                    if (lstApps.Count > u)
                        lstAppsFirst.Add(lstApps[u]);
                }

                int multiplyer = abitList.First().EntryType;
                string code = ((multiplyer * 100000) + abitList.First().CommitIntNumber).ToString();

                //добавляем первый файл
                lstFiles.Add(GetDisableApplicationPDF_FirstPage(lstAppsFirst, lstApps, dirPath, "DisableApplication2014_page1.pdf", code, FIO, sVersion ));
                acrFlds.SetField("Version", sVersion);

                //остальные - по 5 на новую страницу
                int appcount = 4;
                while (appcount < lstApps.Count)
                {
                    lstAppsFirst = new List<ShortAppcation>();
                    for (int u = 0; u < 5; u++)
                    {
                        if (lstApps.Count > appcount)
                            lstAppsFirst.Add(lstApps[appcount]);
                        else
                            break;
                        appcount++;
                    }
                    if (appcount >= lstApps.Count)
                        lstFiles.Add(GetDisableApplicationPDF_NextPage(lstAppsFirst, lstApps, dirPath, "DisableApplication2014_page3.pdf", FIO, sVersion));
                    else
                        lstFiles.Add(GetDisableApplicationPDF_NextPage(lstAppsFirst, lstApps, dirPath, "DisableApplication2014_page2.pdf", "", ""));
                }

                context.SaveChanges();

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                if (lstApps.Count <= 4)
                    lstFiles.Add(ms.ToArray());

                byte[] pdfData = MergePdfFiles(lstFiles.ToList());
                DateTime dateTime = DateTime.Now;

                query = @"INSERT INTO PersonFile (  [Id],[PersonId],
                                                    [FileName],[FileExtention],[FileData],[FileSize],
                                                    [Comment],[LoadDate],[MimeType],
                                                    [IsReadOnly],[PersonFileTypeId])  
                                            VALUES (@Id, @PersonId,
                                                    @FileName, @FileExtention, @FileData, @FileSize,
                                                    @Comment, @LoadDate, @MimeType,
                                                    @IsReadOnly, 7)";
                SortedList<string, object> prms = new SortedList<string, object>();
                prms.Clear();
                prms.Add("@Id", Guid.NewGuid());
                prms.Add("@PersonId", PersonId);
                prms.Add("@FileName", person.Surname + " " + person.Name.FirstOrDefault() + "_Отказ от заявления (" + code + ").pdf");
                prms.Add("@FileExtention", ".pdf");
                prms.Add("@FileData", pdfData);
                prms.Add("@FileSize", pdfData.Length);
                prms.Add("@IsReadOnly", true);
                prms.Add("@LoadDate", dateTime);
                prms.Add("@Comment", "Заявление об отказе от участия в конкурсе (" + sVersion + ", "+ code+") " + abitList.FirstOrDefault().StudyLevelGroupName);
                prms.Add("@MimeType", "[Application]/pdf");
                Util.AbitDB.ExecuteQuery(query, prms);
                bool result = true;
                return result;
            }
        }
        public static byte[] GetDisableApplicationPDF_FirstPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string code, string FIO, string Version)
        {
            MemoryStream ms = new MemoryStream();

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            //pdfStm.SetEncryption(PdfWriter.STRENGTH128BITS, "", "", PdfWriter.ALLOW_SCREENREADERS | PdfWriter.ALLOW_PRINTING | PdfWriter.AllowPrinting);

            Barcode128 barcode = new Barcode128();
            barcode.Code = code;
            PdfContentByte cb = pdfStm.GetOverContent(1);
            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            img.SetAbsolutePosition(440, 740);
            cb.AddImage(img);

            AcroFields acrFlds = pdfStm.AcroFields;
            acrFlds.SetField("Version", Version);
            acrFlds.SetField("FIO", FIO);

            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");
                rwind++;
            }

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
        public static byte[] GetDisableApplicationPDF_NextPage(List<ShortAppcation> lst, List<ShortAppcation> lstFullSource, string dirPath, string dotName, string FIO, string version)
        {
            MemoryStream ms = new MemoryStream();

            byte[] templateBytes;
            using (FileStream fs = new FileStream(dirPath + dotName, FileMode.Open, FileAccess.Read))
            {
                templateBytes = new byte[fs.Length];
                fs.Read(templateBytes, 0, templateBytes.Length);
            }

            PdfReader pdfRd = new PdfReader(templateBytes);
            PdfStamper pdfStm = new PdfStamper(pdfRd, ms);
            AcroFields acrFlds = pdfStm.AcroFields;
            int rwind = 1;
            foreach (var p in lst.OrderBy(x => x.Priority))
            {
                acrFlds.SetField("Profession" + rwind, p.LicenseProgramName);
                acrFlds.SetField("ObrazProgram" + rwind, p.ObrazProgramName);
                acrFlds.SetField("Specialization" + rwind, p.ProfileName);
                acrFlds.SetField("StudyForm" + p.StudyFormId.ToString() + rwind.ToString(), "1");
                acrFlds.SetField("StudyBasis" + p.StudyBasisId.ToString() + rwind.ToString(), "1");
                rwind++;
            }
            if (!String.IsNullOrEmpty(version)) acrFlds.SetField("Version", version);
            if (!String.IsNullOrEmpty(FIO)) acrFlds.SetField("FIO", FIO);

            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }
    } 
}