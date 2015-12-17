﻿/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, phosphorusfive@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Security;
using System.Configuration;
using System.Collections.Generic;
using p5.exp;
using p5.core;
using p5.exp.exceptions;
using p5.io.authorization.helpers;

namespace p5.io.authorization
{
    /// <summary>
    ///     Class wrapping authorization for files in Phosphorus Five
    /// </summary>
    internal static class Authorization
    {
        /// <summary>
        ///     Throws an exception if user is not authorized to read the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.read-file", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_read_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeReadFile (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to modify/create the given file
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.modify-file", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_modify_file (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeModifyFile (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to read from the given folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.read-folder", Protection = EventProtection.NativeClosed)]
        private static void _authorize_read_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeReadFolder (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }

        /// <summary>
        ///     Throws an exception if user is not authorized to modify the given folder
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Active Event arguments</param>
        [ActiveEvent (Name = "p5.io.authorize.modify-folder", Protection = EventProtection.NativeClosed)]
        private static void p5_io_authorize_modify_folder (ApplicationContext context, ActiveEventArgs e)
        {
            AuthorizationHelper.AuthorizeModifyFolder (
                context, 
                e.Args.Get<string> (context), 
                e.Args ["args"].Get<Node> (context));
        }
    }
}