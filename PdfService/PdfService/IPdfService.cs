using PdfService.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace PdfService
{
	[ServiceContract]
	public interface IPdfService
	{
		[OperationContract]
		ServiceResult CreatePDFFile(Dictionary<string, string> pdfData, string pdfTemplate, bool lockForm);

		[OperationContract]
		ServiceResult CreateMultiPagePDFFile(List<Dictionary<string, string>> pdfDataLst, string pdfTemplate, bool lockForm);
	}
}
