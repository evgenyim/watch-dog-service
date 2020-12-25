using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using Model.DataStorage.Mappers;
using Model.DTO;
using Model.SqlHelper;
using Model.ServiceStorage;

namespace Model.DataStorage.DataProviders
{
    public class WebServiceDataProvider
    {
        public static IList<WebServiceDTO> GetAllServices()
        {
            string sqlQuery = XmlStrings.GetString(Tables.WebServices, "GetAllServices");

            var result = DBHelper.GetData(
                new WebServiceDTOMapper(),
                sqlQuery);
            return result;
        }

        public static IList<WebServiceDTO> DeleteById(int Id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.WebServices, "DeleteById");
            SqlParameter paramId = new SqlParameter("@Id", Id);

            var result = DBHelper.GetData(
                new WebServiceDTOMapper(),
                sqlQuery, paramId);
            return result;
        }

        public static IList<WebServiceDTO> InsertService(WebService service)
        {
            string sqlQuery = XmlStrings.GetString(Tables.WebServices, "InsertService");
            SqlParameter paramId = new SqlParameter("@Id", service.Id);
            SqlParameter paramUrl = new SqlParameter("@Url", service.Url);
            SqlParameter paramCheckUrl = new SqlParameter("@CheckUrl", service.CheckUrl);
            SqlParameter paramTimeCheck = new SqlParameter("@TimeCheck", service.TimeCheck);

            var result = DBHelper.GetData(
                new WebServiceDTOMapper(),
                sqlQuery, paramId, paramUrl, paramCheckUrl, paramTimeCheck);
            return result;
        }
    }
}
