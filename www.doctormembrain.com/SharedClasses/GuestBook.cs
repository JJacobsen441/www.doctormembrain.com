using System.Collections.Generic;
using System.Xml.Linq;

namespace www.doctormembrain.com.SharedClasses
{
    public class Entry
    {
        public string Date { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Content { get; set; }
    }
    public class GuestBook_Dto 
    {
        public List<Entry> entrys;
        public Entry message;
    }
    public class GuestBook
    {
        //private Session session = new Session(new HttpContextAccessor());
        public List<Entry> GetEntrys()
        {
            List<Entry> entrys = new List<Entry>();
            string path = "";
            if (Statics.IsDebug)
                path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\www.doctormembrain.com\\App_Data\\GuestBook.xml";
            else
                path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\App_Data\\GuestBook.xml";

            var xdoc = XElement.Load(path);
            var guestbook = xdoc.Element("guestbook");

            foreach (XElement entry in guestbook.Elements())
            {
                entrys.Add(new Entry()
                {
                    Date = entry.Attribute("date").Value,
                    Country = entry.Element("country").Value,
                    Name = entry.Element("name").Value,
                    Content = entry.Element("content").Value,
                }); ;
            }
            return entrys;
        }
        public void AddEntry(Entry ent)
        {
            List<Storie> entrys = new List<Storie>();
            string path = "";
            if (Statics.IsDebug)
                path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\www.doctormembrain.com\\App_Data\\GuestBook.xml";
            else
                path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\App_Data\\GuestBook.xml";

            entrys = new List<Storie>();

            var xdoc = XElement.Load(path);
            var g_book = xdoc.Element("guestbook");

            XElement entry = new XElement("entry");
            entry.Add(new XAttribute("date", ent.Date));
            entry.Add(new XElement("country", ent.Country.ToLower()));
            entry.Add(new XElement("name", ent.Name));
            entry.Add(new XElement("content", ent.Content));
            g_book.AddFirst(entry);

            xdoc.Save(path);
        }
    }
}
