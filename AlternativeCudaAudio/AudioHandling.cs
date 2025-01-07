using ManagedCuda.BasicTypes;
using NAudio.Wave;

namespace AlternativeCudaAudio
{
	public class AudioHandling
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public CudaHandling CudaH;

		public List<AudioObject> Tracks = [];




		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTOR ~~~~~ ~~~~~ ~~~~~ \\
		public AudioHandling(CudaHandling cudah)
		{
			// Set CUDA handling object
			CudaH = cudah;
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void AddTrack(string filepath)
		{
			// Create new audio object
			AudioObject track = new(CudaH, filepath);

			// Add to list
			Tracks.Add(track);
		}




	}



	public class AudioObject
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public string Filepath;
		public string Name;

		public byte[] Bytes;

		public long Length = 0;
		public long Samplerate = 44100;
		public int Bitdepth = 16;
		public int Channels = 2;

		public WaveOutEvent Player = new();
		public long Position = 0;

		public CudaHandling CudaH;
		public long Ptr = -1;



		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTOR ~~~~~ ~~~~~ ~~~~~ \\
		public AudioObject(CudaHandling cudah, string filepath)
		{
			// Set CUDA handling object
			CudaH = cudah;

			Filepath = filepath;
			Name = Path.GetFileName(filepath);

			// Read audio file (Bytes)
			Bytes = ReadAudiofile(filepath);
		}




		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public byte[] ReadAudiofile(string filepath)
		{
			// Use NAudio to read audio file
			AudioFileReader reader = new(filepath);

			// Get audio data
			Length = reader.Length;
			Samplerate = reader.WaveFormat.SampleRate;
			Bitdepth = reader.WaveFormat.BitsPerSample;
			Channels = reader.WaveFormat.Channels;

			// Read audio data
			byte[] bytes = new byte[Length];
			reader.Read(bytes, 0, (int) Length);

			// Close reader & return bytes
			reader.Close();
			return bytes;
		}

		public float[] GetFloats()
		{
			if (Bytes == null || Bytes.Length == 0)
			{
				return Array.Empty<float>();
			}

			int sampleSize = Bitdepth / 8;
			int sampleCount = Bytes.Length / sampleSize;
			float[] floatSamples = new float[sampleCount];

			for (int i = 0; i < sampleCount; i++)
			{
				int byteIndex = i * sampleSize;
				switch (Bitdepth)
				{
					case 8:
						floatSamples[i] = (Bytes[byteIndex] - 128) / 128f;
						break;
					case 16:
						floatSamples[i] = BitConverter.ToInt16(Bytes, byteIndex) / 32768f;
						break;
					case 24:
						int sample24 = (Bytes[byteIndex + 2] << 16) | (Bytes[byteIndex + 1] << 8) | Bytes[byteIndex];
						if ((sample24 & 0x800000) != 0) sample24 |= unchecked((int) 0xFF000000);
						floatSamples[i] = sample24 / 8388608f;
						break;
					case 32:
						floatSamples[i] = BitConverter.ToInt32(Bytes, byteIndex) / 2147483648f;
						break;
					default:
						throw new NotSupportedException($"Bitdepth {Bitdepth} wird nicht unterstützt.");
				}
			}

			return floatSamples;
		}


		public void MoveDataToCuda()
		{
			// Abort if no bytes or Ctx is null
			if (Bytes.Length == 0 || CudaH.Ctx == null)
			{
				return;
			}

			// Using CudaH, move audio data to device
			Ptr = CudaH.ToCuda(Bytes);

			// Free memory
			Bytes = [];
		}

		public void MoveDataToHost()
		{
			// Abort if no pointer or pointer is -1
			if (Ptr == -1)
			{
				return;
			}

			// Get data from device
			Bytes = CudaH.ToHost(Ptr);

			// Delete Ptr
			Ptr = -1;
		}

		public Bitmap GetWaveform(Point size, int resolution = 128)
		{
			int width = size.X;
			int height = size.Y;

			// Create bitmap
			Bitmap bmp = new(width, height);

			// Create graphics
			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.White);

			if (Bytes.Length == 0)
			{
				return bmp;
			}

			// Get data
			float[] samples = GetFloats();

			// New pen
			Pen pen = new(Color.BlueViolet, 1);

			// Calculate number of pixels
			int pixels = width;
			int samplesPerPixel = resolution;

			// Draw waveform
			for (int x = 0; x < pixels; x++)
			{
				int startSample = x * samplesPerPixel;
				int endSample = Math.Min(startSample + samplesPerPixel, samples.Length);

				float min = float.MaxValue;
				float max = float.MinValue;

				for (int i = startSample; i < endSample; i++)
				{
					float sample = samples[i];
					if (sample < min) min = sample;
					if (sample > max) max = sample;
				}

				int yMin = (int) ((1 - (min + 1) / 2) * height);
				int yMax = (int) ((1 - (max + 1) / 2) * height);

				g.DrawLine(pen, x, yMin, x, yMax);
			}

			// Return bitmap
			return bmp;
		}

	}
}