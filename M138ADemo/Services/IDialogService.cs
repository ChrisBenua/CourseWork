using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M138ADemo.Services
{
    public interface IDialogService
    {

        void ShowMessage(string message);
        void ShowMessage(string message, string caption);// показ сообщения
        string FilePath { get; set; }   // путь к выбранному файлу
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();

    }
}
