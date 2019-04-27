using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using M138ADemo.Models;
using System.Windows.Media;
using M138ADemo.MainClasses;
using M138ADemo.Services;

namespace M138ADemo
{
    /// <summary>
    /// Main settings view model.
    /// </summary>
    public class MainSettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Label clicks.
        /// </summary>
        public enum LabelClicks
        {
            Encrypt, Decrypt, Manual, Automatic, ExtendedWorkspace
        }
         
        /// <summary>
        /// The dialog service.
        /// </summary>
        private IDialogService dialogService;

        /// <summary>
        /// The model.
        /// </summary>
        private MainSettingsModel _model;

        /// <summary>
        /// The is next button enabled.
        /// </summary>
        private bool _isNextButtonEnabled = false;

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public MainSettingsModel Model
        {
            get => _model;

            set
            {
                _model = value;
                
                OnPropertyChanged("Model");
            }
        }

        /// <summary>
        /// The is checked extended workspace.
        /// </summary>
        private bool _isCheckedExtendedWorkspace;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.MainSettingsViewModel"/> is checked
        /// extended workspace.
        /// </summary>
        /// <value><c>true</c> if is checked extended workspace; otherwise, <c>false</c>.</value>
        public bool IsCheckedExtendedWorkspace
        {
            get => _isCheckedExtendedWorkspace;

            set
            {
                _isCheckedExtendedWorkspace = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The on extended workspace button command.
        /// </summary>
        private RelayCommand _onExtendedWorkspaceButtonCommand;

        /// <summary>
        /// Gets the on extended workspace button command.
        /// </summary>
        /// <value>The on extended workspace button command.</value>
        public RelayCommand OnExtendedWorkspaceButtonCommand
        {
            get
            {
                return _onExtendedWorkspaceButtonCommand ?? (_onExtendedWorkspaceButtonCommand = new RelayCommand(obj =>
                {
                    IsCheckedExtendedWorkspace = !IsCheckedExtendedWorkspace;
                }));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:M138ADemo.MainSettingsViewModel"/> is next button enabled.
        /// </summary>
        /// <value><c>true</c> if is next button enabled; otherwise, <c>false</c>.</value>
        public bool isNextButtonEnabled
        {
            get
            {
                return _isNextButtonEnabled;
            }
            set
            {
                _isNextButtonEnabled = value;
                OnPropertyChanged("isNextButtonEnabled");
                if (value)
                {
                    NextButtonColor = Brushes.Aqua;
                } 
                else
                {
                    NextButtonColor = Brushes.LightGray;
                }
            }
        }

        /// <summary>
        /// The color of the next button.
        /// </summary>
        private Brush _nextButtonColor = Brushes.LightGray;

        /// <summary>
        /// Gets or sets the color of the next button.
        /// </summary>
        /// <value>The color of the next button.</value>
        public Brush NextButtonColor
        {
            get => _nextButtonColor;
            set
            {
                _nextButtonColor = value;
                OnPropertyChanged("NextButtonColor");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:M138ADemo.MainSettingsViewModel"/> class.
        /// </summary>
        public MainSettingsViewModel()
        {
            dialogService = new DefaultDialogService();

            _model = new MainSettingsModel();
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Message" || e.PropertyName == "Automatic")
                {
                    if (_model.Message.Length > 0 || !this.Model.Automatic)
                    {
                        isNextButtonEnabled = true;
                    }
                    else
                    {
                        isNextButtonEnabled = false;
                    }
                }
            }; 
        }

        /// <summary>
        /// The save to settings command.
        /// </summary>
        private RelayCommand _saveToSettingsCommand;

        /// <summary>
        /// Gets the save to settings command.
        /// </summary>
        /// <value>The save to settings command.</value>
        public RelayCommand SaveToSettingsCommand
        {
            get
            {
                return _saveToSettingsCommand ?? (_saveToSettingsCommand = new RelayCommand(arg =>
                {
                    if (Model.Encrypt)
                    {
                        Configuration.Encrypt = true;
                    }
                    else
                    {
                        Configuration.Decrypt = true;
                    }

                    if (Model.Automatic)
                    {
                        Configuration.Automatic = true;
                    }
                    else
                    {
                        Configuration.Manual = true;
                    }
                    Configuration.IsCompactWorkSpace = !this.IsCheckedExtendedWorkspace;
                    Configuration.Message = Model.Message;
                    OnSaveHappend?.Invoke(true);
                }));
            }
        }

        /// <summary>
        /// The open recent file command.
        /// </summary>
        private RelayCommand _openRecentFileCommand;

        /// <summary>
        /// Gets the open recent file command.
        /// </summary>
        /// <value>The open recent file command.</value>
        public RelayCommand OpenRecentFileCommand
        {
            get
            {
                return _openRecentFileCommand ?? (_openRecentFileCommand = new RelayCommand(obj =>
                {
                    int index = (int)obj;
                    if (index == -1)
                    {
                        dialogService.ShowMessage("Странная ошибка, очень, такого не должно случаться", "Странная Ошибка");
                        return;
                    }
                    Configuration.IsCompactWorkSpace = !this.IsCheckedExtendedWorkspace;
                    string openedWindowTitle = null;
                    DeviceState state = null;

                    (state, openedWindowTitle) = IOHelper.LoadDeviceState(RecentFiles.MachineStatesShared.RecentFileNames[index]);
                    if (state != null)
                    {
                        Configuration.DeviceState = state;
                        Configuration.Message = null;
                        OnOpenRecentFileHappend?.Invoke(openedWindowTitle);
                    }

                }));
            }
        }
        /// <summary>
        /// The open file command.
        /// </summary>
        private RelayCommand _openFileCommand;

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        /// <value>The open file command.</value>
        public RelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ??
                  (_openFileCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              Configuration.IsCompactWorkSpace = !this.IsCheckedExtendedWorkspace;

                              var deviceState = IOHelper.LoadDeviceState(dialogService.FilePath);
                              if (deviceState.Item1 != null)
                              {
                                  Configuration.DeviceState = deviceState.Item1;
                                  OnOpenFileHappend?.Invoke(deviceState.Item2);
                              }
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        private RelayCommand _openDocsCommand;

        public RelayCommand OpenDocsCommand
        {
            get
            {
                return _openDocsCommand ?? (_openDocsCommand = new RelayCommand(obj =>
                {
                    DocsWindow w = new DocsWindow();
                    w.Show();
                }));
            }
        }
        /// <summary>
        /// The on label click command.
        /// </summary>
        private RelayCommand _onLabelClickCommand;

        /// <summary>
        /// Gets the on label click command.
        /// </summary>
        /// <value>The on label click command.</value>
        public RelayCommand OnLabelClickCommand
        {
            get
            {
                return _onLabelClickCommand ?? (_onLabelClickCommand = new RelayCommand(obj =>
                {
                    LabelClicks click = (LabelClicks)obj;
                    switch(click)
                    {
                        case LabelClicks.Automatic:
                            this.Model.Automatic = true;
                            break;
                        case LabelClicks.Decrypt:
                            this.Model.Encrypt = false;
                            break;
                        case LabelClicks.ExtendedWorkspace:
                            this.IsCheckedExtendedWorkspace = !this.IsCheckedExtendedWorkspace;
                            break;
                        case LabelClicks.Manual:
                            this.Model.Automatic = false;
                            break;
                        case LabelClicks.Encrypt:
                            this.Model.Encrypt = true;
                            break;
                    }
                }));
            }
        }
        /// <summary>
        /// Occurs when on open recent file happend.
        /// </summary>
        public event Action<String> OnOpenRecentFileHappend;

        /// <summary>
        /// Occurs when on open file happend.
        /// </summary>
        public event Action<String> OnOpenFileHappend;

        /// <summary>
        /// Occurs when on save happend.
        /// </summary>
        public event Action<bool> OnSaveHappend;

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Ons the property changed.
        /// </summary>
        /// <param name="prop">Property.</param>
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
