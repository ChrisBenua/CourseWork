﻿using System;
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
    public class MainSettingsViewModel : INotifyPropertyChanged
    {

        private IDialogService dialogService;

        private MainSettingsModel _model;

        private bool _isNextButtonEnabled = false;

        public MainSettingsModel Model
        {
            get => _model;

            set
            {
                _model = value;
                
                OnPropertyChanged("Model");
            }
        }

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

        private Brush _nextButtonColor = Brushes.LightGray;

        public Brush NextButtonColor
        {
            get => _nextButtonColor;
            set
            {
                _nextButtonColor = value;
                OnPropertyChanged("NextButtonColor");
            }
        }

        public MainSettingsViewModel()
        {
            dialogService = new DefaultDialogService();

            _model = new MainSettingsModel();
            _model.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Message")
                {
                    if (_model.Message.Length > 0)
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

        private RelayCommand _saveToSettingsCommand;

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
                    Configuration.Message = Model.Message;
                    OnSaveHappend?.Invoke(true);
                }));
            }
        }

        private RelayCommand _openRecentFileCommand;

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

                    string openedWindowTitle = null;
                    DeviceState state = null;

                    (state, openedWindowTitle) = IOHelper.LoadDeviceState(RecentFiles.MachineStatesShared.RecentFileNames[index]);
                    if (state != null)
                    {
                        Configuration.deviceState = state;
                        OnOpenRecentFileHappend?.Invoke(openedWindowTitle);
                    }

                }));
            }
        }

        private RelayCommand _openFileCommand;

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
                              var deviceState = IOHelper.LoadDeviceState(dialogService.FilePath);
                              if (deviceState.Item1 != null)
                              {
                                  Configuration.deviceState = deviceState.Item1;
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

        public event Action<String> OnOpenRecentFileHappend;
        public event Action<String> OnOpenFileHappend;
        public event Action<bool> OnSaveHappend;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}