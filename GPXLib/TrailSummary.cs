using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Xml.Serialization;
using System.Xml;

namespace GPXLib
{
	public class TrailSummary
	{
		private string _gpxPath;
		private GPXLib.wptType _highPoint = new GPXLib.wptType();
		private GPXLib.wptType _lowPoint = new GPXLib.wptType();
		private decimal _totalSpan; // the total span in meters from lowest to highest point
		private decimal _sectionSpan; // total span div. by 5 for each colored section
		private double _totalDistance; // total distance from start trackpoint to end trackpoint
		private double _totalElevChange; // total elevation change based on diff between each trackpoint
		private double _totalAscent;
		private GPXLib.gpxType _gpxDataSet;
		private List<GPXLib.wptType> _trackPointArray = new List<GPXLib.wptType>();
		private List<GPXLib.wptType> _waypointArray = new List<GPXLib.wptType>();
		//private ArrayList _comparePoints = new ArrayList();
		private string _elevLegend;
		//protected string gpxPath;

		/// <summary>
		/// 
		///  The following waypoint reference taken from Google maps, http://code.google.com/p/waypoints/wiki/Design
		/// 'lat' - Latitude (attribute) - The latitude of the point. Decimal degrees, WGS84 datum.
		/// 'lon' - Longitude (attribute) - The latitude of the point. Decimal degrees, WGS84 datum. Position Informatioin /// 
		/// 'ele' - Elevation (in meters) of the point. /// 
		/// 'time' - Creation/modification timestamp for element. Date and time in are in Univeral Coordinated Time (UTC), not local time! Conforms to ISO 8601 specification for date/time representation. Fractional seconds are allowed for millisecond timing in tracklogs. ///
		/// 'magvar' - Magnetic variation (in degrees) at the point /// 
		/// 'geoidheight' - Height (in meters) of geoid (mean sea level) above WGS84 earth ellipsoid. As defined in NMEA GGA message. Description Information /// 'name' - The GPS name of the waypoint. This field will be transferred to and from the GPS. GPX does not place restrictions on the length of this field or the characters contained in it. It is up to the receiving application to validate the field before sending it to the GPS. 
		/// 'cmt' - GPS waypoint comment. Sent to GPS as comment.
		/// 'desc' - A text description of the element. Holds additional information about the element intended for the user, not the GPS.
		/// 'src' - Source of data. Included to give user some idea of reliability and accuracy of data. "Garmin eTrex", "USGS quad Boston North", e.g. link - Link to additional information about the waypoint.
		/// 'url' - URL (GPX 1.0 only) /// 'urlname' - Name of URL (GPX 1.0 only)
		/// 'link' - Link to additional information about the waypoint. (GPX 1.1 only)
		/// 'sym' - Text of GPS symbol name. For interchange with other programs, use the exact spelling of the symbol as displayed on the GPS. If the GPS abbreviates words, spell them out.
		/// 'type' - Type (classification) of the waypoint. Accuracy Information
		/// 'fix' - Type of GPX fix.
		/// 'sat' - Number of satellites used to calculate the GPX fix.
		/// 'hdop' - Horizontal dilution of precision.
		/// 'vdop' - Vertical dilution of precision.
		/// 'pdop' - Position dilution of precision.
		/// 'ageofdgpsdata' - Number of seconds since last DGPS update.
		/// 'dgpsid' - ID of DGPS station used in differential correction. Routes Tracks Extensions (GPX 1.1 only) Version (not stored in SQL db) Creator (not stored in SQL db) 
		/// </summary>
		/// <param name="gpxPath"></param>
		public TrailSummary(string gpxPath)
		{
			this._gpxPath = gpxPath;
			GetGPXData();
		}
		/// <summary>
		/// Default constructor does nothing.
		/// </summary>
		public TrailSummary()
		{ }

		public virtual void GetGPXData()
		{
			if (System.IO.File.Exists(GPXPath))
			{

				XmlSerializer xs = new XmlSerializer(typeof(GPXLib.gpxType));
				XmlReader xr = XmlReader.Create(GPXPath);

				GPXLib.gpxType gpx = (GPXLib.gpxType)xs.Deserialize(xr);

				List<wptType> combinedTrackPoints = new List<wptType>();
				foreach (GPXLib.trkType trk in gpx.trk)
				{
					for (int i = 0; i < trk.trkseg.Length; i++)
					{
						GPXLib.trksegType trkseg = trk.trkseg[i];
						combinedTrackPoints.AddRange(trkseg.trkpt);
					}
				}

				decimal startPointLat = combinedTrackPoints.First().lat;
				decimal startPointLon = combinedTrackPoints.First().lon;
				decimal endPointLat = combinedTrackPoints.Last().lat;
				decimal endPointLon = combinedTrackPoints.Last().lon;

				// accumulated distance in miles from start
				double runDistFromStart = 0;
				// accumulated elevation change in meters from start
				double runElevChange = 0;

				// Get trackpoints
				for (int i = 0; i + 1 < combinedTrackPoints.Count(); i++)
				{
					// this is the current trackpoint
					GPXLib.wptType wrCurr = combinedTrackPoints[i];

					// this is the previous trackpoint, get ready to use it if needed
					GPXLib.wptType wrPrev;

					GPXLib.wptType tp = new GPXLib.wptType();
					tp.ele = Conversions.MetersToFeet(Convert.ToDecimal(wrCurr.ele));
					tp.lat = Convert.ToDecimal(wrCurr.lat);
					tp.lon = Convert.ToDecimal(wrCurr.lon);

                    if (wrCurr.time != null)
					{
						tp.time = Convert.ToDateTime(wrCurr.time);
					}
					if (i > 0)
					{
						// this is the previous trackpoint
						wrPrev = combinedTrackPoints[i - 1];

                        // accumulate the running distance from start
    					runDistFromStart += DistanceCalculator.Calc(Convert.ToDouble(wrPrev.lat), Convert.ToDouble(wrPrev.lon), Convert.ToDouble(wrCurr.lat), Convert.ToDouble(wrCurr.lon));

						// add to the running elevation change if ascent
						decimal rise = tp.ele - Conversions.MetersToFeet(wrPrev.ele);
						if (rise > 0)
						{
							runElevChange += (double)rise;
						}
					}

					_trackPointArray.Add(tp);
					// get high and low point while traversing the trackpoints
					if (_lowPoint.ele == -9999 ||
						_lowPoint == null ||
						_lowPoint.ele > tp.ele)
					{
						_lowPoint.ele = tp.ele;
					}
					if (_highPoint == null ||
						_highPoint.ele < tp.ele)
					{
						_highPoint.ele = tp.ele;
					}
				}

				this._totalAscent = runElevChange;
				this._totalDistance = runDistFromStart;
                if (gpx.wpt != null)
                {
                    this._waypointArray = gpx.wpt.ToList();
                }
			}
		}

		/// <summary>
		/// This method is used to find trackpoints closest to where pictures were taken. Returns nearest trackpoint in time AFTER the passed in time.
		/// </summary>
		/// <param name="myTime">The time to compare against trackpoint times.</param>
		/// <returns></returns>
		public GPXLib.wptType GetClosestTrackpoint(DateTime myTime)
		{
			long tmpPointDif = 999999999;
			long tmpPointDifCur = 0;
			//TrackPoint tmpTrackPoint = new TrackPoint();
			for (int j = 0; j < _trackPointArray.Count; j++)
			{
				GPXLib.wptType tp = (GPXLib.wptType)_trackPointArray[j];
				// run through the trackpoints until we find the one we're looking for
				if (tp.time.Ticks < myTime.Ticks)
				{
					tmpPointDifCur = Math.Abs(tp.time.Ticks - myTime.Ticks);
					if (tmpPointDifCur < tmpPointDif)
					{
						tmpPointDif = tmpPointDifCur;
						//tmpTrackPoint = tp;
					}
				}
				else if (j > 0)
				{
					return _trackPointArray[j - 1];
				}
			}
			return new GPXLib.wptType();
		}


		#region Public Accessor Methods

		public virtual string GPXPath
		{
			get { return _gpxPath; }
			set { _gpxPath = value; }
		}
		public GPXLib.wptType HighPoint
		{
			get { return _highPoint; }
			set { _highPoint = value; }
		}

		public GPXLib.wptType LowPoint
		{
			get { return _lowPoint; }
			set { _lowPoint = value; }
		}
		/// <summary>
		///  Elevation difference in meters from lowest to highest point (not accum. elev. change). Used for color coding the track on a map.
		/// </summary>
		public decimal TotalSpan
		{
			get { return _totalSpan; }
			set { _totalSpan = value; }
		}
		/// <summary>
		///  Elevation difference in meters for each section of the legend used when color coding a track on a map.
		/// </summary>
		public decimal SectionSpan
		{
			get { return _sectionSpan; }
			set { _sectionSpan = value; }
		}
		/// <summary>
		///  Total distance in miles from start to end of trail, based on total of distances between each trackpoint.
		/// </summary>
		public double TotalDistance
		{
			get { return _totalDistance; }
			set { _totalDistance = value; }
		}
		/// <summary>
		/// Total elevation change based on diff between each trackpoint
		/// </summary>
		public double TotalElevChange
		{
			get { return _totalElevChange; }
			set { _totalElevChange = value; }
		}
		public double TotalAscent
		{
			get { return _totalAscent; }
			set { _totalAscent = value; }
		}
		public virtual string ElevLegend
		{
			get { return _elevLegend; }
			set { _elevLegend = value; }
		}
		public virtual GPXLib.gpxType GPXDataSet
		{
			get { return _gpxDataSet; }
			set { _gpxDataSet = value; }
		}
		public virtual List<GPXLib.wptType> TrackPointArray
		{
			get { return _trackPointArray; }
			set { _trackPointArray = value; }
		}
		public virtual List<GPXLib.wptType> WaypointArray
		{
			get { return _waypointArray; }
			set { _waypointArray = value; }
		}
		#endregion
	}
}
