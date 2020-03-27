using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static public class Helper
{
    static public string multiply(string str, int amount)
    {
        string ret = "";
        for(int i = 0; i < amount; ++i)
        {
            ret += str;
        }
        return ret;
    }
}
