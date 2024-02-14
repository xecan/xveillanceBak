using Neurotec.Images;
using Neurotec.Samples.Code;
using System;

namespace Neurotec.Samples.Forms
{
	public interface ISurveillanceView: IDisposable
	{
		void SetCaptureEventData(CaptureEventData data);
		SourceController State { get; set; }
		double FpsMeasureTime { get; set; }
	}
}
