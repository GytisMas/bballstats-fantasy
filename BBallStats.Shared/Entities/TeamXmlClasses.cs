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
    }

    [XmlRoot(ElementName = "clubs")]
    public class Clubs
    {

        [XmlElement(ElementName = "club")]
        public List<Club> ClubsList;
    }


}
