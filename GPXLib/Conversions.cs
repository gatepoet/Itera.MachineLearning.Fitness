using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPXLib
{
	/// <summary>
	/// Summary description for Conversions
	/// </summary>
	public class Conversions
	{
		public Conversions()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static decimal MetersToFeet(decimal meters)
		{
			return meters * (decimal)3.28083989501312;
		}

		public static double MetersToFeet(double meters)
		{
			return meters * (double)3.28083989501312;
		}
	}
}
