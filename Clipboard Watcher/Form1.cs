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
        private static bool isEnabled = true;

        private static int backCounter = 0;

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

                checkBox1.Checked = true;

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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            oldText = Clipboard.GetText();
            isEnabled = checkBox1.Checked;
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
                if (isEnabled && System.Windows.Forms.Clipboard.ContainsText())
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

                // backCounter was set to non-zero
                // This means, the app is disabled temporarily;
                // Count it back to zero and enable the app
                if (backCounter > 0)
                {
                    backCounter--;

                    if (backCounter == 0)
                    {
                        isEnabled = true;
                        oldText = Clipboard.GetText();
                    }
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------

        // Prevent self-duplicating while copying directly from the App's RichEdit
        private void richTextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.Insert))
            {
                if (isEnabled)
                {
                    isEnabled = false;
                    backCounter = 3;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------------
    }
}

#pragma warning restore IDE1006
