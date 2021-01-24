using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace www.doctormembrain.com.SharedClasses
{
    public class Session
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISession _session;
        public Session(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }

        public string AccessGuid
        {
            get
            {
                ISession _session = _httpContextAccessor.HttpContext.Session;
                if (_session.GetString("access") != null)
                    return (string)_session.GetString("access");
                return "";
            }
            set
            {
                ISession _session = _httpContextAccessor.HttpContext.Session;
                string s = "";
                if (!string.IsNullOrEmpty(value))
                    s = value;
                _session.SetString("access", s);
            }
        }

        public Access Access
        {
            get
            {
                ISession _session = _httpContextAccessor.HttpContext.Session;
                if (!string.IsNullOrEmpty(_session.GetString("access")))
                    return new Access((string)_session.GetString("access"));
                throw new Exception("A-OK, Handled");
            }
            //set
            //{
            //    ISession _session = _httpContextAccessor.HttpContext.Session;
            //    string s = "";
            //    if (!string.IsNullOrEmpty(value))
            //        s = value;
            //    _session.SetString("access", s);
            //}
        }

        public int Counter
        {
            get
            {
                if (_session.GetInt32("counter") != null)
                    return (int)_session.GetInt32("counter");
                return 0;
            }
            set
            {
                _session.SetInt32("counter", value);
            }
        }
        public List<string> Top5
        {
            get
            {
                return Extensions.Extensions.GetObjectAsJson<List<string>>(_session, "top5");
            }
            set
            {
                Extensions.Extensions.SetObjectAsJson(_session, "top5", value);
            }
        }
    }
}
