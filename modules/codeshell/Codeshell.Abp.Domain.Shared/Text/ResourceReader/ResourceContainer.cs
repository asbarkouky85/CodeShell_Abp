﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Codeshell.Abp.Text.ResourceReader;

namespace Codeshell.Abp.Text.ResourceReader
{
    [XmlRoot(ElementName ="root")]
    public class ResourceContainer
    {
        [XmlElement(ElementName = "resheader")]
        public ResHeaderItem[] Headers { get; set; }
        [XmlElement(ElementName = "data")]
        public DataItem[] DataItems { get; set; } = new DataItem[0];

    }
}
