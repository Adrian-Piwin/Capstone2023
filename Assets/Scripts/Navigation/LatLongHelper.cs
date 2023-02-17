using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LatLongHelper
{   
    

    /// <summary>
    /// Calculate the distance between to lat-lon coordinates in meters.
    /// </summary>
    /// <param name="loc1">LatLon coordinates of first location</param>
    /// <param name="loc2">LatLon coordinates of second location</param>
    /// <returns>Distance between the 2 inputs in meters</returns>
    public static double DistanceBetween(double loc1_long, double loc1_lat, double loc2_long, double loc2_lat)
    {
        var cos = Math.Cos(loc1_lat * Math.PI / 180);
		var cos2 = 2 * cos * cos - 1;
		var cos3 = 2 * cos * cos2 - cos;
		var cos4 = 2 * cos * cos3 - cos2;
		var cos5 = 2 * cos * cos4 - cos3;

        // km
        // factor = 1.0d;
        // Meters
        double factor = 1000.0d;
        double _kx = factor * (111.41513 * cos - 0.09455 * cos3 + 0.00012 * cos5);
	    double _ky = factor * (111.13209 - 0.56605 * cos2 + 0.0012 * cos4);
    
        var dx = (loc1_long - loc2_long) * _kx;
		var dy = (loc1_lat - loc2_lat) * _ky;
		return Math.Sqrt(dx * dx + dy * dy);
    }
}
