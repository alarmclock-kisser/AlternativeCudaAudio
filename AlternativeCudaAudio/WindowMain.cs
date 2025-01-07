namespace AlternativeCudaAudio
{
	public partial class WindowMain : Form
	{
		// ~~~~~ ~~~~~ ~~~~~ ATTRIBUTES ~~~~~ ~~~~~ ~~~~~ \\
		public CudaHandling CudaH;
		public AudioHandling AudioH;



		// ~~~~~ ~~~~~ ~~~~~ CONSTRUCTOR ~~~~~ ~~~~~ ~~~~~ \\
		public WindowMain()
		{
			InitializeComponent();

			// Set window position to primary top left corner
			StartPosition = FormStartPosition.Manual;
			Location = new Point(0, 0);

			// Init. objects
			CudaH = new CudaHandling();
			AudioH = new AudioHandling(CudaH);


			// Register events
			listBox_hostTracks.Click += ImportAudio;

			// Fill CUDA devices
			FillCudaDevices();

			// Update CUDA stats
			UpdateCudaStats();
		}






		// ~~~~~ ~~~~~ ~~~~~ METHODS ~~~~~ ~~~~~ ~~~~~ \\
		public void FillCudaDevices()
		{
			// Get device count
			int count = CudaH.GetDeviceCount();

			// Get device names
			string[] names = CudaH.GetDeviceNames();

			// Fill combo box
			comboBox_cudaDevices.Items.Clear();
			for (int i = 0; i < count; i++)
			{
				comboBox_cudaDevices.Items.Add(names[i]);
			}

			// Add last entry to delete context
			comboBox_cudaDevices.Items.Add("No CUDA device");
		}

		public void UpdateCudaStats()
		{
			// Get device id & name
			int id = CudaH.DeviceId;
			string name = CudaH.DeviceName;

			// Get device memory
			long memTotal = CudaH.GetVramTotal(true);
			long memFree = CudaH.GetVramFree(true);
			long memUsed = CudaH.GetVramUsed(true);

			// Update label
			label_cudaVram.Text = $"VRAM: {memUsed} / {memTotal} MB";

			// Update progress bar
			progressBar_cudaVram.Maximum = (int) memTotal;
			progressBar_cudaVram.Value = (int) memUsed;
		}

		public void UpdateCudaInfo()
		{
			string[] info = CudaH.GetCudaInfo();

			// Fill list box
			listBox_cudaInfo.Items.Clear();
			foreach (string line in info)
			{
				listBox_cudaInfo.Items.Add(line);
			}
		}

		public void UpdateTrackList()
		{
			// Clear list box
			listBox_hostTracks.Items.Clear();

			// Add each track
			foreach (AudioObject track in AudioH.Tracks)
			{
				string entry = track.Name;

				// If has Ptr, add (CUDA) to entry
				if (track.Ptr != -1)
				{
					entry = "(CUDA) " + entry;
				}

				listBox_hostTracks.Items.Add(entry);
			}
		}

		public void UpdatePointerList()
		{
			// Clear list box
			listBox_cudaPointers.Items.Clear();

			// Add each pointer
			foreach (AudioObject track in AudioH.Tracks)
			{
				// If has Ptr, add to list
				if (track.Ptr != -1)
				{
					listBox_cudaPointers.Items.Add(track.Ptr);
				}
			}
		}

		public void UpdateWaveform()
		{
			// Get selected track
			string name = listBox_hostTracks.SelectedItem?.ToString() ?? "";

			// Find track by name
			AudioObject? track = AudioH.Tracks.Find(t => t.Name == name) ?? null;

			// If track found
			if (track == null)
			{
				pictureBox_waveform.Image = null;
				return;
			}

			// New Point for size
			Point size = new(pictureBox_waveform.Size.Width, pictureBox_waveform.Size.Height);

			// Get waveform
			pictureBox_waveform.Image = track.GetWaveform(size, 256);
			pictureBox_waveform.Refresh();
		}







		// ~~~~~ ~~~~~ ~~~~~ EVENTS ~~~~~ ~~~~~ ~~~~~ \\
		private void comboBox_cudaDevices_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Get selected device id
			int id = comboBox_cudaDevices.SelectedIndex;

			// Init. CUDA
			CudaH.Init(id);

			// Update stats
			UpdateCudaStats();

			// Update info
			UpdateCudaInfo();
		}

		private void listBox_hostTracks_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Update waveform
			UpdateWaveform();
		}

		private void ImportAudio(object? sender, EventArgs e)
		{
			// Abort if not CTRL + click
			if (Control.ModifierKeys != Keys.Control)
			{
				return;
			}

			// OFD at MyMusic, multiple files, audio files only (WAV, MP3, FLAC)
			OpenFileDialog ofd = new()
			{
				Title = "Import audio files",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
				Multiselect = true,
				CheckFileExists = true,
				Filter = "Audio files|*.wav;*.mp3;*.flac"
			};

			// Show dialog
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				// Add each file
				foreach (string file in ofd.FileNames)
				{
					AudioH.AddTrack(file);
				}

				// Update track list
				UpdateTrackList();
			}
		}

		private void button_toCuda_Click(object sender, EventArgs e)
		{
			// Find track in list by name
			string name = listBox_hostTracks.SelectedItem?.ToString() ?? "";

			// Find track by name && move data to CUDA
			AudioH.Tracks.Find(t => t.Name == name)?.MoveDataToCuda();

			// Update pointer list
			UpdatePointerList();

			// Update CUDA stats
			UpdateCudaStats();

			// Update track list
			UpdateTrackList();

			// Unselect track
			listBox_hostTracks.ClearSelected();
			UpdateWaveform();
		}

		private void button_toHost_Click(object sender, EventArgs e)
		{
			// Get long of selected item or -1
			long ptr = listBox_cudaPointers.SelectedItem is long ptrLong ? ptrLong : -1;

			// Find track by pointer && move data to host
			AudioH.Tracks.Find(t => t.Ptr == ptr)?.MoveDataToHost();

			// Update pointer list
			UpdatePointerList();

			// Update CUDA stats
			UpdateCudaStats();

			// Update track list
			UpdateTrackList();

			// Unselect pointer
			listBox_cudaPointers.ClearSelected();
			UpdateWaveform();
		}


	}
}
