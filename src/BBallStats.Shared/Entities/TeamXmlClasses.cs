using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BBallStats.Shared.Entities
{
    [XmlRoot(ElementName = "club")]
    public class Club
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code;

        [XmlElement(ElementName = "clubname")]
        public string Clubname;

        [XmlElement(ElementName = "roster")]
        public XmlRoster Roster { get; set; }
    }

    [XmlRoot(ElementName = "player")]
    public class XmlPlayer
    {

        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
    }

    [XmlRoot(ElementName = "roster")]
    public class XmlRoster
    {

        [XmlElement(ElementName = "player")]
        public List<XmlPlayer> Players { get; set; }
    }

    [XmlRoot(ElementName = "clubs")]
    public class Clubs
    {

        [XmlElement(ElementName = "club")]
        public List<Club> ClubsList;
    }


}
