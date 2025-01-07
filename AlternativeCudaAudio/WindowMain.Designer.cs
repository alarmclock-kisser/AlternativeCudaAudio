namespace AlternativeCudaAudio
{
    partial class WindowMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			comboBox_cudaDevices = new ComboBox();
			progressBar_cudaVram = new ProgressBar();
			label_cudaVram = new Label();
			listBox_cudaInfo = new ListBox();
			listBox_hostTracks = new ListBox();
			button_toCuda = new Button();
			listBox_cudaPointers = new ListBox();
			button_toHost = new Button();
			pictureBox_waveform = new PictureBox();
			((System.ComponentModel.ISupportInitialize) pictureBox_waveform).BeginInit();
			SuspendLayout();
			// 
			// comboBox_cudaDevices
			// 
			comboBox_cudaDevices.FormattingEnabled = true;
			comboBox_cudaDevices.Location = new Point(12, 12);
			comboBox_cudaDevices.Name = "comboBox_cudaDevices";
			comboBox_cudaDevices.Size = new Size(250, 23);
			comboBox_cudaDevices.TabIndex = 0;
			comboBox_cudaDevices.Text = "Select CUDA device (initialize)";
			comboBox_cudaDevices.SelectedIndexChanged += comboBox_cudaDevices_SelectedIndexChanged;
			// 
			// progressBar_cudaVram
			// 
			progressBar_cudaVram.Location = new Point(12, 41);
			progressBar_cudaVram.Name = "progressBar_cudaVram";
			progressBar_cudaVram.Size = new Size(250, 10);
			progressBar_cudaVram.TabIndex = 1;
			// 
			// label_cudaVram
			// 
			label_cudaVram.AutoSize = true;
			label_cudaVram.Location = new Point(12, 54);
			label_cudaVram.Name = "label_cudaVram";
			label_cudaVram.Size = new Size(90, 15);
			label_cudaVram.TabIndex = 2;
			label_cudaVram.Text = "VRAM: 0 / 0 MB";
			// 
			// listBox_cudaInfo
			// 
			listBox_cudaInfo.FormattingEnabled = true;
			listBox_cudaInfo.ItemHeight = 15;
			listBox_cudaInfo.Location = new Point(12, 72);
			listBox_cudaInfo.Name = "listBox_cudaInfo";
			listBox_cudaInfo.Size = new Size(250, 154);
			listBox_cudaInfo.TabIndex = 3;
			// 
			// listBox_hostTracks
			// 
			listBox_hostTracks.FormattingEnabled = true;
			listBox_hostTracks.ItemHeight = 15;
			listBox_hostTracks.Location = new Point(12, 695);
			listBox_hostTracks.Name = "listBox_hostTracks";
			listBox_hostTracks.Size = new Size(250, 214);
			listBox_hostTracks.TabIndex = 4;
			listBox_hostTracks.SelectedIndexChanged += listBox_hostTracks_SelectedIndexChanged;
			// 
			// button_toCuda
			// 
			button_toCuda.Location = new Point(268, 886);
			button_toCuda.Name = "button_toCuda";
			button_toCuda.Size = new Size(75, 23);
			button_toCuda.TabIndex = 5;
			button_toCuda.Text = "-> CUDA";
			button_toCuda.UseVisualStyleBackColor = true;
			button_toCuda.Click += button_toCuda_Click;
			// 
			// listBox_cudaPointers
			// 
			listBox_cudaPointers.FormattingEnabled = true;
			listBox_cudaPointers.ItemHeight = 15;
			listBox_cudaPointers.Location = new Point(442, 695);
			listBox_cudaPointers.Name = "listBox_cudaPointers";
			listBox_cudaPointers.Size = new Size(250, 214);
			listBox_cudaPointers.TabIndex = 6;
			// 
			// button_toHost
			// 
			button_toHost.Location = new Point(361, 886);
			button_toHost.Name = "button_toHost";
			button_toHost.Size = new Size(75, 23);
			button_toHost.TabIndex = 7;
			button_toHost.Text = "Host <-";
			button_toHost.UseVisualStyleBackColor = true;
			button_toHost.Click += button_toHost_Click;
			// 
			// pictureBox_waveform
			// 
			pictureBox_waveform.BackColor = Color.White;
			pictureBox_waveform.Location = new Point(12, 589);
			pictureBox_waveform.Name = "pictureBox_waveform";
			pictureBox_waveform.Size = new Size(680, 100);
			pictureBox_waveform.TabIndex = 8;
			pictureBox_waveform.TabStop = false;
			// 
			// WindowMain
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(704, 921);
			Controls.Add(pictureBox_waveform);
			Controls.Add(button_toHost);
			Controls.Add(listBox_cudaPointers);
			Controls.Add(button_toCuda);
			Controls.Add(listBox_hostTracks);
			Controls.Add(listBox_cudaInfo);
			Controls.Add(label_cudaVram);
			Controls.Add(progressBar_cudaVram);
			Controls.Add(comboBox_cudaDevices);
			MaximizeBox = false;
			MaximumSize = new Size(720, 960);
			MinimumSize = new Size(720, 960);
			Name = "WindowMain";
			Text = "Alternative CUDA Backend";
			((System.ComponentModel.ISupportInitialize) pictureBox_waveform).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ComboBox comboBox_cudaDevices;
		private ProgressBar progressBar_cudaVram;
		private Label label_cudaVram;
		private ListBox listBox_cudaInfo;
		private ListBox listBox_hostTracks;
		private Button button_toCuda;
		private ListBox listBox_cudaPointers;
		private Button button_toHost;
		private PictureBox pictureBox_waveform;
	}
}
