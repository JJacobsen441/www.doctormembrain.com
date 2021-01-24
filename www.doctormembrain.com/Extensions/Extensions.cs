using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace www.doctormembrain.com.Extensions
{
    public static class Extensions
    {
        //public static MvcHtmlString ActionImage(this HtmlHelper html, string action, string controllerName, object routeValues, string imagePath, string data_src, string alt = null, string[] classes_img = null, string[] classes_a = null)
        //{
        //    var url = new UrlHelper(html.ViewContext.RequestContext);

        //    // build the <img> tag
        //    var imgBuilder = new TagBuilder("img");
        //    if (imagePath != null)
        //        imgBuilder.MergeAttribute("src", url.Content(imagePath));
        //    if (data_src != null)
        //        imgBuilder.MergeAttribute("data-src", url.Content(data_src));
        //    if (alt != null)
        //        imgBuilder.MergeAttribute("alt", alt);
        //    string classes_img_string = "";
        //    foreach (string cssclass in classes_img)
        //    {
        //        if (cssclass != null)
        //            classes_img_string += " " + cssclass;
        //    }
        //    imgBuilder.MergeAttribute("class", classes_img_string);
        //    string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

        //    // build the <a> tag
        //    var anchorBuilder = new TagBuilder("a");
        //    if (action != "")
        //        anchorBuilder.MergeAttribute("href", url.Action(action, controllerName, routeValues));
        //    else
        //        anchorBuilder.MergeAttribute("href", "#/");
        //    if (classes_a != null)
        //    {
        //        string classes_a_string = "";
        //        foreach (string cssclass in classes_a)
        //        {
        //            if (cssclass != null)
        //                classes_a_string += " " + cssclass;
        //        }
        //        anchorBuilder.MergeAttribute("class", classes_a_string);
        //    }
        //    anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
        //    string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

        //    return MvcHtmlString.Create(anchorHtml);
        //}
        //public static MvcHtmlString ActionCheckBox(this HtmlHelper html, string name, bool isChecked, string action, string controllerName, object routeValues, string[] classes = null)
        //{
        //    var url = new UrlHelper(html.ViewContext.RequestContext);

        //    // build the <img> tag
        //    var checkBuilder = new TagBuilder("input");
        //    checkBuilder.MergeAttribute("type", "checkbox");
        //    if (name != null)
        //        checkBuilder.MergeAttribute("id", name);

        //    if (isChecked)
        //        checkBuilder.MergeAttribute("checked", "checked");

        //    string classes_string = "";
        //    foreach (string cssclass in classes)
        //    {
        //        if (cssclass != null)
        //            classes_string += " " + cssclass;
        //    }
        //    checkBuilder.MergeAttribute("class", classes_string);
        //    checkBuilder.MergeAttribute("onclick", "location.href='" + url.Action(action, controllerName, routeValues) + "'");// + "?c=" + routeValues);
        //    string checkHtml = checkBuilder.ToString(TagRenderMode.SelfClosing);

        //    // build the <a> tag
        //    //var anchorBuilder = new TagBuilder("a");
        //    //anchorBuilder.MergeAttribute("href", url.Action(action, controllerName, routeValues));
        //    //anchorBuilder.InnerHtml = checkHtml; // include the <img> tag inside
        //    //string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

        //    return MvcHtmlString.Create(checkHtml);
        //}
        //public static HtmlString DisplayWithBreaksFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    //var model = html.Encode(metadata.Model).Replace("\r\n", "<br />\r\n");
        //    var model = html.Encode(metadata.Model).Replace(Environment.NewLine, "<br />");

        //    if (String.IsNullOrEmpty(model))
        //        return MvcHtmlString.Empty;

        //    return HtmlString.Create(model);
        //}
        //public static string HtmlEncode(string html)
        //{
        //    var httpUtil = new HttpServerUtilityWrapper(HttpContext.Current.Server);
        //    //string encoded = httpUtil.HtmlEncode(html).Replace("\r\n", "<br />\r\n");
        //    string encoded = httpUtil.HtmlEncode(html).Replace(Environment.NewLine, "<br />");

        //    //if (String.IsNullOrEmpty(encoded))
        //    //    return MvcHtmlString.Empty;

        //    //return MvcHtmlString.Create(encoded);
        //    return encoded;
        //}
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            Random rnd = new Random();
            return source.OrderBy<T, int>((item) => rnd.Next());
        }
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObjectAsJson<T>(this ISession session, string key) where T : new()
        {
            var value = session.GetString(key);
            return value == null ? new T() : JsonConvert.DeserializeObject<T>(value);
        }
    }
}