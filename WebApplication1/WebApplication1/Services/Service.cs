using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using WebApplication1.Moduls.DTOs;

namespace WebApplication1.Services;

public class Service : IService
{
    private string _connectionString;
    public Service(IConfiguration configuration)
    {
       _connectionString = configuration.GetConnectionString("Default");
    }

    public async Task<AppointmentDto> GetAppointmentAsync(string appointmentId)
    {
        AppointmentDto appointmentDto = new AppointmentDto();

        string sqlTxtCommand = "SELECT date,Patient.first_name,Patient.last_name,Patient.date_of_birth,Doctor.doctor_id,Doctor.PWZ FROM Appointment\nINNER JOIN Patient ON Appointment.patient_id=Patient.patient_id\nINNER JOIN Doctor ON Appointment.doctor_id=Doctor.doctor_id\nWHERE appoitment_id =@appointmentId";
        
        await using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        await using (SqlCommand sqlCommand = new SqlCommand(sqlTxtCommand, sqlConnection))
        {
            await sqlCommand.Connection.OpenAsync();
            sqlCommand.Parameters.AddWithValue("@appointmentId", appointmentId);
            
            var reader = await sqlCommand.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                PatientDto patientDto = new PatientDto()
                {
                    firstName =  reader.GetString(1),
                    lastName = reader.GetString(2),
                    dateOfBirth = reader.GetDateTime(3),
                    
                };

                DoctorDto doctorDto = new DoctorDto()
                {
                    id = reader.GetInt32(4),
                    pwz=reader.GetString(5),

                };
                
                appointmentDto = new AppointmentDto()
                {
                    date = reader.GetDateTime(0),
                    patient = patientDto,
                    doctor = doctorDto,
                };
            }
            
        }
        return appointmentDto;
    }

    public async Task AddAppointmentsAsync(NewAppointmentDto newAppointmentDto)
    {

        string checkPatientExixtsCommandtxt = "SELECT 1 WHERE EXISTS(SELECT * FROM Patient WHERE patient_id = @patientId)";
        string checkDoctorExixtsCommandtxt = "SELECT 1 WHERE EXISTS(SELECT * FROM Doctor WHERE pwz = @pwz)";
        string checkAppointmentExistsCommandtxt = "SELECT 1 WHERE EXISTS(SELECT * FROM Appointment WHERE appointment_id = @appointmentId)";
        await using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        {
            await sqlConnection.OpenAsync();
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try{
                await using (SqlCommand sqlCommand1 =
                             new SqlCommand(checkPatientExixtsCommandtxt, sqlConnection, sqlTransaction))
                {
sqlCommand1.Parameters.AddWithValue("@patientId", newAppointmentDto.patientId);
                    
                    if (await sqlCommand1.ExecuteScalarAsync() == null)
                    {
                        throw new NotFoundExpection__();
                    }

                }

                await using (SqlCommand sqlCommand2 =
                             new SqlCommand(checkDoctorExixtsCommandtxt, sqlConnection, sqlTransaction))
                {
                    sqlCommand2.Parameters.AddWithValue("@pwz", newAppointmentDto.pwz);
                    if (await sqlCommand2.ExecuteScalarAsync() == null)
                    {
                        throw new NotFoundExpection__();
                    }
                }
                
                await using (SqlCommand sqlCommand =
                             new SqlCommand(checkAppointmentExistsCommandtxt, sqlConnection, sqlTransaction))
                {
                    sqlCommand.Parameters.AddWithValue("@pwz", newAppointmentDto.pwz);
                    if (await sqlCommand.ExecuteScalarAsync() != null)
                    {
                        throw new ConflictExpection();
                    }
                }

                string insertAppointmenttxt = "INSERT INTO Appointment VALUES()";
                string insertSevicetxt= "INSERT INTO ";
                
                int appoinmentid = newAppointmentDto.appointmentId;
                int patientId = newAppointmentDto.patientId;
                string pwz = newAppointmentDto.pwz;

                List<ServiceDto> services = newAppointmentDto.services;
                
                await using (SqlCommand sqlCommand = new SqlCommand(insertAppointmenttxt, sqlConnection, sqlTransaction))
                {
                    sqlCommand.Parameters.AddWithValue("@appointmentId", appoinmentid);
                    sqlCommand.Parameters.AddWithValue("@patientId", patientId);
                    sqlCommand.Parameters.AddWithValue("@pwz", pwz);
                }

                await using (SqlCommand sqlCommand = new SqlCommand(insertSevicetxt, sqlConnection, sqlTransaction))
                {
                    foreach (ServiceDto service in services)
                    {
                        sqlCommand.Parameters.AddWithValue("@serviceName", service.serviceName);
                        sqlCommand.Parameters.AddWithValue("@ServiceFee", service.serviceFee);
                        await sqlCommand.ExecuteNonQueryAsync();
                    }

                }
                
                sqlTransaction.Commit();
            } catch(Exception)
            {
                sqlTransaction.Rollback();
                throw; 
            }
        }
            
    }
}