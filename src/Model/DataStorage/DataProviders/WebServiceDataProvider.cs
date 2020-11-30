﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml;
using DataStorage.Mappers;
using DTO;
using SqlHelper;
using Model;

namespace DataStorage.DataProviders
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

        public static IList<WebServiceDTO> InsertService(int Id, WebService service)
        {
            string sqlQuery = XmlStrings.GetString(Tables.WebServices, "InsertService");
            SqlParameter paramId = new SqlParameter("@Id", Id);
            SqlParameter paramUrl = new SqlParameter("@Url", service.url);
            SqlParameter paramCheckUrl = new SqlParameter("@CheckUrl", service.checkUrl);
            SqlParameter paramTimeCheck = new SqlParameter("@TimeCheck", service.timeCheck);

            var result = DBHelper.GetData(
                new WebServiceDTOMapper(),
                sqlQuery, paramId, paramUrl, paramCheckUrl, paramTimeCheck);
            return result;
        }
    }
}
