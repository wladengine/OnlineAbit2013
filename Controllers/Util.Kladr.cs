using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

using HibernateKLADR;

namespace OnlineAbit2013.Controllers
{
    public static partial class Util
    {
        private static List<Kladr> _regions;

        public static Dictionary<string, string> GetCityListByRegion(string sRegionKladrCode)
        {
            Kladr s = null;
            if (_regions == null)
                _regions = DAOFactory.getInstance().getDAOKladr().getAllRegions().ToList();
            s = _regions.Where(x => x.Code == sRegionKladrCode).FirstOrDefault();
            if (s == null)
                return new System.Collections.Generic.Dictionary<string, string>();

            return DAOFactory.getInstance().getDAOKladr().getAllGorodaNasPunkti(s).Select(x => new { x.Code, x.Name }).Distinct().ToDictionary(x => x.Code, y => y.Name);
        }

        public static Dictionary<string, string> GetStreetListByRegion(string sRegionKladrCode, string sCityName)
        {
            Kladr s = null;

            if (_regions == null)
                _regions = DAOFactory.getInstance().getDAOKladr().getAllRegions().ToList();
            s = _regions.Where(x => x.Code == sRegionKladrCode).FirstOrDefault();
            if (s == null)
                return new System.Collections.Generic.Dictionary<string, string>();

            //Kladr city = DAOFactory.getInstance().getDAOKladr().getAllGorodaNasPunkti(s).Where(x => x.Name == sCityName).FirstOrDefault();
            Kladr city = DAOFactory.getInstance().getDAOKladr().getGorodNasPunktByName(s, sCityName).FirstOrDefault();
            if (city == null)
                return new System.Collections.Generic.Dictionary<string, string>();

            return DAOFactory.getInstance().getDAOStreet().getAll(city).Select(x => new { x.Code, x.Name, x.Socr }).Distinct().ToDictionary(x => x.Code, x => x.Name + " " + x.Socr + ".");
        }

        public static List<string> GetHouseListByStreet(string sRegionKladrCode, string sCityName, string sStreetName)
        {
            Kladr s = null;

            if (_regions == null)
                _regions = DAOFactory.getInstance().getDAOKladr().getAllRegions().ToList();

            s = _regions.Where(x => x.Code == sRegionKladrCode).FirstOrDefault();
            if (s == null)
                return new List<string>();

            //Kladr city = DAOFactory.getInstance().getDAOKladr().getAllGorodaNasPunkti(s).Where(x => x.Name == sCityName).FirstOrDefault();
            Kladr city = DAOFactory.getInstance().getDAOKladr().getGorodNasPunktByName(s, sCityName).FirstOrDefault();
            if (city == null)
                return new List<string>();

            Street street = DAOFactory.getInstance().getDAOStreet().getAll(city).Where(x => (x.Name + " " + x.Socr + ".") == sStreetName).FirstOrDefault();
            if (street == null)
                return new List<string>();

            var houses = DAOFactory.getInstance().getDAODoma().getAll(street).Select(x => new { x.Name, x.Code });
            if (houses == null)
                return new List<string>();

            List<string> lstHouses = new List<string>();

            foreach (var house in houses)
            {
                string[] h = house.Name.Split(',');
                foreach(string hh in h)
                {
                    lstHouses.Add(hh);
                }
            }

            return lstHouses;
        }

        public static string GetKladrCodeByAddress(string sRegionKladrCode, string sCityName, string sStreetName, string sHouseName)
        {
            string sKladrCode = string.Empty;

            Kladr klRegion = null;
            if (_regions == null)
                _regions = DAOFactory.getInstance().getDAOKladr().getAllRegions().ToList();

            klRegion = _regions.Where(x => x.Code == sRegionKladrCode).FirstOrDefault();

            if (klRegion == null)
                return sRegionKladrCode;

            Kladr klCity = DAOFactory.getInstance().getDAOKladr().getGorodNasPunktByName(klRegion, sCityName).FirstOrDefault();
            if (klCity == null)
                return klRegion.Code;

            Street klStreet = DAOFactory.getInstance().getDAOStreet().getAll(klCity).Where(x => (x.Name + " " + x.Socr + ".") == sStreetName).FirstOrDefault();
            if (klStreet == null)
                return klCity.Code;

            var houses = DAOFactory.getInstance().getDAODoma().getAll(klStreet).Select(x => new { x.Name, x.Code });

            var house = houses.Where(x => x.Name.IndexOf(sHouseName, 0, StringComparison.OrdinalIgnoreCase) > -1).FirstOrDefault();
            if (house == null)
                return klStreet.Code;

            sKladrCode = house.Code;

            return sKladrCode;
        }

        public static string GetRegionKladrCodeByRegionId(string regionId)
        {
            string query = "SELECT KladrCode FROM Region WHERE Id=@Id";
            int iRegionId = 0;
            int.TryParse(regionId, out iRegionId);
            SortedList<string, object> dic = new SortedList<string, object>();
            dic.Add("@Id", iRegionId);
            DataTable tbl = Util.AbitDB.GetDataTable(query, dic);

            return Util.AbitDB.GetStringValue(query, dic);
        }
    }
}