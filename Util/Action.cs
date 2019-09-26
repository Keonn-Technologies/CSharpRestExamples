using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Util
{
    public class Action {
		public String id;
		private String type;

		public enum Actions {
			MULTI_GPO_ACTION, GPO_ACTION, SPEAKER_ACTION, BUZZER_ACTION
		};

		public int total, ton, toff;

		public Action(String id, int total, int ton, int toff) {
			this.id = id;
			this.total = total;
			this.ton = ton;
			this.toff = toff;
			this.type = "none";
		}

		public String getType() {
			return this.type;
		}

		public XmlElement toXMLElement(XmlDocument doc) {
			return null;
		}
	}
    
	public class Buzzer : Action {

		public Buzzer(int total, int ton, int toff) : base(Actions.BUZZER_ACTION.ToString(), total, ton, toff)
        {
		}

        /// <summary>
        /// Creates the XML definition for the Buzzer Action
        /// </summary>
        /// <param name="doc">The XmlDocument to create the XMLElement</param>
        /// <returns>An XMLElement with the definition for the Buzzer Action</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
		public XmlElement toXMLElement(XmlDocument doc) {
			XmlElement action = doc.CreateElement("action");
			XmlElement id = doc.CreateElement("id");
			id.AppendChild(doc.CreateTextNode(this.id));
			action.AppendChild(id);
            
			XmlElement total = doc.CreateElement("total");
			total.AppendChild(doc.CreateTextNode(Convert.ToString(this.total)));
			action.AppendChild(total);

			XmlElement ton = doc.CreateElement("ton");
			ton.AppendChild(doc.CreateTextNode(Convert.ToString(this.ton)));
			action.AppendChild(ton);

			XmlElement toff = doc.CreateElement("toff");
			toff.AppendChild(doc.CreateTextNode(Convert.ToString(this.toff)));
			action.AppendChild(toff);

			return action;
		}
	}

	public class Speaker : Action {
		public int freq, volume;

		public Speaker(int total, int ton, int toff, int freq, int volume) : 
            base(Actions.SPEAKER_ACTION.ToString(), total, ton, toff) 
        {
			this.freq = freq;
			this.volume = volume;
		}

        /// <summary>
        /// Creates the XML definition for the Buzzer Action
        /// </summary>
        /// <param name="doc">The XmlDocument to create the XMLElement</param>
        /// <returns>An XMLElement with the definition for the Buzzer Action</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
		public XmlElement toXMLElement(XmlDocument doc) {
			XmlElement action = doc.CreateElement("action");
			XmlElement id = doc.CreateElement("id");
			id.AppendChild(doc.CreateTextNode(this.id));
			action.AppendChild(id);

			XmlElement total = doc.CreateElement("total");
			total.AppendChild(doc.CreateTextNode(Convert.ToString(this.total)));
			action.AppendChild(total);

			XmlElement ton = doc.CreateElement("ton");
			ton.AppendChild(doc.CreateTextNode(Convert.ToString(this.ton)));
			action.AppendChild(ton);

			XmlElement toff = doc.CreateElement("toff");
			toff.AppendChild(doc.CreateTextNode(Convert.ToString(this.toff)));
			action.AppendChild(toff);

			XmlElement freq = doc.CreateElement("freq");
			freq.AppendChild(doc.CreateTextNode(Convert.ToString(this.freq)));
			action.AppendChild(freq);

			XmlElement volume = doc.CreateElement("volume");
			volume.AppendChild(doc.CreateTextNode(Convert.ToString(this.volume)));
			action.AppendChild(volume);

			return action;
		}
	}

	public class GPOAction : Action {
		public int line;
		public bool high;

		public GPOAction(int total, int ton, int toff, int line, bool high) 
            : base(Actions.GPO_ACTION.ToString(), total, ton, toff)
        {
			this.line = line;
			this.high = high;
		}

        /// <summary>
        /// Creates the XML definition for the Buzzer Action
        /// </summary>
        /// <param name="doc">The XmlDocument to create the XMLElement</param>
        /// <returns>An XMLElement with the definition for the Buzzer Action</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
		public XmlElement toXMLElement(XmlDocument doc) {
			XmlElement action = doc.CreateElement("action");
			XmlElement id = doc.CreateElement("id");
			id.AppendChild(doc.CreateTextNode(this.id));
			action.AppendChild(id);

			XmlElement total = doc.CreateElement("total");
			total.AppendChild(doc.CreateTextNode(Convert.ToString(this.total)));
			action.AppendChild(total);

			XmlElement ton = doc.CreateElement("ton");
			ton.AppendChild(doc.CreateTextNode(Convert.ToString(this.ton)));
			action.AppendChild(ton);

			XmlElement toff = doc.CreateElement("toff");
			toff.AppendChild(doc.CreateTextNode(Convert.ToString(this.toff)));
			action.AppendChild(toff);

			XmlElement line = doc.CreateElement("line");
			line.AppendChild(doc.CreateTextNode(Convert.ToString(this.line)));
			action.AppendChild(line);

			XmlElement high = doc.CreateElement("high");
			high.AppendChild(doc.CreateTextNode(Convert.ToString(this.high)));
			action.AppendChild(high);

			return action;
		}
	}

	public class MultiGPOAction : Action {
		public int line;
		public bool high;
		public String lines;

        public MultiGPOAction(int total, int ton, int toff, bool high, String lines)
            : base(Actions.MULTI_GPO_ACTION.ToString(), total, ton, toff)
        {
			this.line = 1;
			this.high = high;
			this.lines = lines;
		}

        /// <summary>
        /// Creates the XML definition for the Buzzer Action
        /// </summary>
        /// <param name="doc">The XmlDocument to create the XMLElement</param>
        /// <returns>An XMLElement with the definition for the Buzzer Action</returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
		public XmlElement toXMLElement(XmlDocument doc) {
			XmlElement action = doc.CreateElement("action");
			XmlElement id = doc.CreateElement("id");
			id.AppendChild(doc.CreateTextNode(this.id));
			action.AppendChild(id);

			XmlElement total = doc.CreateElement("total");
			total.AppendChild(doc.CreateTextNode(Convert.ToString(this.total)));
			action.AppendChild(total);

			XmlElement ton = doc.CreateElement("ton");
			ton.AppendChild(doc.CreateTextNode(Convert.ToString(this.ton)));
			action.AppendChild(ton);

			XmlElement toff = doc.CreateElement("toff");
			toff.AppendChild(doc.CreateTextNode(Convert.ToString(this.toff)));
			action.AppendChild(toff);

			XmlElement line = doc.CreateElement("line");
			line.AppendChild(doc.CreateTextNode(Convert.ToString(this.line)));
			action.AppendChild(line);

			XmlElement high = doc.CreateElement("high");
			high.AppendChild(doc.CreateTextNode(Convert.ToString(this.high)));
			action.AppendChild(high);

			XmlElement lines = doc.CreateElement("lines");
			lines.AppendChild(doc.CreateTextNode(this.lines));
			action.AppendChild(lines);

			return action;
		}
	}
}
