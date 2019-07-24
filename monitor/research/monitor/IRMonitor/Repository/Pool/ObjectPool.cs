using System;
using System.Collections;
using System.Timers;

namespace Repository.Pool
{
    public abstract class ObjectPool
    {
        //Last Checkout time of any object from the pool.
        private long mLastCheckOut;

        //Hashtable of the check-out objects.
        private Hashtable mLocked;

        //Hashtable of available objects
        private Hashtable mUnlocked;

        //Clean-Up interval
        internal long GARBAGE_INTERVAL = 90 * 1000; //90 seconds

        internal ObjectPool()
        {
            mLocked = Hashtable.Synchronized(new Hashtable());
            mUnlocked = Hashtable.Synchronized(new Hashtable());

            mLastCheckOut = DateTime.Now.Ticks;

            //Create a Time to track the expired objects for cleanup.
            Timer aTimer = new Timer();
            aTimer.Enabled = true;
            aTimer.Interval = GARBAGE_INTERVAL;
            aTimer.Elapsed += new ElapsedEventHandler(CollectGarbage);
        }

        protected abstract object Create();

        protected abstract bool Validate(object o);

        protected abstract void Expire(object o);

        internal object GetObjectFromPool()
        {
            long now = DateTime.Now.Ticks;
            mLastCheckOut = now;
            object o = null;

            lock (this) {
                try {
                    foreach (DictionaryEntry myEntry in mUnlocked) {
                        o = myEntry.Key;
                        mUnlocked.Remove(o);
                        if (Validate(o)) {
                            mLocked.Add(o, now);
                            return o;
                        }
                        else {
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception) { }
                o = Create();
                mLocked.Add(o, now);
            }
            return o;
        }

        internal void ReturnObjectToPool(object o)
        {
            if (o != null) {
                lock (this) {
                    mLocked.Remove(o);
                    if (!mUnlocked.Contains(o))
                        mUnlocked.Add(o, DateTime.Now.Ticks);
                }
            }
        }

        private void CollectGarbage(object sender, ElapsedEventArgs ea)
        {
            lock (this) {
                object o;
                long now = DateTime.Now.Ticks;
                IDictionaryEnumerator e = mUnlocked.GetEnumerator();

                try {
                    while (e.MoveNext()) {
                        o = e.Key;

                        if ((now - (long)mUnlocked[o]) > GARBAGE_INTERVAL) {
                            mUnlocked.Remove(o);
                            Expire(o);
                            o = null;
                        }
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
