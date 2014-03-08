using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDD2Project1
{
    /// <summary>
    /// ContentObject is the top level base type for any object contained in the
    /// GameContentManager.
    /// </summary>
    public class ContentObject
    {
        protected String _name;

        public String Name
        {
            get { return _name; }
        }

        public ContentObject(String name)
        {
            _name = name;
        }
    }
}
