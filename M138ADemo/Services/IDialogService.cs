using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M138ADemo.Services
{
    /// <summary>
    /// Dialog service.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">Message.</param>
        void ShowMessage(string message);

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="caption">Caption.</param>
        void ShowMessage(string message, string caption);// показ сообщения

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        string FilePath { get; set; }   // путь к выбранному файлу

        /// <summary>
        /// Opens the file dialog.
        /// </summary>
        /// <returns><c>true</c>, if file dialog was opened, <c>false</c> otherwise.</returns>
        bool OpenFileDialog();  // открытие файла

        /// <summary>
        /// Saves the file dialog.
        /// </summary>
        /// <returns><c>true</c>, if file dialog was saved, <c>false</c> otherwise.</returns>
        bool SaveFileDialog();

    }
}
