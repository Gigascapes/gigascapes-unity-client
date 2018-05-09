using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

using System;

[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
public struct LidarDataB
{
    [MarshalAs(UnmanagedType.Bool)]
    public bool syncBit;
    [MarshalAs(UnmanagedType.R4)]
    public float theta;
    [MarshalAs(UnmanagedType.Bool)]
    public float distant;
    [MarshalAs(UnmanagedType.SysUInt)]
    public uint quality;
};

public class RplidarBindingB
{

    [DllImport("RplidarCpp2.dll")]
    public static extern int OnConnectS(string port);
    [DllImport("RplidarCpp2.dll")]
    public static extern bool OnDisconnectS();

    [DllImport("RplidarCpp2.dll")]
    public static extern bool StartMotorS();
    [DllImport("RplidarCpp2.dll")]
    public static extern bool EndMotorS();

    [DllImport("RplidarCpp2.dll")]
    public static extern bool StartScanS();
    [DllImport("RplidarCpp2.dll")]
    public static extern bool EndScanS();

    [DllImport("RplidarCpp2.dll")]
    public static extern bool ReleaseDriveS();

    [DllImport("RplidarCpp2.dll")]
    public static extern int GetLDataSizeS();

    [DllImport("RplidarCpp2.dll")]
    private static extern void GetLDataSampleArray(IntPtr ptr);

    [DllImport("RplidarCpp2.dll")]
    private static extern int GrabDataS(IntPtr ptr);

    public static LidarData[] GetSampleData()
    {
        var d = new LidarData[2];
        var handler = GCHandle.Alloc(d, GCHandleType.Pinned);
        GetLDataSampleArray(handler.AddrOfPinnedObject());
        handler.Free();
        return d;
    }

    public static int GetDataS(ref LidarData[] data)
    {
        var handler = GCHandle.Alloc(data, GCHandleType.Pinned);
        int count = GrabDataS(handler.AddrOfPinnedObject());
        handler.Free();

        return count;
    }
}

public class RplidarBinding1B
{

	[DllImport("RplidarCpp2.dll")]
	public static extern int OnConnectD(string port);
	[DllImport("RplidarCpp2.dll")]
	public static extern bool OnDisconnectD();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool StartMotorD();
	[DllImport("RplidarCpp2.dll")]
	public static extern bool EndMotorD();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool StartScanD();
	[DllImport("RplidarCpp2.dll")]
	public static extern bool EndScanD();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool ReleaseDriveD();

	[DllImport("RplidarCpp2.dll")]
	public static extern int GetLDataSize();

	[DllImport("RplidarCpp2.dll")]
	private static extern void GetLDataSampleArray(IntPtr ptr);

	[DllImport("RplidarCpp2.dll")]
	private static extern int GrabDataD(IntPtr ptr);

	public static LidarData[] GetSampleData()
	{
		var d = new LidarData[2];
		var handler = GCHandle.Alloc(d, GCHandleType.Pinned);
		GetLDataSampleArray(handler.AddrOfPinnedObject());
		handler.Free();
		return d;
	}

	public static int GetDataD(ref LidarData[] data)
	{
		var handler = GCHandle.Alloc(data, GCHandleType.Pinned);
		int count = GrabDataD(handler.AddrOfPinnedObject());
		handler.Free();

		return count;
	}
}

public class RplidarBinding2B
{

	[DllImport("RplidarCpp2.dll")]
	public static extern int OnConnectC(string port);
	[DllImport("RplidarCpp2.dll")]
	public static extern bool OnDisconnectC();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool StartMotorC();
	[DllImport("RplidarCpp2.dll")]
	public static extern bool EndMotorC();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool StartScanC();
	[DllImport("RplidarCpp2.dll")]
	public static extern bool EndScanC();

	[DllImport("RplidarCpp2.dll")]
	public static extern bool ReleaseDriveC();

	[DllImport("RplidarCpp2.dll")]
	public static extern int GetLDataSize();

	[DllImport("RplidarCpp2.dll")]
	private static extern void GetLDataSampleArray(IntPtr ptr);

	[DllImport("RplidarCpp2.dll")]
	private static extern int GrabDataC(IntPtr ptr);

	public static LidarData[] GetSampleData()
	{
		var d = new LidarData[2];
		var handler = GCHandle.Alloc(d, GCHandleType.Pinned);
		GetLDataSampleArray(handler.AddrOfPinnedObject());
		handler.Free();
		return d;
	}

	public static int GetDataC(ref LidarData[] data)
	{
		var handler = GCHandle.Alloc(data, GCHandleType.Pinned);
		int count = GrabDataC(handler.AddrOfPinnedObject());
		handler.Free();

		return count;
	}
}
