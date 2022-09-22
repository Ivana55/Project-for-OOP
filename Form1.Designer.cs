namespace Pmfst_GameSDK
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnIgraj = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnIgraj
            // 
            this.btnIgraj.BackColor = System.Drawing.Color.Salmon;
            this.btnIgraj.Font = new System.Drawing.Font("Comic Sans MS", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIgraj.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnIgraj.Location = new System.Drawing.Point(452, 194);
            this.btnIgraj.Name = "btnIgraj";
            this.btnIgraj.Size = new System.Drawing.Size(195, 67);
            this.btnIgraj.TabIndex = 3;
            this.btnIgraj.Text = "Igraj!";
            this.btnIgraj.UseVisualStyleBackColor = false;
            this.btnIgraj.Click += new System.EventHandler(this.btnIgraj_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(800, 502);
            this.Controls.Add(this.btnIgraj);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnIgraj;
    }
}