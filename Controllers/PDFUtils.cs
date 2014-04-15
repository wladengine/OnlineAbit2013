﻿using System;
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
    public static class PDFUtils
    {
        /// <summary>
        /// PDF Мотивационное письмо
        /// </summary>
        /// <param name="mailId"></param>
        /// <param name="fontspath"></param>
        /// <returns></returns>
        public static byte[] GetMotivateMail(string mailId, string fontspath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                Guid MailId;
                if (!Guid.TryParse(mailId, out MailId))
                    return Encoding.Unicode.GetBytes("");

                MemoryStream ms = new MemoryStream();

                //string query = "SELECT Surname, Name, SecondName, Phone, Mobiles, [User].Email, MotivationMail.MailText, Entry.ObrazProgramName FROM Person " +
                //    "INNER JOIN [Application] ON [Application].PersonId=Person.Id " +
                //    "INNER JOIN Entry ON [Application].EntryId=Entry.Id " +
                //    "INNER JOIN MotivationMail ON MotivationMail.ApplicationId=[Application].Id " +
                //    "INNER JOIN [User] ON [User].Id=Person.Id WHERE MotivationMail.Id=@MailId";
                //DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@MailId", MailId } });
                string FIO, phone, program, text;
                try
                {
                    var data = (from person in context.Person
                                join app in context.Application
                                on person.Id equals app.PersonId
                                join motivMail in context.MotivationMail
                                on app.Id equals motivMail.ApplicationId
                                join user in context.User
                                on person.Id equals user.Id

                                select new
                                {
                                    person.Surname,
                                    person.Name,
                                    person.SecondName,
                                    person.PersonContacts.Phone,
                                    person.PersonContacts.Mobiles,
                                    user.Email,
                                    motivMail.MailText,
                                    app.Entry.ObrazProgramName
                                }).FirstOrDefault();

                    FIO = data.Surname + " " + data.Name + " " + data.SecondName;
                    phone = data.Email + "\n" + data.Phone + "\n" + data.Mobiles;
                    program = data.ObrazProgramName;
                    text = data.MailText;
                }
                catch
                {
                    return new byte[1] { 0x00 };
                }

                FIO = FIO.Trim();

                using (Document doc = new Document())
                {
                    BaseFont baseFont = BaseFont.CreateFont(fontspath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font font12 = new iTextSharp.text.Font(baseFont, 12, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font font16 = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font font16U = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.UNDERLINE);
                    iTextSharp.text.Font font16B = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);

                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                    doc.Open();


                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 40f, 25f, 35f });

                    //table.SetWidthPercentage(new float[] { 10f, 40f, 15f, 15f, 20f }, doc.PageSize);

                    PdfPCell cell = new PdfPCell(new Phrase("Санкт-Петербургский Государственный Университет", font16));
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(""));
                    cell.Border = 0;
                    table.AddCell(cell);

                    Phrase ph = new Phrase();
                    ph.Add(new Chunk("(ФИО) ", font12));
                    ph.Add(new Chunk(FIO, font16U));
                    cell = new PdfPCell(ph);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase("Приемная комиссия", font16));
                    cell.HorizontalAlignment = Element.ALIGN_BOTTOM & Element.ALIGN_LEFT;
                    cell.Border = 0;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(""));
                    cell.Border = 0;
                    table.AddCell(cell);

                    ph = new Phrase();
                    ph.Add(new Chunk("e-mail, тел. ", font12));
                    ph.Add(new Chunk(phone, font16U));
                    cell = new PdfPCell(ph);
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Border = 0;
                    table.AddCell(cell);

                    doc.Add(table);

                    Paragraph p = new Paragraph("Мотивационное письмо \n к заявлению на участие в конкурсе \n по магистерской программе:\n" + program, font16B);
                    p.Alignment = Element.ALIGN_CENTER;
                    doc.Add(p);
                    string[] paragraphs = text.Split('\n');
                    foreach (string par in paragraphs)
                    {
                        p = new Paragraph(par, font12);
                        p.FirstLineIndent = 30;
                        p.Alignment = Element.ALIGN_JUSTIFIED;
                        doc.Add(p);
                    }

                    p = new Paragraph("\nДата: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), font16);
                    doc.Add(p);

                    p = new Paragraph("\nПодпись: ", font16);
                    doc.Add(p);

                    doc.Close();
                }

                return ms.ToArray();
            }
        }

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
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", personId } });

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
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", personId } });
                var PersonFile =
                    from DataRow rw in tbl.Rows
                    select new
                    {
                        Comment = rw.Field<string>("Comment"),
                        FileName = rw.Field<string>("FileName"),
                        LoadDate = rw.Field<DateTime?>("LoadDate").HasValue ? rw.Field<DateTime>("LoadDate").ToShortDateString() : "нет"
                    };
                query = "SELECT FileName, Comment, LoadDate FROM ApplicationFile WHERE ApplicationId=@AppId";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@AppId", applicationId } });
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
        public static byte[] GetApplicationPDF(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                Specialization = x.Entry.ProfileName,
                                x.Entry.StudyFormId,
                                x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                EntryType = (x.Entry.StudyLevelId == 17 ? 2 : 1),
                                x.Entry.StudyLevelId,
                                x.HostelEduc
                            }).FirstOrDefault();

                string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });
                string email = tbl.Rows[0].Field<string>("Email");

                var person = (from x in context.Person
                              where x.Id == abit.PersonId
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
                                  Qualification = x.PersonHighEducationInfo.Qualification != null ? x.PersonHighEducationInfo.Qualification.Name : "",
                                  x.PersonEducationDocument.SchoolTypeId,
                                  EducationDocumentSeries = x.PersonEducationDocument.Series,
                                  EducationDocumentNumber = x.PersonEducationDocument.Number,
                                  x.PersonEducationDocument.AttestatRegion,
                                  x.PersonEducationDocument.AttestatSeries,
                                  x.PersonEducationDocument.AttestatNumber,
                                  Language = x.PersonEducationDocument.Language.Name
                              }).FirstOrDefault();

                MemoryStream ms = new MemoryStream();
                string dotName;

                if (abit.EntryType == 2)//mag
                    dotName = "MagApplication2013.pdf";
                else
                    dotName = "Application2013.pdf";

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
                int multiplyer = abit.EntryType == 2 ? 2 : 1;
                string code = "";
                if (abit.EntryType == 2)
                    code = (2000000 + abit.Barcode).ToString();
                else
                    code = (1000000 + abit.Barcode).ToString();

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
                if (abit.EntryType == 2)
                    acrFlds.SetField("StudyFormName", abit.StudyFormName);

                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");

                acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");
                if (abit.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");
                acrFlds.SetField("HostelAbitYes", (person.HostelAbit ?? false) ? "1" : "0");
                acrFlds.SetField("HostelAbitNo", (person.HostelAbit ?? false) ? "0" : "1");
                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Male", person.Sex ? "1" : "0");
                acrFlds.SetField("Female", person.Sex ? "0" : "1");
                acrFlds.SetField("Nationality", person.Nationality);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                acrFlds.SetField("EnglishMark", person.EnglishMark.ToString());
                if (person.StartEnglish ?? false)
                    acrFlds.SetField("chbEnglishYes", "1");
                else
                    acrFlds.SetField("chbEnglishNo", "1");

                acrFlds.SetField("Address1", string.Format("{0} {1}, {2}, ", person.Code ?? "", person.Region ?? "", person.City ?? ""));
                acrFlds.SetField("Address2", string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом "
                    + person.House, person.Korpus == string.Empty ? "" : "корп. " + person.Korpus, person.Flat == string.Empty ? "" : "кв. " + person.Flat));

                string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");
                string[] splitStr = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), splitStr[i -1]);

                acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
                acrFlds.SetField("School", person.SchoolName ?? "");
                acrFlds.SetField("HighEducation", person.ProgramName ?? "");
                acrFlds.SetField("Qualification", person.Qualification ?? "");
                acrFlds.SetField("Original", "0");
                acrFlds.SetField("Copy", "0");
                acrFlds.SetField("CountryEduc", person.CountryEduc ?? "");
                acrFlds.SetField("Language", person.Language ?? "");
                string Attestat = person.SchoolTypeId == 1 ? ("аттестат серия " + (person.AttestatRegion + " " ?? "") + (person.AttestatSeries ?? "") + " №" + (person.AttestatNumber ?? "")) : 
                    ("диплом серия " + person.EducationDocumentSeries ?? ""  + " №" + person.EducationDocumentNumber ?? "");
                acrFlds.SetField("Attestat", Attestat);
                acrFlds.SetField("Extra", person.AddInfo ?? "");

                string extraPerson = person.Parents ?? "";
                splitStr = GetSplittedStrings(extraPerson, 40, 70, 4);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), splitStr[i]);

                if (abit.EntryType == 1)
                {
                    //EGE
                    query = "SELECT ExamName, MarkValue, Number FROM EgeMarksAll WHERE PersonId=@PersonId";
                    tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
                    var exams =
                        from DataRow rw in tbl.Rows
                        select new { ExamName = rw.Field<string>("ExamName"), MarkValue = rw.Field<int?>("MarkValue"), Number = rw.Field<string>("Number") };
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
                }

                if (abit.EntryType != 2)//no mag application
                {
                    acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                    query = "SELECT WorkPlace, WorkProfession, Stage FROM PersonWork WHERE PersonId=@PersonId";
                    tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
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
                        acrFlds.SetField("NoStag", "0");


                    if (person.SchoolTypeId == 1)//no school
                        acrFlds.SetField("NoEduc", "1");
                    else
                    {
                        acrFlds.SetField("HasEduc", "1");
                        acrFlds.SetField("HighEducation", person.SchoolName);
                    }

                    //bacheloor - specialist
                    if (!string.IsNullOrEmpty(abit.ProfessionCode))
                    {
                        if (abit.ProfessionCode.EndsWith("00"))
                            acrFlds.SetField("chbBak", "1");
                        else
                            acrFlds.SetField("chbSpec", "1");
                    }

                    if (!string.IsNullOrEmpty(person.SchoolName))
                        acrFlds.SetField("chbSchoolFinished", "1");
                }

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //1курс-магистратура иностранцы (AbitTypeId = 2)
        //PREFIX 3000000
        public static byte[] GetApplicationPDFForeign(Guid appId, string dirPath)
        {
            try
            {
                using (OnlinePriemEntities context = new OnlinePriemEntities())
                {
                    var abit = (from x in context.Application
                                where x.Id == appId
                                select new
                                {
                                    x.PersonId,
                                    x.Barcode,
                                    Faculty = x.Entry.FacultyName,
                                    Profession = x.Entry.LicenseProgramName,
                                    ProfessionCode = x.Entry.LicenseProgramCode,
                                    ObrazProgram = x.Entry.ObrazProgramName,
                                    Specialization = x.Entry.ProfileName,
                                    x.Entry.StudyFormId,
                                    x.Entry.StudyFormName,
                                    x.Entry.StudyBasisId,
                                    EntryType = (x.Entry.StudyLevelId == 17 ? 2 : 1),
                                    x.Entry.StudyLevelId,
                                    x.HostelEduc
                                }).FirstOrDefault();

                    string query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });
                    string email = tbl.Rows[0].Field<string>("Email");

                    query = "SELECT LanguageNameRus, LevelNameRus FROM extForeignPersonLanguage WHERE PersonId=@PersonId";
                    DataTable tblLangs = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
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
        public static byte[] GetApplicationPDFTransfer(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                x.Entry.ObrazProgramCrypt,
                                Specialization = x.Entry.ProfileName,
                                StudyFormId = x.Entry.StudyFormId,
                                StudyFormName = x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                x.EntryType,
                                x.HostelEduc,
                                SemesterName = x.Entry.Semester.Name,
                                EducYear = x.Entry.Semester.EducYear,
                                x.Entry.StudyLevelId
                            }).FirstOrDefault();
                string email = context.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

                var person = context.Person.Where(x => x.Id == abit.PersonId).FirstOrDefault();

                var PersonContacts = person.PersonContacts;
                if (PersonContacts == null)
                    PersonContacts = new PersonContacts();

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
                string code = (3000000 + abit.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
                acrFlds.SetField("Specialization", abit.Specialization);
                acrFlds.SetField("Faculty", abit.Faculty);
                acrFlds.SetField("ObrazProgram", (abit.ObrazProgramCrypt + " " ?? "") + abit.ObrazProgram);
                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");

                acrFlds.SetField("Course", abit.EducYear.ToString());
                acrFlds.SetField("Semester", abit.SemesterName);

                switch (abit.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("chbBak", "1"); break; }
                    case 17: { acrFlds.SetField("chbMag", "1"); break; }
                    case 18: { acrFlds.SetField("chbSpec", "1"); break; }
                }

                if (abit.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (person.PersonAddInfo.HostelAbit ?? false)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                if (person.Sex ?? false)
                    acrFlds.SetField("Male", "1");
                else
                    acrFlds.SetField("Female", "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality.Name);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = (PersonContacts.Code ?? "") + " " +
                    (PersonContacts.City ?? "") + " " +
                    (PersonContacts.Street ?? "") + " " +
                    (PersonContacts.House ?? "") + " " +
                    (PersonContacts.Korpus ?? "") + " " +
                    (PersonContacts.Flat ?? "");

                string[] strSplit = GetSplittedStrings(Address, 50, 70, 2);

                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("Address" + i.ToString(), strSplit[i - 1]);

                string phones = (PersonContacts.Phone ?? "") + ", e-mail: " + email + ", " + (PersonContacts.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                var PersonEducationDocument = person.PersonEducationDocument;
                if (PersonEducationDocument == null)
                    PersonEducationDocument = new PersonEducationDocument();

                var PersonCurrentEducation = person.PersonCurrentEducation;
                if (PersonCurrentEducation == null)
                    PersonCurrentEducation = new PersonCurrentEducation();

                var PersonAddInfo = person.PersonAddInfo;
                if (PersonAddInfo == null)
                    PersonAddInfo = new PersonAddInfo();

                strSplit = GetSplittedStrings(PersonAddInfo.Parents, 50, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Parents" + i.ToString(), strSplit[i - 1]);

                acrFlds.SetField("CurrentEducationName", PersonCurrentEducation.EducName);
                acrFlds.SetField((PersonCurrentEducation.HasAccreditation ?? false) ? "HasAccred" : "NoAccred", "1");
                string AccredInfo = (PersonCurrentEducation.AccreditationNumber ?? "") +
                    (PersonCurrentEducation.AccreditationDate.HasValue ? " от " + PersonCurrentEducation.AccreditationDate.Value.ToShortDateString() : "");
                acrFlds.SetField("EducationAccreditationNumber", (PersonCurrentEducation.HasAccreditation ?? false) ? AccredInfo : "");
                if (PersonCurrentEducation.Semester != null)
                    acrFlds.SetField("CurrentCourse", PersonCurrentEducation.Semester.EducYear.ToString());

                switch (PersonCurrentEducation.StudyLevelId ?? 16)
                {
                    case 16: { acrFlds.SetField("CurrentBak", "1"); break; }
                    case 17: { acrFlds.SetField("CurrentMag", "1"); break; }
                    case 18: { acrFlds.SetField("CurrentSpec", "1"); break; }
                }

                acrFlds.SetField("ExitYear", PersonEducationDocument.SchoolExitYear ?? "");
                acrFlds.SetField("School", PersonEducationDocument.SchoolName ?? "");
                acrFlds.SetField("EducationDocument", (PersonEducationDocument.Series ?? "") + " " + (PersonEducationDocument.Number ?? ""));
                if (PersonCurrentEducation.HasScholarship ?? false)
                    acrFlds.SetField("HasScholarship", "1");
                else
                    acrFlds.SetField("NoScholarship", "1");

                acrFlds.SetField("Extra", PersonAddInfo.AddInfo ?? "");
                acrFlds.SetField("Copy", "1");

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //перевод иностранцев (AbitTypeId = 4)
        public static byte[] GetApplicationPDFTransferForeign(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                Specialization = x.Entry.ProfileName,
                                StudyFormId = x.Entry.StudyFormId,
                                StudyFormName = x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                x.EntryType,
                                x.HostelEduc,
                                SemesterName = x.Entry.Semester.Name,
                                EducYear = x.Entry.Semester.EducYear,
                                x.Entry.StudyLevelId
                            }).FirstOrDefault();
                string email = context.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

                var person = context.Person.Where(x => x.Id == abit.PersonId).FirstOrDefault();

                var PersonContacts = person.PersonContacts;
                if (PersonContacts == null)
                    PersonContacts = new PersonContacts();

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
                string code = (4000000 + abit.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
                acrFlds.SetField("Specialization", abit.Specialization);
                acrFlds.SetField("Faculty", abit.Faculty);
                acrFlds.SetField("ObrazProgram", abit.ObrazProgram);
                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");

                acrFlds.SetField("Course", abit.EducYear.ToString());
                acrFlds.SetField("Semester", abit.SemesterName);

                switch (abit.StudyLevelId)
                {
                    case 16: { acrFlds.SetField("chbBak", "1"); break; }
                    case 17: { acrFlds.SetField("chbMag", "1"); break; }
                    case 18: { acrFlds.SetField("chbSpec", "1"); break; }
                }

                if (abit.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (person.PersonAddInfo.HostelAbit ?? false)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                if (person.Sex ?? false)
                    acrFlds.SetField("Male", "1");
                else
                    acrFlds.SetField("Female", "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality.Name);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = (PersonContacts.Code ?? "") + " " +
                    (PersonContacts.City ?? "") + " " +
                    (PersonContacts.Street ?? "") + " " +
                    (PersonContacts.House ?? "") + " " +
                    (PersonContacts.Korpus ?? "") + " " +
                    (PersonContacts.Flat ?? "");

                string[] strSplit = GetSplittedStrings(Address, 40, 70, 2);

                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("Address" + i.ToString(), strSplit[i - 1]);

                string phones = (PersonContacts.Phone ?? "") + ", e-mail: " + email + ", " + (PersonContacts.Mobiles ?? "");

                strSplit = GetSplittedStrings(phones, 70, 70, 2);
                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                var PersonEducationDocument = person.PersonEducationDocument;
                if (PersonEducationDocument == null)
                    PersonEducationDocument = new PersonEducationDocument();

                var PersonCurrentEducation = person.PersonCurrentEducation;
                if (PersonCurrentEducation == null)
                    PersonCurrentEducation = new PersonCurrentEducation();

                var PersonAddInfo = person.PersonAddInfo;
                if (PersonAddInfo == null)
                    PersonAddInfo = new PersonAddInfo();

                strSplit = GetSplittedStrings(PersonAddInfo.Parents, 50, 70, 3);
                for (int i = 1; i <= 3; i++)
                    acrFlds.SetField("Parents" + i.ToString(), strSplit[i - 1]);

                strSplit = GetSplittedStrings(PersonAddInfo.SocialStatus, 50, 70, 2);
                for (int i = 1; i < 3; i++)
                    acrFlds.SetField("SocialStatus" + i.ToString(), strSplit[i - 1]);
                acrFlds.SetField("MaritalStatus", PersonAddInfo.MaritalStatus);

                acrFlds.SetField("CurrentEducationName", PersonCurrentEducation.EducName);
                acrFlds.SetField((PersonCurrentEducation.HasAccreditation ?? false) ? "HasAccred" : "NoAccred", "1");
                string AccredInfo = (PersonCurrentEducation.AccreditationNumber ?? "") +
                    (PersonCurrentEducation.AccreditationDate.HasValue ? PersonCurrentEducation.AccreditationDate.Value.ToShortDateString() : "");
                acrFlds.SetField("EducationAccreditationNumber", (PersonCurrentEducation.HasAccreditation ?? false) ? AccredInfo : "");
                if (PersonCurrentEducation.Semester != null)
                    acrFlds.SetField("CurrentSemester", PersonCurrentEducation.Semester.EducYear.ToString() + " курс, " + PersonCurrentEducation.Semester.Name);

                switch (PersonCurrentEducation.StudyLevelId ?? 16)
                {
                    case 16: { acrFlds.SetField("CurrentBak", "1"); break; }
                    case 17: { acrFlds.SetField("CurrentMag", "1"); break; }
                    case 18: { acrFlds.SetField("CurrentSpec", "1"); break; }
                }

                acrFlds.SetField("ExitYear", PersonEducationDocument.SchoolExitYear ?? "");
                acrFlds.SetField("School", PersonEducationDocument.SchoolName ?? "");
                acrFlds.SetField("SchoolName", PersonEducationDocument.SchoolName ?? "");
                acrFlds.SetField("EducationDocument", (PersonEducationDocument.Series ?? "") + (PersonEducationDocument.Number ?? ""));
                acrFlds.SetField("CountryEduc", PersonEducationDocument.CountryEducId.HasValue ? PersonEducationDocument.CountryEduc.Name : "");

                if (PersonCurrentEducation.HasScholarship ?? false)
                    acrFlds.SetField("HasScholarship", "1");
                else
                    acrFlds.SetField("NoScholarship", "1");

                acrFlds.SetField("Extra", PersonAddInfo.AddInfo ?? "");

                strSplit = GetSplittedStrings(PersonAddInfo.Parents, 30, 70, 3);
                for (int i = 1; i < 4; i++)
                    acrFlds.SetField("Parents", strSplit[i - 1]);

                acrFlds.SetField("Copy", "1");

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //восстановление (AbitTypeId = 5)
        public static byte[] GetApplicationPDFRecover(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                x.Entry.ObrazProgramCrypt,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                Specialization = x.Entry.ProfileName,
                                StudyFormId = x.Entry.StudyFormId,
                                StudyFormName = x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                x.EntryType,
                                x.HostelEduc,
                                SemesterName = x.Entry.Semester.Name,
                                EducYear = x.Entry.Semester.EducYear
                            }).FirstOrDefault();
                string email = context.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

                var person = context.Person.Where(x => x.Id == abit.PersonId).FirstOrDefault();

                var PersonContacts = person.PersonContacts;
                if (PersonContacts == null)
                    PersonContacts = new PersonContacts();

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
                string code = (5000000 + abit.Barcode).ToString();

                //добавляем штрихкод
                Barcode128 barcode = new Barcode128();
                barcode.Code = code;
                PdfContentByte cb = pdfStm.GetOverContent(1);
                iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);

                img.SetAbsolutePosition(280, 780);
                cb.AddImage(img);

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
                acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
                acrFlds.SetField("Specialization", abit.Specialization);
                acrFlds.SetField("Faculty", abit.Faculty);
                acrFlds.SetField("ObrazProgram", abit.ObrazProgramCrypt + " " + abit.ObrazProgram);
                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");

                acrFlds.SetField("Course", abit.EducYear.ToString());
                acrFlds.SetField("Semester", abit.SemesterName);

                if (abit.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (person.PersonAddInfo.HostelAbit ?? false)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                if (person.Sex ?? false)
                    acrFlds.SetField("Male", "1");
                else
                    acrFlds.SetField("Female", "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = (PersonContacts.Code ?? "") + " " +
                    (PersonContacts.City ?? "") + " " +
                    (PersonContacts.Street ?? "") + " " +
                    (PersonContacts.House ?? "") + " " +
                    (PersonContacts.Korpus ?? "") + " " +
                    (PersonContacts.Flat ?? "");

                string[] strSplit = GetSplittedStrings(Address, 50, 70, 2);

                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Address" + i.ToString(), strSplit[i - 1]);

                string phones = (PersonContacts.Phone ?? "") + ", e-mail: " + email + ", "/* + (person.Mobiles ?? "")*/;

                strSplit = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i <= 2; i++)
                    acrFlds.SetField("Phone" + i.ToString(), strSplit[i - 1]);

                var PersonDisorderInfo = person.PersonDisorderInfo;
                if (PersonDisorderInfo == null)
                    PersonDisorderInfo = new PersonDisorderInfo();

                acrFlds.SetField("DisorderYear", PersonDisorderInfo.YearOfDisorder);
                acrFlds.SetField("DisorderProgram1", PersonDisorderInfo.EducationProgramName ?? "");

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //перевод на бюджет (AbitTypeId = 6)
        public static byte[] GetApplicationPDFChangeStudyForm(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                Specialization = x.Entry.ProfileName,
                                StudyFormId = x.Entry.StudyFormId,
                                StudyFormName = x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                x.EntryType,
                                x.HostelEduc,
                            }).FirstOrDefault();
                string email = context.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

                var person = context.Person.Where(x => x.Id == abit.PersonId).FirstOrDefault();

                var PersonContacts = person.PersonContacts;
                if (PersonContacts == null)
                    PersonContacts = new PersonContacts();

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
                string code = (6000000 + abit.Barcode).ToString();

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

                acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") /*+ " " + (person.SecondName ?? "")*/).Trim());
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

                if (person.PersonAddInfo.HostelAbit ?? false)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                if (person.Sex ?? false)
                    acrFlds.SetField("Male", "1");
                else
                    acrFlds.SetField("Female", "1");

                string Reason = "";
                if (person.PersonChangeStudyFormReason != null)
                    Reason = person.PersonChangeStudyFormReason.Reason;

                string[] ss = GetSplittedStrings(Reason, 60, 60, 3);
                for (int i = 1; i <= 3; i++)
                {
                    acrFlds.SetField("Reason" + i, ss[i - 1]);
                }

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality.Name);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                //acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                //acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = (PersonContacts.Code ?? "") + " " +
                    (PersonContacts.City ?? "") + " " +
                    (PersonContacts.Street ?? "") + " " +
                    (PersonContacts.House ?? "") + " " +
                    (PersonContacts.Korpus ?? "") + " " +
                    (PersonContacts.Flat ?? "");

                string[] splitted = GetSplittedStrings(Address, 50, 70, 2);
                for (int i = 1; i < 2; i++)
                {
                    acrFlds.SetField("Address" + i.ToString(), splitted[i - 1]);
                }

                string phones = (PersonContacts.Phone ?? "") + ", e-mail: " + email + ", "/* + (person.Mobiles ?? "")*/;
                splitted = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i < 2; i++)
                {
                    acrFlds.SetField("Phone" + i.ToString(), splitted[i - 1]);
                }

                var PersonEducationDocument = person.PersonEducationDocument;
                if (PersonEducationDocument == null)
                    PersonEducationDocument = new PersonEducationDocument();

                var PersonAddInfo = person.PersonAddInfo;
                if (PersonAddInfo == null)
                    PersonAddInfo = new PersonAddInfo();

                pdfStm.FormFlattening = true;
                pdfStm.Close();
                pdfRd.Close();

                return ms.ToArray();
            }
        }
        //смена обр. программы (AbitTypeId = 7)
        public static byte[] GetApplicationPDFChangeObrazProgram(Guid appId, string dirPath)
        {
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var abit = (from x in context.Application
                            where x.Id == appId
                            select new
                            {
                                x.PersonId,
                                x.Barcode,
                                Faculty = x.Entry.FacultyName,
                                Profession = x.Entry.LicenseProgramName,
                                ProfessionCode = x.Entry.LicenseProgramCode,
                                x.Entry.ObrazProgramCrypt,
                                ObrazProgram = x.Entry.ObrazProgramName,
                                Specialization = x.Entry.ProfileName,
                                StudyFormId = x.Entry.StudyFormId,
                                StudyFormName = x.Entry.StudyFormName,
                                x.Entry.StudyBasisId,
                                x.EntryType,
                                x.HostelEduc,
                            }).FirstOrDefault();
                string email = context.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

                var person = context.Person.Where(x => x.Id == abit.PersonId).FirstOrDefault();

                var PersonContacts = person.PersonContacts;
                if (PersonContacts == null)
                    PersonContacts = new PersonContacts();

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
                string code = (7000000 + abit.Barcode).ToString();

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
                acrFlds.SetField("ObrazProgram", (abit.ObrazProgramCrypt + " " ?? "") + abit.ObrazProgram);
                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");

                if (abit.HostelEduc)
                    acrFlds.SetField("HostelEducYes", "1");
                else
                    acrFlds.SetField("HostelEducNo", "1");

                if (person.PersonAddInfo.HostelAbit ?? false)
                    acrFlds.SetField("HostelAbitYes", "1");
                else
                    acrFlds.SetField("HostelAbitNo", "1");

                if (person.Sex ?? false)
                    acrFlds.SetField("Male", "1");
                else
                    acrFlds.SetField("Female", "1");

                acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
                acrFlds.SetField("BirthPlace", person.BirthPlace);
                acrFlds.SetField("Nationality", person.Nationality.Name);
                acrFlds.SetField("PassportSeries", person.PassportSeries);
                acrFlds.SetField("PassportNumber", person.PassportNumber);
                acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
                acrFlds.SetField("PassportAuthor", person.PassportAuthor);

                string Address = (PersonContacts.Code ?? "") + " " +
                    (PersonContacts.City ?? "") + " " +
                    (PersonContacts.Street ?? "") + " " +
                    (PersonContacts.House ?? "") + " " +
                    (PersonContacts.Korpus ?? "") + " " +
                    (PersonContacts.Flat ?? "");

                string[] splitted = GetSplittedStrings(Address, 30, 70, 2);
                for (int i = 1; i < 2; i++)
                {
                    acrFlds.SetField("Address" + i.ToString(), splitted[i - 1]);
                }

                string phones = (PersonContacts.Phone ?? "") + ", e-mail: " + email + ", "/* + (person.Mobiles ?? "")*/;
                splitted = GetSplittedStrings(phones, 30, 70, 2);
                for (int i = 1; i < 2; i++)
                {
                    acrFlds.SetField("Phone" + i.ToString(), splitted[i - 1]);
                }

                var PersonEducationDocument = person.PersonEducationDocument;
                if (PersonEducationDocument == null)
                    PersonEducationDocument = new PersonEducationDocument();

                var PersonAddInfo = person.PersonAddInfo;
                if (PersonAddInfo == null)
                    PersonAddInfo = new PersonAddInfo();

                var PersonCurrentEducation = person.PersonCurrentEducation;
                if (PersonCurrentEducation == null)
                    PersonCurrentEducation = new OnlineAbit2013.PersonCurrentEducation();

                acrFlds.SetField("CurrentProfession", PersonCurrentEducation.LicenseProgramId.HasValue ?
                    context.Entry.Where(x => x.LicenseProgramId == PersonCurrentEducation.LicenseProgramId)
                    .Select(x => x.LicenseProgramName).FirstOrDefault() : "");
                acrFlds.SetField("CurrentCourse", PersonCurrentEducation.SemesterId.HasValue ? PersonCurrentEducation.Semester.EducYear.ToString() : "-");
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
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", appId } });

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
                     HostelEduc = rw.Field<bool>("HostelEduc"),
                     ClassNum = rw.Field<string>("EntryClassNum"),
                     ManualExam = rw.Field<string>("ManualExam")
                 }).FirstOrDefault();

            query = "SELECT Surname, Name, SecondName FROM Person WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });

            var person =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName"),
                 }).FirstOrDefault();

            query = "SELECT PrintName, DocumentNumber FROM AG_AllPriveleges WHERE PersonId=@PersonId";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
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
            if (abit.HostelEduc)
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
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", appId }, { "@PersonId", abit.PersonId } });
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

        public static byte[] GetApplicationPDF_SPO(Guid appId, string dirPath)
        {
            string query = "SELECT Application.Barcode, PersonId, FacultyName, LicenseProgramName, LicenseProgramCode, ObrazProgramName, Entry.StudyFormId, " +
                " Entry.StudyBasisId, Entry.ProfileName, HostelEduc " +
                " FROM [Application] INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE [Application].Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", appId } });

            var abit =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     PersonId = rw.Field<Guid?>("PersonId"),
                     Barcode = rw.Field<int>("Barcode"),
                     Faculty = rw.Field<string>("FacultyName"),
                     Profession = rw.Field<string>("LicenseProgramName"),
                     ProfessionCode = rw.Field<string>("LicenseProgramCode"),
                     ObrazProgram = rw.Field<string>("ObrazProgramName"),
                     StudyFormId = rw.Field<int>("StudyFormId"),
                     StudyBasisId = rw.Field<int>("StudyBasisId"),
                     HostelEduc = rw.Field<bool>("HostelEduc")
                 }).FirstOrDefault();

            query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });
            string email = tbl.Rows[0].Field<string>("Email");
            //string email = Util.ABDB.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

            query = "SELECT Barcode, Surname, Name, SecondName, HostelAbit, BirthDate, BirthPlace, Sex, Nationality, Country, PassportType, PassportSeries, " +
                " PassportNumber, PassportDate, PassportCode, PassportAuthor, PassportCode, Code, City, Street, House, Korpus, Flat, Region, Phone, " +
                " Mobiles, SchoolTypeId, SchoolType, SchoolExitYear, SchoolName, AttestatRegion, AttestatSeries, AttestatNumber, Series, Number, " +
                " AddInfo, Parents, CountryEduc, Language, Stag, WorkPlace, OtherSportQualification, SportQualificationId, SportQualificationName, " +
                " SportQualificationLevel, HasPrivileges FROM extPerson_SPO WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });

            var person =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     //Barcode = rw.Field<int>("Barcode"),
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName") ?? "",
                     HostelAbit = rw.Field<bool>("HostelAbit"),
                     BirthDate = rw.Field<DateTime?>("BirthDate"),
                     BirthPlace = rw.Field<string>("BirthPlace") ?? "",
                     Sex = rw.Field<bool>("Sex"),
                     Nationality = rw.Field<string>("Nationality"),
                     Country = rw.Field<string>("Country"),
                     PassportType = rw.Field<string>("PassportType") ?? "",
                     PassportSeries = rw.Field<string>("PassportSeries") ?? "",
                     PassportNumber = rw.Field<string>("PassportNumber") ?? "",
                     PassportDate = rw.Field<DateTime?>("PassportDate"),
                     PassportAuthor = rw.Field<string>("PassportAuthor") ?? "",
                     PassportCode = rw.Field<string>("PassportCode") ?? "",
                     PostCode = rw.Field<string>("Code") ?? "",
                     City = rw.Field<string>("City") ?? "",
                     Street = rw.Field<string>("Street") ?? "",
                     House = rw.Field<string>("House") ?? "",
                     Korpus = rw.Field<string>("Korpus") ?? "",
                     Flat = rw.Field<string>("Flat") ?? "",
                     Region = rw.Field<string>("Region"),
                     Phone = rw.Field<string>("Phone") ?? "",
                     Mobiles = rw.Field<string>("Mobiles") ?? "",
                     SchoolTypeId = rw.Field<int>("SchoolTypeId"),
                     SchoolType = rw.Field<string>("SchoolType") ?? "",
                     SchoolExitYear = rw.Field<string>("SchoolExitYear"),
                     SchoolName = rw.Field<string>("SchoolName") ?? "",
                     AttestatRegion = rw.Field<string>("AttestatRegion") ?? "",
                     AttestatSeries = rw.Field<string>("AttestatSeries") ?? "",
                     AttestatNum = rw.Field<string>("AttestatNumber") ?? "",
                     DiplomaSeries = rw.Field<string>("Series") ?? "",
                     DiplomaNumber = rw.Field<string>("Number") ?? "",
                     AddInfo = rw.Field<string>("AddInfo") ?? "",
                     Parents = rw.Field<string>("Parents") ?? "",
                     SchoolCountry = rw.Field<string>("CountryEduc"),
                     Language = rw.Field<string>("Language"),
                     SportQualification = rw.Field<string>("OtherSportQualification"),
                     SportQualificationId = rw.Field<int?>("SportQualificationId"),
                     SportQualificationName = rw.Field<string>("SportQualificationName"),
                     SportQualificationLevel = rw.Field<string>("SportQualificationLevel"),
                     HasPrivilege = rw.Field<bool?>("HasPrivileges") ?? false
                 }).FirstOrDefault();

            MemoryStream ms = new MemoryStream();
            string dotName = "Application_SPO.pdf";

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
            int multiplyer = 9;
            string code = (9000000 + abit.Barcode).ToString();

            //добавляем штрихкод
            Barcode128 barcode = new Barcode128();
            barcode.Code = code;
            PdfContentByte cb = pdfStm.GetOverContent(1);
            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            img.SetAbsolutePosition(440, 740);
            cb.AddImage(img);

            acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
            acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
            //acrFlds.SetField("Specialization", abit.Specialization);
            acrFlds.SetField("Faculty", abit.Faculty);
            acrFlds.SetField("ObrazProgram", abit.ObrazProgram);
            acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
            acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");
            if (abit.HostelEduc)
                acrFlds.SetField("HostelEducYes", "1");
            else
                acrFlds.SetField("HostelEducNo", "1");
            acrFlds.SetField("HostelAbitYes", person.HostelAbit ? "1" : "0");
            acrFlds.SetField("HostelAbitNo", person.HostelAbit ? "0" : "1");
            acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
            acrFlds.SetField("BirthPlace", person.BirthPlace);
            acrFlds.SetField("Male", person.Sex ? "1" : "0");
            acrFlds.SetField("Female", person.Sex ? "0" : "1");
            acrFlds.SetField("Nationality", person.Nationality);
            acrFlds.SetField("PassportSeries", person.PassportSeries);
            acrFlds.SetField("PassportNumber", person.PassportNumber);
            acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
            acrFlds.SetField("PassportAuthor", person.PassportAuthor);
            acrFlds.SetField("Privileges", person.HasPrivilege ? "1" : "0");
            acrFlds.SetField("Address1", string.Format("{0} {1}, {2}, ", person.PostCode ?? "", person.Region ?? "", person.City ?? ""));
            acrFlds.SetField("Address2", string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом "
                + person.House, person.Korpus == string.Empty ? "" : "корп. " + person.Korpus, person.Flat == string.Empty ? "" : "кв. " + person.Flat));

            string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");
            int index = 0, startindex = 0;
            for (int i = 1; i < 3; i++)
            {
                if (phones.Length > startindex && startindex >= 0)
                {
                    int rowLength = 30;//длина строки, разная у первых строк
                    if (i > 1) //длина остальных строк одинакова
                        rowLength = 70;
                    index = startindex + rowLength;
                    if (index < phones.Length)
                    {
                        index = phones.IndexOf(" ", index);
                        string val = index > 0 ?
                            phones.Substring(startindex, index - startindex)
                            : phones.Substring(startindex);
                        acrFlds.SetField("Phone" + i.ToString(), val);
                    }
                    else
                        acrFlds.SetField("Phone" + i.ToString(),
                            phones.Substring(startindex));
                }
                startindex = index;
            }

            if (!string.IsNullOrEmpty(person.SchoolName))
                acrFlds.SetField("chbSchoolFinished", "1");

            acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
            acrFlds.SetField("School", person.SchoolName ?? "");
            acrFlds.SetField("Original", "0");
            acrFlds.SetField("Copy", "0");
            acrFlds.SetField("CountryEduc", person.SchoolCountry ?? "");
            acrFlds.SetField("Language", person.Language ?? "");
            acrFlds.SetField("Attestat", person.SchoolTypeId == 1 ?
                (person.AttestatRegion ?? "") + " " + (person.AttestatSeries ?? "") + " " + (person.AttestatNum) :
                (person.DiplomaSeries ?? "") + " " + (person.DiplomaNumber ?? ""));
            acrFlds.SetField("Extra", person.AddInfo ?? "");

            if (person.SportQualificationId.HasValue && person.SportQualificationId.Value != 44)
                acrFlds.SetField("SportQualification", (person.SportQualificationName ?? "") + "; pазряд: " + person.SportQualificationLevel ?? "-");
            else
                acrFlds.SetField("SportQualification", person.SportQualification ?? "");

            string extraPerson = person.Parents ?? "";
            int ind = 0, startInd = 0;
            for (int i = 1; i < 3; i++)
            {
                if (extraPerson.Length > startInd && startInd >= 0)
                {
                    ind = startInd + 70;
                    if (ind < extraPerson.Length)
                    {
                        ind = extraPerson.IndexOf(" ", ind);
                        acrFlds.SetField("Parents" + i.ToString(),
                            ind > 0 ?
                            extraPerson.Substring(startInd, ind - startInd)
                            : extraPerson.Substring(startInd));
                    }
                    else
                        acrFlds.SetField("Parents" + i.ToString(),
                            extraPerson.Substring(startInd));
                }
                startInd = ind;
            }

            //EGE
            query = "SELECT ExamName, MarkValue, Number FROM EgeMarksAll WHERE PersonId=@PersonId";
            DataTable _tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
            var exams =
                from DataRow rw in _tbl.Rows
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

            // work&sport
            var work =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         WorkPlace = rw.Field<string>("WorkPlace"),
                         Stage = rw.Field<string>("Stag")
                     }).FirstOrDefault();
            if (!string.IsNullOrEmpty(work.WorkPlace) && !string.IsNullOrEmpty(work.Stage))
            {
                acrFlds.SetField("HasStag", "1");
                acrFlds.SetField("WorkPlace", work.WorkPlace);
                acrFlds.SetField("Stag", work.Stage);
            }
            else
                acrFlds.SetField("NoStag", "1");

            int[] ssuz = new int[] { 2, 5 };
            if (ssuz.Contains(person.SchoolTypeId))//no school, no vuz, no NPO
            {
                acrFlds.SetField("HasEduc", "1");
                acrFlds.SetField("HighEducation", person.SchoolName);
            }
            else
            {
                acrFlds.SetField("NoEduc", "1");
            }
            /*
            if (abit.EntryType != 2)//no mag application
            {
                acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
                query = "SELECT WorkPlace, WorkProfession, Stage FROM PersonWork WHERE PersonId=@PersonId";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
                

                //bacheloor - specialist
                if (!string.IsNullOrEmpty(abit.ProfessionCode))
                {
                    if (abit.ProfessionCode.EndsWith("00"))
                        acrFlds.SetField("chbBak", "1");
                    else
                        acrFlds.SetField("chbSpec", "1");
                }
            }
            */
            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static byte[] GetApplicationPDF_Aspirant(Guid appId, string dirPath)
        {
            string query = "SELECT Application.Barcode, PersonId, FacultyName, LicenseProgramName, LicenseProgramCode, ObrazProgramName, Entry.StudyFormId, " +
                " Entry.StudyBasisId, Entry.ProfileName, HostelEduc " +
                " FROM [Application] INNER JOIN Entry ON [Application].EntryId=Entry.Id WHERE [Application].Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", appId } });

            var abit =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     PersonId = rw.Field<Guid?>("PersonId"),
                     Barcode = rw.Field<int>("Barcode"),
                     Faculty = rw.Field<string>("FacultyName"),
                     Profession = rw.Field<string>("LicenseProgramName"),
                     ProfessionCode = rw.Field<string>("LicenseProgramCode"),
                     ObrazProgram = rw.Field<string>("ObrazProgramName"),
                     StudyFormId = rw.Field<int>("StudyFormId"),
                     StudyBasisId = rw.Field<int>("StudyBasisId"),
                     HostelEduc = rw.Field<bool>("HostelEduc")
                 }).FirstOrDefault();

            query = "SELECT Email, IsForeign FROM [User] WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });
            string email = tbl.Rows[0].Field<string>("Email");
            //string email = Util.ABDB.User.Where(x => x.Id == abit.PersonId).Select(x => x.Email).FirstOrDefault();

            query = "SELECT Barcode, Surname, Name, SecondName, AbitHostel AS HostelAbit, BirthDate, BirthPlace, Sex, Nationality, Country, PassportType, PassportSeries, " +
                " PassportNumber, PassportDate, PassportCode, PassportAuthor, PassportCode, Code, City, Street, House, Korpus, Flat, Region, Phone, " +
                " Mobiles, SchoolTypeId, SchoolType, SchoolExitYear, SchoolName, EducationDocumentSeries AS Series, EducationDocumentNumber AS Number, " +
                " AddInfo, Parents, CountryEduc, Language, HasPrivileges FROM extPerson_All WHERE Id=@Id";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", abit.PersonId } });

            var person =
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     //Barcode = rw.Field<int>("Barcode"),
                     Surname = rw.Field<string>("Surname"),
                     Name = rw.Field<string>("Name"),
                     SecondName = rw.Field<string>("SecondName") ?? "",
                     HostelAbit = rw.Field<bool>("HostelAbit"),
                     BirthDate = rw.Field<DateTime?>("BirthDate"),
                     BirthPlace = rw.Field<string>("BirthPlace") ?? "",
                     Sex = rw.Field<bool>("Sex"),
                     Nationality = rw.Field<string>("Nationality"),
                     Country = rw.Field<string>("Country"),
                     PassportType = rw.Field<string>("PassportType") ?? "",
                     PassportSeries = rw.Field<string>("PassportSeries") ?? "",
                     PassportNumber = rw.Field<string>("PassportNumber") ?? "",
                     PassportDate = rw.Field<DateTime?>("PassportDate"),
                     PassportAuthor = rw.Field<string>("PassportAuthor") ?? "",
                     PassportCode = rw.Field<string>("PassportCode") ?? "",
                     PostCode = rw.Field<string>("Code") ?? "",
                     City = rw.Field<string>("City") ?? "",
                     Street = rw.Field<string>("Street") ?? "",
                     House = rw.Field<string>("House") ?? "",
                     Korpus = rw.Field<string>("Korpus") ?? "",
                     Flat = rw.Field<string>("Flat") ?? "",
                     Region = rw.Field<string>("Region"),
                     Phone = rw.Field<string>("Phone") ?? "",
                     Mobiles = rw.Field<string>("Mobiles") ?? "",
                     SchoolTypeId = rw.Field<int>("SchoolTypeId"),
                     SchoolType = rw.Field<string>("SchoolType") ?? "",
                     SchoolExitYear = rw.Field<string>("SchoolExitYear"),
                     SchoolName = rw.Field<string>("SchoolName") ?? "",
                     DiplomaSeries = rw.Field<string>("Series") ?? "",
                     DiplomaNumber = rw.Field<string>("Number") ?? "",
                     AddInfo = rw.Field<string>("AddInfo") ?? "",
                     Parents = rw.Field<string>("Parents") ?? "",
                     SchoolCountry = rw.Field<string>("CountryEduc"),
                     Language = rw.Field<string>("Language"),
                     HasPrivilege = rw.Field<bool?>("HasPrivileges") ?? false
                 }).FirstOrDefault();

            MemoryStream ms = new MemoryStream();
            string dotName = "ApplicationAspirant2013.pdf";

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
            string code = "A" + ((1000000 + abit.Barcode).ToString()).Substring(1);

            //добавляем штрихкод
            Barcode128 barcode = new Barcode128();
            barcode.Code = code;
            PdfContentByte cb = pdfStm.GetOverContent(1);
            iTextSharp.text.Image img = barcode.CreateImageWithBarcode(cb, null, null);
            img.SetAbsolutePosition(440, 740);
            cb.AddImage(img);

            acrFlds.SetField("FIO", ((person.Surname ?? "") + " " + (person.Name ?? "") + " " + (person.SecondName ?? "")).Trim());
            acrFlds.SetField("Profession", abit.ProfessionCode + " " + abit.Profession);
            //acrFlds.SetField("Specialization", abit.Specialization);
            acrFlds.SetField("Faculty", abit.Faculty);
            acrFlds.SetField("ObrazProgram", abit.ObrazProgram);
            acrFlds.SetField("StudyForm" + abit.StudyFormId, "1");
            acrFlds.SetField("StudyBasis" + abit.StudyBasisId, "1");
            if (abit.HostelEduc)
                acrFlds.SetField("HostelEducYes", "1");
            else
                acrFlds.SetField("HostelEducNo", "1");
            acrFlds.SetField("HostelAbitYes", person.HostelAbit ? "1" : "0");
            acrFlds.SetField("HostelAbitNo", person.HostelAbit ? "0" : "1");
            acrFlds.SetField("BirthDate", person.BirthDate.Value.ToShortDateString());
            acrFlds.SetField("BirthPlace", person.BirthPlace);
            acrFlds.SetField("Male", person.Sex ? "1" : "0");
            acrFlds.SetField("Female", person.Sex ? "0" : "1");
            acrFlds.SetField("Nationality", person.Nationality);
            acrFlds.SetField("PassportSeries", person.PassportSeries);
            acrFlds.SetField("PassportNumber", person.PassportNumber);
            acrFlds.SetField("PassportDate", person.PassportDate.Value.ToShortDateString());
            acrFlds.SetField("PassportAuthor", person.PassportAuthor);
            acrFlds.SetField("Privileges", person.HasPrivilege ? "1" : "0");
            acrFlds.SetField("Address1", string.Format("{0} {1}, {2}, ", person.PostCode ?? "", person.Region ?? "", person.City ?? ""));
            acrFlds.SetField("Address2", string.Format("{0} {1} {2} {3}", person.Street ?? "", person.House == string.Empty ? "" : "дом "
                + person.House, person.Korpus == string.Empty ? "" : "корп. " + person.Korpus, person.Flat == string.Empty ? "" : "кв. " + person.Flat));

            string phones = (person.Phone ?? "") + ", e-mail: " + email + ", " + (person.Mobiles ?? "");
            int index = 0, startindex = 0;
            for (int i = 1; i < 3; i++)
            {
                if (phones.Length > startindex && startindex >= 0)
                {
                    int rowLength = 30;//длина строки, разная у первых строк
                    if (i > 1) //длина остальных строк одинакова
                        rowLength = 70;
                    index = startindex + rowLength;
                    if (index < phones.Length)
                    {
                        index = phones.IndexOf(" ", index);
                        string val = index > 0 ?
                            phones.Substring(startindex, index - startindex)
                            : phones.Substring(startindex);
                        acrFlds.SetField("Phone" + i.ToString(), val);
                    }
                    else
                        acrFlds.SetField("Phone" + i.ToString(),
                            phones.Substring(startindex));
                }
                startindex = index;
            }

            if (!string.IsNullOrEmpty(person.SchoolName))
                acrFlds.SetField("chbSchoolFinished", "1");

            acrFlds.SetField("ExitYear", person.SchoolExitYear.ToString());
            acrFlds.SetField("School", person.SchoolName ?? "");
            acrFlds.SetField("Original", "0");
            acrFlds.SetField("Copy", "0");
            acrFlds.SetField("CountryEduc", person.SchoolCountry ?? "");
            acrFlds.SetField("Language", person.Language ?? "");
            acrFlds.SetField("Attestat", (person.DiplomaSeries ?? "") + " " + (person.DiplomaNumber ?? ""));
            acrFlds.SetField("Extra", person.AddInfo ?? "");

            // work&sport
            query = "SELECT Stage, WorkPlace, WorkProfession FROM PersonWork WHERE PersonId=@PersonId ORDER BY Stage DESC";
            tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonId", abit.PersonId } });
            var work =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         WorkPlace = rw.Field<string>("WorkPlace") + "; " + rw.Field<string>("WorkProfession"),
                         Stage = rw.Field<string>("Stage")
                     }).FirstOrDefault();
            if (work != null && !string.IsNullOrEmpty(work.WorkPlace) && !string.IsNullOrEmpty(work.Stage))
            {
                acrFlds.SetField("HasStag", "1");
                acrFlds.SetField("WorkPlace", work.WorkPlace);
                acrFlds.SetField("Stag", work.Stage);
            }
            else
                acrFlds.SetField("NoStag", "1");

            int[] ssuz = new int[] { 2, 5 };
            if (ssuz.Contains(person.SchoolTypeId))//no school, no vuz, no NPO
            {
                acrFlds.SetField("HasEduc", "1");
                acrFlds.SetField("HighEducation", person.SchoolName);
            }
            else
            {
                acrFlds.SetField("NoEduc", "1");
            }
            
            pdfStm.FormFlattening = true;
            pdfStm.Close();
            pdfRd.Close();

            return ms.ToArray();
        }

        public static string[] GetSplittedStrings(string sourceStr, int firstStrLen, int strLen, int numOfStrings)
        {
            sourceStr = sourceStr ?? "";
            string[] retStr = new string[numOfStrings];
            int index = 0, startindex = 0;
            for (int i = 1; i < numOfStrings; i++)
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
    }
}