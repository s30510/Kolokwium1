namespace WebApplication1.Moduls.DTOs;

public class AppointmentDto
{
    public DateTime date { get; set; }
    public PatientDto patient { get; set; }
    public DoctorDto doctor { get; set; }
    public List<AppointmentDto> services { get; set; }
    
    
}


public class PatientDto
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
    
}
public class DoctorDto
{
    public int id { get; set; }
    public string pwz { get; set; }
    
}

public class AppointmentServiceDto
{
    
}