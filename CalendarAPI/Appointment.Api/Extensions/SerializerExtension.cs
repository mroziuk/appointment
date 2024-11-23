using Appointment.Domain.DTO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Appointment.Common;
using Appointment.Domain.DTO.Patient;
using Appointment.Domain.DTO.Room;
using Appointment.Domain.DTO.Visit;
using Appointment.Domain.DTO.Availability;
using Appointment.Domain.DTO.User;

namespace Appointment.Api.Extensions
{
    public static class SerializerExtension
    {
        public static void AddSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            AddUpdateModelsConverter(serializerSettings);
            AddDtoModelConverter(serializerSettings);

            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private static void AddUpdateModelsConverter(JsonSerializerSettings serializerSettings)
        {
            UpdateModelConverter<UpdateUserDto>.Register(serializerSettings);
            UpdateModelConverter<UpdateRoomDto>.Register(serializerSettings);
            UpdateModelConverter<UpdatePatientDto>.Register(serializerSettings);
            UpdateModelConverter<UpdateVisitDto>.Register(serializerSettings);
            UpdateModelConverter<UpdateAvailabilityDto>.Register(serializerSettings);
        }
        private static void AddDtoModelConverter(JsonSerializerSettings serializerSettings)
        {
            DtoJsonConverter<AddAvailabilityDto>.Register(serializerSettings);
        }
    }
}
