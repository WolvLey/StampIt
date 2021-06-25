﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StampIt
{
    public partial class Form1 : Form
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        Dictionary<int, string> comboSourceHotkeyStart = null;

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Win = 8
        }

        public Form1()
        {
            InitializeComponent();

            RegisterHotKey(this.Handle, 0, (int)KeyModifier.Alt, Keys.A.GetHashCode());
            RegisterHotKey(this.Handle, 1, (int)KeyModifier.Alt, Keys.S.GetHashCode());
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0:
                        Console.WriteLine("A pressed");
                        FileManager.HandleStart();
                        break;
                    case 1:
                        Console.WriteLine("B pressed");
                        FileManager.PutStamp();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbDirPath.Text = Properties.Settings.Default.StoreLocation;

            ComboBinder comboBinder = new ComboBinder(cbHotykeyStart);
            comboBinder.AddEntries(new (int, string)[]{
                (0, "None"),
                (1, "Alt"),
                (2, "Control"),
                (4, "Shift"),
                (8, "Win"),
            });
            cbHotykeyStart.DataSource = comboBinder.GetBindingSource();  //new BindingSource(comboSourceHotkeyStart, null);
            cbHotykeyStart.DisplayMember = "Value";
            cbHotykeyStart.ValueMember = "Key";
            cbHotykeyStart.SelectedItem =  comboBinder.Source[Properties.Settings.Default.HotkeyStartModificatorKey];

            //reuse data for different combobox
            comboBinder.ComboBox = cbHotykeyStamp;
            cbHotykeyStamp.DataSource = comboBinder.GetBindingSource();
            cbHotykeyStamp.DisplayMember = "Value";
            cbHotykeyStamp.ValueMember = "Key";
            cbHotykeyStamp.SelectedItem = comboBinder.Source[Properties.Settings.Default.HotkeyStampModificatiorKey];

        }

        private void btnOpenBrowserDialog_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            var selectedDir = folderBrowserDialog1.SelectedPath;

            tbDirPath.Text = selectedDir;

            FileManager.path = tbDirPath.Text;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            UnregisterHotKey(this.Handle, 1);

            //if (e.CloseReason == CloseReason.UserClosing)
            //{
            //    if (MessageBox.Show("Exit Program?", "Confirm Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }



        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                StampIt.Visible = true;
            }
        }

        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }
        private void TbDirPath_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.StoreLocation = tbDirPath.Text;
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HotkeyStartModificatorKey = ((KeyValuePair<int, string>)cbHotykeyStart.SelectedItem).Key;
        }

        private void tbHotkeyStart_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HotkeyStart = tbHotkeyStart.Text;
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            StampIt.Visible = false;
        }

        private void tbHotkeyStamp_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HotkeyStart = tbHotkeyStamp.Text;
        }
    }
}
