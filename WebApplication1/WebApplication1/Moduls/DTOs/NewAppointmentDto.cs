namespace WebApplication1.Moduls.DTOs;

public class NewAppointmentDto
{
    
    public int appointmentId { get; set; }
    public int patientId { get; set; }
    public string pwz { get; set; }
    
    public List<ServiceDto> services { get; set; }
    
}

public class ServiceDto
{
    public string serviceName { get; set; }
    public Decimal serviceFee { get; set; }
}