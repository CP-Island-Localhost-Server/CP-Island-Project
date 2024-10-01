using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ByteReader
{
	private byte[] mBuffer;

	private int mOffset = 0;

	private static BetterList<string> mTemp = new BetterList<string>();

	public bool canRead
	{
		get
		{
			return mBuffer != null && mOffset < mBuffer.Length;
		}
	}

	public ByteReader(byte[] bytes)
	{
		mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		mBuffer = asset.bytes;
	}

	public static ByteReader Open(string path)
	{
		FileStream fileStream = File.OpenRead(path);
		if (fileStream != null)
		{
			fileStream.Seek(0L, SeekOrigin.End);
			byte[] array = new byte[fileStream.Position];
			fileStream.Seek(0L, SeekOrigin.Begin);
			fileStream.Read(array, 0, array.Length);
			fileStream.Close();
			return new ByteReader(array);
		}
		return null;
	}

	private static string ReadLine(byte[] buffer, int start, int count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	public string ReadLine()
	{
		return ReadLine(true);
	}

    public string ReadLine(bool skipEmptyLines)
    {
        string str;
        int length = (int)this.mBuffer.Length;
        if (skipEmptyLines)
        {
            while (true)
            {
                if ((this.mOffset >= length ? true : this.mBuffer[this.mOffset] >= 32))
                {
                    break;
                }
                this.mOffset++;
            }
        }
        int num = this.mOffset;
        if (num >= length)
        {
            this.mOffset = length;
            str = null;
        }
        else
        {
            while (true)
            {
                if (num >= length)
                {
                    num++;
                    break;
                }
                else
                {
                    int num1 = num;
                    num = num1 + 1;
                    int num2 = this.mBuffer[num1];
                    if ((num2 == 10 ? true : num2 == 13))
                    {
                        break;
                    }
                }
            }
            string str1 = ByteReader.ReadLine(this.mBuffer, this.mOffset, num - this.mOffset - 1);
            this.mOffset = num;
            str = str1;
        }
        return str;
    }

    public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] separator = new char[1]
		{
			'='
		};
		while (canRead)
		{
			string text = ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				string[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 2)
				{
					string key = array[0].Trim();
					string text3 = dictionary[key] = array[1].Trim().Replace("\\n", "\n");
				}
			}
		}
		return dictionary;
	}

	public BetterList<string> ReadCSV()
	{
		mTemp.Clear();
		string text = "";
		bool flag = false;
		int num = 0;
		while (canRead)
		{
			if (flag)
			{
				string text2 = ReadLine(false);
				if (text2 == null)
				{
					return null;
				}
				text2 = text2.Replace("\\n", "\n");
				text = text + "\n" + text2;
				num++;
			}
			else
			{
				text = ReadLine(true);
				if (text == null)
				{
					return null;
				}
				text = text.Replace("\\n", "\n");
				num = 0;
			}
			int i = num;
			for (int length = text.Length; i < length; i++)
			{
				switch (text[i])
				{
				case ',':
					if (!flag)
					{
						mTemp.Add(text.Substring(num, i - num));
						num = i + 1;
					}
					break;
				case '"':
					if (flag)
					{
						if (i + 1 >= length)
						{
							mTemp.Add(text.Substring(num, i - num).Replace("\"\"", "\""));
							return mTemp;
						}
						if (text[i + 1] != '"')
						{
							mTemp.Add(text.Substring(num, i - num));
							flag = false;
							if (text[i + 1] == ',')
							{
								i++;
								num = i + 1;
							}
						}
						else
						{
							i++;
						}
					}
					else
					{
						num = i + 1;
						flag = true;
					}
					break;
				}
			}
			if (num < text.Length)
			{
				if (flag)
				{
					continue;
				}
				mTemp.Add(text.Substring(num, text.Length - num));
			}
			return mTemp;
		}
		return null;
	}
}
