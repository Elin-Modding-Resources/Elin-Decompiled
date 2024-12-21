using System;
using System.Collections.Generic;

public class FastString
{
	private string m_stringGenerated = "";

	private bool m_isStringGenerated;

	private char[] m_buffer;

	private int m_bufferPos;

	private int m_charsCapacity;

	private List<char> m_replacement;

	private object m_valueControl;

	private int m_valueControlInt = int.MinValue;

	public FastString(int initialCapacity = 32)
	{
		m_buffer = new char[m_charsCapacity = initialCapacity];
	}

	public bool IsEmpty()
	{
		if (!m_isStringGenerated)
		{
			return m_bufferPos == 0;
		}
		return m_stringGenerated == null;
	}

	public override string ToString()
	{
		if (!m_isStringGenerated)
		{
			m_stringGenerated = new string(m_buffer, 0, m_bufferPos);
			m_isStringGenerated = true;
		}
		return m_stringGenerated;
	}

	public bool IsModified(int newControlValue)
	{
		bool num = newControlValue != m_valueControlInt;
		if (num)
		{
			m_valueControlInt = newControlValue;
		}
		return num;
	}

	public bool IsModified(object newControlValue)
	{
		bool num = !newControlValue.Equals(m_valueControl);
		if (num)
		{
			m_valueControl = newControlValue;
		}
		return num;
	}

	public void Set(string str)
	{
		Clear();
		Append(str);
		m_stringGenerated = str;
		m_isStringGenerated = true;
	}

	public void Set(object str)
	{
		Set(str.ToString());
	}

	public void Set<T1, T2>(T1 str1, T2 str2)
	{
		Clear();
		Append(str1);
		Append(str2);
	}

	public void Set<T1, T2, T3>(T1 str1, T2 str2, T3 str3)
	{
		Clear();
		Append(str1);
		Append(str2);
		Append(str3);
	}

	public void Set<T1, T2, T3, T4>(T1 str1, T2 str2, T3 str3, T4 str4)
	{
		Clear();
		Append(str1);
		Append(str2);
		Append(str3);
		Append(str4);
	}

	public void Set(params object[] str)
	{
		Clear();
		for (int i = 0; i < str.Length; i++)
		{
			Append(str[i]);
		}
	}

	public FastString Clear()
	{
		m_bufferPos = 0;
		m_isStringGenerated = false;
		return this;
	}

	public FastString Append(string value)
	{
		ReallocateIFN(value.Length);
		int length = value.Length;
		for (int i = 0; i < length; i++)
		{
			m_buffer[m_bufferPos + i] = value[i];
		}
		m_bufferPos += length;
		m_isStringGenerated = false;
		return this;
	}

	public FastString Append(object value)
	{
		Append(value.ToString());
		return this;
	}

	public FastString Append(int value)
	{
		ReallocateIFN(16);
		if (value < 0)
		{
			value = -value;
			m_buffer[m_bufferPos++] = '-';
		}
		int num = 0;
		do
		{
			m_buffer[m_bufferPos++] = (char)(48 + value % 10);
			value /= 10;
			num++;
		}
		while (value != 0);
		for (int num2 = num / 2 - 1; num2 >= 0; num2--)
		{
			char c = m_buffer[m_bufferPos - num2 - 1];
			m_buffer[m_bufferPos - num2 - 1] = m_buffer[m_bufferPos - num + num2];
			m_buffer[m_bufferPos - num + num2] = c;
		}
		m_isStringGenerated = false;
		return this;
	}

	public FastString Append(float valueF)
	{
		double num = valueF;
		m_isStringGenerated = false;
		ReallocateIFN(32);
		if (num == 0.0)
		{
			m_buffer[m_bufferPos++] = '0';
			return this;
		}
		if (num < 0.0)
		{
			num = 0.0 - num;
			m_buffer[m_bufferPos++] = '-';
		}
		int num2 = 0;
		while (num < 1000000.0)
		{
			num *= 10.0;
			num2++;
		}
		long num3 = (long)Math.Round(num);
		int num4 = 0;
		bool flag = true;
		while (num3 != 0L || num2 >= 0)
		{
			if (num3 % 10 != 0L || num2 <= 0)
			{
				flag = false;
			}
			if (!flag)
			{
				m_buffer[m_bufferPos + num4++] = (char)(48 + num3 % 10);
			}
			if (--num2 == 0 && !flag)
			{
				m_buffer[m_bufferPos + num4++] = '.';
			}
			num3 /= 10;
		}
		m_bufferPos += num4;
		for (int num5 = num4 / 2 - 1; num5 >= 0; num5--)
		{
			char c = m_buffer[m_bufferPos - num5 - 1];
			m_buffer[m_bufferPos - num5 - 1] = m_buffer[m_bufferPos - num4 + num5];
			m_buffer[m_bufferPos - num4 + num5] = c;
		}
		return this;
	}

	public FastString Replace(string oldStr, string newStr)
	{
		if (m_bufferPos == 0)
		{
			return this;
		}
		if (m_replacement == null)
		{
			m_replacement = new List<char>();
		}
		for (int i = 0; i < m_bufferPos; i++)
		{
			bool flag = false;
			if (m_buffer[i] == oldStr[0])
			{
				int j;
				for (j = 1; j < oldStr.Length && m_buffer[i + j] == oldStr[j]; j++)
				{
				}
				flag = j >= oldStr.Length;
			}
			if (flag)
			{
				i += oldStr.Length - 1;
				if (newStr != null)
				{
					for (int k = 0; k < newStr.Length; k++)
					{
						m_replacement.Add(newStr[k]);
					}
				}
			}
			else
			{
				m_replacement.Add(m_buffer[i]);
			}
		}
		ReallocateIFN(m_replacement.Count - m_bufferPos);
		for (int l = 0; l < m_replacement.Count; l++)
		{
			m_buffer[l] = m_replacement[l];
		}
		m_bufferPos = m_replacement.Count;
		m_replacement.Clear();
		m_isStringGenerated = false;
		return this;
	}

	private void ReallocateIFN(int nbCharsToAdd)
	{
		if (m_bufferPos + nbCharsToAdd > m_charsCapacity)
		{
			m_charsCapacity = Math.Max(m_charsCapacity + nbCharsToAdd, m_charsCapacity * 2);
			char[] array = new char[m_charsCapacity];
			m_buffer.CopyTo(array, 0);
			m_buffer = array;
		}
	}
}
