using UnityEngine;

namespace TEngine.Runtime
{
    public class HotUpdateMgr : UnitySingleton<HotUpdateMgr>
    {
        public enum ServiceType
        {
            Dev, Stage, Prod
        }
        public ServiceType serviceType = ServiceType.Dev;
        string devHead = "https://ecwinabdev.oss-cn-shanghai.aliyuncs.com/Launcher/";
        string stageHead = "https://ecwinabstag.oss-cn-shanghai.aliyuncs.com/Launcher/";
        string prodHead = "https://ecwinabprod.oss-cn-shanghai.aliyuncs.com/Launcher/";

        public string GetUrlHead()
        {
            switch (serviceType)
            {
                case ServiceType.Dev: return devHead;
                case ServiceType.Stage: return stageHead;
                case ServiceType.Prod: return prodHead;
                default: return stageHead;
            }
        }

        BuildInfo m_BuildInfo = null;
        public BuildInfo BuildInfo
        {
            get
            {
                if (m_BuildInfo == null)
                {
                    m_BuildInfo = new BuildInfo();
                    m_BuildInfo.GameVersion = Application.version;
                    m_BuildInfo.InternalGameVersion = 0;
                    switch (serviceType)
                    {
                        case ServiceType.Dev:
                            m_BuildInfo.CheckVersionUrl = "http://127.0.0.1:8080/AssetsBundle/navigation/{0}Version.txt";
                            m_BuildInfo.WindowsAppUrl = null;
                            m_BuildInfo.MacOSAppUrl = null;
                            m_BuildInfo.IOSAppUrl = null;
                            m_BuildInfo.AndroidAppUrl = null;
                            break;
                        case ServiceType.Stage:
                            m_BuildInfo.CheckVersionUrl = "http://127.0.0.1:8080/AssetsBundle/navigation/{0}Version.txt";
                            m_BuildInfo.WindowsAppUrl = null;
                            m_BuildInfo.MacOSAppUrl = null;
                            m_BuildInfo.IOSAppUrl = null;
                            m_BuildInfo.AndroidAppUrl = null;
                            break;
                        case ServiceType.Prod:
                            m_BuildInfo.CheckVersionUrl = "http://127.0.0.1:8080/AssetsBundle/navigation/{0}Version.txt";
                            m_BuildInfo.WindowsAppUrl = null;
                            m_BuildInfo.MacOSAppUrl = null;
                            m_BuildInfo.IOSAppUrl = null;
                            m_BuildInfo.AndroidAppUrl = null;
                            break;
                    }
                }
                return m_BuildInfo;
            }
        }
        //public void InitBuildInfo()
        //{
        //    Log.Info("Build info can not be found or empty.");


        //    m_BuildInfo = Utility.Json.ToObject<BuildInfo>(m_BuildInfoTextAsset.text);
        //    if (m_BuildInfo == null)
        //    {
        //        Log.Warning("Parse build info failure.");
        //        return;
        //    }
        //}
    }
}