namespace LibDayDataExtractor.Forms
{
    partial class ExceptionForm
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
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.SplitContainer splitContainer1;
            this.m_messageTextBox = new System.Windows.Forms.TextBox();
            this.m_stackTraceTextBox = new System.Windows.Forms.TextBox();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_messageTextBox
            // 
            this.m_messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_messageTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.m_messageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_messageTextBox.Location = new System.Drawing.Point(8, 25);
            this.m_messageTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.m_messageTextBox.Multiline = true;
            this.m_messageTextBox.Name = "m_messageTextBox";
            this.m_messageTextBox.ReadOnly = true;
            this.m_messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_messageTextBox.Size = new System.Drawing.Size(725, 206);
            this.m_messageTextBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.AutoSize = true;
            groupBox1.Controls.Add(this.m_messageTextBox);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Location = new System.Drawing.Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(741, 241);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Message";
            // 
            // m_stackTraceTextBox
            // 
            this.m_stackTraceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_stackTraceTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.m_stackTraceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_stackTraceTextBox.Location = new System.Drawing.Point(8, 22);
            this.m_stackTraceTextBox.Multiline = true;
            this.m_stackTraceTextBox.Name = "m_stackTraceTextBox";
            this.m_stackTraceTextBox.ReadOnly = true;
            this.m_stackTraceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.m_stackTraceTextBox.Size = new System.Drawing.Size(725, 204);
            this.m_stackTraceTextBox.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.m_stackTraceTextBox);
            groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox2.Location = new System.Drawing.Point(0, 0);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(741, 237);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Stack trace";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Size = new System.Drawing.Size(741, 482);
            splitContainer1.SplitterDistance = 241;
            splitContainer1.TabIndex = 7;
            // 
            // ExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 482);
            this.Controls.Add(splitContainer1);
            this.Name = "ExceptionForm";
            this.Text = "ExceptionForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox m_messageTextBox;
        private System.Windows.Forms.TextBox m_stackTraceTextBox;
    }
}