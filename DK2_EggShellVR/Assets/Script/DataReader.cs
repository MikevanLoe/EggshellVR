using System;
using System.IO;
using System.Text;

public static class DataReader
{
	public static string GetAllText(string filename)
	{
		string data = "";
		try
		{
			string line;
			StreamReader theReader = new StreamReader(filename, Encoding.Default);
			using (theReader)
			{
				// While there's lines left in the text file, do this:
				do
				{
					line = theReader.ReadLine();
					
					if (line != null)
					{
						data += line;
					}
				}
				while (line != null);
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
			}
		}
		catch (System.Exception e)
		{
			throw e;
		}
		return data;
	}
}