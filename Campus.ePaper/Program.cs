using FISCA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.ePaper
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            Aspose.Words.License lic_word = new Aspose.Words.License();
            System.IO.MemoryStream ms_word = new System.IO.MemoryStream(Properties.Resources.Aspose_Total_201402);
            lic_word.SetLicense(ms_word);

            Aspose.Pdf.License lic_pdf = new Aspose.Pdf.License();
            System.IO.MemoryStream ms_pdf = new System.IO.MemoryStream(Properties.Resources.Aspose_Total_201402);
            lic_pdf.SetLicense(ms_pdf);

            Aspose.Pdf.License lic_excel = new Aspose.Pdf.License();
            System.IO.MemoryStream ms_excel = new System.IO.MemoryStream(Properties.Resources.Aspose_Total_201402);
            lic_excel.SetLicense(ms_excel);


        }
    }
}
