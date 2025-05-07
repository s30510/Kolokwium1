using WebApplication1.Moduls.DTOs;

namespace WebApplication1.Services;

public  interface IService
{
    public Task<AppointmentDto> GetAppointmentAsync(string appointmentId);
    public Task AddAppointmentsAsync(NewAppointmentDto newAppointmentDto);
}