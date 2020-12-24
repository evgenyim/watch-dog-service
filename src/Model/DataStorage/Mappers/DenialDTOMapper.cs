using System;
using System.Data.SqlClient;
using DTO;

namespace DataStorage.Mappers
{
    class DenialDTOMapper : IMapper<DenialDTO>
    {
        public DenialDTO ReadItem(SqlDataReader rd)
        {
            int i = (int) rd["StartWorking"];
            bool status;
            if (i == 1)
                status = true;
            else
                status = false;
            return new DenialDTO
            {
                Id = (int)rd["Id"],
                ServiceId = (int)rd["ServiceId"],
                StartWorking = status,
                Time = (DateTime)rd["Time"]
            };
        }
    }
}
