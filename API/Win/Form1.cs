using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        ///  点击加密按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AE_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                return;
            }

            textBox2.Text = CoreHelper.Encrypt.DES2.Encrypt(textBox1.Text);





        }
        /// <summary>
        /// 点击解密按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DE_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                return;
            }


            textBox1.Text = CoreHelper.Encrypt.DES2.Decrypt(textBox2.Text);



        }






    }
}
