using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineAbit2013.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI;
using Recaptcha;
using System.IO;
using BDClassLib;

namespace OnlineAbit2013.Controllers
{
    public static partial class Util
    {
        #region Fields

        public static int iPriemYear { get; private set; }
        public static string sPriemYear { get; private set; }


        private static SQLClass _abitDB;
        private static SQLClass _studDB;
        private static SQLClass _priem2012DB;
        private static string _entitiesConnString = "metadata=res://*/OnlineAbit.csdl|res://*/OnlineAbit.ssdl|res://*/OnlineAbit.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=SRVPRIEM;Initial Catalog=OnlineAbit20132012;User ID=OnlineAbit2013User;Password=**********\"";

        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass AbitDB 
        { 
            get 
            {
                if (!_abitDB.IsOpen)
                    _abitDB.OpenDatabase(ConstClass.AbitDB);
                return _abitDB; 
            }
        }
        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass StudDB { get { return _studDB; } }
        /// <summary>
        /// ADO-Powered
        /// </summary>
        public static SQLClass Priem2012DB { get { return _priem2012DB; } }
        public static string EntitiesConnString { get { return _entitiesConnString; } }

        public static string ServerAddress { get; set; }
        public static string FilesPath { get; set; }
        public static bool bUseRedirection { get; set; }

        /// <summary>
        /// Названия всех экзаменов ЕГЭ
        /// </summary>
        public static Dictionary<int, string> EgeExamsAll { get; set; }
        /// <summary>
        /// Названия всех видов научной работы
        /// </summary>
        public static Dictionary<int, string> ScienceWorkTypeAll { get; set; }
        /// <summary>
        /// Формы обучения
        /// </summary>
        public static Dictionary<int, string> StudyFormAll { get; set; }
        /// <summary>
        /// Английские наименования форм обучения
        /// </summary>
        public static Dictionary<int, string> ForeignStudyFormAll { get; set; }
        /// <summary>
        /// Основы обучения
        /// </summary>
        public static Dictionary<int, string> StudyBasisAll { get; set; }

        public static Dictionary<int, string> LanguagesAll { get; set; }
        public static Dictionary<int, string> SchoolTypesAll { get; set; }
        public static Dictionary<int, string> CountriesAll { get; set; }
        public static Dictionary<int, string> RegionsAll { get; set; }
        public static Dictionary<int, string> FileTypesAll { get; set; }
        public static Dictionary<int, string> QualifaicationAll { get; set; }
        public static Dictionary<int, string> QualifaicationForAspirant { get; set; }

        public static SortedList<string, Guid> CacheSID_User { get; set; }

        #endregion

        //статический конструктор
        static Util()
        {
            InitDB(); 
            string query = "SELECT Id, Name  FROM {0} WHERE 1=@x ORDER BY Id";
            SortedList<string, object> dic = new SortedList<string, object>() { { "@x", 1 } };
            DataTable tbl = _abitDB.GetDataTable(string.Format(query, "EgeExam"), dic);

            CacheSID_User = new SortedList<string, Guid>();

            EgeExamsAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "ScienceWorkType"), dic);
            ScienceWorkTypeAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "StudyForm"), dic);
            StudyFormAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable("SELECT Id, NameEng FROM StudyForm", null);
            ForeignStudyFormAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("NameEng") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "StudyBasis"), dic);
            StudyBasisAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "Language"), dic);
            LanguagesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "SchoolType"), dic);
            SchoolTypesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable("SELECT Id, Name FROM [Country] ORDER BY LevelOfUsing DESC, Name", null);
            CountriesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable("SELECT Id, Name FROM [PersonFileType] ORDER BY Id", null);
            FileTypesAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);
           
            tbl = _abitDB.GetDataTable("SELECT Id, Name  FROM Region WHERE RegionNumber IS NOT NULL", dic);
            RegionsAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            tbl = _abitDB.GetDataTable(string.Format(query, "Qualification"), dic);
            QualifaicationAll =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            query = "SELECT Id, Name FROM Qualification WHERE IsForAspirant=1";
            tbl = _abitDB.GetDataTable(query, null);
            QualifaicationForAspirant =
                (from DataRow rw in tbl.Rows
                 select new { Id = rw.Field<int>("Id"), Name = rw.Field<string>("Name") }).ToDictionary(x => x.Id, x => x.Name);

            sPriemYear = GetValueByKey("PriemYear");
            if (string.IsNullOrEmpty(sPriemYear))
            {
                iPriemYear = DateTime.Now.Year;
                sPriemYear = iPriemYear.ToString();
            }
            else
            {
                int r = 0;
                if (!int.TryParse(sPriemYear, out r))
                    r = DateTime.Now.Year;
                iPriemYear = r;
            }

            FilesPath = @"D:\Projects\OnlineAbit2013\OnlineAbit2013\Content\Files\";
            ServerAddress = WebConfigurationManager.AppSettings["ServerName"];//in web.config
            bUseRedirection = WebConfigurationManager.AppSettings["bUseRedirection"] == "1" ? true : false;//in web.config
        }

        private static string GetValueByKey(string key)
        {
            string query = "SELECT [Value] FROM _appsettings WHERE [Key]=@Key";
            return _abitDB.GetStringValue(query, new SortedList<string, object>() { { "@Key", key } });
        }

        /// <summary>
        /// Возвращает Guid пользователя в базе по указанному SID. Если такого пользователя нет, возвращается пустой Guid
        /// </summary>
        /// <param name="SID"></param>
        /// <returns></returns>
        public static Guid GetIdBySID(string SID)
        {
            Guid id = Guid.Empty;

            if (CacheSID_User.ContainsKey(SID))
                id = CacheSID_User[SID];

            string sId = AbitDB.GetStringValue("SELECT Id FROM [User] WHERE SID=@SID", new SortedList<string, object>() { { "@SID", SID } });
            try
            {
                id = Guid.Parse(sId);
                if (!CacheSID_User.ContainsKey(SID) && id != Guid.Empty)
                    CacheSID_User.Add(SID, id);
            }
            catch { }

            return id;
        }

        /// <summary>
        /// Возвращает SID пользователя по указанному Guid. Если такого не найдено, возвращается пустая строка
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetSIDById(Guid id)
        {
            return AbitDB.GetStringValue("SELECT SID FROM User WHERE Id=@Id", new SortedList<string, object>() { { "@Id", id } });
        }

        public static bool CheckRegistrationInfo(string password, string email, out List<string> errList)
        {
            bool res = true;
            errList = new List<string>();

            string query = "SELECT Id FROM [User] WHERE [Email]=@Email";
            string result = AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Email", email } });
            if (!string.IsNullOrEmpty(result))
            {
                res = false;
                errList.Add(//isForeigner ? "User with this email already exists" : 
                    "Пользователь с данным адресом электронной почты уже существует");
            }
            if (password.Length < 6)
            {
                res = false;
                errList.Add(//isForeigner ? "Password is too short" : 
                    "Пароль слишком короткий");
            }
            if (!Regex.IsMatch(email, @"^[-a-zA-Z0-9!#$%&'*+/=?^_`{|}~]+(\.[-a-z0-9!#$%&'*+/=?^_`{|}~]+)*@([a-zA-Z0-9]([-a-z0-9]{0,61}[a-z0-9])?\.)*(aero|arpa|asia|biz|cat|com|coop|edu|gov|info|int|jobs|mil|mobi|museum|name|net|org|pro|tel|travel|[a-zA-Z][a-zA-Z])$"))
            {
                res = false;
                errList.Add(//isForeigner ? "Incorrect email address" : 
                    "Некорректный адрес электронной почты");
            }
            return res;
        }

        /// <summary>
        /// Создаёт нового пользователя в базе абитуриентов. Возвращает Guid нового пользователя. Exception в случае проблем при создании.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static Guid CreateNewUser(string password, string email)
        {
            Guid id = Guid.NewGuid();
            string sid = MD5Str(id.ToByteArray());
            string md5pwd = MD5Str(password);

            string query = "INSERT INTO [User] (Id, Password, SID, Email, IsApproved, EmailTicket) VALUES (@Id, @Password, @SID, @Email, @IsApproved, @EmailTicket)";
            SortedList<string, object> dic = new SortedList<string, object>()
            {
                {"@Id", id},
                {"@Password", md5pwd},
                {"@SID", sid},
                {"@Email", email},
                {"@IsApproved", false},
                {"@EmailTicket", Guid.NewGuid().ToString("N")},
            };
            AbitDB.ExecuteQuery(query, dic);

            query = "INSERT INTO AuthTicket (UserId, Ticket) VALUES (@UserId, @Ticket)";
            dic.Clear();
            dic.Add("@UserId", id);
            dic.Add("Ticket", Guid.NewGuid().ToString("N"));
            AbitDB.ExecuteQuery(query, dic);

            return id;
        }

        public static bool CreateNewSimpleUser(string password, string email, Guid id)
        {
            using (System.Transactions.TransactionScope tran = new System.Transactions.TransactionScope())
            {
                try
                {
                    string sid = MD5Str(id.ToByteArray());
                    string md5pwd = MD5Str(password);

                    string query = "INSERT INTO [User] (Id, Password, SID, Email, IsApproved, EmailTicket, IsForeign, IsDormsAccount) VALUES (@Id, @Password, @SID, @Email, @IsApproved, @EmailTicket, @IsForeign, @IsDormsAccount)";
                    SortedList<string, object> dic = new SortedList<string, object>()
                    {
                        {"@Id", id},
                        {"@Password", md5pwd},
                        {"@SID", sid},
                        {"@Email", email},
                        {"@IsApproved", false},
                        {"@EmailTicket", Guid.NewGuid().ToString("N")},
                        {"@IsForeign", false},
                        {"@IsDormsAccount", true}
                    };
                    AbitDB.ExecuteQuery(query, dic);

                    query = "INSERT INTO AuthTicket (UserId, Ticket) VALUES (@UserId, @Ticket)";
                    dic.Clear();
                    dic.Add("@UserId", id);
                    dic.Add("Ticket", Guid.NewGuid().ToString("N"));
                    AbitDB.ExecuteQuery(query, dic);

                    tran.Complete();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Проверяет состояние регистрации пользователя и, если у него не заполнен до конца профиль, то возвращает false. 
        /// IntStage = этап регистрации, на который нужно перекинуть пользователя
        /// </summary>
        /// <param name="PersonId"></param>
        /// <param name="IntStage"></param>
        /// <returns></returns>
        public static bool CheckPersonRegStatus(Guid PersonId, out int IntStage)
        {
            IntStage = 1;
            string query = "SELECT RegistrationStage FROM Person WHERE Id=@Id";
            string stage = AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Id", PersonId } });
            if (string.IsNullOrEmpty(stage))
                return false;

            int iStage = 0;
            if (!int.TryParse(stage, out iStage))
                return false;

            IntStage = iStage;
            if (iStage < 100)
                return false;
            else
                return true;
        }

        public static bool CheckIsForeign(Guid PersonId)
        {
            string query = "SELECT IsForeign FROM [User] WHERE Id=@Id";
            string res = AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Id", PersonId } });
            if (!string.IsNullOrEmpty(res) && string.Compare(res, "true", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }
            return false;
        }

        public static int CheckAbitType(Guid PersonId)
        {
            string query = "SELECT [AbiturientTypeId] FROM [Person] WHERE Id=@Id";
            int? res = (int?)AbitDB.GetValue(query, new SortedList<string, object>() { { "@Id", PersonId } });
            if (res.HasValue)
                return res.Value;
            else
                return 1;
        }

        public static bool CheckIsNew(Guid PersonId)
        {
            string query = "SELECT Id FROM [Person] WHERE Id=@Id";
            string res = AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Id", PersonId } });
            if (string.IsNullOrEmpty(res))
                return true;
            else
                return false;
        }

        public static bool CreateNew(Guid PersonId)
        {
            string query = "INSERT INTO [Person] (Id, UserId, RegistrationStage, AbiturientTypeId) VALUES (@Id, @Id, 1, 1)";
            AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Id", PersonId } });

            query = "SELECT Id FROM [Person] WHERE Id=@Id";
            string res = AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@Id", PersonId } });
            
            if (string.IsNullOrEmpty(res))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Проверяет пользователя на необходимость блокировки
        /// </summary>
        /// <param name="PersonId"></param>
        /// <returns></returns>
        public static bool CheckPersonReadOnlyStatus(Guid PersonId)
        {
            string query = "SELECT COUNT(Application.Id) FROM [Application] INNER JOIN Entry ON Entry.Id=[Application].EntryId WHERE PersonId=@PersonId AND Enabled=@Enabled AND IsCommited=1";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Enabled", true);

            int res = (int)AbitDB.GetValue(query, dic);

            query = "SELECT COUNT(Id) FROM [AG_Application] WHERE PersonId=@PersonId AND Enabled=@Enabled AND IsCommited=1";
            res += (int)AbitDB.GetValue(query, dic);
            
            if (res > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Проверяет пользователя на необходимость блокировки
        /// </summary>
        /// <param name="PersonId"></param>
        /// <returns></returns>
        public static bool CheckPersonReadOnlyStatus_AG(Guid PersonId)
        {
            string query = "SELECT COUNT([AG_Application].Id) FROM [AG_Application] INNER JOIN AG_Entry ON AG_Entry.Id=[AG_Application].EntryId WHERE PersonId=@PersonId AND Enabled=@Enabled";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@Enabled", true);

            int res = (int)AbitDB.GetValue(query, dic);
            if (res > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Возвращает MD5-строку от строки-источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Str(string source)
        {
            byte[] md5 = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(source));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Возвращает MD5-строку от byte[] источника
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Str(byte[] source)
        {
            byte[] md5 = MD5.Create().ComputeHash(source);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in md5)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string GetFileSize(int fileSize)
        {
            if (fileSize > 1024)
            {
                string fSizeRaw = ((double)fileSize / 1024.0).ToString();
                int limpos = fSizeRaw.IndexOf(',');
                int delta = fSizeRaw.Length - limpos;
                if (delta > 2)
                    return fSizeRaw.Substring(0, limpos + 2) + " Kb";
                else
                    return fSizeRaw + " Kb";
            }
            else if (fileSize > 1024 * 1024)
            {
                string fSizeRaw = ((double)fileSize / (1024.0 * 1024.0)).ToString();
                int limpos = fSizeRaw.IndexOf(',');
                int delta = fSizeRaw.Length - limpos;
                if (delta > 2)
                    return fSizeRaw.Substring(0, limpos + 2) + " Mb";
                else
                    return fSizeRaw + " Mb";
            }
            return fileSize + " b";
        }

        public static bool CheckAuthCookies(HttpCookieCollection cookies, out Guid personId)
        {
            SetThreadCultureByCookies(cookies);
            personId = Guid.Empty;
            string sid = "";
            if (cookies["sid"] != null)
                sid = cookies["sid"].Value;
            else
                return false;

            string ticket = "";
            if (cookies["t"] != null)
                ticket = cookies["t"].Value;
            else
                return false;

            if (!string.IsNullOrEmpty(sid))
                personId = GetIdBySID(sid);
            else
                return false;

            Guid uid = personId;

            string t = AbitDB.GetStringValue("SELECT Ticket FROM AuthTicket WHERE UserId=@UserId", new SortedList<string, object>() { { "@UserId", personId } });
            //string t = ABDB.AuthTicket.Where(x => x.UserId == uid).Select(x => x.Ticket).FirstOrDefault();
            if (string.IsNullOrEmpty(t) || (t != ticket))
                return false;

            if (personId == Guid.Empty)
                return false;

            //Оптимистичное блокирование - только проверка ограничений
            return true;
        }

        public static bool CheckAdminRights(Guid personId)
        {
            string query = "SELECT Id, Approved FROM Admins WHERE Id=@Id AND Approved='True'";
            try
            {
                DataTable tbl = AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", personId } });
                if (tbl.Rows.Count == 1)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static void SetThreadCultureByCookies(this HttpCookieCollection cookies)
        {
            string uilang = cookies["uilang"] != null ? cookies["uilang"].Value : "";
            if (uilang.StrCmp("en"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture
                    = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture
                    = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
            }
        }

        public static string GetCurrentThreadLanguage()
        {
            if (System.Threading.Thread.CurrentThread.CurrentUICulture ==
                    System.Globalization.CultureInfo.GetCultureInfo("en-US"))
            {
                return "en";
            }
            else
            {
                return "ru";
            }
        }

        public static bool GetCurrentThreadLanguageIsEng()
        {
            return GetCurrentThreadLanguage() == "en";
        }

        public static bool SetUILang(Guid PersonId, string lang)
        {
            if (lang.StrCmp("ru"))
                lang = "ru";
            else if (lang.StrCmp("en"))
                lang = "en";

            string query = "UPDATE [User] SET UILanguage=@Language WHERE Id=@Id";
            try
            {
                AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Language", lang }, { "@Id", PersonId } });
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetUILang(Guid PersonId)
        {
            string query = "SELECT UILanguage, IsForeign FROM [User] WHERE Id=@Id";
            DataTable tbl = AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", PersonId } });
            if (tbl.Rows.Count == 0)
                return "ru";
            string UILang = tbl.Rows[0].Field<string>("UILanguage");
            bool IsForeigner = tbl.Rows[0].Field<bool?>("IsForeign").HasValue ?
                tbl.Rows[0].Field<bool>("IsForeign") : false;
            if (string.IsNullOrEmpty(UILang))
            {
                if (IsForeigner)
                    UILang = "en";
                else
                    UILang = "ru";
            }

            return UILang;
        }

        public static string StrictUILang(string lang)
        {
            if (string.IsNullOrEmpty(lang))
                return "ru";

            if (lang.StrCmp("ru"))
                return "ru";
            if (lang.StrCmp("en"))
                return "en";

            return "ru";
        }

        /// <summary>
        /// Парсит строку в число. Если строка кривая, то 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ParseSafe(string str)
        {
            int ret;
            if (!int.TryParse(str, out ret))
                ret = 0;

            return ret;
        }

        /// <summary>
        /// Возвращает контент-тип файла по указанному расширению
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GetMimeFromExtention(string ext)
        {
            switch (ext)
            {
                case ".323": return "text/h323";
                case ".3g2": return "video/3gpp2";
                case ".3gp": return "video/3gpp";
                case ".3gp2": return "video/3gpp2";
                case ".3gpp": return "video/3gpp";
                case ".7z": return "application/x-7z-compressed";
                case ".aa": return "audio/audible";
                case ".AAC": return "audio/aac";
                case ".aaf": return "application/octet-stream";
                case ".aax": return "audio/vnd.audible.aax";
                case ".ac3": return "audio/ac3";
                case ".aca": return "application/octet-stream";
                case ".accda": return "application/msaccess.addin";
                case ".accdb": return "application/msaccess";
                case ".accdc": return "application/msaccess.cab";
                case ".accde": return "application/msaccess";
                case ".accdr": return "application/msaccess.runtime";
                case ".accdt": return "application/msaccess";
                case ".accdw": return "application/msaccess.webapplication";
                case ".accft": return "application/msaccess.ftemplate";
                case ".acx": return "application/internet-property-stream";
                case ".AddIn": return "text/xml";
                case ".ade": return "application/msaccess";
                case ".adobebridge": return "application/x-bridge-url";
                case ".adp": return "application/msaccess";
                case ".ADT": return "audio/vnd.dlna.adts";
                case ".ADTS": return "audio/aac";
                case ".afm": return "application/octet-stream";
                case ".ai": return "application/postscript";
                case ".aif": return "audio/x-aiff";
                case ".aifc": return "audio/aiff";
                case ".aiff": return "audio/aiff";
                case ".air": return "application/vnd.adobe.air-application-installer-package+zip";
                case ".amc": return "application/x-mpeg";
                case ".application": return "application/x-ms-application";
                case ".art": return "image/x-jg";
                case ".asa": return "application/xml";
                case ".asax": return "application/xml";
                case ".ascx": return "application/xml";
                case ".asd": return "application/octet-stream";
                case ".asf": return "video/x-ms-asf";
                case ".ashx": return "application/xml";
                case ".asi": return "application/octet-stream";
                case ".asm": return "text/plain";
                case ".asmx": return "application/xml";
                case ".aspx": return "application/xml";
                case ".asr": return "video/x-ms-asf";
                case ".asx": return "video/x-ms-asf";
                case ".atom": return "application/atom+xml";
                case ".au": return "audio/basic";
                case ".avi": return "video/x-msvideo";
                case ".axs": return "application/olescript";
                case ".bas": return "text/plain";
                case ".bcpio": return "application/x-bcpio";
                case ".bin": return "application/octet-stream";
                case ".bmp": return "image/bmp";
                case ".c": return "text/plain";
                case ".cab": return "application/octet-stream";
                case ".caf": return "audio/x-caf";
                case ".calx": return "application/vnd.ms-office.calx";
                case ".cat": return "application/vnd.ms-pki.seccat";
                case ".cc": return "text/plain";
                case ".cd": return "text/plain";
                case ".cdda": return "audio/aiff";
                case ".cdf": return "application/x-cdf";
                case ".cer": return "application/x-x509-ca-cert";
                case ".chm": return "application/octet-stream";
                case ".class": return "application/x-java-applet";
                case ".clp": return "application/x-msclip";
                case ".cmx": return "image/x-cmx";
                case ".cnf": return "text/plain";
                case ".cod": return "image/cis-cod";
                case ".config": return "application/xml";
                case ".contact": return "text/x-ms-contact";
                case ".coverage": return "application/xml";
                case ".cpio": return "application/x-cpio";
                case ".cpp": return "text/plain";
                case ".crd": return "application/x-mscardfile";
                case ".crl": return "application/pkix-crl";
                case ".crt": return "application/x-x509-ca-cert";
                case ".cs": return "text/plain";
                case ".csdproj": return "text/plain";
                case ".csh": return "application/x-csh";
                case ".csproj": return "text/plain";
                case ".css": return "text/css";
                case ".csv": return "application/octet-stream";
                case ".cur": return "application/octet-stream";
                case ".cxx": return "text/plain";
                case ".dat": return "application/octet-stream";
                case ".datasource": return "application/xml";
                case ".dbproj": return "text/plain";
                case ".dcr": return "application/x-director";
                case ".def": return "text/plain";
                case ".deploy": return "application/octet-stream";
                case ".der": return "application/x-x509-ca-cert";
                case ".dgml": return "application/xml";
                case ".dib": return "image/bmp";
                case ".dif": return "video/x-dv";
                case ".dir": return "application/x-director";
                case ".disco": return "text/xml";
                case ".dll": return "application/x-msdownload";
                case ".dll.config": return "text/xml";
                case ".dlm": return "text/dlm";
                case ".doc": return "application/msword";
                case ".docm": return "application/vnd.ms-word.document.macroEnabled.12";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".dot": return "application/msword";
                case ".dotm": return "application/vnd.ms-word.template.macroEnabled.12";
                case ".dotx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case ".dsp": return "application/octet-stream";
                case ".dsw": return "text/plain";
                case ".dtd": return "text/xml";
                case ".dtsConfig": return "text/xml";
                case ".dv": return "video/x-dv";
                case ".dvi": return "application/x-dvi";
                case ".dwf": return "drawing/x-dwf";
                case ".dwp": return "application/octet-stream";
                case ".dxr": return "application/x-director";
                case ".eml": return "message/rfc822";
                case ".emz": return "application/octet-stream";
                case ".eot": return "application/octet-stream";
                case ".eps": return "application/postscript";
                case ".etl": return "application/etl";
                case ".etx": return "text/x-setext";
                case ".evy": return "application/envoy";
                case ".exe": return "application/octet-stream";
                case ".exe.config": return "text/xml";
                case ".fdf": return "application/vnd.fdf";
                case ".fif": return "application/fractals";
                case ".filters": return "Application/xml";
                case ".fla": return "application/octet-stream";
                case ".flr": return "x-world/x-vrml";
                case ".flv": return "video/x-flv";
                case ".fsscript": return "application/fsharp-script";
                case ".fsx": return "application/fsharp-script";
                case ".generictest": return "application/xml";
                case ".gif": return "image/gif";
                case ".group": return "text/x-ms-group";
                case ".gsm": return "audio/x-gsm";
                case ".gtar": return "application/x-gtar";
                case ".gz": return "application/x-gzip";
                case ".h": return "text/plain";
                case ".hdf": return "application/x-hdf";
                case ".hdml": return "text/x-hdml";
                case ".hhc": return "application/x-oleobject";
                case ".hhk": return "application/octet-stream";
                case ".hhp": return "application/octet-stream";
                case ".hlp": return "application/winhlp";
                case ".hpp": return "text/plain";
                case ".hqx": return "application/mac-binhex40";
                case ".hta": return "application/hta";
                case ".htc": return "text/x-component";
                case ".htm": return "text/html";
                case ".html": return "text/html";
                case ".htt": return "text/webviewhtml";
                case ".hxa": return "application/xml";
                case ".hxc": return "application/xml";
                case ".hxd": return "application/octet-stream";
                case ".hxe": return "application/xml";
                case ".hxf": return "application/xml";
                case ".hxh": return "application/octet-stream";
                case ".hxi": return "application/octet-stream";
                case ".hxk": return "application/xml";
                case ".hxq": return "application/octet-stream";
                case ".hxr": return "application/octet-stream";
                case ".hxs": return "application/octet-stream";
                case ".hxt": return "text/html";
                case ".hxv": return "application/xml";
                case ".hxw": return "application/octet-stream";
                case ".hxx": return "text/plain";
                case ".i": return "text/plain";
                case ".ico": return "image/x-icon";
                case ".ics": return "application/octet-stream";
                case ".idl": return "text/plain";
                case ".ief": return "image/ief";
                case ".iii": return "application/x-iphone";
                case ".inc": return "text/plain";
                case ".inf": return "application/octet-stream";
                case ".inl": return "text/plain";
                case ".ins": return "application/x-internet-signup";
                case ".ipa": return "application/x-itunes-ipa";
                case ".ipg": return "application/x-itunes-ipg";
                case ".ipproj": return "text/plain";
                case ".ipsw": return "application/x-itunes-ipsw";
                case ".iqy": return "text/x-ms-iqy";
                case ".isp": return "application/x-internet-signup";
                case ".ite": return "application/x-itunes-ite";
                case ".itlp": return "application/x-itunes-itlp";
                case ".itms": return "application/x-itunes-itms";
                case ".itpc": return "application/x-itunes-itpc";
                case ".IVF": return "video/x-ivf";
                case ".jar": return "application/java-archive";
                case ".java": return "application/octet-stream";
                case ".jck": return "application/liquidmotion";
                case ".jcz": return "application/liquidmotion";
                case ".jfif": return "image/pjpeg";
                case ".jnlp": return "application/x-java-jnlp-file";
                case ".jpb": return "application/octet-stream";
                case ".jpe": return "image/jpeg";
                case ".jpeg": return "image/jpeg";
                case ".jpg": return "image/jpeg";
                case ".js": return "application/x-javascript";
                case ".jsx": return "text/jscript";
                case ".jsxbin": return "text/plain";
                case ".latex": return "application/x-latex";
                case ".library-ms": return "application/windows-library+xml";
                case ".lit": return "application/x-ms-reader";
                case ".loadtest": return "application/xml";
                case ".lpk": return "application/octet-stream";
                case ".lsf": return "video/x-la-asf";
                case ".lst": return "text/plain";
                case ".lsx": return "video/x-la-asf";
                case ".lzh": return "application/octet-stream";
                case ".m13": return "application/x-msmediaview";
                case ".m14": return "application/x-msmediaview";
                case ".m1v": return "video/mpeg";
                case ".m2t": return "video/vnd.dlna.mpeg-tts";
                case ".m2ts": return "video/vnd.dlna.mpeg-tts";
                case ".m2v": return "video/mpeg";
                case ".m3u": return "audio/x-mpegurl";
                case ".m3u8": return "audio/x-mpegurl";
                case ".m4a": return "audio/m4a";
                case ".m4b": return "audio/m4b";
                case ".m4p": return "audio/m4p";
                case ".m4r": return "audio/x-m4r";
                case ".m4v": return "video/x-m4v";
                case ".mac": return "image/x-macpaint";
                case ".mak": return "text/plain";
                case ".man": return "application/x-troff-man";
                case ".manifest": return "application/x-ms-manifest";
                case ".map": return "text/plain";
                case ".master": return "application/xml";
                case ".mda": return "application/msaccess";
                case ".mdb": return "application/x-msaccess";
                case ".mde": return "application/msaccess";
                case ".mdp": return "application/octet-stream";
                case ".me": return "application/x-troff-me";
                case ".mfp": return "application/x-shockwave-flash";
                case ".mht": return "message/rfc822";
                case ".mhtml": return "message/rfc822";
                case ".mid": return "audio/mid";
                case ".midi": return "audio/mid";
                case ".mix": return "application/octet-stream";
                case ".mk": return "text/plain";
                case ".mmf": return "application/x-smaf";
                case ".mno": return "text/xml";
                case ".mny": return "application/x-msmoney";
                case ".mod": return "video/mpeg";
                case ".mov": return "video/quicktime";
                case ".movie": return "video/x-sgi-movie";
                case ".mp2": return "video/mpeg";
                case ".mp2v": return "video/mpeg";
                case ".mp3": return "audio/mpeg";
                case ".mp4": return "video/mp4";
                case ".mp4v": return "video/mp4";
                case ".mpa": return "video/mpeg";
                case ".mpe": return "video/mpeg";
                case ".mpeg": return "video/mpeg";
                case ".mpf": return "application/vnd.ms-mediapackage";
                case ".mpg": return "video/mpeg";
                case ".mpp": return "application/vnd.ms-project";
                case ".mpv2": return "video/mpeg";
                case ".mqv": return "video/quicktime";
                case ".ms": return "application/x-troff-ms";
                case ".msi": return "application/octet-stream";
                case ".mso": return "application/octet-stream";
                case ".mts": return "video/vnd.dlna.mpeg-tts";
                case ".mtx": return "application/xml";
                case ".mvb": return "application/x-msmediaview";
                case ".mvc": return "application/x-miva-compiled";
                case ".mxp": return "application/x-mmxp";
                case ".nc": return "application/x-netcdf";
                case ".nsc": return "video/x-ms-asf";
                case ".nws": return "message/rfc822";
                case ".ocx": return "application/octet-stream";
                case ".oda": return "application/oda";
                case ".odc": return "text/x-ms-odc";
                case ".odh": return "text/plain";
                case ".odl": return "text/plain";
                case ".odp": return "application/vnd.oasis.opendocument.presentation";
                case ".ods": return "application/oleobject";
                case ".odt": return "application/vnd.oasis.opendocument.text";
                case ".one": return "application/onenote";
                case ".onea": return "application/onenote";
                case ".onepkg": return "application/onenote";
                case ".onetmp": return "application/onenote";
                case ".onetoc": return "application/onenote";
                case ".onetoc2": return "application/onenote";
                case ".orderedtest": return "application/xml";
                case ".osdx": return "application/opensearchdescription+xml";
                case ".p10": return "application/pkcs10";
                case ".p12": return "application/x-pkcs12";
                case ".p7b": return "application/x-pkcs7-certificates";
                case ".p7c": return "application/pkcs7-mime";
                case ".p7m": return "application/pkcs7-mime";
                case ".p7r": return "application/x-pkcs7-certreqresp";
                case ".p7s": return "application/pkcs7-signature";
                case ".pbm": return "image/x-portable-bitmap";
                case ".pcast": return "application/x-podcast";
                case ".pct": return "image/pict";
                case ".pcx": return "application/octet-stream";
                case ".pcz": return "application/octet-stream";
                case ".pdf": return "application/pdf";
                case ".pfb": return "application/octet-stream";
                case ".pfm": return "application/octet-stream";
                case ".pfx": return "application/x-pkcs12";
                case ".pgm": return "image/x-portable-graymap";
                case ".pic": return "image/pict";
                case ".pict": return "image/pict";
                case ".pkgdef": return "text/plain";
                case ".pkgundef": return "text/plain";
                case ".pko": return "application/vnd.ms-pki.pko";
                case ".pls": return "audio/scpls";
                case ".pma": return "application/x-perfmon";
                case ".pmc": return "application/x-perfmon";
                case ".pml": return "application/x-perfmon";
                case ".pmr": return "application/x-perfmon";
                case ".pmw": return "application/x-perfmon";
                case ".png": return "image/png";
                case ".pnm": return "image/x-portable-anymap";
                case ".pnt": return "image/x-macpaint";
                case ".pntg": return "image/x-macpaint";
                case ".pnz": return "image/png";
                case ".pot": return "application/vnd.ms-powerpoint";
                case ".potm": return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case ".potx": return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case ".ppa": return "application/vnd.ms-powerpoint";
                case ".ppam": return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case ".ppm": return "image/x-portable-pixmap";
                case ".pps": return "application/vnd.ms-powerpoint";
                case ".ppsm": return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case ".ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case ".ppt": return "application/vnd.ms-powerpoint";
                case ".pptm": return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".prf": return "application/pics-rules";
                case ".prm": return "application/octet-stream";
                case ".prx": return "application/octet-stream";
                case ".ps": return "application/postscript";
                case ".psc1": return "application/PowerShell";
                case ".psd": return "application/octet-stream";
                case ".psess": return "application/xml";
                case ".psm": return "application/octet-stream";
                case ".psp": return "application/octet-stream";
                case ".pub": return "application/x-mspublisher";
                case ".pwz": return "application/vnd.ms-powerpoint";
                case ".qht": return "text/x-html-insertion";
                case ".qhtm": return "text/x-html-insertion";
                case ".qt": return "video/quicktime";
                case ".qti": return "image/x-quicktime";
                case ".qtif": return "image/x-quicktime";
                case ".qtl": return "application/x-quicktimeplayer";
                case ".qxd": return "application/octet-stream";
                case ".ra": return "audio/x-pn-realaudio";
                case ".ram": return "audio/x-pn-realaudio";
                case ".rar": return "application/octet-stream";
                case ".ras": return "image/x-cmu-raster";
                case ".rat": return "application/rat-file";
                case ".rc": return "text/plain";
                case ".rc2": return "text/plain";
                case ".rct": return "text/plain";
                case ".rdlc": return "application/xml";
                case ".resx": return "application/xml";
                case ".rf": return "image/vnd.rn-realflash";
                case ".rgb": return "image/x-rgb";
                case ".rgs": return "text/plain";
                case ".rm": return "application/vnd.rn-realmedia";
                case ".rmi": return "audio/mid";
                case ".rmp": return "application/vnd.rn-rn_music_package";
                case ".roff": return "application/x-troff";
                case ".rpm": return "audio/x-pn-realaudio-plugin";
                case ".rqy": return "text/x-ms-rqy";
                case ".rtf": return "application/rtf";
                case ".rtx": return "text/richtext";
                case ".ruleset": return "application/xml";
                case ".s": return "text/plain";
                case ".safariextz": return "application/x-safari-safariextz";
                case ".scd": return "application/x-msschedule";
                case ".sct": return "text/scriptlet";
                case ".sd2": return "audio/x-sd2";
                case ".sdp": return "application/sdp";
                case ".sea": return "application/octet-stream";
                case ".searchConnector-ms": return "application/windows-search-connector+xml";
                case ".setpay": return "application/set-payment-initiation";
                case ".setreg": return "application/set-registration-initiation";
                case ".settings": return "application/xml";
                case ".sgimb": return "application/x-sgimb";
                case ".sgml": return "text/sgml";
                case ".sh": return "application/x-sh";
                case ".shar": return "application/x-shar";
                case ".shtml": return "text/html";
                case ".sit": return "application/x-stuffit";
                case ".sitemap": return "application/xml";
                case ".skin": return "application/xml";
                case ".sldm": return "application/vnd.ms-powerpoint.slide.macroEnabled.12";
                case ".sldx": return "application/vnd.openxmlformats-officedocument.presentationml.slide";
                case ".slk": return "application/vnd.ms-excel";
                case ".sln": return "text/plain";
                case ".slupkg-ms": return "application/x-ms-license";
                case ".smd": return "audio/x-smd";
                case ".smi": return "application/octet-stream";
                case ".smx": return "audio/x-smd";
                case ".smz": return "audio/x-smd";
                case ".snd": return "audio/basic";
                case ".snippet": return "application/xml";
                case ".snp": return "application/octet-stream";
                case ".sol": return "text/plain";
                case ".sor": return "text/plain";
                case ".spc": return "application/x-pkcs7-certificates";
                case ".spl": return "application/futuresplash";
                case ".src": return "application/x-wais-source";
                case ".srf": return "text/plain";
                case ".SSISDeploymentManifest": return "text/xml";
                case ".ssm": return "application/streamingmedia";
                case ".sst": return "application/vnd.ms-pki.certstore";
                case ".stl": return "application/vnd.ms-pki.stl";
                case ".sv4cpio": return "application/x-sv4cpio";
                case ".sv4crc": return "application/x-sv4crc";
                case ".svc": return "application/xml";
                case ".swf": return "application/x-shockwave-flash";
                case ".t": return "application/x-troff";
                case ".tar": return "application/x-tar";
                case ".tcl": return "application/x-tcl";
                case ".testrunconfig": return "application/xml";
                case ".testsettings": return "application/xml";
                case ".tex": return "application/x-tex";
                case ".texi": return "application/x-texinfo";
                case ".texinfo": return "application/x-texinfo";
                case ".tgz": return "application/x-compressed";
                case ".thmx": return "application/vnd.ms-officetheme";
                case ".thn": return "application/octet-stream";
                case ".tif": return "image/tiff";
                case ".tiff": return "image/tiff";
                case ".tlh": return "text/plain";
                case ".tli": return "text/plain";
                case ".toc": return "application/octet-stream";
                case ".tr": return "application/x-troff";
                case ".trm": return "application/x-msterminal";
                case ".trx": return "application/xml";
                case ".ts": return "video/vnd.dlna.mpeg-tts";
                case ".tsv": return "text/tab-separated-values";
                case ".ttf": return "application/octet-stream";
                case ".tts": return "video/vnd.dlna.mpeg-tts";
                case ".txt": return "text/plain";
                case ".u32": return "application/octet-stream";
                case ".uls": return "text/iuls";
                case ".user": return "text/plain";
                case ".ustar": return "application/x-ustar";
                case ".vb": return "text/plain";
                case ".vbdproj": return "text/plain";
                case ".vbk": return "video/mpeg";
                case ".vbproj": return "text/plain";
                case ".vbs": return "text/vbscript";
                case ".vcf": return "text/x-vcard";
                case ".vcproj": return "Application/xml";
                case ".vcs": return "text/plain";
                case ".vcxproj": return "Application/xml";
                case ".vddproj": return "text/plain";
                case ".vdp": return "text/plain";
                case ".vdproj": return "text/plain";
                case ".vdx": return "application/vnd.ms-visio.viewer";
                case ".vml": return "text/xml";
                case ".vscontent": return "application/xml";
                case ".vsct": return "text/xml";
                case ".vsd": return "application/vnd.visio";
                case ".vsi": return "application/ms-vsi";
                case ".vsix": return "application/vsix";
                case ".vsixlangpack": return "text/xml";
                case ".vsixmanifest": return "text/xml";
                case ".vsmdi": return "application/xml";
                case ".vspscc": return "text/plain";
                case ".vss": return "application/vnd.visio";
                case ".vsscc": return "text/plain";
                case ".vssettings": return "text/xml";
                case ".vssscc": return "text/plain";
                case ".vst": return "application/vnd.visio";
                case ".vstemplate": return "text/xml";
                case ".vsto": return "application/x-ms-vsto";
                case ".vsw": return "application/vnd.visio";
                case ".vsx": return "application/vnd.visio";
                case ".vtx": return "application/vnd.visio";
                case ".wav": return "audio/wav";
                case ".wave": return "audio/wav";
                case ".wax": return "audio/x-ms-wax";
                case ".wbk": return "application/msword";
                case ".wbmp": return "image/vnd.wap.wbmp";
                case ".wcm": return "application/vnd.ms-works";
                case ".wdb": return "application/vnd.ms-works";
                case ".wdp": return "image/vnd.ms-photo";
                case ".webarchive": return "application/x-safari-webarchive";
                case ".webtest": return "application/xml";
                case ".wiq": return "application/xml";
                case ".wiz": return "application/msword";
                case ".wks": return "application/vnd.ms-works";
                case ".WLMP": return "application/wlmoviemaker";
                case ".wlpginstall": return "application/x-wlpg-detect";
                case ".wlpginstall3": return "application/x-wlpg3-detect";
                case ".wm": return "video/x-ms-wm";
                case ".wma": return "audio/x-ms-wma";
                case ".wmd": return "application/x-ms-wmd";
                case ".WMD": return "application/x-ms-wmd";
                case ".wmf": return "application/x-msmetafile";
                case ".wml": return "text/vnd.wap.wml";
                case ".wmlc": return "application/vnd.wap.wmlc";
                case ".wmls": return "text/vnd.wap.wmlscript";
                case ".wmlsc": return "application/vnd.wap.wmlscriptc";
                case ".wmp": return "video/x-ms-wmp";
                case ".wmv": return "video/x-ms-wmv";
                case ".wmx": return "video/x-ms-wmx";
                case ".wmz": return "application/x-ms-wmz";
                case ".wpl": return "application/vnd.ms-wpl";
                case ".wps": return "application/vnd.ms-works";
                case ".wri": return "application/x-mswrite";
                case ".wrl": return "x-world/x-vrml";
                case ".wrz": return "x-world/x-vrml";
                case ".wsc": return "text/scriptlet";
                case ".wsdl": return "text/xml";
                case ".wvx": return "video/x-ms-wvx";
                case ".x": return "application/directx";
                case ".xaf": return "x-world/x-vrml";
                case ".xaml": return "application/xaml+xml";
                case ".xap": return "application/x-silverlight-app";
                case ".xbap": return "application/x-ms-xbap";
                case ".xbm": return "image/x-xbitmap";
                case ".xdr": return "text/plain";
                case ".xht": return "application/xhtml+xml";
                case ".xhtml": return "application/xhtml+xml";
                case ".xla": return "application/vnd.ms-excel";
                case ".xlam": return "application/vnd.ms-excel.addin.macroEnabled.12";
                case ".xlc": return "application/vnd.ms-excel";
                case ".xld": return "application/vnd.ms-excel";
                case ".xlk": return "application/vnd.ms-excel";
                case ".xll": return "application/vnd.ms-excel";
                case ".xlm": return "application/vnd.ms-excel";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsb": return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case ".xlsm": return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".xlt": return "application/vnd.ms-excel";
                case ".xltm": return "application/vnd.ms-excel.template.macroEnabled.12";
                case ".xltx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case ".xlw": return "application/vnd.ms-excel";
                case ".xml": return "text/xml";
                case ".xmta": return "application/xml";
                case ".xof": return "x-world/x-vrml";
                case ".XOML": return "text/plain";
                case ".xpm": return "image/x-xpixmap";
                case ".xps": return "application/vnd.ms-xpsdocument";
                case ".xrm-ms": return "text/xml";
                case ".xsc": return "application/xml";
                case ".xsd": return "text/xml";
                case ".xsf": return "text/xml";
                case ".xsl": return "text/xml";
                case ".xslt": return "text/xml";
                case ".xsn": return "application/octet-stream";
                case ".xss": return "application/xml";
                case ".xtp": return "application/octet-stream";
                case ".xwd": return "image/x-xwindowdump";
                case ".z": return "application/x-compress";
                case ".zip": return "application/x-zip-compressed";

                default:
                    return "application/octet-stream";
            }
        }

        /// <summary>
        /// Добавляет куки авторизации к указанной HttpCookieCollection
        /// </summary>
        /// <param name="outCookies"></param>
        /// <param name="userId"></param>
        /// <param name="usrTime"></param>
        /// <param name="createPersistCookie"></param>
        public static void SetAuthCookies(this HttpCookieCollection outCookies, Guid userId, DateTime usrTime, bool createPersistCookie)
        {
            //User person = ABDB.User.Where(x => x.Id == userId).FirstOrDefault();
            string query = "SELECT SID, Ticket, UILanguage FROM [User] LEFT JOIN AuthTicket ON AuthTicket.UserId=[User].Id " +
                "WHERE [User].Id=@Id";
            DataTable tbl = AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Id", userId } });
            string sid = tbl.Rows[0].Field<string>("SID");
            //AbitDB.GetStringValue("SELECT SID FROM [User] WHERE Id=@Id", new SortedList<string, object>() { { "@Id", userId } });
            if (string.IsNullOrEmpty(sid))
                return;

            string ticket = tbl.Rows[0].Field<string>("Ticket");
            //AbitDB.GetStringValue("SELECT Ticket FROM AuthTicket WHERE UserId=@Id", new SortedList<string, object>() { { "@Id", userId } });
            //AuthTicket ticket = ABDB.AuthTicket.Where(x => x.UserId == userId).FirstOrDefault();
            if (string.IsNullOrEmpty(ticket))
                return;

            if (outCookies["sid"] != null)
            {
                outCookies["sid"].Value = sid;
                outCookies["sid"].Path = "/";
                outCookies["sid"].Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2);
            }
            else
                outCookies.Add(new HttpCookie("sid") { Path = "/", Value = sid, Expires = createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2) });

            if (outCookies["t"] != null)
            {
                outCookies["t"].Value = ticket;
                outCookies["t"].Path = "/";
                outCookies["t"].Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2);
            }
            else
                outCookies.Add(new HttpCookie("t") { Path = "/", Value = ticket, Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2) });

            if (outCookies["uilang"] != null)
            {
                outCookies["uilang"].Value = GetUILang(userId);
                outCookies["uilang"].Path = "/";
                outCookies["uilang"].Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2);
            }
            else
                outCookies.Add(new HttpCookie("uilang") { Path = "/", Value = ticket, Expires = !createPersistCookie ? usrTime.AddHours(3) : usrTime.AddYears(2) });
        }

        public static bool StrCmp(this string source, string str)
        {
            return (source.IndexOf(str, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Добавляет в коллекцию Пару ключ-значение. Если value == null, то  value = DBNull
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddItem(this SortedList<string, object> dic, string key, object value)
        {
            if (dic.ContainsKey(key))
                return;

            if (value == null)
                value = DBNull.Value;

            dic.Add(key, value);
        }

        public static bool CheckCaptcha(HttpRequestBase request, out string errMsg)
        {

            string ChallengeFieldKey = "recaptcha_challenge_field";
            string ResponseFieldKey = "recaptcha_response_field";

            var captchaChallengeValue = request.Form[ChallengeFieldKey];
            var captchaResponseValue = request.Form[ResponseFieldKey];

            var captchaValidtor = new Recaptcha.RecaptchaValidator
            {

                PrivateKey = WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"],
                RemoteIP = request.UserHostAddress,
                Challenge = captchaChallengeValue,
                Response = captchaResponseValue
            };
            try
            {
                var recaptchaResponse = captchaValidtor.Validate();
                errMsg = recaptchaResponse.ErrorMessage;
                return recaptchaResponse.IsValid;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        public static List<PersonalMessage> GetNewPersonalMessages(Guid PersonId)
        {
            List<PersonalMessage> lst = new List<PersonalMessage>();

            string query = "SELECT Id, Type, Text, Time FROM PersonalMessage WHERE PersonId=@PersonId AND IsRead=@IsRead ORDER BY Time";
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@PersonId", PersonId);
            dic.Add("@IsRead", false);
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                var rawMessages =
                    (from DataRow rw in tbl.Rows
                     select new
                     {
                         Id = rw.Field<Guid>("Id").ToString("N"),
                         Type = rw.Field<int>("Type"),
                         Text = rw.Field<string>("Text"),
                         Time = rw.Field<DateTime>("Time")
                     }).ToList();

                foreach (var tmp in rawMessages)
                {
                    PersonalMessage msg = new PersonalMessage();
                    msg.Id = tmp.Id;
                    switch (tmp.Type)
                    {
                        case 1: { msg.Type = MessageType.CommonMessage; break; }
                        case 2: { msg.Type = MessageType.CriticalMessage; break; }
                        case 3: { msg.Type = MessageType.TipMessage; break; }
                        default: { msg.Type = MessageType.CommonMessage; break; }
                    }
                    msg.Text = tmp.Text;
                    msg.Time = tmp.Time;

                    lst.Add(msg);
                }
            }
            catch { }


            return lst;
        }

        public static List<SelectListItem> GetCountryList()
        {
            bool isEng = GetCurrentThreadLanguageIsEng();
            string query = string.Format("SELECT Id, Name, NameEng FROM [Country] ORDER BY LevelOfUsing DESC, {0}", isEng ? "NameEng" : "Name" );
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            List<SelectListItem> lst =
                (from DataRow rw in tbl.Rows
                 select new SelectListItem()
                 {
                     Value = rw.Field<int>("Id").ToString(),
                     Text = isEng ? rw.Field<string>("NameEng") : rw.Field<string>("Name")
                 }).ToList();

            return lst;
        }
         
        public static string GetMailBody(string path)
        {
            string rVal = null;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                rVal = sr.ReadToEnd();
                sr.Close();
            }

            return rVal;
        }

        public static void LogError(string errMessage)
        {
            string query = "INSERT INTO LogMessages(Message) VALUES (@Message)";
            try
            {
                AbitDB.ExecuteQuery(query, new SortedList<string, object>() { { "@Message", errMessage } });
            }
            catch (Exception e)
            {
                LogToFile(e.Message);
            }
        }

        public static void LogToFile(string message)
        {
            using (StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "log.txt", true))
            {
                try
                {
                    sw.WriteLine(message);
                }
                catch { }
            }
        }

        public static void SendMail(System.Net.Mail.SmtpClient client, System.Net.Mail.MailMessage msg)
        {
            try
            {
                client.Send(msg);
                LogMailMessage(msg.To.FirstOrDefault().Address, msg);
            }
            catch (Exception e)
            {
                LogError(e.Message + ";" + (e.InnerException != null ? e.InnerException.Message : ""));
            }
        }

        public static void LogMailMessage(string email, System.Net.Mail.MailMessage msg)
        {
            string query = "INSERT INTO User_SentEmails([From], [Email], [Text]) VALUES (@From, @Email, @Text)";
            SortedList<string, object> dic = new SortedList<string, object>();
            try
            {
                dic.Add("@From", "no-reply@spb.edu");
                dic.Add("@Email", email);
                dic.Add("@Text", msg.Body);
                Util.AbitDB.ExecuteQuery(query, dic);
            }
            catch (Exception exc)
            {
                try
                {
                    query = "INSERT INTO User_SentEmails([From], [Email], [Text], [FailStatus]) VALUES (@From, @Email, @Text, @FailStatus)";
                    dic.Clear();
                    dic.Add("@From", "no-reply@spb.edu");
                    dic.Add("@Email", email);
                    dic.Add("@Text", msg.Body);
                    dic.Add("@FailStatus", exc.Message);
                    Util.AbitDB.ExecuteQuery(query, dic);
                    LogError(exc.Message + ";" + (exc.InnerException != null ? " " + exc.InnerException.Message : ""));
                }
                catch { }//вдруг база сломалась, тогда всё, не залогировать никак
            }
        }

        public static List<AppendedFile> GetFileList(Guid PersonId)
        {
            return GetFileList(PersonId, null);
        }

        public static List<AppendedFile> GetFileList(Guid PersonId, string FileType)
        {
            List<AppendedFile> lst = new List<AppendedFile>();

            string query = @"
SELECT PersonFile.Id, FileName, FileSize, Comment, IsApproved, IsReadOnly, LoadDate, ISNULL(PersonFileType.Name, 'нет') AS Name
FROM PersonFile 
LEFT JOIN PersonFileType ON PersonFile.PersonFileTypeId = PersonFileType.Id 
WHERE PersonId=@PersonId ";
            string where = "";
            if (!string.IsNullOrEmpty(FileType))
            {
                where = " and PersonFileTypeId=" + FileType + " ";
            }
            string order = " order by LoadDate desc";
            query = query + where + order;
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@PersonId", PersonId);
            
            try
            {
                DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

                lst =
                    (from DataRow rw in tbl.Rows
                     select new AppendedFile()
                     {
                         Id = rw.Field<Guid>("Id"),
                         FileName = rw.Field<string>("FileName"),
                         FileSize = rw.Field<int>("FileSize"),
                         Comment = rw.Field<string>("Comment"),
                         LoadDate = Convert.ToString(rw.Field<DateTime>("LoadDate")),
                         FileType = rw.Field<string>("Name"),
                         IsApproved = rw.Field<bool?>("IsApproved").HasValue ?
                         rw.Field<bool>("IsApproved") ? ApprovalStatus.Approved : ApprovalStatus.Rejected : ApprovalStatus.NotSet,
                         IsReadOnly = rw.Field<bool?>("IsReadOnly").HasValue ? rw.Field<bool?>("IsReadOnly").Value : false
                     }).ToList(); 
            }
            catch { }  
            return lst;
        }

        public static int GetRess(Guid PersonId)
        { 
            string query = "SELECT [NationalityId] FROM [Person] WHERE Id=@PersonId";
            int? res_nat = (int?)AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

            query = "SELECT HasRussianNationality FROM [Person] WHERE Id=@PersonId";
            bool HasRussianNationality = (bool?)AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } }) ?? false;
           
            query = "SELECT [CountryId] FROM [PersonContacts] WHERE PersonId=@PersonId";
            int? res_coun = (int?)AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });

            if (res_nat.HasValue)
            {
                if (res_nat == 193)
                {
                    if (res_coun.HasValue)
                    {
                        if (res_coun == 193)
                        { return 1; } // 193-193
                        else
                        { return 3; } // 193- !193
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    if (res_coun.HasValue)
                    {
                        if (HasRussianNationality)
                        {
                            return 3;
                        }
                        if (res_coun == 193)
                        { return 2;  } // !193 - 193
                        else
                        { return 4; } // !193-!193
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            } 
        }
        public static int IsGosLine(Guid PersonId)
        {
            int result=GetRess(PersonId);
            if (result == 1) // Рф - Рф (всегда общий прием)
                return 0; // только общий прием
            else if (result == 3) // рф -нерф (дог = общий прием, бюдж = выбор)
                return -1; // есть выбор
            else
            {  // для договора - только гослиния, для бюджета в зависимости (Снг/не Снг)
                string query = "SELECT IsSNG from Country Inner Join Person on NationalityId=Country.Id WHERE Person.Id=@PersonId";
                bool? res_nat = (bool?)AbitDB.GetValue(query, new SortedList<string, object>() { { "@PersonId", PersonId } });
                if (res_nat == true)
                    return -1; // есть выбор 
                else
                    return -1; // есть выбор (раньше было только по гослинии)
                //  return 1; //только по гослинии
            }
        }

        public static List<Mag_ApplicationSipleEntity> GetApplicationListInCommit(Guid CommitId, Guid PersonId)
        {
            List<Mag_ApplicationSipleEntity> lstRet = new List<Mag_ApplicationSipleEntity>();
            bool bisEng = GetCurrentThreadLanguageIsEng();
            using (OnlinePriemEntities context = new OnlinePriemEntities())
            {
                var AppList =
                    (from App in context.Application
                     join Entry in context.Entry on App.EntryId equals Entry.Id
                     join Semester in context.Semester on Entry.SemesterId equals Semester.Id
                     where App.PersonId == PersonId && App.CommitId == CommitId
                     orderby App.Priority
                     select new
                     {
                         App.Id,
                         Entry.StudyBasisId,
                         Entry.StudyBasisName,
                         Entry.StudyBasisNameEng,
                         Entry.StudyFormId,
                         Entry.StudyFormName,
                         Entry.StudyFormNameEng,
                         Entry.IsReduced,
                         Entry.IsParallel,
                         Entry.IsSecond,
                         Entry.FacultyName,
                         Entry.LicenseProgramId,
                         Entry.LicenseProgramName,
                         Entry.LicenseProgramNameEng,
                         Entry.ObrazProgramId,
                         Entry.ObrazProgramName,
                         Entry.ProfileId,
                         Entry.ObrazProgramNameEng,
                         Entry.ProfileName,
                         Entry.ProfileNameEng, 
                         SemestrName = Semester.Name,
                         App.HostelEduc,
                         App.IsGosLine,
                         Entry.StudyLevelGroupId,
                         StudyLevelGrName = bisEng? Entry.StudyLevelGroupNameEng : Entry.StudyLevelGroupNameRus,
                         DateOfClose = Entry.DateOfClose
                     }).ToList();
                foreach (var App in AppList)
                {
                    var Ent = new Mag_ApplicationSipleEntity()
                    {
                        Id = App.Id,
                        StudyFormId = (App.StudyFormId),
                        StudyFormName = bisEng ? (String.IsNullOrEmpty(App.StudyFormNameEng) ? App.StudyFormName : App.StudyFormNameEng) : App.StudyFormName,
                        StudyBasisId = App.StudyBasisId,
                        StudyBasisName = bisEng ? (String.IsNullOrEmpty(App.StudyBasisNameEng) ? App.StudyBasisName : App.StudyBasisNameEng) : App.StudyBasisName, 
                        IsReduced = App.IsReduced,
                        IsParallel = App.IsParallel,
                        IsSecond = App.IsSecond,
                        FacultyName = App.FacultyName,
                        ProfessionId = App.LicenseProgramId,
                        ProfessionName = bisEng ? (String.IsNullOrEmpty(App.LicenseProgramNameEng) ? App.LicenseProgramName : App.LicenseProgramNameEng) : App.LicenseProgramName, 
                        ObrazProgramId = App.ObrazProgramId,
                        ObrazProgramName = bisEng ? (String.IsNullOrEmpty(App.ObrazProgramNameEng) ? App.ObrazProgramName : App.ObrazProgramNameEng) : App.ObrazProgramName, 
                        SpecializationId = (App.ProfileId == null ? Guid.Empty : App.ProfileId.Value),
                        SpecializationName = bisEng ? (String.IsNullOrEmpty(App.ProfileNameEng) ? App.ProfileName : App.ProfileNameEng) : App.ProfileName, 
                        Hostel = App.HostelEduc,
                        SemestrName = App.SemestrName,
                        IsGosLine = App.IsGosLine,
                        StudyLevelGroupId = App.StudyLevelGroupId,
                        DateOfClose = App.DateOfClose,
                        StudyLevelGroupName = App.StudyLevelGrName
                    };
                    string query = @"  Select SP_LicenseProgram.Id as Id, SP_LicenseProgram.Name as Name from Entry 
                                        Inner join SP_StudyLevel on SP_StudyLevel.Id=StudyLevelId
                                        Inner join SP_LicenseProgram on SP_LicenseProgram.Id = Entry.LicenseProgramId 
                                        where StudyBasisId=@StudyBasisId and StudyFormId =@StudyFormId and SemesterId=1 and SP_StudyLevel.StudyLevelGroupId = @StudyLevelId";
                    DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@StudyBasisId", Ent.StudyBasisId }, { "@StudyFormId", Ent.StudyFormId }, { "@StudyLevelId", Ent.StudyLevelGroupId } });
                    var ProfessionList =
                        (from DataRow rw in tbl.Rows
                         select new
                         {
                             Id = rw.Field<int>("Id"),
                             Name = rw.Field<string>("Name")
                         }).ToList();
                    Ent.ProfessionList = ProfessionList.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    lstRet.Add(Ent);
                    query = @"Select Reason from PersonChangeStudyFormReason
                                where PersonId=@PersonId and ApplicationId =@AppId and CommitId=@CommitId";
                    Ent.ChangeStudyFormReason = Util.AbitDB.GetStringValue(query, new SortedList<string, object>() { { "@PersonId", PersonId }, { "@AppId", App.Id }, { "@CommitId", CommitId } });

                    //ВАЖНО!!! При изменении заявления нужно, чтобы все конкурсы были "не удалёнными"
                    var Applic = context.Application.Where(x => x.Id == Ent.Id).FirstOrDefault();
                    if (Applic != null)
                        Applic.IsDeleted = false;
                }

                context.SaveChanges();

                return lstRet;
            }
        }

        public static List<SelectListItem> GetStudyLevelGroupList()
        {
            string query = "SELECT Id, NameRus FROM StudyLevelGroup ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("Id"),
                     Text = rw.Field<string>("NameRus")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static List<SelectListItem> GetStudyBasisList()
        {
            bool isEng = GetCurrentThreadLanguageIsEng();
            
            string query = "SELECT DISTINCT StudyBasisId, StudyBasisName, StudyBasisNameEng FROM Entry ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("StudyBasisId"),
                     Text = isEng ?
                     (string.IsNullOrEmpty(rw.Field<string>("StudyBasisNameEng")) ? rw.Field<string>("StudyBasisName") : rw.Field<string>("StudyBasisNameEng"))
                     : rw.Field<string>("StudyBasisName")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static List<SelectListItem> GetQualificationList(bool IsEng)
        {
            string query = "SELECT Id, Name, NameEng FROM Qualification ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("Id"),
                     Text = IsEng ? (String.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng")) : rw.Field<string>("Name")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static List<SelectListItem> GetStudyFormList()
        {
            bool isEng = GetCurrentThreadLanguageIsEng();

            string query = "SELECT DISTINCT StudyFormId, StudyFormName, StudyFormNameEng FROM Entry ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("StudyFormId"),
                     Text = isEng ? 
                     (string.IsNullOrEmpty(rw.Field<string>("StudyFormNameEng")) ? rw.Field<string>("StudyFormName") : rw.Field<string>("StudyFormNameEng"))
                     : rw.Field<string>("StudyFormName")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static List<SelectListItem> GetSemestrList()
        {
            string query = "SELECT DISTINCT Semester.Id as Id, Semester.Name as Name FROM Semester INNER JOIN Entry ON Entry.SemesterId = Semester.Id WHERE Semester.Id > 1 AND ((Entry.DateOfStart<@Date AND Entry.DateOfClose>@Date)or(Entry.DateOfStart_GosLine<@Date AND Entry.DateOfClose_GosLine>@Date)or(Entry.DateOfStart_Foreign<@Date AND Entry.DateOfClose_Foreign>@Date) )";
            /*if (bIsIGA)
                query += " AND Semester.IsIGA=1 ";
            else
                query += " AND Semester.IsIGA=0 ";
            */
            DataTable tbl = Util.AbitDB.GetDataTable(query, new SortedList<string, object>() { { "@Date", DateTime.Now }});
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("Id"),
                     Text = rw.Field<string>("Name")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList(); 
        }

        public static List<SelectListItem> GetLanguageList()
        {
            bool isEng = GetCurrentThreadLanguageIsEng();

            string query = "SELECT Id, Name, NameEng FROM Language ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("Id"),
                     Text = isEng ?
                     (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                     : rw.Field<string>("Name")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static List<SelectListItem> GetPersonFileTypeList()
        {
            bool isEng = GetCurrentThreadLanguageIsEng();

            string query = "SELECT Id, Name, NameEng FROM PersonFileType ORDER BY 1";
            DataTable tbl = Util.AbitDB.GetDataTable(query, null);
            return
                (from DataRow rw in tbl.Rows
                 select new
                 {
                     Value = rw.Field<int>("Id"),
                     Text = isEng ?
                     (string.IsNullOrEmpty(rw.Field<string>("NameEng")) ? rw.Field<string>("Name") : rw.Field<string>("NameEng"))
                     : rw.Field<string>("Name")
                 }).AsEnumerable()
                    .Select(x => new SelectListItem() { Text = x.Text, Value = x.Value.ToString() })
                    .ToList();
        }

        public static void CommitApplication(Guid CommitId, Guid PersonId, OnlinePriemEntities context)
        {
            var Ids = context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && !x.IsDeleted).Select(x => x.Id).ToList();
            foreach (var AppId in Ids)
            {
                var App = context.Application.Where(x => x.Id == AppId).FirstOrDefault();
                if (App == null)
                    continue;

                App.IsCommited = true;
                var AppDetails_exists = context.extApplicationDetails.Where(x => x.ApplicationId == AppId).Select(x => new
                    {
                        EntryId = App.EntryId,
                        ObrazProgramInEntryId = x.ObrazProgramInEntryId,
                        ProfileInObrazProgramInEntryId = x.ProfileInObrazProgramInEntryId
                    }).ToList();
                var AppDetails_clearFullList = context.extDefaultEntryDetails.Where(x => x.EntryId == App.EntryId).Select(x => new
                {
                    EntryId = App.EntryId,
                    ObrazProgramInEntryId = x.ObrazProgramInEntryId,
                    ProfileInObrazProgramInEntryId = x.ProfileInObrazProgramInEntryId,
                    x.DefaultPriorityValue,
                    x.ProfileInObrazProgramInEntryDefaultPriorityValue
                }).ToList();
                foreach (var Details in AppDetails_clearFullList)
                {
                    var lstI = AppDetails_exists.Where(x => x.ObrazProgramInEntryId == Details.ObrazProgramInEntryId);
                    if (Details.ProfileInObrazProgramInEntryId.HasValue)
                        lstI = lstI.Where(x => x.ProfileInObrazProgramInEntryId == Details.ProfileInObrazProgramInEntryId);
                    else
                        lstI = lstI.Where(x => x.ProfileInObrazProgramInEntryId == null);
                    if (lstI.Count() == 0)
                    {
                        context.ApplicationDetails.AddObject(new ApplicationDetails()
                        {
                            Id = Guid.NewGuid(),
                            ApplicationId = AppId,
                            ObrazProgramInEntryId = Details.ObrazProgramInEntryId,
                            ObrazProgramInEntryPriority = Details.DefaultPriorityValue,
                            ProfileInObrazProgramInEntryId = Details.ProfileInObrazProgramInEntryId,
                            ProfileInObrazProgramInEntryPriority = Details.ProfileInObrazProgramInEntryDefaultPriorityValue
                        });
                    }
                }
            }
            context.SaveChanges();

            //всё, что вне коммита - удаляем
            Ids = context.Application.Where(x => x.PersonId == PersonId && x.CommitId != CommitId && x.IsCommited == false).Select(x => x.Id).ToList();
            foreach (var AppId in Ids)
            {
                var App = context.Application.Where(x => x.Id == AppId).FirstOrDefault();
                context.Application.DeleteObject(App);
            }

            //всё, что удалено из коммита - удаляем
            Ids = context.Application.Where(x => x.PersonId == PersonId && x.CommitId == CommitId && x.IsDeleted == true).Select(x => x.Id).ToList();
            foreach (var AppId in Ids)
            {
                var App = context.Application.Where(x => x.Id == AppId).FirstOrDefault();
                context.Application.DeleteObject(App);
            }
            context.SaveChanges();
        }

        public static Constants getConstInfo()
        {
            Constants constant = new Constants();

            constant.Surname = 150;
            constant.Name = 150;
            constant.SecondName = 150;

            constant.BirthPlace = 500;
            constant.PassportAuthor = 2500;

            constant.AddInfo = 4000;
            constant.Parents = 4000;
 
            return constant;
        }

        public static void CopyApplicationsInAnotherCommit(Guid CommitId, Guid gComm, Guid PersonId)
        {
 
            using (OnlinePriemEntities context = new OnlinePriemEntities()){

                var abitList = (from x in context.Application
                                join Commit in context.ApplicationCommit on x.CommitId equals Commit.Id
                                join Entry in context.Entry on x.EntryId equals Entry.Id
                                where x.CommitId == CommitId
                                select new
                                {
                                    x.Id,
                                    x.PersonId,
                                    x.EntryId, 
                                    x.HostelEduc,
                                    x.Priority,
                                    x.EntryType,
                                    x.IsGosLine,
                                    x.DateOfStart,
                                    x.SecondTypeId,
                                    x.IsImported,
                                    x.CompetitionId, 
                                    x.ApproverName, 
                                    x.DocInsertDate, 
                                    x.DateReviewDocs
                                }).OrderBy(x => x.Priority).ToList();
                foreach (var s in abitList)
                {
                    Guid appId = Guid.NewGuid();
                    context.Application.AddObject(new Application()
                    {
                        Id = appId,
                        PersonId = PersonId,
                        EntryId = s.EntryId,
                        HostelEduc = s.HostelEduc,
                        Priority = s.Priority,
                        Enabled = true,
                        EntryType = s.EntryType,
                        DateOfStart = DateTime.Now,
                        CommitId = gComm,
                        IsGosLine = s.IsGosLine,
                        IsCommited = false,
                        SecondTypeId = s.SecondTypeId,
                        IsImported = s.IsImported,
                        CompetitionId = s.CompetitionId, 
                        ApproverName = s.ApproverName, 
                        DocInsertDate =s.DocInsertDate, 
                        DateReviewDocs=s.DateReviewDocs
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
