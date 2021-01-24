using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using www.doctormembrain.com.Extensions;

namespace www.doctormembrain.com.SharedClasses
{
    public class Storie
    {
        public string Header { get; set; }
        public string HeadLine { get; set; }
        public string Content { get; set; }
        public List<string> Category { get; set; }
        public int Count { get; set; }
        public int Showing { get; set; }
    }
    public class StorieHandler
    {
        private Session session = new Session(new HttpContextAccessor());
        private static List<Storie> stories = new List<Storie>();
        private static bool is_setup = false;
        public static void Setup() 
        {
            if (!is_setup)
            {
                string path = "";
                if (Statics.IsDebug)
                    path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\www.doctormembrain.com\\App_Data\\Stories.xml";
                else
                    path = "C:\\inetpub\\wwwroot\\www.doctormembrain.com\\App_Data\\Stories.xml";

                stories = new List<Storie>();
                
                var xdoc = XElement.Load(path);
                var x_stories = xdoc.Element("stories");
                                
                foreach (XElement storie in x_stories.Elements())
                {
                    List<string> categorys = new List<string>();
                    if (storie.Attribute("feature").Value == "true")
                        categorys.Add("feature");
                    if (storie.Attribute("love").Value == "true")
                        categorys.Add("love");
                    if (storie.Attribute("top").Value == "true")
                        categorys.Add("top");
                    stories.Add(new Storie()
                    {
                        Header = storie.Element("header").Value,
                        HeadLine = storie.Element("headline").Value,
                        Content = storie.Element("content").Value,
                        Category = categorys
                    }); ;
                }

                
                /*stories.Add(new Storie()//1
                {
                    Header = "Playing now!",
                    HeadLine = "\"The Search For The Missing Brain\"",
                    Content = "They had looked everywhere! In the kitchen, on the table, behind the couch, but it was nowhere to be found! Will they find it in time? Tune in to find out!",
                    Category = new List<string>() 
                });*/
            }

            is_setup = true;
            //if (session.Counter % 10 == 0)
            //    stories = stories.Randomize<Storie>().ToList();
            //session.Counter++;
        }
        public void Reset() 
        {
            is_setup = false;
            Setup();
        }
        public Storie StorieRand()
        {
            Setup();
            Random r = new Random();
            int showing = r.Next(0, stories.Count());
            int count = stories.Count();
            Storie s = stories.ElementAt(showing);
            s.Showing = showing + 1;
            s.Count = count;
            return s;
        }
        public Storie StorieLove()
        {
            Setup();
            Random r = new Random();
            List<Storie> love = stories.Where(s => s.Category.Contains("love")).ToList();
            int showing = r.Next(0, love.Count());
            int count = love.Count();
            Storie s = love.ElementAt(showing);
            s.Showing = showing + 1;
            s.Count = count;

            return s;
        }
        public Storie StorieTop5()
        {
            Setup();
            Random r = new Random();
            List<Storie> top = stories.Where(s => s.Category.Contains("top")).ToList();
            if (session.Top5.Count() >= 5)
                session.Top5 = new List<string>();

            int count = 5;
            int showing = r.Next(0, count);

            bool ok;
            Storie s = top.ElementAt(showing);
            while (session.Top5.Contains(StringHelper.OnlyAlphanumeric(s.HeadLine, false, true, "", new char[0], out ok)))
            {
                showing = r.Next(0, count);
                s = top.ElementAt(showing);
            }
            List<string> top5 = session.Top5;
            top5.Add(StringHelper.OnlyAlphanumeric(s.HeadLine, false, true, "", new char[0], out ok));
            session.Top5 = top5;

            s.Showing = showing + 1;
            s.Count = count;
            return s;
        }
        public Storie StorieFeature()
        {
            Setup();
            Storie s = stories.Where(s => s.Category.Contains("feature")).FirstOrDefault();
            s.Showing = 1;
            s.Count = 1;
            return s;
        }
    }
}
