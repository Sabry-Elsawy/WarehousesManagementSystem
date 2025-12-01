using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels;

public class LoginViewModel
{
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
