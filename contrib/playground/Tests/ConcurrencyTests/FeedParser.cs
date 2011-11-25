/*    
 * Copyright (C) 2011, Hüseyin Uslu - shalafiraistlin@gmail.com
 *  
 * This program is free software: you can redistribute it and/or modify it under the terms of the GNU General 
 * Public License as published by the Free Software Foundation, either version 3 of the License, or (at your 
 * option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the 
 * implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License 
 * for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program.  If not, see 
 * <http://www.gnu.org/licenses/>. 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ConcurrencyTests
{
    public class FeedParser
    {
        private string _url;
        public readonly List<FeedItem> Stories = new List<FeedItem>();

        public FeedParser(string url)
        {
            this._url = url;

            WebReader.Result result = WebReader.Read(url);
            if (result.State != WebReader.States.Success) return;

            this.Parse(result.Response, this.Stories);
        }

        public bool Parse(string xml, List<FeedItem> items)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xml);

                if (xdoc.Root == null) return false;
                XNamespace defaultNS = xdoc.Root.GetDefaultNamespace();

                var entries = from item in xdoc.Descendants(defaultNS + "item")
                              select new
                              {
                                  Id = (string)item.Element(defaultNS + "guid") ?? "",
                                  Title = (string)item.Element(defaultNS + "title") ?? "",
                                  Link = (string)item.Element(defaultNS + "link") ?? "",
                              };

                items.AddRange(entries.Select(entry => new FeedItem(entry.Title, entry.Id, entry.Link)));
                return items.Count > 0;
            }
            catch (Exception) { return false; } 
        }
    }

    public class FeedItem
    {
        public string Title { get; private set; }
        public string Id { get; private set; }
        public string Link { get; private set; }

        public FeedItem(string title, string id, string link)
        {
            this.Title = title;
            this.Id = id;
            this.Link = link;
        }
    }
}
