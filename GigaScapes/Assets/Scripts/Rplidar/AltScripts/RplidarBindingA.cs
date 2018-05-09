using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

using System;

[StructLayout(LayoutKind.Sequential,CharSet = CharSet.Ansi)]
public struct LidarDataA
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

public class RplidarBindingA
{

    [DllImport("RplidarCpp3.dll")]
    public static extern int OnConnectS(string port);
    [DllImport("RplidarCpp3.dll")]
    public static extern bool OnDisconnectS();

    [DllImport("RplidarCpp3.dll")]
    public static extern bool StartMotorS();
    [DllImport("RplidarCpp3.dll")]
    public static extern bool EndMotorS();

    [DllImport("RplidarCpp3.dll")]
    public static extern bool StartScanS();
    [DllImport("RplidarCpp3.dll")]
    public static extern bool EndScanS();

    [DllImport("RplidarCpp3.dll")]
    public static extern bool ReleaseDriveS();

    [DllImport("RplidarCpp3.dll")]
    public static extern int GetLDataSize();

    [DllImport("RplidarCpp3.dll")]
    private static extern void GetLDataSampleArray(IntPtr ptr);

    [DllImport("RplidarCpp3.dll")]
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

public class RplidarBinding1
{

	[DllImport("RplidarCpp3.dll")]
	public static extern int OnConnectD(string port);
	[DllImport("RplidarCpp3.dll")]
	public static extern bool OnDisconnectD();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool StartMotorD();
	[DllImport("RplidarCpp3.dll")]
	public static extern bool EndMotorD();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool StartScanD();
	[DllImport("RplidarCpp3.dll")]
	public static extern bool EndScanD();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool ReleaseDriveD();

	[DllImport("RplidarCpp3.dll")]
	public static extern int GetLDataSize();

	[DllImport("RplidarCpp3.dll")]
	private static extern void GetLDataSampleArray(IntPtr ptr);

	[DllImport("RplidarCpp3.dll")]
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

public class RplidarBinding2
{

	[DllImport("RplidarCpp3.dll")]
	public static extern int OnConnectC(string port);
	[DllImport("RplidarCpp3.dll")]
	public static extern bool OnDisconnectC();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool StartMotorC();
	[DllImport("RplidarCpp3.dll")]
	public static extern bool EndMotorC();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool StartScanC();
	[DllImport("RplidarCpp3.dll")]
	public static extern bool EndScanC();

	[DllImport("RplidarCpp3.dll")]
	public static extern bool ReleaseDriveC();

	[DllImport("RplidarCpp3.dll")]
	public static extern int GetLDataSize();

	[DllImport("RplidarCpp3.dll")]
	private static extern void GetLDataSampleArray(IntPtr ptr);

	[DllImport("RplidarCpp3.dll")]
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
