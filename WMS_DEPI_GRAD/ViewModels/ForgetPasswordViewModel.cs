using System.ComponentModel.DataAnnotations;

namespace WMS_DEPI_GRAD.ViewModels
{
	public class ForgetPasswordViewModel
	{
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "Email is required")]
		public string Email { get; set; } = null!;
	}
}
