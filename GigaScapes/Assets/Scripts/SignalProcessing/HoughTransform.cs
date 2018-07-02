﻿using UnityEngine;
using System.Collections;
using System.Text;
//using Emgu.cv;

public class NewBehaviourScript1 : MonoBehaviour
{

	//void thing()
	//{
	//	StringBuilder msgBuilder = new StringBuilder("Performance: ");

	//	//Load the image from file and resize it for display
	//	Image<Bgr, Byte> img =
	//	   new Image<Bgr, byte>(fileNameTextBox.Text)
	//	   .Resize(400, 400, Emgu.CV.CvEnum.Inter.Linear, true);

	//	//Convert the image to grayscale and filter out the noise
	//	UMat uimage = new UMat();
	//	CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

	//	//use image pyr to remove noise
	//	UMat pyrDown = new UMat();
	//	CvInvoke.PyrDown(uimage, pyrDown);
	//	CvInvoke.PyrUp(pyrDown, uimage);

	//	//Image<Gray, Byte> gray = img.Convert<Gray, Byte>().PyrDown().PyrUp();

	//	#region circle detection
	//	Stopwatch watch = Stopwatch.StartNew();
	//	double cannyThreshold = 180.0;
	//	double circleAccumulatorThreshold = 120;
	//	CircleF[] circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);

	//	watch.Stop();
	//	msgBuilder.Append(String.Format("Hough circles - {0} ms; ", watch.ElapsedMilliseconds));
	//	#endregion

	//	#region Canny and edge detection
	//	watch.Reset(); watch.Start();
	//	double cannyThresholdLinking = 120.0;
	//	UMat cannyEdges = new UMat();
	//	CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

	//	LineSegment2D[] lines = CvInvoke.HoughLinesP(
	//	   cannyEdges,
	//	   1, //Distance resolution in pixel-related units
	//	   Math.PI / 45.0, //Angle resolution measured in radians.
	//	   20, //threshold
	//	   30, //min Line width
	//	   10); //gap between lines

	//	watch.Stop();
	//	msgBuilder.Append(String.Format("Canny & Hough lines - {0} ms; ", watch.ElapsedMilliseconds));
	//	#endregion

	//	#region Find triangles and rectangles
	//	watch.Reset(); watch.Start();
	//	List<Triangle2DF> triangleList = new List<Triangle2DF>();
	//	List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle

	//	using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
	//	{
	//		CvInvoke.FindContours(cannyEdges, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
	//		int count = contours.Size;
	//		for (int i = 0; i < count; i++)
	//		{
	//			using (VectorOfPoint contour = contours[i])
	//			using (VectorOfPoint approxContour = new VectorOfPoint())
	//			{
	//				CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
	//				if (CvInvoke.ContourArea(approxContour, false) > 250) //only consider contours with area greater than 250
	//				{
	//					if (approxContour.Size == 3) //The contour has 3 vertices, it is a triangle
	//					{
	//						Point[] pts = approxContour.ToArray();
	//						triangleList.Add(new Triangle2DF(
	//						   pts[0],
	//						   pts[1],
	//						   pts[2]
	//						   ));
	//					}
	//					else if (approxContour.Size == 4) //The contour has 4 vertices.
	//					{
	//						#region determine if all the angles in the contour are within [80, 100] degree
	//						bool isRectangle = true;
	//						Point[] pts = approxContour.ToArray();
	//						LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

	//						for (int j = 0; j < edges.Length; j++)
	//						{
	//							double angle = Math.Abs(
	//							   edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
	//							if (angle < 80 || angle > 100)
	//							{
	//								isRectangle = false;
	//								break;
	//							}
	//						}
	//						#endregion

	//						if (isRectangle) boxList.Add(CvInvoke.MinAreaRect(approxContour));
	//					}
	//				}
	//			}
	//		}
	//	}

	//	watch.Stop();
	//	msgBuilder.Append(String.Format("Triangles & Rectangles - {0} ms; ", watch.ElapsedMilliseconds));
	//	#endregion

	//	originalImageBox.Image = img;
	//	this.Text = msgBuilder.ToString();

	//	#region draw triangles and rectangles
	//	Image<Bgr, Byte> triangleRectangleImage = img.CopyBlank();
	//	foreach (Triangle2DF triangle in triangleList)
	//		triangleRectangleImage.Draw(triangle, new Bgr(Color.DarkBlue), 2);
	//	foreach (RotatedRect box in boxList)
	//		triangleRectangleImage.Draw(box, new Bgr(Color.DarkOrange), 2);
	//	triangleRectangleImageBox.Image = triangleRectangleImage;
	//	#endregion

	//	#region draw circles
	//	Image<Bgr, Byte> circleImage = img.CopyBlank();
	//	foreach (CircleF circle in circles)
	//		circleImage.Draw(circle, new Bgr(Color.Brown), 2);
	//	circleImageBox.Image = circleImage;
	//	#endregion

	//	#region draw lines
	//	Image<Bgr, Byte> lineImage = img.CopyBlank();
	//	foreach (LineSegment2D line in lines)
	//		lineImage.Draw(line, new Bgr(Color.Green), 2);
	//	lineImageBox.Image = lineImage;
	//	#endregion
	//}
}
