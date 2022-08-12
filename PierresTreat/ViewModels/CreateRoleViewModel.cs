using System.ComponentModel.DataAnnotations;

namespace PierresTreat.ViewModels
{
  public class CreateRoleViewModel
  {
    [Required]
    public string RoleName { get; set; }
  }
}