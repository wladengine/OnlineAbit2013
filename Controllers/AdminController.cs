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
    public class AdminController : Controller
    {
        private DataTable GetEntryList(string faculty, string profession, string obrazprogram, string profile)
        {
            SortedList<string, object> dic = new SortedList<string, object>();
            int iFacultyId = 0;
            if (!int.TryParse(faculty, out iFacultyId))
                iFacultyId = 1;

            int iProfessionId = 0;
            if (int.TryParse(profession, out iProfessionId))
                dic.Add("@ProfessionId", iProfessionId);

            int iObrazProgramId = 0;
            if (int.TryParse(obrazprogram, out iObrazProgramId))
                dic.Add("@ObrazProgramId", iObrazProgramId);

            int iProfileId = 0;
            if (int.TryParse(profile, out iProfileId))
                dic.Add("@ProfileId", iProfileId);

            //string query = "SELECT Person.Surname, Person.Name, Person.SecondName, extAbitsAll.Profession, extAbitsAll.ObrazProgram, " +
            //    "extAbitsAll.Specialization, extAbitsAll.StudyForm, extAbitsAll.StudyBasis FROM extAbitsAll " +
            //    "INNER JOIN Person ON Person.Id=extAbitsAll.PersonId WHERE extAbitsAll.FacultyId=";

            return null;
        }

        public ActionResult Index()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (!Util.CheckAdminRights(PersonId))
                return View("Restricted");

            return View();
        }

        public ActionResult AbitCountList(string entrytype)
        {
            int iEntryType = 1;
            if (!int.TryParse(entrytype, out iEntryType))
                iEntryType = 1;
            string query = "SELECT FacultyId, Faculty, ProfessionId, Profession, ObrazProgramId, ObrazProgram, SpecializationId, Specialization, " +
                " Count(Id) AS 'CNT' FROM extApplicationAll WHERE StudyBasisId={0} AND EntryType=@EType GROUP BY FacultyId, Faculty, ProfessionId, Profession, ObrazProgramId, " +
                " ObrazProgram, SpecializationId, Specialization";
            DataTable tblBudzh = Util.AbitDB.GetDataTable(string.Format(query, "1"), new SortedList<string, object>() { { "@EType", iEntryType } });
            DataTable tblPlatn = Util.AbitDB.GetDataTable(string.Format(query, "2"), new SortedList<string, object>() { { "@EType", iEntryType } });

            var cntB =
                from DataRow rw in tblBudzh.Rows
                select new
                {
                    FacId = rw.Field<int>("FacultyId"),
                    FacName = rw.Field<string>("Faculty"),
                    ProgramId = rw.Field<int>("ProfessionId"),
                    ProgramName = rw.Field<string>("Profession"),
                    ObrazProgramId = rw.Field<int>("ObrazProgramId"),
                    ObrazProgramName = rw.Field<string>("ObrazProgram"),
                    SpecializationId = rw.Field<int?>("SpecializationId"),
                    SpecializationName = rw.Field<string>("Specialization"),
                    Cnt = rw.Field<int>("CNT")
                };

            var cntP =
                from DataRow rw in tblPlatn.Rows
                select new
                {
                    FacId = rw.Field<int>("FacultyId"),
                    FacName = rw.Field<string>("Faculty"),
                    ProgramId = rw.Field<int>("ProfessionId"),
                    ProgramName = rw.Field<string>("Profession"),
                    ObrazProgramId = rw.Field<int>("ObrazProgramId"),
                    ObrazProgramName = rw.Field<string>("ObrazProgram"),
                    SpecializationId = rw.Field<int?>("SpecializationId"),
                    SpecializationName = rw.Field<string>("Specialization"),
                    Cnt = rw.Field<int>("CNT")
                };

            var full = cntB.Select(x => new { x.FacId, x.FacName, x.ProgramId, x.ProgramName, x.ObrazProgramId, x.ObrazProgramName, x.SpecializationId, x.SpecializationName }).
                Union(cntP.Select(x => new { x.FacId, x.FacName, x.ProgramId, x.ProgramName, x.ObrazProgramId, x.ObrazProgramName, x.SpecializationId, x.SpecializationName })).
                Distinct();

            AdminAbitsModel mdl = new AdminAbitsModel();
            mdl.List = new List<AbitCountModel>();
            foreach (var c in full.OrderBy(x => x.ObrazProgramName).OrderBy(x => x.ProgramName).OrderBy(x => x.FacId))
            {
                int iCntPlat = cntP.Where(x => x.FacId == c.FacId && x.ProgramId == c.ProgramId
                    && x.ObrazProgramId == c.ObrazProgramId && x.SpecializationId == c.SpecializationId).
                    Select(x => x.Cnt).DefaultIfEmpty(0).First();
                int iCntBudzh = cntB.Where(x => x.FacId == c.FacId && x.ProgramId == c.ProgramId
                    && x.ObrazProgramId == c.ObrazProgramId && x.SpecializationId == c.SpecializationId).
                    Select(x => x.Cnt).DefaultIfEmpty(0).First();
                mdl.List.Add(new AbitCountModel()
                {
                    Faculty = new KeyValuePair<int, string>(c.FacId, c.FacName),
                    Profession = new KeyValuePair<int, string>(c.ProgramId, c.ProgramName),
                    ObrazProgram = new KeyValuePair<int, string>(c.ObrazProgramId, c.ObrazProgramName),
                    Profile = new KeyValuePair<int, string>(c.SpecializationId.HasValue ? c.SpecializationId.Value : -1, c.SpecializationName),
                    CntBudzh = iCntBudzh.ToString(),
                    CntPlatn = iCntPlat.ToString()
                });
            }

            return View(mdl);
        }

        public ActionResult EntryList(string faculty)
        {
            return Json(null);
        }

        public ActionResult SetDorms()
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return RedirectToAction("LogOn", "Account");

            if (!Util.CheckAdminRights(PersonId))
                return View("Restricted");

            try
            {
                string query = @"SELECT DISTINCT Person.Barcode, 
(CASE WHEN Person.RegionId = 1 THEN CAST('False' AS bit) ELSE CAST('True' AS bit) END) AS IsSpb
FROM ed.Abiturient
INNER JOIN ed.Person ON Person.Id = Abiturient.PersonId
INNER JOIN ed.extEntryView ON extEntryView.AbiturientId = Abiturient.Id
WHERE Person.Barcode > 0";
                DataTable tbl = Util.Priem2012DB.GetDataTable(query, null);
                var barcsPriem =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Barcode = rw.Field<int>("Barcode"),
                         IsSpb = rw.Field<bool>("IsSpb")
                     }).ToList();

                query = @"SELECT PersonBarcode FROM ApprovedHostel";
                tbl = Util.AbitDB.GetDataTable(query, null);
                List<int> lstExists =
                    (from DataRow rw in tbl.Rows
                     select rw.Field<int>("PersonBarcode")).ToList();

                var toInsert = barcsPriem.Where(x => !lstExists.Contains(x.Barcode));
                query = "INSERT INTO ApprovedHostel (PersonBarcode, IsFirstCourse, IsSpb) VALUES (@PersonBarcode, @IsFirstCourse, @IsSpb)";
                SortedList<string, object> dic = new SortedList<string, object>();
                foreach (var ins in toInsert)
                {
                    dic.Clear();
                    dic.Add("@PersonBarcode", ins.Barcode);
                    dic.Add("@IsSpb", ins.IsSpb);
                    dic.Add("@IsFirstCourse", true);
                    Util.AbitDB.ExecuteQuery(query, dic);
                }

                return Json("OK");
            }
            catch
            {
                return Json("GG");
            }
        }
    }
}
