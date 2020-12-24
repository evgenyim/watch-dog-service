using System;
using System.Data.SqlClient;
using Model.DTO;

namespace Model.DataStorage.Mappers
{
    class WebServiceDTOMapper : IMapper<WebServiceDTO>
    {
        public WebServiceDTO ReadItem(SqlDataReader rd)
        {
            return new WebServiceDTO
            {
                Id = (int)rd["Id"],
                Type = (string)rd["Type"],
                Url = (string)rd["Url"],
                CheckUrl = (string)rd["CheckUrl"],
                TimeCheck = (int)rd["TimeCheck"],
            };
        }
    }
}
