using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable IDE1006

namespace Clipboard_Watcher
{
    public partial class Form1 : Form
    {
        private readonly System.Threading.Thread th = null;

        private static string oldText = "";
        private static string newText = "";
        private static bool isRunning = false;

        // ---------------------------------------------------------------------------------------------------------------

        public Form1()
        {
            InitializeComponent();

            if (th == null)
            {
                isRunning = true;

                th = new System.Threading.Thread(threadFunc)
                {
                    IsBackground = true,                                        // Does not prevent the app from terminating
                    Priority = System.Threading.ThreadPriority.Normal,
                    Name = nameof(threadFunc)
                };

                th.SetApartmentState(System.Threading.ApartmentState.STA);
                th.Start();
            }
        }

        // ---------------------------------------------------------------------------------------------------------------

        // Copy all the text to clipboard
        private void bCopy_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Length > 0)
            {
                oldText = richTextBox1.Text;

                Clipboard.SetText(oldText);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------

        // Clear all the text
        private void bClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        // ---------------------------------------------------------------------------------------------------------------

        // Main thread func
        private void threadFunc()
        {
            // Prevent posting the text that is already in the Clipboard when the app starts
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                oldText = Clipboard.GetText();
            }

            while (isRunning)
            {
                if (System.Windows.Forms.Clipboard.ContainsText())
                {
                    newText = Clipboard.GetText();

                    if (tbFilter.Text.Length == 0 || newText.Contains(tbFilter.Text))
                    {
                        if (newText != oldText)
                        {
                            oldText = newText;

                            richTextBox1.Invoke(new MethodInvoker(delegate
                            {

                                richTextBox1.AppendText(newText);
                                richTextBox1.AppendText(Environment.NewLine);

                            }));
                        }
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------
    }
}

#pragma warning restore IDE1006
