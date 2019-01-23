using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Hosting;
//using iText.Forms;
//using iText.Kernel.Pdf;
using iTextSharp.text.pdf;
using log4net;
using PdfService.DataContract;

namespace PdfService
{
	public class PdfService : IPdfService
	{
		private static readonly ILog log = LogManager.GetLogger("PdfService");

		public ServiceResult CreatePDFFile(Dictionary<string, string> pdfData, string pdfTemplate)
		{
			ServiceResult wsResult = new ServiceResult();

			try
			{
				if (pdfData == null || pdfData.Count == 0)
				{
					wsResult.KodOdpovede = 1;
					wsResult.StatusText = "Nevyplnený vstupný parameter pdfData";

					if (log.IsDebugEnabled)
						log.Debug(wsResult.StatusText);

					return wsResult;
				}

				if (string.IsNullOrWhiteSpace(pdfTemplate))
				{
					wsResult.KodOdpovede = 1;
					wsResult.StatusText = "Nevyplnený vstupný parameter pdfTemplate";

					if (log.IsDebugEnabled)
						log.Debug(wsResult.StatusText);

					return wsResult;
				}

				pdfTemplate = HostingEnvironment.MapPath(pdfTemplate);

				using (MemoryStream ms = new MemoryStream())
				{
					using (PdfReader pdfReader = new PdfReader(pdfTemplate))
					{
						PdfStamper pdfStamper = new PdfStamper(pdfReader, ms);
						AcroFields pdfFormFields = pdfStamper.AcroFields;
						// set form pdfFormFields
						foreach (KeyValuePair<string, string> kvp in pdfData)
						{
							pdfFormFields.SetField(kvp.Key, kvp.Value);
						}

						// flatten the form to remove editting options, set it to false  
						// to leave the form open to subsequent manual edits  
						pdfStamper.FormFlattening = false;
						// close the pdf  
						pdfStamper.Close();

						wsResult.FileData = ms.ToArray();
					}
				}

				// iText7
				//using (var ms = new MemoryStream())
				//{
				//	PdfReader reader = new PdfReader(pdfTemplate); //Iput
				//	PdfWriter writer = new PdfWriter(ms); //output
				//	PdfDocument pdfDoc = new PdfDocument(reader, writer);
				//	PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);
				//	var pdfFormFields = form.GetFormFields();

				//	foreach (KeyValuePair<string, string> kvp in pdfData)
				//	{
				//		if(kvp.Value != null)
				//		{
				//			if (pdfFormFields.ContainsKey(kvp.Key))
				//			{
				//				pdfFormFields[kvp.Key].SetValue(kvp.Value);
				//			}
				//		}
				//	}

				//	//form.FlattenFields();
				//	pdfDoc.Close();
				//	wsResult.FileData = ms.ToArray();
				//}

				return wsResult;
			}
			catch (Exception ex)
			{
				if (log.IsErrorEnabled)
					log.Error("Interna chyba: ", ex);

				wsResult.KodOdpovede = -1;
				wsResult.StatusText = string.Concat("Chyba pri spracovaní dokumentu: ", ex.Message);
				return wsResult;
			}
		}
	}
}
