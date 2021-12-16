namespace ReaderSvg.WinFormsSample
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.openButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.scaleToFitCheckBox = new System.Windows.Forms.CheckBox();
            this.showNextButton = new System.Windows.Forms.Button();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.openButton);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.scaleToFitCheckBox);
            this.panel1.Controls.Add(this.showNextButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 485);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 288);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "GDI+ bitmap:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Location = new System.Drawing.Point(13, 307);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(190, 166);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(12, 42);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(191, 23);
            this.openButton.TabIndex = 6;
            this.openButton.Text = "Open svg file from disk";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(9, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Read options:";
            // 
            // scaleToFitCheckBox
            // 
            this.scaleToFitCheckBox.AutoSize = true;
            this.scaleToFitCheckBox.Checked = true;
            this.scaleToFitCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.scaleToFitCheckBox.Location = new System.Drawing.Point(12, 115);
            this.scaleToFitCheckBox.Name = "scaleToFitCheckBox";
            this.scaleToFitCheckBox.Size = new System.Drawing.Size(76, 17);
            this.scaleToFitCheckBox.TabIndex = 1;
            this.scaleToFitCheckBox.Text = "Scale to fit";
            this.scaleToFitCheckBox.UseVisualStyleBackColor = true;
            // 
            // showNextButton
            // 
            this.showNextButton.Location = new System.Drawing.Point(12, 12);
            this.showNextButton.Name = "showNextButton";
            this.showNextButton.Size = new System.Drawing.Size(191, 23);
            this.showNextButton.TabIndex = 0;
            this.showNextButton.Text = "Show next svg sample";
            this.showNextButton.UseVisualStyleBackColor = true;
            this.showNextButton.Click += new System.EventHandler(this.showNextButton_Click);
            // 
            // elementHost1
            // 
            this.elementHost1.BackColor = System.Drawing.Color.White;
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(217, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(585, 485);
            this.elementHost1.TabIndex = 1;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 52);
            this.label3.TabIndex = 11;
            this.label3.Text = "With ZoomPanel control you can \r\nexplore the vector based svg image\r\nin all detai" +
    "ls with zoming into\r\nthe image with mouse wheel.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 485);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(0, 380);
            this.Name = "Form1";
            this.Text = "Ab2d.ReaderSvg WinForms sample";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.Button showNextButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox scaleToFitCheckBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

