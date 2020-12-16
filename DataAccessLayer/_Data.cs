using DataAccessLayer.Custom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLayer
{
    /// <summary>
    /// wrapper methods of _Base here...
    /// </summary>
    public class _Data: _Base
    {
        public static _Base BaseSQL {
            get { 
                return new _Base(CONSTANTS.PROVIDER_TYPE_SQL, CONSTANTS.CONNECTION_STRING_SQL); 
            }
        }

        public static _Base BaseMYSQL
        {
            get
            {
                return new _Base(CONSTANTS.PROVIDER_TYPE_MYSQL, CONSTANTS.CONNECTION_STRING_MYSQL);
            }
        }
    }
}
