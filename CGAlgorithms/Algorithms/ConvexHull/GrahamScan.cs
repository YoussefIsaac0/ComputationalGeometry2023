using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            
            Stack<Point> EP = new Stack<Point>();
            if (points.Count() <= 3)
            {
                outPoints = points;
            }
            else
            {
                //get vertex
                double MinY = 10000;
                Point vertex = null;
                foreach (Point p in points)
                {
                    if (p.Y < MinY) { MinY = p.Y; vertex = p; } 
                    else if(p.Y == MinY) 
                    {
                        if (p.X < vertex.X)
                        {
                            vertex = p;
                        }
                    }

                }

                EP.Push(vertex);

                Line l1 = new Line(new Point(vertex.X-2, vertex.Y), vertex);
                Point vector1 = HelperMethods.GetVector(l1);
                List<KeyValuePair<double, Point>> sortedPoints = new List<KeyValuePair<double, Point>>();
                
                foreach(Point p in points)
                {
                    if (p.X != vertex.X || p.Y != vertex.Y)
                    {
                        Line l2 = new Line(vertex, p);
                        Point vector2 = HelperMethods.GetVector(l2);
                        double theta = CalculateAngle(vector1, vector2);
                        sortedPoints.Add(new KeyValuePair<double, Point>(theta, p));
                    }
                }

                sortedPoints.Sort(CompareByKeys);
                KeyValuePair<double, Point> init = sortedPoints[0];
                List<Point> finalpoints = new List<Point>();

                for(int i = 1; i < sortedPoints.Count; i++)
                {
                    if(init.Key == sortedPoints[i].Key)
                    {
                        double dis1 = calculate_distance(vertex, init.Value);
                        double dis2 = calculate_distance(vertex, sortedPoints[i].Value);
                        if (dis1 != 0 && dis2 != 0)
                        {
                            if (dis1 < dis2)
                            {
                                init = sortedPoints[i];
                            }
                            
                        }
                        if(i == sortedPoints.Count() - 1)
                        {
                            finalpoints.Add(init.Value);
                        }
                    }
                    else
                    {
                        finalpoints.Add(init.Value);
                        init = sortedPoints[i];
                        if (i == sortedPoints.Count()-1)
                        {
                            finalpoints.Add(sortedPoints[i].Value);
                        }
                    }
                }


                foreach(Point dd in finalpoints)
                {
                    bool found = false;
                    if(dd == sortedPoints[0].Value)
                    {
                            EP.Push(dd);                        
                    }
                    else
                    {
                        while (!found &&EP.Count()>0) {
                            Point top = EP.Pop();
                            Point prev = null;
                            if (EP.Count() > 0)
                            {
                                prev = EP.Peek();
                            }
                            else
                            {
                                prev = l1.Start;
                            }
                            Line newline = new Line(prev, top);
                            if(HelperMethods.CheckTurn(newline, dd) == Enums.TurnType.Left)
                            {
                                EP.Push(top);
                                EP.Push(dd);
                                found = true;
                            }
                            else if(HelperMethods.CheckTurn(newline, dd) == Enums.TurnType.Colinear)
                            {
                                found = true;
                                double dis1 = calculate_distance(vertex, dd);
                                double dis2 = calculate_distance(vertex, top);
                                if (dis1 != 0 && dis2 != 0)
                                {
                                    if (dis1 > dis2)
                                    {
                                        EP.Push(dd);
                                    }
                                    else
                                    {
                                        EP.Push(top);
                                    }
                                }
                            }
                           
                        }
                    }
                }
                outPoints = EP.ToList();
            }
        }
        public double CalculateAngle(Point vector1, Point vector2)
        {
            double dot_product = vector1.X * vector2.X + vector1.Y * vector2.Y;
            double mag1 = vector1.Magnitude();
            double mag2 = vector2.Magnitude();
            double costheta = dot_product / (mag1 * mag2);
            double thetaRad = Math.Acos(costheta);
            double thetadeg = thetaRad * (180.0 / Math.PI);

            return thetadeg;
        }
        static int CompareByKeys(KeyValuePair<double, Point> p1, KeyValuePair<double, Point> p2)
        {
            return p1.Key.CompareTo(p2.Key);
        }
        public double calculate_distance(Point A, Point B)
        {
            //sqrt((x1-x0)^2+(y1-y0)^2)
            double deltaX = B.X - A.X;
            double deltaY = B.Y - A.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
