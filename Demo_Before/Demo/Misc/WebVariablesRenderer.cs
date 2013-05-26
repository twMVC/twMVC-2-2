﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
using System.Globalization;
using NLog;
using NLog.Config;
using System.Xml;
using System.Web;

namespace Demo.Misc
{
    [LayoutRenderer("web_variables")]
    public class WebVariablesRenderer : LayoutRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebVariablesRenderer"/> class.
        /// </summary>
        public WebVariablesRenderer()
        {
            this.Format = string.Empty;
            this.Culture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the culture used for rendering..
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets or sets the date format. Can be any argument accepted by DateTime.ToString(format).
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        [DefaultParameter]
        public string Format { get; set; }

        /// <summary>
        /// Renders the current date and appends it to the specified .
        /// </summary>summary>
        /// <param name="builder">The  to append the rendered data to.
        /// <param name="logEvent">Logging event.
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);

            writer.WriteStartElement("error");

            // -----------------------------------------
            // Server Variables
            // -----------------------------------------
            writer.WriteStartElement("serverVariables");

            foreach (string key in HttpContext.Current.Request.ServerVariables.AllKeys)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("name", key);

                writer.WriteStartElement("value");
                writer.WriteAttributeString("string", HttpContext.Current.Request.ServerVariables[key].ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            // -----------------------------------------
            // Cookies
            // -----------------------------------------
            writer.WriteStartElement("cookies");

            foreach (string key in HttpContext.Current.Request.Cookies.AllKeys)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("name", key);

                writer.WriteStartElement("value");
                writer.WriteAttributeString("string", HttpContext.Current.Request.Cookies[key].Value.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            // -----------------------------------------

            writer.WriteEndElement();
            // -----------------------------------------

            writer.Flush();
            writer.Close();

            string xml = sb.ToString();

            builder.Append(xml);
        }

    }
}