namespace Win
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.AE = new System.Windows.Forms.Button();
            this.DE = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(95, 68);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(974, 99);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(95, 303);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(974, 99);
            this.textBox2.TabIndex = 0;
            // 
            // AE
            // 
            this.AE.Location = new System.Drawing.Point(192, 204);
            this.AE.Name = "AE";
            this.AE.Size = new System.Drawing.Size(150, 71);
            this.AE.TabIndex = 1;
            this.AE.Text = "加密";
            this.AE.UseVisualStyleBackColor = true;
            this.AE.Click += new System.EventHandler(this.AE_Click);
            // 
            // DE
            // 
            this.DE.Location = new System.Drawing.Point(794, 204);
            this.DE.Name = "DE";
            this.DE.Size = new System.Drawing.Size(150, 71);
            this.DE.TabIndex = 1;
            this.DE.Text = "解密";
            this.DE.UseVisualStyleBackColor = true;
            this.DE.Click += new System.EventHandler(this.DE_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1215, 490);
            this.Controls.Add(this.DE);
            this.Controls.Add(this.AE);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button AE;
        private System.Windows.Forms.Button DE;
    }
}

