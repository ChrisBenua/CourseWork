using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace M138ADemo.Services
{
    /// <summary>
    /// Default dialog service.
    /// </summary>
    public class DefaultDialogService : IDialogService
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get; set; }

        /// <summary>
        /// Opens the file dialog.
        /// </summary>
        /// <returns><c>true</c>, if file dialog was opened, <c>false</c> otherwise.</returns>
        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "XML-File | *.xml";
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the file dialog.
        /// </summary>
        /// <returns><c>true</c>, if file dialog was saved, <c>false</c> otherwise.</returns>
        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "XML-File | *.xml";
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="caption">Caption.</param>
        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption);
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">Message.</param>
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
