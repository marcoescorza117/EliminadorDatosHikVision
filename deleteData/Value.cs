using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace deleteData
{
    class Value : IEquatable<Value>
    {
        public string idPersona { get; set; }

        public string NumeroEmpleado { get; set; }

        public override string ToString()
        {
            return "ID: " + idPersona + "   NumeroEmpleado: " + NumeroEmpleado;
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Value objAsPart = obj as Value;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }


        public bool Equals(Value other)
        {
            if (other == null) return false;
            return (this.idPersona.Equals(other.idPersona));
        }
    }
}
