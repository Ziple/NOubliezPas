using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kT.GUI
{
	/// <summary>
	/// Little enumeration of the different text styles.
	/// </summary>
	public enum TextStyle
	{
		EndLine = -1,
		Normal = 0,
		Italic = 1,
		Bold = 2
	}

	/// <summary>
	/// Base class to hold a text representation.
	/// </summary>
	public class TextString
	{
		string stringText = null;
        uint characterSize;

		public List<KeyValuePair<TextStyle, string>> formatedText = new List<KeyValuePair<TextStyle, string>>();

        public TextString(string text, uint size)
		{
			Text = text;
            characterSize = size;
		}

        public TextString(string text, TextStyle style, uint size)
        {
            characterSize = size;
            if( style == TextStyle.Bold )
                Text = "<b>" + text + "</b>";
            else if( style == TextStyle.Italic )
                Text = "<i>" + text + "</i>";
            else
                Text = text;
        }

		public string Text
		{
			get { return stringText; }
			set
			{
				stringText = value;

				// Format the text according to the style markers.
				string workString = stringText;

				formatedText.Clear();
				while (true)
				{
					int italicBegin = workString.IndexOf("<i>");
					int boldBegin = workString.IndexOf("<b>");
					// The string begins with italic text.
					if (italicBegin == 0)
					{
						int italicEnd = workString.IndexOf("</i>");

						if (italicEnd != -1)
						{
							formatedText.Add(
								new KeyValuePair<TextStyle, string>(
									TextStyle.Italic,
									workString.Substring(
										italicBegin + 3,
										italicEnd - italicBegin - 3)
									)
								);
							if (italicEnd + 4 < workString.Length)
								workString = workString.Substring(italicEnd + 4);
							else
								workString = "";
						}
					}
					// The string begins with bold text.
					else if (boldBegin == 0)
					{
						int boldEnd = workString.IndexOf("</b>");

						if (boldEnd != -1)
						{
							formatedText.Add(
								new KeyValuePair<TextStyle, string>(
									TextStyle.Bold,
									workString.Substring(
										boldBegin + 3,
										boldEnd - boldBegin - 3)
									)
								);

							if (boldEnd + 4 < workString.Length)
								workString = workString.Substring(boldEnd + 4);
							else
								workString = "";
						}
					}
					// There is some normal text at the beginning.
					else if ((boldBegin > 0 || italicBegin > 0)
						|| (boldBegin < 0 && italicBegin < 0))
					{
						int min = 0;

						if (boldBegin > 0 && italicBegin > 0)
							min = boldBegin < italicBegin ? boldBegin : italicBegin;
						else if (boldBegin > 0)
							min = boldBegin;
						else if (italicBegin > 0)
							min = italicBegin;
						else
							min = workString.Length;

						formatedText.Add(
							new KeyValuePair<TextStyle, string>(
								TextStyle.Normal,
								workString.Substring(0, min)
								)
							);
						workString = workString.Substring(min, workString.Length - min);
					}

					if (workString.Length == 0)
						break;
				}

				List<KeyValuePair<TextStyle, string>> realFormatedText = new List<KeyValuePair<TextStyle, string>>();
				// Detect the line ends.
				for (int i = 0; i < formatedText.Count; i++)
				{
					TextStyle style = formatedText[i].Key;
					string str = formatedText[i].Value;


					while (str.Contains("\n"))
					{
						int markPlace = str.IndexOf("\n");

						// some text before the marker.
						if (markPlace > 0)
						{
							realFormatedText.Add(new KeyValuePair<TextStyle, string>(style, str.Substring(0, markPlace)));
							str = str.Substring(markPlace);
						}
						else if (markPlace == 0)
						{
							realFormatedText.Add(new KeyValuePair<TextStyle, string>(TextStyle.EndLine, ""));
							if (2 < str.Length)
							{
								str = str.Substring(1);
							}
							else
								str = "";
						}
					}

					if (str != "")
						realFormatedText.Add(new KeyValuePair<TextStyle, string>(style, str));
				}
				formatedText = realFormatedText;
			}
		}

        public uint CharacterSize
        {
            get { return characterSize; }
        }

		public List<KeyValuePair<TextStyle, string>> FormatedText
		{
			get { return formatedText; }
		}
	}
}
