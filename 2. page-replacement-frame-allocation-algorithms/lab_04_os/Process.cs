using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab_04_os
{
    class Process
    {
        int pages = 0;
        bool priority = false;

        public Process(int pages, bool priority)
        {
            this.pages = pages;
            this.priority = priority;
           
        }

        public Process()
        { bool priority = false; }

        public int getPages()
        {
            return pages;
        }

        public bool getPriority()
        {
            return priority;
        }

        public void setPages(int pages)
        {
            this.pages = pages;
        }

        public void setPriority(bool priority)
        {
            this.priority = priority;
        }

    }
}
