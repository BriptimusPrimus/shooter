using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Game2DKit
{

    public class GenericProcess
    {
        protected Thread thread;

        protected bool running; //flag that indicates if the thread is runing

        public bool Running
        {
            get { return running; }
        }

        //virtual method for child classes
        //this will be the work done in any thread
        protected virtual void work()
        {
        }

        //this is the method called from the user
        public bool Start()
        {
            if (running)
            {
                return true;
            }

            try
            {
                running = true;
                //start the sending thread
                thread = new Thread(work);
                thread.Start();

            }
            catch //pokemon exception handling
            {
                return false;
            }

            return true;
        }

        public bool Finish()
        {
            try
            {
                thread.Abort();                
            }
            catch
            {
                return false;
            }

            //stop the running thread by turning the flag off
            running = false;
            return true;
        }

    }    

}
