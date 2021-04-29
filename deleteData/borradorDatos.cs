using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deleteData
{
    class borradorDatos
    {
        public static int m_UserID = -1;
        public int m_lGetCardCfgHandle = -1;
        public int m_lSetCardCfgHandle = -1;
        public int m_lDelCardCfgHandle = -1;

        public int M_UserID
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }


        public int M_lGetCardCfgHandle
        {
            get { return m_lGetCardCfgHandle; }
            set { m_lGetCardCfgHandle = value; }
        }

        public int M_lSetCardCfgHandle
        {
            get { return m_lSetCardCfgHandle; }
            set { m_lSetCardCfgHandle = value; }
        }

        public int M_lDelCardCfgHandle
        {
            get { return m_lDelCardCfgHandle; }
            set { m_lDelCardCfgHandle = value; }
        }



        public void iniciarServicios()
        {
            if (CHCNetSDK.NET_DVR_Init() == false)
            {
                Console.WriteLine("NET_DVR_Init error!");
                Console.ReadKey();
                return;

            }
            //comboBoxLanguage.SelectedIndex = 0;
            CHCNetSDK.NET_DVR_SetLogToFile(3, "./", false);

        }

        public void login(string ip, string usr, string psw)
        {
            string portt = "8000";
            if (M_UserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout_V30(m_UserID);
                M_UserID = -1;
            }


            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];


            struLoginInfo.sDeviceAddress = ip;
            struLoginInfo.sUserName = usr;
            struLoginInfo.sPassword = psw;
            ushort.TryParse(portt, out struLoginInfo.wPort);

            int lUserID = -1;
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (lUserID >= 0)
            {
                M_UserID = lUserID;
                Console.WriteLine("Login Successful");


                //startRemoteService();
                //Llamar arreglo de sql para borrar LOS ELEMENTOS SINCRONIZADOS


                //Desde aqui --4:55 pm 27-abril-2021 
               obtenerDatosBD datab = new obtenerDatosBD();
                datab.leerXML();

                foreach (var item in datab.values)
                {

                    Console.WriteLine(item.idPersona);
                    startRemoteService(item.idPersona.ToString());
                    Thread.Sleep(170);

                }
                //Console.ReadKey();
               

                //hasta aqui --4:55 pm 27-abril-2021


            }
            else
            {
                uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    Console.WriteLine("user name or password error!");
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
                        Console.WriteLine(strTemp1);
                    }
                }
                else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format("user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
                        Console.WriteLine(strTemp1);
                    }
                }
                else
                {
                    Console.WriteLine("net error or dvr is busy!");

                }

            }



        }

        public void startRemoteService(string idBorrar)
        {

            if (M_lDelCardCfgHandle != -1)
            {
                if (CHCNetSDK.NET_DVR_StopRemoteConfig(M_lDelCardCfgHandle))
                {
                    M_lDelCardCfgHandle = -1;
                }
            }
            CHCNetSDK.NET_DVR_CARD_COND struCond = new CHCNetSDK.NET_DVR_CARD_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);
            struCond.dwCardNum = 1;
            IntPtr ptrStruCond = Marshal.AllocHGlobal((int)struCond.dwSize);
            Marshal.StructureToPtr(struCond, ptrStruCond, false);

            CHCNetSDK.NET_DVR_CARD_SEND_DATA struSendData = new CHCNetSDK.NET_DVR_CARD_SEND_DATA();
            struSendData.Init();
            struSendData.dwSize = (uint)Marshal.SizeOf(struSendData);
            byte[] byTempCardNo = new byte[CHCNetSDK.ACS_CARD_NO_LEN];
            byTempCardNo = System.Text.Encoding.UTF8.GetBytes(idBorrar);
            for (int i = 0; i < byTempCardNo.Length; i++)
            {
                struSendData.byCardNo[i] = byTempCardNo[i];
            }
            IntPtr ptrStruSendData = Marshal.AllocHGlobal((int)struSendData.dwSize);
            Marshal.StructureToPtr(struSendData, ptrStruSendData, false);

            CHCNetSDK.NET_DVR_CARD_STATUS struStatus = new CHCNetSDK.NET_DVR_CARD_STATUS();
            struStatus.Init();
            struStatus.dwSize = (uint)Marshal.SizeOf(struStatus);
            IntPtr ptrdwState = Marshal.AllocHGlobal((int)struStatus.dwSize);
            Marshal.StructureToPtr(struStatus, ptrdwState, false);


            //M_lDelCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(M_UserID, CHCNetSDK.NET_DVR_SET_CARD, ptrStruCond, (int)struCond.dwSize, null, IntPtr.Zero);

            M_lGetCardCfgHandle = CHCNetSDK.NET_DVR_StartRemoteConfig(M_UserID, CHCNetSDK.NET_DVR_DEL_CARD, ptrStruCond, (int)struCond.dwSize, null, IntPtr.Zero);

            Console.WriteLine(M_lGetCardCfgHandle);

            if (m_lGetCardCfgHandle < 0)
            {

                Console.WriteLine("NET_DVR_DEL_CARD error:" + CHCNetSDK.NET_DVR_GetLastError());
                Marshal.FreeHGlobal(ptrStruCond);
                return;
            }
            else
            {

                Console.WriteLine(M_lGetCardCfgHandle);
                int dwState = (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS;
                uint dwReturned = 0;
                while (true)
                {
                    dwState = CHCNetSDK.NET_DVR_SendWithRecvRemoteConfig(m_lGetCardCfgHandle, ptrStruSendData, struSendData.dwSize, ptrdwState, struStatus.dwSize, ref dwReturned);
                    if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_NEEDWAIT)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FAILED)
                    {
                        Console.WriteLine("NET_DVR_DEL_CARD fail error: " + CHCNetSDK.NET_DVR_GetLastError());
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_SUCCESS)
                    {
                        Console.WriteLine("NET_DVR_DEL_CARD success");
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_FINISH)
                    {
                        Console.WriteLine("NET_DVR_DEL_CARD finish");
                        break;
                    }
                    else if (dwState == (int)CHCNetSDK.NET_SDK_SENDWITHRECV_STATUS.NET_SDK_CONFIG_STATUS_EXCEPTION)
                    {
                        Console.WriteLine("NET_DVR_DEL_CARD exception error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                    else
                    {
                        Console.WriteLine("unknown status error: " + CHCNetSDK.NET_DVR_GetLastError());
                        break;
                    }
                }

            }

            CHCNetSDK.NET_DVR_StopRemoteConfig(M_lGetCardCfgHandle);
            M_lGetCardCfgHandle = -1;
            Marshal.FreeHGlobal(ptrStruSendData);
            Marshal.FreeHGlobal(ptrdwState);
        }
    }
}
