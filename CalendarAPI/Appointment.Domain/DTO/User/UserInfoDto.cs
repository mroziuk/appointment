using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Domain.DTO.User;

public class UserInfoDto
{
    public UserInfoDto(int id, string firstName, string lastName, string email, string role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Role = role;
    }
    public UserInfoDto() { }
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; init; }
    public string Role { get; init; }
}
