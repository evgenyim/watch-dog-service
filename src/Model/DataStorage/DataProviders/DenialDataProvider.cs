using System.Collections.Generic;
using System.Data.SqlClient;
using Model.DataStorage.Mappers;
using Model.DTO;
using Model.SqlHelper;
using Model.ServiceStorage;

namespace Model.DataStorage.DataProviders
{
    public class DenialDataProvider
    {
        public static IList<DenialDTO> GetAllDenials()
        {
            string sqlQuery = XmlStrings.GetString(Tables.Denials, "GetAllDenials");

            var result = DBHelper.GetData(
                new DenialDTOMapper(),
                sqlQuery);
            return result;
        }

        public static IList<DenialDTO> DeleteById(int Id)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Denials, "DeleteById");
            SqlParameter paramId = new SqlParameter("@Id", Id);

            var result = DBHelper.GetData(
                new DenialDTOMapper(),
                sqlQuery, paramId);
            return result;
        }

        public static IList<DenialDTO> DeleteByServiceId(int ServiceId)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Denials, "DeleteById");
            SqlParameter paramId = new SqlParameter("@ServiceId", ServiceId);

            var result = DBHelper.GetData(
                new DenialDTOMapper(),
                sqlQuery, paramId);
            return result;
        }

        public static IList<DenialDTO> InsertDenial(int Id, Denial denial)
        {
            string sqlQuery = XmlStrings.GetString(Tables.Denials, "InsertDenial");
            SqlParameter paramId = new SqlParameter("@Id", Id);
            SqlParameter paramServiceId = new SqlParameter("@ServiceId", denial.ServiceId);
            SqlParameter paramStartWorking = new SqlParameter("@StartWorking", denial.StartWorking);
            SqlParameter paramTime = new SqlParameter("@Time", denial.Time);

            var result = DBHelper.GetData(
                new DenialDTOMapper(),
                sqlQuery, paramId, paramServiceId, paramStartWorking, paramTime);
            return result;
        }
    }
}
