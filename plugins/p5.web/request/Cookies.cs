/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details.
 */

using System;
using System.Web;
using p5.exp;
using p5.core;

namespace p5.web.ui.request
{
    /// <summary>
    ///     Helper to retrieve and set Cookie values
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        ///     Retrieves Cookie object(s)
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "get-cookie", Protection = EventProtection.LambdaClosed)]
        public static void get_cookie (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.Get (context, e.Args, delegate (string key) {

                // Fetching cookie
                var cookie = HttpContext.Current.Request.Cookies.Get (key);
                if (cookie != null && !string.IsNullOrEmpty (cookie.Value)) {

                    // Adding key node, and values beneath key node
                    return HttpUtility.UrlDecode (cookie.Value);
                }
                return null;
            });
        }

        /// <summary>
        ///     Lists all keys in the Cookie object of client
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "list-cookie-keys", Protection = EventProtection.LambdaClosed)]
        public static void list_cookie_keys (ApplicationContext context, ActiveEventArgs e)
        {
            CollectionBase.List (context, e.Args, HttpContext.Current.Request.Cookies.AllKeys);
        }
    }
}
