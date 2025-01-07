using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;

namespace AlternativeCudaAudio
{
	public class GraphicsBackend
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public int DeviceId = -1;
		public int DeviceCount = -1;
		public string[] DeviceNames = [];
		public Context? Ctx= null;



		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTOR ~~~~~ ~~~~~ ~~~~~ \\
		public GraphicsBackend()
		{
			// Init
			Init();
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public int GetDeviceCount()
		{
			// Abort if no context
			if (Ctx == null)
			{
				return -1;
			}

			// Get device count
			int count = Ctx.Devices.Length;

			// Return
			return count;
		}

		public string[] GetDeviceNames()
		{
			// Abort if no context
			if (Ctx == null)
			{
				return new string[0];
			}

			// Get device names
			string[] names = new string[Ctx.Devices.Length];
			for (int i = 0; i < Ctx.Devices.Length; i++)
			{
				names[i] = Ctx.Devices[i].Name;
			}

			// Return
			return names;
		}

		public int GetStrongestDeviceId()
		{
			// Abort if no context
			if (Ctx == null)
			{
				return -1;
			}

			// Get strongest device id
			int id = 0;
			for (int i = 1; i < Ctx.Devices.Length; i++)
			{
				if (Ctx.Devices[i].NumMultiprocessors > Ctx.Devices[id].NumMultiprocessors)
				{
					id = i;
				}
			}

			// Return
			return id;
		}

		public void Init(int id = -1)
		{
			// Create context
			Ctx = Context.CreateDefault();

			// Get device count
			DeviceCount = GetDeviceCount();

			// Get device names
			DeviceNames = GetDeviceNames();

			// Get strongest device id if id is -1
			if (id == -1)
			{
				DeviceId = GetStrongestDeviceId();
			}

			else
			{
				DeviceId = id;
			}

			// Init device
			string name  = CudaAccelerator.Current?.Name ?? "error";

		}



	}
}