using NCRMODEL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCRBAL.Interface
{
    public interface INCR
    {
        NCRModel GetNCRData();
        NCRModel DataByPOID(NCRModel POID);
        NCRModel DataByMRNID(NCRModel POID);
        NCRModel GetItemDescription(NCRModel POID);
        NCRModel AddOrEdit(NCRModel POID);
        string NCRCountt();
        NCRModel NCREmailDetail(string NCRNumber,string Email);
        string NCRDataRefresh();
        List<ReportModel> ReportDetail(ReportModel model);

    }
}
