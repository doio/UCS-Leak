namespace KosmoSC
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BtnSCfileInfo = new System.Windows.Forms.Button();
            this.BtnSCfileTexture = new System.Windows.Forms.Button();
            this.BtnStart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(9, 83);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(403, 207);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // BtnSCfileInfo
            // 
            this.BtnSCfileInfo.Location = new System.Drawing.Point(10, 11);
            this.BtnSCfileInfo.Margin = new System.Windows.Forms.Padding(2);
            this.BtnSCfileInfo.Name = "BtnSCfileInfo";
            this.BtnSCfileInfo.Size = new System.Drawing.Size(402, 19);
            this.BtnSCfileInfo.TabIndex = 1;
            this.BtnSCfileInfo.Text = "BtnSCfileInfo";
            this.BtnSCfileInfo.UseVisualStyleBackColor = true;
            this.BtnSCfileInfo.Click += new System.EventHandler(this.BtnSCfileInfo_Click);
            // 
            // BtnSCfileTexture
            // 
            this.BtnSCfileTexture.Location = new System.Drawing.Point(10, 35);
            this.BtnSCfileTexture.Margin = new System.Windows.Forms.Padding(2);
            this.BtnSCfileTexture.Name = "BtnSCfileTexture";
            this.BtnSCfileTexture.Size = new System.Drawing.Size(402, 19);
            this.BtnSCfileTexture.TabIndex = 2;
            this.BtnSCfileTexture.Text = "BtnSCfileTexture";
            this.BtnSCfileTexture.UseVisualStyleBackColor = true;
            this.BtnSCfileTexture.Click += new System.EventHandler(this.BtnSCfileTexture_Click);
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(10, 59);
            this.BtnStart.Margin = new System.Windows.Forms.Padding(2);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(402, 19);
            this.BtnStart.TabIndex = 3;
            this.BtnStart.Text = "BtnStart";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 300);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.BtnSCfileTexture);
            this.Controls.Add(this.BtnSCfileInfo);
            this.Controls.Add(this.pictureBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button BtnSCfileInfo;
        private System.Windows.Forms.Button BtnSCfileTexture;
        private System.Windows.Forms.Button BtnStart;
    }
}

