using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PdfService.DataContract
{
	[DataContract]
	public class ServiceResult
	{
		public ServiceResult()
		{
			this.KodOdpovede = 0;
			this.StatusText = string.Empty;
			this.FileData = new byte[0];
		}

		[DataMember]
		public int KodOdpovede { get; set; }

		[DataMember]
		public string StatusText { get; set; }

		[DataMember]
		public byte[] FileData { get; set; }
	}
}