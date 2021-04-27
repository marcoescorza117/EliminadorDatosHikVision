using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace deleteData
{
    class obtenerDatosBD
    {

        //String conexion
        private string constring = "Data Source=localhost\\SQLEXPRESS50;Database=siiaControl; User Id = ControlAcceso; Password=T30UBROE";


        //referencia a clase para convertir a OBJ
        public List<Value> values = new List<Value>();

        //Metodo para asignar y retornar valores de checador

        public string checador;
        public string Checador
        {

            get { return checador; }
            set { checador = value; }
        }


        //Metodo para leer XML resultado, debe de estar en raiz
        public void leerXML()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader("resultado.xml");
                while (reader.Read())
                {

                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Text:
                            //Console.Write(reader.Value);
                            String valoridChecador = reader.Value;
                            //Int32.TryParse(valoridChecador, out idChecador);
                            Console.WriteLine(valoridChecador);
                            Checador = valoridChecador;
                            // Si lee el dato, hara la conexion a la base de datos
                            conexion();
                            break;
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }


        private void conexion()
        {
            SqlConnection connection = new SqlConnection(constring);

            try
            {
                connection.Open();
                SqlDataReader reader = null;
                //Aqui se pueden seleccionar todos los elementos que esten dentro del universo del checador (instancia local)
                // Se tiene que generar una nueva consulta para insertar valores,
                //SqlCommand command = new SqlCommand("SELECT distinct [idPersona] FROM [dbo].[vtaC_rhPersona_Huella_Checador] WHERE idChecador = "+idChecado+ " and HUELLA IS NOT NULL", connection);

                //SqlCommand command = new SqlCommand("SELECT distinct vtaC_rhPersona_Huella_Checador.idPersona as idPer, nombreCompleto FROM[dbo].[vtaC_rhPersona_Huella_Checador] inner join dbo.vtaC_rhPersona on vtaC_rhPersona_Huella_Checador.idPersona = vtaC_rhPersona.idPersona WHERE idChecador = " + Checador + " and HUELLA IS NOT NULL order by nombreCompleto", connection);
                SqlCommand command = new SqlCommand("SELECT distinct vtaC_rhPersona_Huella_Checador.idPersona as idPer, numeroEmpleado FROM[dbo].[vtaC_rhPersona_Huella_Checador] inner join dbo.vtaC_rhPersona on vtaC_rhPersona_Huella_Checador.idPersona = vtaC_rhPersona.idPersona WHERE idChecador =  " + Checador + " and HUELLA IS NOT NULL", connection);


                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    String numeroEmpleado = reader["numeroEmpleado"].ToString();
                    String idPeople = reader["idPer"].ToString();
                    Console.WriteLine(String.Format("{0} {1}", idPeople, numeroEmpleado));
                    values.Add(new Value() { idPersona = idPeople, NumeroEmpleado = numeroEmpleado });


                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
            finally
            {

                connection.Close();

            }
        }


    }
}
