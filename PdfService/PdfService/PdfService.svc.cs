using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using PdfService.DataContract;

namespace PdfService
{
	public class PdfService : IPdfService
	{
		private static readonly ILog log = LogManager.GetLogger("PdfService");

		public ServiceResult CreatePDFFile(Dictionary<string, string> pdfData, string pdfTemplate, bool lockForm)
		{
			if (log.IsDebugEnabled)
				log.Debug("Start");

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
				wsResult.FileData = CreatePdf(pdfData, pdfTemplate, lockForm);

				if (log.IsDebugEnabled)
					log.Debug("Stop");

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

		public ServiceResult CreateMultiPagePDFFile(List<Dictionary<string, string>> pdfDataLst, string pdfTemplate, bool lockForm)
		{
			if (log.IsDebugEnabled)
				log.Debug("Start");

			ServiceResult wsResult = new ServiceResult();

			try
			{
				if (pdfDataLst == null || pdfDataLst.Count == 0)
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

				using (Document document = new Document())
				{
					using (MemoryStream msDoc = new MemoryStream())
					{
						PdfCopy copy = new PdfSmartCopy(document, msDoc);
						document.Open();

						foreach (Dictionary<string, string> pdfData in pdfDataLst)
						{
							using (PdfReader pdfReaderDoc = new PdfReader(CreatePdf(pdfData, pdfTemplate, lockForm)))
							{
								copy.AddDocument(pdfReaderDoc);
								pdfReaderDoc.Close();
							}
						}

						document.Close();
						wsResult.FileData = msDoc.ToArray();
					}
				}

				if (log.IsDebugEnabled)
					log.Debug("Stop");

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

		private byte[] CreatePdf(Dictionary<string, string> pdfData, string pdfTemplate, bool lockForm)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (PdfReader pdfReader = new PdfReader(pdfTemplate))
				{
					PdfReader.unethicalreading = true;
					using (PdfStamper pdfStamper = new PdfStamper(pdfReader, ms))
					{
						AcroFields pdfFormFields = pdfStamper.AcroFields;
						// set form pdfFormFields
						foreach (KeyValuePair<string, string> kvp in pdfData)
						{
							pdfFormFields.SetField(kvp.Key, kvp.Value);
						}

						// flatten the form to remove editting options, set it to false  
						// to leave the form open to subsequent manual edits  
						pdfStamper.FormFlattening = lockForm;
						// close the pdf  
						pdfStamper.Close();
						pdfReader.Close();

						if (log.IsDebugEnabled)
							log.Debug("Stop");

						return ms.ToArray();
					}
				}
			}
		}
	}
}
