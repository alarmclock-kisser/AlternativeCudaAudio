using ManagedCuda;
using ManagedCuda.BasicTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternativeCudaAudio
{
	public class CudaHandling
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public int DeviceId = -1;
		public string DeviceName = "N/A";
		public PrimaryContext? Ctx = null;

		public Dictionary<CUdeviceptr, long> Pointers = [];


		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTOR ~~~~~ ~~~~~ ~~~~~ \\
		public CudaHandling()
		{
			// Init. objects


			// Register events


		}



		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void Init(int id = -1)
		{
			// Dispose old context
			Ctx?.Dispose();
			Ctx = null;

			// Get device id
			DeviceId = id;
			if (DeviceId == -1)
			{
				DeviceId = GetStrongestDeviceId();
			}

			if (DeviceId >= CudaContext.GetDeviceCount() || DeviceId < 0)
			{
				DeviceId = -1;
				DeviceName = "N/A";
				return;
			}

			// Create context & set current
			Ctx = new PrimaryContext(DeviceId);
			Ctx.SetCurrent();

			// Get device name
			DeviceName = CudaContext.GetDeviceName(DeviceId);
		}


		// ~~~~~ Info ~~~~~ \\
		public int GetStrongestDeviceId()
		{
			int max = 0;
			int maxId = 0;
			for (int i = 0; i < CudaContext.GetDeviceCount(); i++)
			{
				int score = CudaContext.GetDeviceComputeCapability(i).Major * 10 + CudaContext.GetDeviceComputeCapability(i).Minor;
				if (score > max)
				{
					max = score;
					maxId = i;
				}
			}

			return maxId;
		}

		public int GetDeviceCount()
		{
			return CudaContext.GetDeviceCount();
		}

		public string[] GetDeviceNames()
		{
			// Get device names of every device
			string[] names = new string[GetDeviceCount()];

			for (int i = 0; i < GetDeviceCount(); i++)
			{
				names[i] = CudaContext.GetDeviceName(i);
			}

			return names;
		}

		public void Dispose()
		{
			// Dispose context
			Ctx?.Dispose();
			Ctx = null;
		}

		public long GetVramTotal(bool readable = false)
		{
			long total = Ctx?.GetTotalDeviceMemorySize() ?? 0;

			if (readable)
			{
				return total / 1024 / 1024;
			}

			return total;
		}

		public long GetVramFree(bool readable = false)
		{
			long free = Ctx?.GetFreeDeviceMemorySize() ?? 0;

			if (readable)
			{
				return free / 1024 / 1024;
			}

			return free;
		}

		public long GetVramUsed(bool readable = false)
		{
			long total = GetVramTotal();
			long free = GetVramFree();
			long used = total - free;

			if (readable)
			{
				return used / 1024 / 1024;
			}

			return used;
		}

		public string[] GetCudaInfo()
		{
			// Get device id & name
			int id = Ctx?.GetDeviceInfo().PciDeviceId ?? -1;
			string name = Ctx?.GetDeviceInfo().DeviceName ?? "N/A";

			// Get device multiprocessors
			int processors = Ctx?.GetDeviceInfo().MultiProcessorCount ?? 0;

			// Get device threads
			int threads = Ctx?.GetDeviceInfo().MaxThreadsPerBlock ?? 0;

			// Get device compute capability
			int capability = Ctx?.GetDeviceInfo().ComputeCapability.Major * 10 + Ctx?.GetDeviceInfo().ComputeCapability.Minor ?? 0;

			// Get device clockings
			int coreclock = Ctx?.GetDeviceInfo().ClockRate ?? 0;
			int memclock = Ctx?.GetDeviceInfo().MemoryClockRate ?? 0;

			// Get device memory
			long memTotal = GetVramTotal(true);

			// Get device memory bus width
			int buswidth = Ctx?.GetDeviceInfo().GlobalMemoryBusWidth ?? 0;

			// Get driver version
			string version = Ctx?.GetDeviceInfo().DriverVersion.ToString() ?? "N/A";

			// Build info string
			string[] info =
			[
				$"PCIe ID: {id}",
				$"Device Name: {name}",
				$"Multiprocessors: {processors}",
				$"Threads: {threads}",
				$"Compute Capability: {capability}",
				$"Core Clock: {coreclock} MHz",
				$"Memory Clock: {memclock} MHz",
				$"Memory: {memTotal} MB",
				$"Memory Bus Width: {buswidth} Bit",
				$"Driver Version: {version}"
			];

			return info;

		}


		// ~~~~~ Pointers ~~~~~ \\
		public long FreePointer(CUdeviceptr ptr, bool readable = false)
		{
			long size = Pointers[ptr];

			if (readable)
			{
				size /= 1024 / 1024;
			}

			Ctx?.FreeMemory(ptr);
			Pointers.Remove(ptr);

			return size;
		}

		public long FreeAllPointers()
		{
			long total = 0;
			foreach (CUdeviceptr ptr in Pointers.Keys)
			{
				total += FreePointer(ptr);
				Ctx?.FreeMemory(ptr);
			}
			Pointers.Clear();

			return total;
		}

		public long ToCuda(byte[] bytes)
		{
			// Abort if no context
			if (Ctx == null)
			{
				return -1;
			}

			// Allocate memory
			CUdeviceptr ptr = Ctx.AllocateMemory(bytes.Length);

			// Copy data
			Ctx.CopyToDevice(ptr, bytes);

			// Add pointer to list
			Pointers.Add(ptr, bytes.Length);

			return ptr.Pointer;
		}

		public byte[] ToHost(long ptr)
		{
			// Abort if no context or pointer is -1
			if (Ctx == null || ptr == -1)
			{
				return [];
			}

			// Make cudevptr from pointer
			CUdeviceptr pointer = new(ptr);

			// Get size if pointer exists in list
			if (!Pointers.ContainsKey(pointer))
			{
				return [];
			}

			long size = Pointers[pointer];

			// Copy data
			byte[] bytes = new byte[size];
			Ctx.CopyToHost(bytes, pointer);

			// Free memory
			Ctx.FreeMemory(pointer);

			// Remove pointer from list
			Pointers.Remove(pointer);

			return bytes;
		}
	}
}
