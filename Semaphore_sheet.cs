using System;
using System.Threading;

namespace q1
{
    class Semaphore_sheet
    {
        int Current;
        int maxsize;
        Mutex mu = new Mutex();

        public Semaphore_sheet(int num1, int num2)
        {
            this.Current = num1;
            this.maxsize = num2;
        }
        public void Wait()
        {
            mu.WaitOne();
            this.Current++;
            while (this.Current > this.maxsize) ; // busy wait
            mu.ReleaseMutex();
        }
        public void Release(int num = 1)
        {
            mu.WaitOne();
            this.Current -= num;
            mu.ReleaseMutex();
        }
        public void setMax(int num)
        {
            mu.WaitOne();
            this.maxsize = num;
            mu.ReleaseMutex();
        }
    }
}
