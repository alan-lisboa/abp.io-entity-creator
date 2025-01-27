using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator
{

    public class Indentor
    {
        private int count = 0;

        public void Indent()
        {
            count++;
        }

        public void Unindent()
        {
            count--;
        }

        public string Get()
        {
            return new string('\t', count);
        }

        public void Reset()
        {
            count = 0;
        }
    }
}
