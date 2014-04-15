using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Data;
using System.Web;
using OnlineAbit2013.Controllers;

namespace OnlineAbit2013
{
    public class LangItem
    {
        public int Id { get; set; }
        public string TextRU { get; set; }
        public string TextEN { get; set; }
    }

    public static class LangPack
    {
        private static List<string> Rus;
        private static List<string> Eng;

        private static List<LangItem> _items;

        public static string GetValue(int textid, string lang)
        {
            if (lang.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
                return Rus[textid] != null ? Rus[textid] : "";
            if (lang.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                return Eng[textid] != null ? Eng[textid] : "";

            //default language
            return Rus[textid] != null ? Rus[textid] : "";
        }

        static LangPack()
        {
            //string query = "SELECT Id, TextRU, TextEN FROM UILangPack";
            //DataTable tbl = Util.AbitDB.GetDataTable(query, null);

            //_items =
            //    (from DataRow rw in tbl.Rows
            //     select new LangItem()
            //     {
            //         Id = rw.Field<int>("Id"),
            //         TextRU = rw.Field<string>("TextRU"),
            //         TextEN = rw.Field<string>("TextEN")
            //     }).ToList();


            Rus = new List<string>();
            Eng = new List<string>();

            //0
            Rus.Add("");
            Eng.Add("");

            //1
            Rus.Add("Фамилия");
            Eng.Add("Surname");

            //2
            Rus.Add("Имя");
            Eng.Add("Name");

            //3
            Rus.Add("Отчество");
            Eng.Add("Second name");

            //4
            Rus.Add("Пол");
            Eng.Add("Sex");

            //5
            Rus.Add("Мужской");
            Eng.Add("Male");

            //6
            Rus.Add("Женский");
            Eng.Add("Female");

            //7
            Rus.Add("Дата рождения");
            Eng.Add("Birth date");

            //8
            Rus.Add("Место рождения");
            Eng.Add("Birth place");

            //9
            Rus.Add("Гражданство");
            Eng.Add("Citizenship");

            //10
            Rus.Add("Личный кабинет");
            Eng.Add("Personal Office");

            //11
            Rus.Add("Подать заявление");
            Eng.Add("Apply");

            //12
            Rus.Add("Выставить приоритеты");
            Eng.Add("Define priorities");

            //13
            Rus.Add("Загрузить документы");
            Eng.Add("Download documents");

            //14
            Rus.Add("Мотивационное письмо");
            Eng.Add("Motivational letter");

            //15
            Rus.Add("Опись поданых документов");
            Eng.Add("List of documents submitted");

            //16
            Rus.Add("Редактировать данные анкеты");
            Eng.Add("Edit Personal office data");

            //17
            Rus.Add("Изменить пароль");
            Eng.Add("Change your password");

            //18
            Rus.Add("При создании учётной записи возникли следующие ошибки:");
            Eng.Add("There was a problem while creating account:");

            //19
            Rus.Add("Данные учётной записи");
            Eng.Add("Account information");

            //20
            Rus.Add("");
            Eng.Add("User name");

            //21
            Rus.Add("");
            Eng.Add("");

            //22
            Rus.Add("");
            Eng.Add("");

            //23
            Rus.Add("");
            Eng.Add("");

            //24
            Rus.Add("");
            Eng.Add("");

            //25
            Rus.Add("");
            Eng.Add("");

            //26
            Rus.Add("");
            Eng.Add("");

            //27
            Rus.Add("");
            Eng.Add("");

            //28
            Rus.Add("");
            Eng.Add("");

            //29
            Rus.Add("");
            Eng.Add("");

            //30
            Rus.Add("");
            Eng.Add("");

            //31
            Rus.Add("");
            Eng.Add("");

            //32
            Rus.Add("");
            Eng.Add("");

            //33
            Rus.Add("");
            Eng.Add("");

            //34
            Rus.Add("");
            Eng.Add("");

            //35
            Rus.Add("");
            Eng.Add("");

            //36
            Rus.Add("");
            Eng.Add("");

            //37
            Rus.Add("");
            Eng.Add("");

        }

        public static string Value(int id, string lang)
        {
            var val = _items.Where(x => x.Id == id).FirstOrDefault();
            if (val == null)
                return "";

            if (lang.StartsWith("ru", StringComparison.OrdinalIgnoreCase))
                return val.TextRU;
            if (lang.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                return val.TextEN;

            //If we going so far, something wrong
            return "";
        }

        public static void RefreshLangPack()
        {
            lock (_items)//block LangPack while updating _items
            {
                _items.Clear();
                string query = "SELECT Id, NameRU, NameEN FROM UILangPack";
                DataTable tbl = Util.AbitDB.GetDataTable(query, null);

                _items =
                    (from DataRow rw in tbl.Rows
                     select new LangItem()
                     {
                         Id = rw.Field<int>("Id"),
                         TextRU = rw.Field<string>("NameRU"),
                         TextEN = rw.Field<string>("NameEN")
                     }).ToList();
            }
        }
    }
}