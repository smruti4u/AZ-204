using System;
using System.Collections.Generic;
using System.Text;

namespace VsFunctionApp.Entities
{
    public class ToDoItem
    {
        public ToDoItem(string description)
        {
            this.Description = description;
        }
        public string Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }

    public class ToDoCreateViewModel
    {
        public string Description { get; set; }
    }
}
