
namespace Neurotec.Samples.Forms
{
    partial class VerificationCodeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerificationCodeForm));
            this.tbOtp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.verifyOtp = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbOtp
            // 
            this.tbOtp.Location = new System.Drawing.Point(104, 40);
            this.tbOtp.Name = "tbOtp";
            this.tbOtp.Size = new System.Drawing.Size(187, 20);
            this.tbOtp.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(114, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please enter verification code";
            // 
            // verifyOtp
            // 
            this.verifyOtp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verifyOtp.Location = new System.Drawing.Point(148, 78);
            this.verifyOtp.Name = "verifyOtp";
            this.verifyOtp.Size = new System.Drawing.Size(104, 23);
            this.verifyOtp.TabIndex = 2;
            this.verifyOtp.Text = "Verify OTP";
            this.verifyOtp.UseVisualStyleBackColor = true;
            this.verifyOtp.Click += new System.EventHandler(this.verifyOtp_Click);
            // 
            // VerificationCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(461, 124);
            this.Controls.Add(this.verifyOtp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbOtp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VerificationCodeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Verification Code";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbOtp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button verifyOtp;
    }
}