using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRMBytholod.Settings
{
    public class Const
    {
        //local
        //public const string ConnectionString = @"Data Source = DENIS-ПК; Initial Catalog = Bytholod; Integrated Security = True";
        // MAIN
        //public const string ConnectionString = @"Data Source=DENIS-ПК; Initial Catalog=Bytholod;User ID=holodec;Password=9faw8@f2GA2";

        //удаленная БД
        //public const string ConnectionString = @"Data Source=tcp:crm.bytholod.com,1433;Initial Catalog=Bytholod;User ID=SA;Password=111";
        

        public const string ConnectionString = @"Data Source=tcp:crm.bytholod.com,1433; Initial Catalog=Bytholod;User ID=holodec;Password=9faw8@f2GA2";


    }
}
