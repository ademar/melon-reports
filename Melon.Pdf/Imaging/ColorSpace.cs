// created on 3/21/2002 at 12:43 PM
using System.Collections;
using System.Text;

namespace Melon.Pdf.Imaging
{
	public class ColorSpace
	{
		protected ColorDevice m_colorDevice = ColorDevice.DeviceUnknown;

		private readonly ArrayList m_devices = new ArrayList();
		private readonly ArrayList m_ctables = new ArrayList();

		public ColorSpace(ColorDevice colorDevice)
		{
			m_colorDevice = colorDevice;
			m_devices.Add(colorDevice);
		}

		public ColorSpace()
		{
		}

		private static string DeviceName(ColorDevice cd)
		{
			switch (cd)
			{
				case ColorDevice.DeviceGray:
					return "/DeviceGray";
				case ColorDevice.DeviceRGB:
					return "/DeviceRGB";
				case ColorDevice.DeviceCMYK:
					return "/DeviceCMYK";
				case ColorDevice.Indexed:
					return "/Indexed";
				case ColorDevice.DeviceUnknown:
					return "/DeviceUnknown";
				default:
					return "/DeviceUnknown";
			}
		}

		public ColorDevice ColorDevice
		{
			get { return m_colorDevice; }
		}

		public void AddDevice(ColorDevice device)
		{
			m_devices.Add(device);
		}

		public void AddColorTable(ColorTable colortable)
		{
			m_ctables.Add(colortable);
		}

		public string GetRepresentation()
		{
			if (m_devices.Count == 1 && m_ctables.Count == 0)
				return DeviceName((ColorDevice) m_devices[0]);

			if (m_devices.Count > 1)
			{
				var s = new StringBuilder("[ ");
				for (var i = 0; i < m_devices.Count; i++)
				{
					s.Append(DeviceName((ColorDevice) m_devices[i]) + " ");
				}
				for (var i = 0; i < m_ctables.Count; i++)
				{
					s.Append(((ColorTable) m_ctables[i]).GetRepresentation() + " ");
				}
				s.Append("]");
				return s.ToString();
			}

			return string.Empty;
		}
	}
}