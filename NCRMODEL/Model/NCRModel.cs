using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NCRMODEL.Model
{
    public class NCRModel
    {
        public string NCRID { get; set; }
        public string NCRNumber { get; set; }
        public string NCRDate { get; set; }
        [Required(ErrorMessage = "Please Enter PO")]
        public string PO { get; set; }
        [Required(ErrorMessage = "Please Enter MRN No.")]
        public string MRNno { get; set; }
        public List<SelectListItem> MRNNoList { get; set; }
        [Required(ErrorMessage = "Please Enter Item Code")]
        public string ItemCode { get; set; }
        public List<SelectListItem> ItemCodeList { get; set; }
        [Required(ErrorMessage = "Please Enter Vendor")]
        public string VendorCode { get; set; }
        [Required(ErrorMessage = "Please Enter Contact")]
        public string Contact { get; set; }
        public string TableName { get; set; }
        [Required(ErrorMessage = "Please Enter Phone")]
        public string Phone { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Please Enter Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please Enter Email")]
        //[RegularExpression(@"^[a-zA-Z0-9_.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter a valid email")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Item Description")]
        public string ItemDescription { get; set; }
        [Required(ErrorMessage = "Please Enter Detail Of Non-Conformance")]
        public string DetailofNonConformance { get; set; }
        [Required(ErrorMessage = "Please Enter Total Recieved")]
        public string TotalRecieved { get; set; }
        [Required(ErrorMessage = "Please Enter Total Rejected")]

        public string TotalRejected { get; set; }
        public string AddAttachment { get; set; }
        public string ReturnMessage { get; set; }   
        public int ReturnCode { get; set; }
        public HttpPostedFileBase RefrenceImage1 { get; set; }
        public string ImageName1 { get; set; }
        public string ImagePath1 { get; set; }
        public HttpPostedFileBase RefrenceImage2 { get; set; }
        public string ImageName2 { get; set; }
        public string ImagePath2 { get; set; }
        public HttpPostedFileBase RefrenceImage3 { get; set; }
        public string ImageName3 { get; set; }
        public string ImagePath3 { get; set; }
        public HttpPostedFileBase RefrenceImage4 { get; set; }
        public string ImageName4 { get; set; }
        public string ImagePath4 { get; set; }
        //public List<ImageListModel> _ImagePathList { get; set; }
        public List<NCRModel> ncrmodelList { get; set; }
        public string Size { get; set; }
        public string SrNo { get; set; }
    }
    //public class ImageListModel
    //{
    //    public int ImageId { get; set; }
    //    public string ImagePath { get; set; }
    //    public string ImageBase64 { get; set; }
    //    public HttpPostedFileBase HttpPostedFileBase { get; set; }
    //}

    public class ReportModel
    {
        public string SendMailDate { get; set; }
        public string NCRDate { get; set; }
        public string NCRNumber { get; set; }
        public string VendorCode { get; set; }
        public string ItemCode { get; set; }
        public string MRNno { get; set; }
        public string DetailofNonConformance { get; set; }
        public string Email { get; set; }
        public string ItemDescription { get; set; }
        public string PO { get; set; }
        public string TotalRecieved { get; set; }
        public string TotalRejected { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public List<SelectListItem> YearList { get; set; }
        public List<SelectListItem> MonthList { get; set; }
        public List<ReportModel> reportModelsList { get; set; }
    }

}
