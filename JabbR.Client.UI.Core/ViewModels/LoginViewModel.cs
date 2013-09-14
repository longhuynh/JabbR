﻿using Akavache;
using Cirrious.MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JabbR.Client.UI.Core.ViewModels
{
    public class LoginViewModel
        : BaseViewModel
    {
        readonly IJabbRClient _client;
        public LoginViewModel(IJabbRClient client)
        {
            _client = client;
            PageName = "login";
        }

        public void Init()
        {
            if (IsConnected)
            {
                _client.Disconnect();
                IsConnected = false;
            }
            //LoginInfo loginInfo = null;
            ////BlobCache.Secure.GetLoginAsync(_client.SourceUrl)
            ////    .Subscribe(info => loginInfo = info);
            //if (loginInfo != null)
            //{
            //    UserName = loginInfo.UserName;
            //    Password = loginInfo.Password;
            //    DoSignIn();
            //}
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChanged(() => IsConnected);
                RaisePropertyChanged(() => CanDoSignIn);
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged(() => UserName);
                RaisePropertyChanged(() => CanDoSignIn);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
                RaisePropertyChanged(() => CanDoSignIn);
            }
        }

        private ICommand _signInCommand;
        public ICommand SignInCommand
        {
            get
            {
                _signInCommand = _signInCommand ?? new MvxCommand(DoSignIn);
                return _signInCommand;
            }
        }

        public bool CanDoSignIn
        {
            get { return (!_isConnected && !String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password)); }
        }

        private async void DoSignIn()
        {
            if (!CanDoSignIn)
            {
                ErrorMessage = "Please enter a username and password";
                return;
            }
            try
            {
                var info = await _client.Connect(UserName, Password);
                IsConnected = true;
                //BlobCache.Secure.SaveLogin(UserName, Password, _client.SourceUrl, TimeSpan.FromDays(7));

                var user = await _client.GetUserInfo();
                UserName = String.Empty;
                Password = String.Empty;
                ShowViewModel<MainViewModel>(new { userJson = JsonConvert.SerializeObject(user), roomsJson = JsonConvert.SerializeObject(info.Rooms) });
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().Message;
                return;
            }
        }
    }
}