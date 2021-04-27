using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace deleteData
{
    class StartProcess
    {
        //this class read the config.xml file to access the node "direccionIP" value
        private string direccionIP;

        public string DireccionIP { get => direccionIP; set => direccionIP = value; }

        public void readFileXML()
        {
            try
            {
                if (File.Exists("config.xml"))
                {
                    Console.WriteLine("Existe Archivo");
                    XmlDocument doc = new XmlDocument();
                    doc.Load("config.xml");

                    XmlNodeList ipTagName;

                    ipTagName = doc.GetElementsByTagName("direccionIP");
                    DireccionIP = ipTagName[0].InnerXml;
                    startServices();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void startServices()
        {
            
            borradorDatos borrador = new borradorDatos();
            borrador.iniciarServicios();
            borrador.login(DireccionIP, "admin", "uaeh2021");

        }

    }
}
