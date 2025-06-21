using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Maneh.RabbitMQTest.Events
{
    public class MessageAdded
    {
        static MessageAdded()
        {
            if (!File.Exists("./currentId"))
            {
                _saveLastId();
            }
            else
            {
                id = int.Parse(File.ReadAllText("./currentId"));
            }
        }
        static int id = 1;
        public int Id { get; set; }
        public string Text { get; set; }

        public MessageAdded()
        {

        }

        static void _saveLastId()
        {
            File.WriteAllText("./currentId", id.ToString());
        }

        public MessageAdded(string message)
        {
            Id = id++;

            Text = string.Format(message, Id);
            _saveLastId();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
