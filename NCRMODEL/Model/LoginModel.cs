using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NCRMODEL.Model
{
   public class LoginModel
    {
        [Required(ErrorMessage = "please enter UserID")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Special characters & Space are not allowed")]
        public string UserID { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(40, ErrorMessage = "Max 40 characters")]
      
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(40, ErrorMessage = "Max 40 characters")]
        public string OldPassword { get; set; }
        [StringLength(40, ErrorMessage = "Max 40 characters")]
        [Required(ErrorMessage = "Required")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password")]
        [DisplayName("Confirm Password :")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string OTP { get; set; }
        public string EncryptedUserId { get; set; }
        public string ReturnMessage { get; set; }
        public Int32 ReturnCode { get; set; }
    }
    public class LoginRespone
    {
        public string ReturnCode { get; set; } //-1:Error/0:missing or validation /1:success
        public string ReturnMessage { get; set; } // error message/any return messaage
        public UserModel UserDetail { get; set; }
        public UserModel UserInfo { get; set; }
       
    }

    public class UserModel
    {
        [Required(ErrorMessage = "please enter first name")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Special characters & Space are not allowed")]
        public string FirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Special characters & Space are not allowed")]
        public string MI { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Special characters & Space are not allowed")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter valid email")]
        public string OldEmail { get; set; }
        //[Required(ErrorMessage = "Please enter mobile number")]
        [MinLength(14, ErrorMessage = "Minimum 10 digits are required")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Please select Time Zone")]
        public string TimeZone { get; set; }

        [Required(ErrorMessage = "Please select state")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Please enter city")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "Please enter Address Line1")]
        public string AddressLine1 { get; set; }
        public string OldPasswordCompare { get; set; }
        public int UserType { get; set; }
        public string OldPasswordCompare1 { get; set; }
        public string AddressLine2 { get; set; }

        [Required(ErrorMessage = "Please enter ZIP")]
        public string Zip { get; set; }
        public string ProfilePic { get; set; }
        public string EncryptedSessionToken { get; set; }
        public string EncryptedUserId { get; set; }
        public string SessionToken { get; set; }
        public string ISDCode { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Max 40 characters")]
        [DisplayName("Password :")]
        public string Password { get; set; }
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [StringLength(40, ErrorMessage = "Max 40 characters")]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password")]
        [DisplayName("Confirm Password :")]
        public string ConfirmPassword { get; set; }

        public string CurrentPassword { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int Level { get; set; }
        public int DSLevel { get; set; }

        public string Title { get; set; }
        [Required(ErrorMessage = "Required")]
        public int TitleId { get; set; }
        public int TimeOffSet { get; set; }
        public List<SelectListItem> _TitleList { get; set; }

        public bool Status { get; set; }
        public bool IsProfileComplete { get; set; }
        public string UserCode { get; set; }
        public string Search { get; set; }
        public List<UserModel> _UserList { get; set; }
        //public List<MenuModel> _MenuList { get; set; }
        //public List<PermissionModel> _PermissionList { get; set; }

        public List<SelectListItem> _CountryList { get; set; }
        public List<SelectListItem> _StateList { get; set; }
        public List<SelectListItem> _TimeZoneList { get; set; }
        public Int32 CompanyId { get; set; }
        public String CreatedBy { get; set; }
        public string PNICode { get; set; }
        //public ChangePasswordModell ChangePassworMMOdel { get; set; }
    }
}
