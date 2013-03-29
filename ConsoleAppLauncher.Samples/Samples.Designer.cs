namespace SlavaGu.ConsoleAppLauncher.Samples
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonGetIpAddress = new System.Windows.Forms.Button();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.buttonPing = new System.Windows.Forms.Button();
            this.labelPing = new System.Windows.Forms.Label();
            this.buttonVer = new System.Windows.Forms.Button();
            this.labelVer = new System.Windows.Forms.Label();
            this.buttonSkype = new System.Windows.Forms.Button();
            this.labelSkype = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonGetIpAddress
            // 
            this.buttonGetIpAddress.Location = new System.Drawing.Point(12, 43);
            this.buttonGetIpAddress.Name = "buttonGetIpAddress";
            this.buttonGetIpAddress.Size = new System.Drawing.Size(130, 23);
            this.buttonGetIpAddress.TabIndex = 2;
            this.buttonGetIpAddress.Text = "Get local IP Address";
            this.buttonGetIpAddress.UseVisualStyleBackColor = true;
            this.buttonGetIpAddress.Click += new System.EventHandler(this.buttonGetIpAddress_Click);
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(148, 48);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(67, 13);
            this.labelIpAddress.TabIndex = 3;
            this.labelIpAddress.Text = "<ip address>";
            // 
            // buttonPing
            // 
            this.buttonPing.Location = new System.Drawing.Point(12, 72);
            this.buttonPing.Name = "buttonPing";
            this.buttonPing.Size = new System.Drawing.Size(130, 23);
            this.buttonPing.TabIndex = 4;
            this.buttonPing.Text = "Ping google.com";
            this.buttonPing.UseVisualStyleBackColor = true;
            this.buttonPing.Click += new System.EventHandler(this.buttonPing_Click);
            // 
            // labelPing
            // 
            this.labelPing.AutoSize = true;
            this.labelPing.Location = new System.Drawing.Point(148, 77);
            this.labelPing.Name = "labelPing";
            this.labelPing.Size = new System.Drawing.Size(82, 13);
            this.labelPing.TabIndex = 5;
            this.labelPing.Text = "<roundtrip time>";
            // 
            // buttonVer
            // 
            this.buttonVer.Location = new System.Drawing.Point(12, 14);
            this.buttonVer.Name = "buttonVer";
            this.buttonVer.Size = new System.Drawing.Size(130, 23);
            this.buttonVer.TabIndex = 0;
            this.buttonVer.Text = "Get Windows Version";
            this.buttonVer.UseVisualStyleBackColor = true;
            this.buttonVer.Click += new System.EventHandler(this.buttonVer_Click);
            // 
            // labelVer
            // 
            this.labelVer.AutoSize = true;
            this.labelVer.Location = new System.Drawing.Point(148, 19);
            this.labelVer.Name = "labelVer";
            this.labelVer.Size = new System.Drawing.Size(97, 13);
            this.labelVer.TabIndex = 1;
            this.labelVer.Text = "<windows version>";
            // 
            // buttonSkype
            // 
            this.buttonSkype.Location = new System.Drawing.Point(13, 102);
            this.buttonSkype.Name = "buttonSkype";
            this.buttonSkype.Size = new System.Drawing.Size(129, 23);
            this.buttonSkype.TabIndex = 6;
            this.buttonSkype.Text = "Get Skype Firewall rule";
            this.buttonSkype.UseVisualStyleBackColor = true;
            this.buttonSkype.Click += new System.EventHandler(this.buttonSkype_Click);
            // 
            // labelSkype
            // 
            this.labelSkype.AutoSize = true;
            this.labelSkype.Location = new System.Drawing.Point(148, 107);
            this.labelSkype.Name = "labelSkype";
            this.labelSkype.Size = new System.Drawing.Size(102, 13);
            this.labelSkype.TabIndex = 7;
            this.labelSkype.Text = "<skype firewall rule>";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 332);
            this.Controls.Add(this.labelSkype);
            this.Controls.Add(this.buttonSkype);
            this.Controls.Add(this.labelVer);
            this.Controls.Add(this.buttonVer);
            this.Controls.Add(this.labelPing);
            this.Controls.Add(this.buttonPing);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.buttonGetIpAddress);
            this.Name = "Form1";
            this.Text = "ConsoleAppLauncher Samples";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonGetIpAddress;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Button buttonPing;
        private System.Windows.Forms.Label labelPing;
        private System.Windows.Forms.Button buttonVer;
        private System.Windows.Forms.Label labelVer;
        private System.Windows.Forms.Button buttonSkype;
        private System.Windows.Forms.Label labelSkype;
    }
}

