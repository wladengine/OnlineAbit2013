using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

using System.Transactions;

using OnlineAbit2013.Models;

namespace OnlineAbit2013.Controllers
{
    public class DormsController : Controller
    {
        //
        // GET: /Dorms/
        private const int iStartTime = 9;
        private const int iEndTime = 19;
        private const int iMaxQue = 2;

        public ActionResult Index()
        {
            DormsModel model = new DormsModel();

            Guid UserId;
            if (Util.CheckAuthCookies(Request.Cookies, out UserId))
                model.isRegistered = true;
            else
                model.isRegistered = false;

            string query = @"SELECT Person.Barcode, ApprovedHostel.IsFirstCourse, ApprovedHostel.IsSpb FROM Person INNER JOIN ApprovedHostel ON ApprovedHostel.PersonBarcode=Person.Barcode WHERE Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@Id", UserId } });
            int? barc = null;
            DateTime? time = null;
            int? DormsId = null;

            if (tbl.Rows.Count > 0)
            {
                model.isFirstCourse = tbl.Rows[0].Field<bool?>("IsFirstCourse") ?? true;
                model.isSPB = tbl.Rows[0].Field<bool?>("IsSpb") ?? false;
                barc = tbl.Rows[0].Field<int>("Barcode");
                model.hasInEntered = true;
            }
            else
            {
                model.hasInEntered = false;
            }

            model.Rows = new List<TimetableRow>();
            int index = 0;
            for (int i = iStartTime; i <= iEndTime; i++)
            {
                model.Rows.Add(new TimetableRow() { Hour = i, Cells = new List<TimetableCell>() });

                for (int minutes = 0; minutes < 10; minutes++)
                    model.Rows[index].Cells.Add(new TimetableCell() { Minute = minutes * 5, isLocked = false, CountAbits = 0 });

                index++;
            }

            if (barc.HasValue)
            {
                //вычисляем, когда юзер регистрировался в последний раз
                query = "SELECT TOP (1) Id, Date, DormsId FROM Timetable WHERE PersonBarcode=@PersonBarcode ORDER BY Date";
                tbl = Util.AbitDB.GetDataTable(query, new Dictionary<string, object>() { { "@PersonBarcode", barc } });
                //если есть регистрация, то заполняем дату, общагу в модели
                if (tbl.Rows.Count > 0)
                {
                    time = tbl.Rows[0].Field<DateTime?>("Date");
                    DormsId = tbl.Rows[0].Field<int?>("DormsId");
                    model.DormsId = DormsId;
                    model.regDate = time.Value.Date;
                }

                if (time.HasValue && DormsId.HasValue)
                {
                    query = @"SELECT Date, COUNT(Id) AS CNT FROM Timetable WHERE CONVERT(date, Date)=@Date AND DormsId=@DormsId GROUP BY Date 
UNION
SELECT Date, COUNT(Id) AS CNT FROM TimetableLock WHERE CONVERT(date, Date)=@Date AND DormsId=@DormsId AND LockTime > @LockTime GROUP BY Date";
                    tbl = Util.AbitDB.GetDataTable(query,
                        new Dictionary<string, object>()
                        { 
                            { "@Date", time.Value.Date }, { "@DormsId", DormsId.Value }, { "@LockTime", DateTime.Now.AddMinutes(-30) } 
                        });
                    foreach (DataRow rw in tbl.Rows)
                    {
                        DateTime dtFromRow = rw.Field<DateTime>("Date");
                        int cnt = rw.Field<int>("CNT");
                        model.Rows.Where(x => x.Hour == dtFromRow.Hour).Select(x => x.Cells).First()
                            .Where(x => x.Minute == dtFromRow.Minute).First().CountAbits = cnt;
                        if (cnt >= iMaxQue)
                            model.Rows.Where(x => x.Hour == dtFromRow.Hour).Select(x => x.Cells).First()
                            .Where(x => x.Minute == dtFromRow.Minute).First().isLocked = true;
                    }

                    //желтая ячейка на времени регистрации
                    model.Rows.Where(x => x.Hour == time.Value.Hour).Select(x => x.Cells).First()
                            .Where(x => x.Minute == time.Value.Minute).First().isRegistered = true;
                }
            }

            return View(model);
        }

        public ActionResult GetTimetable(string dormsId, string date)
        {
            int iDormsId = 0;
            DateTime dtDate;

            Guid UserId;
            if (!Util.CheckAuthCookies(Request.Cookies, out UserId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });

            if (!int.TryParse(dormsId, out iDormsId))
                return Json(new { IsOk = false, Message = "Неверный идентификатор студгородка" });
            if (!DateTime.TryParse(date, out dtDate))
                return Json(new { IsOk = false, Message = "Неверный формат даты" });

            string query = @"SELECT Date, SUM(t.CNT) AS CNT
FROM
(SELECT Date, COUNT(Id) AS CNT FROM Timetable WHERE CONVERT(date, Date)=@Date AND DormsId=@DormsId GROUP BY Date
UNION
SELECT Date, COUNT(Id) AS CNT FROM TimetableLock WHERE Date=@Date AND DormsId=@DormsId AND LockTime>@LockTime GROUP BY Date) t
GROUP BY t.Date";
            Dictionary<string, object> dic = new Dictionary<string, object>(5);
            dic.Add("@Date", dtDate.Date);
            dic.Add("@DormsId", iDormsId);
            dic.Add("@LockTime", DateTime.Now.AddMinutes(-10));
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

            List<object> lst = new List<object>();

            foreach (DataRow rw in tbl.Rows)
            {
                DateTime dtFromRow = rw.Field<DateTime>("Date");
                int cnt = rw.Field<int>("CNT");
                lst.Add(new { H = dtFromRow.Hour, M = dtFromRow.Minute, Cnt = cnt, Max = iMaxQue, IsLocked = cnt >= iMaxQue });
            }

            //вычисляем баркод юзера
            query = "SELECT Person.Barcode FROM Person INNER JOIN ApprovedHostel ON ApprovedHostel.PersonBarcode=Person.Barcode WHERE Id=@Id";
            dic.Clear();
            dic.Add("@Id", UserId);
            int? barc = (int?)Util.AbitDB.GetValue(query, dic);

            //вычисляем время регистрации в указанный день
            query = "SELECT TOP (1) Date FROM Timetable WHERE PersonBarcode=@PersonBarcode AND DormsId=@DormsId AND CONVERT(date, Date)=@Date ORDER BY Date";
            dic.Clear();
            dic.Add("@PersonBarcode", barc ?? 0);
            dic.Add("@Date", dtDate.Date);
            dic.Add("@DormsId", iDormsId);
            DateTime? dtReg = (DateTime?)Util.AbitDB.GetValue(query, dic);

            //вычисляем нерабочие часы в указанный день. 
            //Если в базе данных на указанное число нет, то считаем, что приёмка не работает
            query = "SELECT Start, [End], BreakStart, BreakEnd FROM DormsWorkingHours WHERE DormsId=@DormsId AND Date=@Date";
            dic.Clear();
            dic.Add("@DormsId", iDormsId);
            dic.Add("@Date", dtDate.Date);
            tbl = Util.AbitDB.GetDataTable(query, dic);

            DateTime dtStart, dtEnd, dtBreakStart, dtBreakEnd;
            List<string> nulls = new List<string>();
            if (tbl.Rows.Count == 0)
            {
                return Json(new { IsOk = true, Values = lst, RegH = dtReg.HasValue ? dtReg.Value.Hour : 0, RegM = dtReg.HasValue ? dtReg.Value.Minute : 0, IsWorkDay = false });
            }
            else
            {
                dtStart = tbl.Rows[0].Field<DateTime>("Start");
                dtEnd = tbl.Rows[0].Field<DateTime>("End");
                dtBreakStart = tbl.Rows[0].Field<DateTime?>("BreakStart") ?? dtDate.Date.AddHours(23);
                dtBreakEnd = tbl.Rows[0].Field<DateTime?>("BreakEnd") ?? dtDate.Date.AddHours(23);
                for (int h = 8; h < 20; h++)
                {
                    for (int m = 0; m < 10; m++)
                    {
                        if (dtStart.Hour > h || (dtStart.Hour == h && dtStart.Minute > m * 5))
                            nulls.Add("H" + h + "M" + (m * 5));
                        if (dtEnd.Hour < h || (dtEnd.Hour == h && dtEnd.Minute < m * 5))
                            nulls.Add("H" + h + "M" + (m * 5));

                        if (dtBreakStart.Hour == h && (dtBreakStart.Minute <= m * 5 || dtBreakStart.Minute == 0))
                            nulls.Add("H" + h + "M" + (m * 5));
                        if (dtBreakEnd.Hour == h && (dtBreakEnd.Minute >= m * 5 && dtBreakEnd.Minute != 0))
                            nulls.Add("H" + h + "M" + (m * 5));
                    }
                }
            }

            return Json(new { IsOk = true, Values = lst, RegH = dtReg.HasValue ? dtReg.Value.Hour : 0, RegM = dtReg.HasValue ? dtReg.Value.Minute : 0, Nulls = nulls });
        }
        public ActionResult CheckTimeAndLock(string dormsId, string date, string h, string m)
        {
            int iDormsId = 0;
            DateTime dtDate;
            int iHour = 0;
            int iMinute = 0;

            Guid UserId = new Guid();
            if (!Util.CheckAuthCookies(Request.Cookies, out UserId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired, Action = "none" });

            if (!int.TryParse(dormsId, out iDormsId))
                return Json(new { IsOk = false, Message = "Неверный идентификатор студгородка", Action = "none" });
            if (!DateTime.TryParse(date, out dtDate))
                return Json(new { IsOk = false, Message = "Неверный формат даты", Action = "none" });
            if (!int.TryParse(h, out iHour))
                return Json(new { IsOk = false, Message = "Неверный формат времени", Action = "none" });
            if (!int.TryParse(m, out iMinute))
                return Json(new { IsOk = false, Message = "Неверный формат времени", Action = "none" });

            dtDate = dtDate.Date.AddHours(iHour).AddMinutes(iMinute);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", UserId);

            //вычисляем баркод
            string query = "SELECT Person.Barcode FROM Person INNER JOIN ApprovedHostel ON ApprovedHostel.PersonBarcode=Person.Barcode WHERE Id=@Id";
            int? barc = (int?)Util.AbitDB.GetValue(query, dic);
            if (!barc.HasValue)
                return Json(new { IsOk = false, Message = "К сожалению, мы не обнаружили Вас в списке на заселение. Если вы не помните, как регистрировались в Личном кабинете, то перейдите <a href=\"../../Account/UserFromData\">сюда</a>", Action = "none" });

            dic.Clear();
            dic.Add("@Date", dtDate);
            dic.Add("@DormsId", iDormsId);
            dic.Add("@Barcode", barc);
            query = "SELECT Id FROM Timetable WHERE Date=@Date AND DormsId=@DormsId AND PersonBarcode=@Barcode";
            Guid? AppId = (Guid?)Util.AbitDB.GetValue(query, dic);
            if (AppId.HasValue)
                return Json(new { IsOk = true, Action = "unregister", Id = AppId.Value.ToString("N") });

            query = @"SELECT SUM(t.CNT)
FROM (
SELECT COUNT(Id) AS CNT FROM Timetable WHERE Date=@Date AND DormsId=@DormsId 
UNION
SELECT COUNT(Id) AS CNT FROM TimetableLock WHERE Date=@Date AND DormsId=@DormsId AND LockTime>@LockTime) t";
            dic.Clear();
            dic.Add("@Date", dtDate);
            dic.Add("@DormsId", dormsId);
            dic.Add("@LockTime", DateTime.Now.AddMinutes(-10));
            int cnt = (int)Util.AbitDB.GetValue(query, dic);

            if (cnt >= iMaxQue)
                return Json(new { IsOk = false, Message = "В данный момент все места в это время заняты", Action = "red", Cnt = cnt, Max = iMaxQue });

            //удаляем старые (если вдруг остались) записи данного пользователя из списка залоченных
            query = "DELETE FROM TimetableLock WHERE PersonBarcode=@Barcode";
            dic.Clear();
            dic.Add("@Barcode", barc.Value);
            Util.AbitDB.ExecuteQuery(query, dic);

            Guid id = Guid.NewGuid();
            query = "INSERT INTO TimetableLock (Id, PersonBarcode, Date, DormsId) VALUES (@Id, @Barcode, @Time, @DormsId)";
            dic.Clear();
            dic.Add("@Id", id);
            dic.Add("@Barcode", barc.Value);
            dic.Add("@Time", dtDate);
            dic.Add("@DormsId", dormsId);
            Util.AbitDB.ExecuteQuery(query, dic);

            return Json(new { IsOk = true, Action = "yellow", Id = id.ToString("N"), Cnt = cnt, Max = iMaxQue });
        }
        public ActionResult Register(string Id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });
            Guid TicketId;
            if (!Guid.TryParse(Id, out TicketId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", TicketId);

            string query = "SELECT TOP 1 PersonBarcode, Date, DormsId FROM TimetableLock WHERE Id=@Id ORDER BY LockTime DESC";
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count == 0)
                return Json(new { IsOk = false, Message = "Запись не найдена. Попробуйте повторить попытку" });

            int iPersBarc = tbl.Rows[0].Field<int>("PersonBarcode");
            int iDormsId = tbl.Rows[0].Field<int>("DormsId");
            DateTime dtDate = tbl.Rows[0].Field<DateTime>("Date");

            dic.Clear();
            dic.Add("@Barcode", iPersBarc);
            dic.Add("@Date", dtDate.Date);
            dic.Add("@DormsId", iDormsId);
            //проверка на уже имеющуюся на сегодняшний день регистрацию
            query = "SELECT TOP (1) Date FROM Timetable WHERE PersonBarcode=@Barcode AND CONVERT(date, Date)=@Date AND DormsId=@DormsId";
            tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count > 0)
                return Json(new
                {
                    IsOk = false,
                    Message = string.Format("Вы уже зарегистрировались на {0}. Снимите регистрацию c указанного времени.", tbl.Rows[0].Field<DateTime>("Date").ToShortTimeString())
                });

            //проверка соответствия баркода и айдишника
            query = "SELECT SUM(t.CNT) FROM (SELECT COUNT(*) AS CNT FROM Person WHERE Id=@Id AND Barcode=@Barcode UNION SELECT COUNT(*) AS CNT FROM ForeignPerson WHERE Id=@Id AND Barcode=@Barcode) t";
            dic.Clear();
            dic.Add("@Barcode", iPersBarc);
            dic.Add("@Id", PersonId);
            int cntBarc = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", PersonId }, { "@Barcode", iPersBarc } });
            if (cntBarc == 0)
                return Json(new { IsOk = false, Message = "Ошибка сопоставления. Попробуйте перезалогиниться." });

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //удаляем из лока
                    query = "DELETE FROM TimetableLock WHERE Id=@Id";
                    dic.Clear();
                    dic.Add("@Id", TicketId);
                    Util.AbitDB.ExecuteQuery(query, dic);
                    //запихиваем в рабочую таблицу
                    query = "INSERT INTO Timetable (Id, PersonBarcode, Date, DormsId) VALUES (@Id, @PersonBarcode, @Date, @DormsId)";
                    dic.Add("@PersonBarcode", iPersBarc);
                    dic.Add("@Date", dtDate);
                    dic.Add("@DormsId", iDormsId);
                    Util.AbitDB.ExecuteQuery(query, dic);

                    scope.Complete();
                }
                catch
                {
                    return Json(new { IsOk = false, Message = "Ошибка при обновлении. Данные не зафиксированы." });
                }
            }

            //обновление данных по времени
            query = "SELECT COUNT(*) FROM Timetable WHERE DormsId=@DormsId AND CONVERT(date, Date)=@Date";
            dic.Clear();
            dic.Add("@Date", dtDate.Date);
            dic.Add("@DormsId", iDormsId);
            int cnt = (int)Util.AbitDB.GetValue(query, dic);

            return Json(new { IsOk = true, Cnt = cnt, Max = iMaxQue, H = dtDate.Hour, M = dtDate.Minute });
        }
        public ActionResult UnRegister(string Id)
        {
            Guid PersonId;
            if (!Util.CheckAuthCookies(Request.Cookies, out PersonId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.AuthorizationRequired });
            Guid TicketId;
            if (!Guid.TryParse(Id, out TicketId))
                return Json(new { IsOk = false, Message = Resources.ServerMessages.IncorrectGUID });

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@Id", TicketId);

            //получение данных по айдишнику
            string query = "SELECT TOP 1 PersonBarcode, Date, DormsId FROM Timetable WHERE Id=@Id";
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);
            if (tbl.Rows.Count == 0)
                return Json(new { IsOk = false, Message = "Запись не найдена. Попробуйте повторить попытку" });

            int iPersBarc = tbl.Rows[0].Field<int>("PersonBarcode");
            int iDormsId = tbl.Rows[0].Field<int>("DormsId");
            DateTime dtDate = tbl.Rows[0].Field<DateTime>("Date");

            //проверка соответствия баркода и айдишника
            query = "SELECT SUM(t.CNT) FROM (SELECT COUNT(*) AS CNT FROM Person WHERE Id=@Id AND Barcode=@Barcode UNION SELECT COUNT(*) AS CNT FROM ForeignPerson WHERE Id=@Id AND Barcode=@Barcode) t";
            dic.Clear();
            dic.Add("@Barcode", iPersBarc);
            dic.Add("@Id", PersonId);
            int cntBarc = (int)Util.AbitDB.GetValue(query, new Dictionary<string, object>() { { "@Id", PersonId }, { "@Barcode", iPersBarc } });
            if (cntBarc == 0)
                return Json(new { IsOk = false, Message = "Ошибка сопоставления. Попробуйте перезалогиниться." });

            //удаление записи
            try
            {
                dic.Clear();
                dic.Add("@Id", TicketId);

                query = "DELETE FROM TimetableLock WHERE Id=@Id";
                Util.AbitDB.ExecuteQuery(query, dic);

                query = "DELETE FROM Timetable WHERE Id=@Id";
                Util.AbitDB.ExecuteQuery(query, dic);

            }
            catch
            {
                return Json(new { IsOk = false, Message = "Ошибка при удалении." });
            }

            //переподсчёт количества записей на указанное время
            query = @"SELECT COUNT(*) AS CNT FROM Timetable WHERE DormsId=@DormsId AND Date=@Date
UNION
SELECT COUNT(*) AS CNT FROM TimetableLock WHERE DormsId=@DormsId AND Date=@Date AND LockTime<@LockTime";
            dic.Clear();
            dic.Add("@DormsId", iDormsId);
            dic.Add("@Date", dtDate);
            dic.Add("@LockTime", DateTime.Now.AddMinutes(-30));

            int cnt = (int)Util.AbitDB.GetValue(query, dic);
            return Json(new { IsOk = true, Cnt = cnt, Max = iMaxQue, H = dtDate.Hour, M = dtDate.Minute });
        }
    }
}
