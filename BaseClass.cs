using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections;

namespace DLLWithFramework
{
    public class BaseCode : MarshalByRefObject
    {
        private Queue QueueMessages = new Queue();
        public Hashtable Fields { get; private set; }

        public BaseCode()
        {
            /*constructor*/
        }

        public void SetField(string fieldName, object fieldValue)
        {
            if (Fields == null)
                Fields = new Hashtable();
            Fields[fieldName] = fieldValue;
        }

        public void LogMessage(string message)
        {
            QueueMessages.Enqueue(message);
        }

        public string GetLogMessage()
        {
            if (QueueMessages.Count > 0)
            {
                return QueueMessages.Dequeue().ToString();
            }
            else
            {
                return null;
            }
        }

        public virtual void Run()
        {
        }
    }
}


