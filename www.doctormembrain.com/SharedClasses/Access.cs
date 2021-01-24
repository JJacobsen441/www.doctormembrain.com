using System;
using System.Collections.Generic;
using System.Threading;

namespace www.doctormembrain.com.SharedClasses
{
    public class Elem
    {
        public string Guid { get; set; }
        private Elem()
        {
        }
        public Elem(string guid)
        {
            Guid = guid;
        }
        public void Wait()
        {
            if (!Line.IsFirst(this.Guid))
            {
                Admin.Notification.Run(Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), Settings.Basic.EMAIL_MAIL(), "Queue!", "");
                Thread.Sleep(2000);
                Wait();
            }
        }
    }
    public class Line
    {
        public static DateTime last;
        public static List<string> q = new List<string>();
        public static bool IsFirst(string guid)
        {
            if (q.Count == 0 || q[0] == guid)
                return true;
            return false;
        }
        public static void PutInLine(Elem elem)
        {
            Reset();
            //e.Number = q.Count + 1;
            q.Add(elem.Guid);
            elem.Wait();
            last = DateTime.Now;
        }
        public static void GetThrough(Elem elem)
        {
            if (q.Contains(elem.Guid))
                q.Remove(elem.Guid);
        }
        private static void Reset()
        {
            if (last == DateTime.MinValue)
                return;
            DateTime now = DateTime.Now;
            if (now.Subtract(last).TotalSeconds > 30.0f)
                q = new List<string>();
        }
    }
    public class Access
    {
        public Elem e;
        private Access() { }
        public Access(string guid)
        {
            e = new Elem(guid);
        }

        public void Queue() =>
            Line.PutInLine(e);

        public void UnQueue() =>
            Line.GetThrough(e);
    }
}
