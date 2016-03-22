namespace GoFish
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
            this.label1 = new System.Windows.Forms.Label();
            this.textName = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textProgress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBooks = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listHand = new System.Windows.Forms.ListBox();
            this.buttonAsk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Your name";
            // 
            // textName
            // 
            this.textName.Location = new System.Drawing.Point(15, 35);
            this.textName.Name = "textName";
            this.textName.Size = new System.Drawing.Size(205, 20);
            this.textName.TabIndex = 1;
            this.textName.Text = "You";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(226, 29);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(184, 23);
            this.buttonStart.TabIndex = 2;
            this.buttonStart.Text = "Start the game!";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Game progress";
            // 
            // textProgress
            // 
            this.textProgress.Location = new System.Drawing.Point(15, 92);
            this.textProgress.Multiline = true;
            this.textProgress.Name = "textProgress";
            this.textProgress.ReadOnly = true;
            this.textProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textProgress.Size = new System.Drawing.Size(395, 348);
            this.textProgress.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 443);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Books";
            // 
            // textBooks
            // 
            this.textBooks.Location = new System.Drawing.Point(15, 460);
            this.textBooks.Multiline = true;
            this.textBooks.Name = "textBooks";
            this.textBooks.ReadOnly = true;
            this.textBooks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBooks.Size = new System.Drawing.Size(395, 86);
            this.textBooks.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(416, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Your hand";
            // 
            // listHand
            // 
            this.listHand.Enabled = false;
            this.listHand.FormattingEnabled = true;
            this.listHand.Location = new System.Drawing.Point(419, 35);
            this.listHand.Name = "listHand";
            this.listHand.Size = new System.Drawing.Size(142, 485);
            this.listHand.TabIndex = 4;
            // 
            // buttonAsk
            // 
            this.buttonAsk.Enabled = false;
            this.buttonAsk.Location = new System.Drawing.Point(419, 523);
            this.buttonAsk.Name = "buttonAsk";
            this.buttonAsk.Size = new System.Drawing.Size(142, 23);
            this.buttonAsk.TabIndex = 2;
            this.buttonAsk.Text = "Ask for a card";
            this.buttonAsk.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 562);
            this.Controls.Add(this.listHand);
            this.Controls.Add(this.textBooks);
            this.Controls.Add(this.textProgress);
            this.Controls.Add(this.buttonAsk);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.textName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Go Fish";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listHand;
        private System.Windows.Forms.Button buttonAsk;
        private System.Windows.Forms.TextBox textBooks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textProgress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textName;
        private System.Windows.Forms.Label label1;
    }
}

