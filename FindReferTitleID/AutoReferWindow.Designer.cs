namespace FindReferTitleID
{
    partial class AutoReferWindow
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
            this.btnTitleID2Sheet = new System.Windows.Forms.Button();
            this.btnSection2TitileID = new System.Windows.Forms.Button();
            this.btnZoom2TitleID = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTitleID2Sheet
            // 
            this.btnTitleID2Sheet.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTitleID2Sheet.Location = new System.Drawing.Point(0, 0);
            this.btnTitleID2Sheet.Name = "btnTitleID2Sheet";
            this.btnTitleID2Sheet.Size = new System.Drawing.Size(120, 55);
            this.btnTitleID2Sheet.TabIndex = 0;
            this.btnTitleID2Sheet.Text = "TileID-Sheet";
            this.btnTitleID2Sheet.UseVisualStyleBackColor = true;
            this.btnTitleID2Sheet.Click += new System.EventHandler(this.btnTitleID2Sheet_Click);
            // 
            // btnSection2TitileID
            // 
            this.btnSection2TitileID.Location = new System.Drawing.Point(0, 61);
            this.btnSection2TitileID.Name = "btnSection2TitileID";
            this.btnSection2TitileID.Size = new System.Drawing.Size(120, 55);
            this.btnSection2TitileID.TabIndex = 1;
            this.btnSection2TitileID.Text = "Section-TitleID";
            this.btnSection2TitileID.UseVisualStyleBackColor = true;
            this.btnSection2TitileID.Click += new System.EventHandler(this.btnSection2TitileID_Click);
            // 
            // btnZoom2TitleID
            // 
            this.btnZoom2TitleID.Location = new System.Drawing.Point(0, 122);
            this.btnZoom2TitleID.Name = "btnZoom2TitleID";
            this.btnZoom2TitleID.Size = new System.Drawing.Size(120, 55);
            this.btnZoom2TitleID.TabIndex = 2;
            this.btnZoom2TitleID.Text = "FindTitleID";
            this.btnZoom2TitleID.UseVisualStyleBackColor = true;
            // 
            // AutoReferWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(120, 176);
            this.Controls.Add(this.btnZoom2TitleID);
            this.Controls.Add(this.btnSection2TitileID);
            this.Controls.Add(this.btnTitleID2Sheet);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoReferWindow";
            this.ShowIcon = false;
            this.Text = "AutoReferWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTitleID2Sheet;
        private System.Windows.Forms.Button btnSection2TitileID;
        private System.Windows.Forms.Button btnZoom2TitleID;
    }
}