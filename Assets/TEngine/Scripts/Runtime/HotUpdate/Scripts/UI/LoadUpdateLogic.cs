﻿using System;

namespace TEngine.Runtime.HotUpdate
{
    public class LoadUpdateLogic
    {
        private static LoadUpdateLogic _instance;

        public Action<int> DownloadCompleteAction    = null;        
        public Action<long> DownProgressAction      = null;
        public Action<bool,GameStatus> UnpackedCompleteAction   = null;
        public Action<float,GameStatus> UnpackedProgressAction = null;

        public static LoadUpdateLogic Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoadUpdateLogic();
                }
                return _instance;
            }
        }
    }
}