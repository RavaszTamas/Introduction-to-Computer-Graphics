using System;
using System.Collections.Generic;
using System.Drawing;
using MathSupport;
using OpenTK;
using Utilities;

namespace Scene3D
{
  public class Construction
  {
    #region Form initialization

    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out string param, out string tooltip)
    {
      name = "Ravasz Tamás";
      param = "bezier,angle=10000,depth=10,,seg=100000,n=8,d=7,scale=1,ratioX=2,ratioY=3,ratioZ=1,noRatioX=false,noRatioY=false,noRatioZ=false,r=1,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi";
      tooltip = "r=<radius>, seg=<segments>, kx,dx,ky,dy,kz,dz .. frequencies and phase shifts,\ne.g. coordinate x = r*cos(kx*t+dx)";
      //impoartant : rose,n=7,d=8,scale=2,ratioX=1,ratioY=1,ratioZ=1,noRatioX=false,noRatioY=false,noRatioZ=false,depth=1,r=1,seg=100000,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi,
      //rose,n=2,d=7,scale=2,ratioX=1,ratioY=3,ratioZ=1,noRatioX=true,noRatioY=false,noRatioZ=false,depth=7,r=1,seg=100000,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi, period=10.0, rad=2.0
      //spiral,angle=100,depth=10,n=8,d=7,scale=70000,ratioX=1,ratioY=1,ratioZ=1,noRatioX=false,noRatioY=false,noRatioZ=false,r=1,seg=100000,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi, 
      //spiral,angle=10000,depth=10000,,seg=10000,n=8,d=7,scale=6000,ratioX=1,ratioY=1,ratioZ=1,noRatioX=false,noRatioY=false,noRatioZ=false,r=1,kx=1.0,dx=1/2pi,ky=3/2,dy=1/2pi,kz=5/3,dz=pi, 
    }

    #endregion

    #region Instance data

    // !!! If you need any instance data, put them here..
    private bool tetrahedron = false;
    private bool octahedron = false;
    private bool koch = false;
    private bool rose = false;
    private bool noRatioX = false;
    private bool noRatioY = false;
    private bool noRatioZ = false;
    private bool spiral = false;
    private bool dragon = false;
    private bool bezier = false;

    private int depthParam = 1;
    private float radius = 1.0f;
    private double scale = 1.0;
    private double n = 1;
    private double d = 1;
    private double ratioX = 1.0;
    private double ratioY = 1.0;
    private double ratioZ = 1.0;
    private double paramAngle = Math.PI;
    private double kx = 1.0;
    private double dx = 0.0;
    private double ky = 1.0;
    private double dy = 0.0;
    private double kz = 1.0;
    private double dz = 0.0;
    private int segments = 1000;
    private double maxT = 2.0 * Math.PI;

    private void parseParams (string param)
    {
      // Defaults.
      radius = 1.0f;
      kx = 1.0;
      dx = 0.0;
      ky = 1.0;
      dy = 0.0;
      kz = 1.0;
      dz = 0.0;
      segments = 1000;

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // r=<double>
        Util.TryParse(p, "r", ref radius);

        // seg=<int>
        if (Util.TryParse(p, "seg", ref segments) &&
            segments < 1)
          segments = 1;

        // kx,dx,ky,dy,kz,dz .. frequencies and phase shifts.
        Util.TryParseRational(p, "kx", ref kx);
        Util.TryParseRational(p, "dx", ref dx);
        Util.TryParseRational(p, "ky", ref ky);
        Util.TryParseRational(p, "dy", ref dy);
        Util.TryParseRational(p, "kz", ref kz);
        Util.TryParseRational(p, "dz", ref dz);
        Util.TryParse(p, "bezier", ref bezier);
        Util.TryParse(p, "dragon", ref dragon);
        Util.TryParse(p, "tetrahedron", ref tetrahedron);
        Util.TryParse(p, "octahedron", ref octahedron);
        Util.TryParse(p, "koch", ref koch);
        Util.TryParse(p, "rose", ref rose);
        Util.TryParse(p, "spiral", ref spiral);
        Util.TryParse(p, "ratioZ", ref ratioZ);
        Util.TryParse(p, "noRatioZ", ref noRatioZ);
        Util.TryParse(p, "ratioX", ref ratioX);
        Util.TryParse(p, "noRatioX", ref noRatioX);
        Util.TryParse(p, "ratioY", ref ratioY);
        Util.TryParse(p, "noRatioY", ref noRatioY);
        if (Util.TryParse(p, "angle", ref paramAngle))
        {
          paramAngle = paramAngle * Math.PI / 180.0;
        }


        if (Util.TryParse(p, "scale", ref scale))
        {
          if (scale < 0)
            scale = 1;
        }
        if (Util.TryParse(p, "n", ref n))
        {
          if (n < 1)
            n = 1;
        }
        if (Util.TryParse(p, "d", ref d))
        {
          if (d < 1)
            d = 1;

        }
        Util.TryParse(p, "rose", ref rose);

        if (Util.TryParse(p, "depth", ref depthParam))
        {
          if (depthParam > 10 && (tetrahedron || koch))
            depthParam = 10;
        }

        // ... you can add more parameters here ...
      }

      // Estimate of upper bound for 't'.
      maxT = 2.0 * Math.PI / Arith.GCD(Arith.GCD(kx, ky), kz);
    }

    #endregion

    public Construction ()
    {
      // {{

      // }}
    }

    #region Mesh construction

    /// <summary>
    /// Construct a new Brep solid (preferebaly closed = regular one).
    /// </summary>
    /// <param name="scene">B-rep scene to be modified</param>
    /// <param name="m">Transform matrix (object-space to world-space)</param>
    /// <param name="param">Shape parameters if needed</param>
    /// <returns>Number of generated faces (0 in case of failure)</returns>
    public int AddMesh (SceneBrep scene, Matrix4 m, string param)
    {
      // {{ TODO: put your Mesh-construction code here

      parseParams(param);
      if (bezier)
      {
        return drawBezier(scene, m, param);

      }
      if (tetrahedron)
      {
        return drawTetrahedron(scene, m, param);
      }
      else if (koch)
      {
        return drawKoch(scene, m, param);
      }
      else if (rose)
      {
        return drawRose(scene, m, param);
      }
      else if (octahedron)
      {
        int ret = 0;
        double s = 0.0;
        double ds = 1.0/depthParam;
        double period = paramAngle;
        double dtheta = period / depthParam;
        for (int i = 1; i <= depthParam; ++i)
        {
          double theta = i * dtheta;
          System.Drawing.Color c = Raster.Draw.ColorRamp(0.5 *(s + 1.0));

          ret += drawOctahedron(scene, m, param, i, new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f), s, theta);
          s += ds;
        }
        return ret;
      }
      else if (spiral)
      {
        return drawSpiral(scene, m, param);
      }
      else if (dragon)
      {
        if (depthParam > 15)
          depthParam = 15;
        return dragonCurve(scene, m, param);
      }
      else
      {
        return drawDefault(scene, m, param);
      }



      // }}
    }

    private int drawLineBezier (SceneBrep scene, Matrix4 m, string param,Vector3 P0, Vector3 P1, Vector3 P2)
    {

      int segm = 200;

      float hun = 100.0f;


      List<int> pointsList = new List<int>();

      addCube(scene, m, param);


      //P0 -= delta; P1 -= delta;P2 -= delta;

      for (int i = 0; i <= 100; i++)
      {
        float t = ((float)(i))/hun;
        Vector3 P = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2);
        //float x = (1-t)*(1-t)*x1 + 2*(1-t)*t*x2+t*t*x3;
        //float y = (1-t)*(1-t)*y1 + 2*(1-t)*t*y2+t*t*y3;
        pointsList.Add(scene.AddVertex(Vector3.TransformPosition(P, m)));
      }

      for (int i = 0; i < pointsList.Count - 1; i++)
      {
        scene.AddLine(pointsList[i], pointsList[i + 1]);
      }

      int p0,p1,p2;
      p0 = scene.AddVertex(Vector3.TransformPosition(P0, m));
      p1 = scene.AddVertex(Vector3.TransformPosition(P1, m));
      p2 = scene.AddVertex(Vector3.TransformPosition(P2, m));

      scene.AddLine(p0, p1);
      scene.AddLine(p1, p2);

      return segm + 2;
    }

    private int drawBezier (SceneBrep scene, Matrix4 m, string param)
    {

      /*Vector3 delta;
      delta.X = 0.5f;
      delta.Y = 0.5f;
      delta.Z = 0.5f;*/
      int retval = 0;
      Vector3 P0,P1,P2;
      P0.X = -6;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = 0.5f;
      P1.Y = -1;
      P1.Z = -5;

      P2.X = 6;
      P2.Y = 0;
      P2.Z = 0;

      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = 6;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = 0.5f;
      P1.Y = -1;
      P1.Z = -5;

      P2.X = 0;
      P2.Y = 10;
      P2.Z = 0;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = 0;
      P0.Y = 10;
      P0.Z = 0;

      P1.X = 0.5f;
      P1.Y = -1;
      P1.Z = -5;

      P2.X = -6;
      P2.Y = 0;
      P2.Z = 0;


      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = -6;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = -2;
      P1.Y = -1;
      P1.Z = 1;

      P2.X = 0.5f;
      P2.Y = 0;
      P2.Z = 4;

      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = 0.5f;
      P0.Y = 0;
      P0.Z = 4;

      P1.X = 2;
      P1.Y = -1;
      P1.Z = 1;



      P2.X = 6;
      P2.Y = 0;
      P2.Z = 0;

      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = 6;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = 0;
      P1.Y = 15;
      P1.Z = 5;



      P2.X = 0;
      P2.Y = 10;
      P2.Z = 0;


      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = 0;
      P0.Y = 10;
      P0.Z = 0;

      P1.X = 0;
      P1.Y = 15;
      P1.Z = 5;



      P2.X = -6;
      P2.Y = 0;
      P2.Z = 0;


      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = 0;
      P0.Y = 10;
      P0.Z = 0;

      P1.X = 0.5f;
      P1.Y = -1;
      P1.Z = -5;



      P2.X = 0.5f;
      P2.Y = 0;
      P2.Z = 4;

      //positions.Add(new Vector3(0, 10, 0));
      //positions.Add(new Vector3(0.5f, -1, -5));
      //positions.Add(new Vector3(0.5f, 0, 4));
      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = 0.5f;
      P0.Y = 0;
      P0.Z = 4;


      P1.X = 0.5f;
      P1.Y = 20;
      P1.Z = 20;


      P2.X = 0;
      P2.Y = 10;
      P2.Z = 0;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);



      P0.X = 10;
      P0.Y = 0;
      P0.Z = 0;


      P1.X = 10;
      P1.Y = 0;
      P1.Z = 10;


      P2.X = 0;
      P2.Y = 0;
      P2.Z = 10;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = 0;
      P0.Y = 0;
      P0.Z = 10;


      P1.X = -10;
      P1.Y = 0;
      P1.Z = 10;


      P2.X = -10;
      P2.Y = 0;
      P2.Z = 0;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = -10;
      P0.Y = 0;
      P0.Z = 0;


      P1.X = -10;
      P1.Y = 0;
      P1.Z = -10;


      P2.X = 0;
      P2.Y = 0;
      P2.Z = -10;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);

      P0.X = 0;
      P0.Y = 0;
      P0.Z = -10;


      P1.X = 10;
      P1.Y = 0;
      P1.Z = -10;


      P2.X = 10;
      P2.Y = 0;
      P2.Z = 0;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);


      P0.X = 10;
      P0.Y = 0;
      P0.Z = 0;


      P1.X = 10;
      P1.Y = 100;
      P1.Z = 0;


      P2.X = -10;
      P2.Y = 0;
      P2.Z = 0;



      retval += drawLineBezier(scene, m, param, P0, P1, P2);



      return retval;

    }

    private void addCube (SceneBrep scene, Matrix4 m, string param)
    {

      Vector3 P0,P1,P2,P3;
      Vector3 D0,D1,D2,D3;
      P0.X = 0;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = 1;
      P1.Y = 0;
      P1.Z = 0;

      P2.X = 1;
      P2.Y = 1;
      P2.Z = 0;

      P3.X = 0;
      P3.Y = 1;
      P3.Z = 0;

      D0.X = 0;
      D0.Y = 0;
      D0.Z = 1;

      D1.X = 1;
      D1.Y = 0;
      D1.Z = 1;

      D2.X = 1;
      D2.Y = 1;
      D2.Z = 1;

      D3.X = 0;
      D3.Y = 1;
      D3.Z = 1;

      /*Vector3 delta;
      delta.X = 0.5f;
      delta.Y = 0.5f;
      delta.Z = 0.5f;

      P0 -= delta;P1 -= delta;P2 -= delta;P3 -= delta;
      D0 -= delta;
      D1 -= delta;
      D2 -= delta;
      D3 -= delta;*/

      int p0 = scene.AddVertex(Vector3.TransformPosition(P0,m));
      int p1 = scene.AddVertex(Vector3.TransformPosition(P1,m));
      int p2 = scene.AddVertex(Vector3.TransformPosition(P2,m));
      int p3 = scene.AddVertex(Vector3.TransformPosition(P3,m));

      int d0 = scene.AddVertex(Vector3.TransformPosition(D0,m));
      int d1 = scene.AddVertex(Vector3.TransformPosition(D1,m));
      int d2 = scene.AddVertex(Vector3.TransformPosition(D2,m));
      int d3 = scene.AddVertex(Vector3.TransformPosition(D3,m));

      scene.AddLine(p0, p1);
      scene.AddLine(p1, p2);
      scene.AddLine(p2, p3);
      scene.AddLine(p3, p0);

      scene.AddLine(d0, d1);
      scene.AddLine(d1, d2);
      scene.AddLine(d2, d3);
      scene.AddLine(d3, d0);

      scene.AddLine(p0, d0);
      scene.AddLine(p1, d1);
      scene.AddLine(p2, d2);
      scene.AddLine(p3, d3);


    }

    private int dragonCurve (SceneBrep scene, Matrix4 m, string param)
    {
      int retValue = 1;
      List<int> turnSequence = new List<int>();
      for (int i = 0; i < depthParam; i++)
      {
        var copy = new List<int>(turnSequence);
        copy.Reverse();
        turnSequence.Add(1);
        foreach (int turn in copy)
        {
          turnSequence.Add(-turn);
        }
      }
      bool increasez = false;
      int one, two;
      double z2;
      double startingAngle = -depthParam * (Math.PI / 4);
      double side = 400 / Math.Pow(2, depthParam / 2.0);
      double angle = startingAngle;
      int x1 = 230, y1 = 350;
      int x2 = x1 + (int)(Math.Cos(angle) * side);
      int y2 = y1 + (int)(Math.Sin(angle) * side);
      Vector3 A = new Vector3(x1,y1,0);
      one = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(x2,y2,0);
      two = scene.AddVertex(Vector3.TransformPosition(A, m));
      scene.AddLine(one, two);
      x1 = x2;
      y1 = y2;
      one = two;
      int turns = turnSequence.Count/100;
      int j = 0;
      z2 = 1;
      int direction  = 0;
      foreach (int turn in turnSequence)
      {
        angle += turn * (Math.PI / 2);
        x2 = x1 + (int)(Math.Cos(angle) * side);
        y2 = y1 + (int)(Math.Sin(angle) * side);
        if (j >= turns)
        {
          j = 0;
          increasez = !increasez;
          if(increasez)
          direction++;
        }
        if(increasez)
        {
          if(direction%2 == 0)
            z2 = z2 + Math.Abs((int)(Math.Cos(angle) * side));
          else
            z2 = z2 - Math.Abs((int)(Math.Cos(angle) * side));

        }

        A = new Vector3(x2, y2, (float)z2);
        two = scene.AddVertex(Vector3.TransformPosition(A, m));
        scene.AddLine(one, two);
        x1 = x2;
        y1 = y2;
        one = two;
        retValue++;
        j++;
      }
      return retValue;

    }

    private int drawOctahedron (SceneBrep scene, Matrix4 m, string param, float sc,Vector3 color,double s, double theta)
    {
      int one,two,three,four,five,six;
      float localScale = (float)sc;
      Vector4 row1 = new Vector4((float)Math.Cos(theta),(float)-Math.Sin(theta),0,0);
      Vector4 row2 = new Vector4((float)Math.Sin(theta),(float)Math.Cos(theta),0,0);
      Vector4 row3 = new Vector4(0,0,1,0);
      Vector4 row4 = new Vector4(0,0,0,1);

      m = new Matrix4(row1,row2,row3,row4);

      Vector3 A = new Vector3(localScale,0,0);
      one = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(0, localScale, 0);
      two = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(-localScale, 0, 0);
      three = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(0, -localScale, 0);
      four = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(0, 0, localScale);
      five = scene.AddVertex(Vector3.TransformPosition(A, m));
      A = new Vector3(0, 0, -localScale);
      six = scene.AddVertex(Vector3.TransformPosition(A, m));

      scene.SetTxtCoord(one, new Vector2((float)s, (float)s));
      scene.SetTxtCoord(two, new Vector2((float)s, (float)s));
      scene.SetTxtCoord(three, new Vector2((float)s, (float)s));
      scene.SetTxtCoord(four, new Vector2((float)s, (float)s));
      scene.SetTxtCoord(five, new Vector2((float)s, (float)s));
      scene.SetTxtCoord(six, new Vector2((float)s, (float)s));

      scene.SetColor(one, color);
      scene.SetColor(two, color);
      scene.SetColor(three, color);
      scene.SetColor(four, color);
      scene.SetColor(five, color);
      scene.SetColor(six, color);


      scene.AddLine(one, two);
      scene.AddLine(two, three);
      scene.AddLine(three, four);
      scene.AddLine(four, one);

      scene.AddLine(five, one);
      scene.AddLine(five, two);
      scene.AddLine(five, three);
      scene.AddLine(five, four);

      scene.AddLine(six, one);
      scene.AddLine(six, two);
      scene.AddLine(six, three);
      scene.AddLine(six, four);


      return 6;
    }

    private int drawSpiral(SceneBrep scene, Matrix4 m, string param)
    {
      int number_of_points = segments;

      double period = Math.PI * scale;
      double dtheta = period / number_of_points;
      List<int> points = new List<int>();
      double ds = 1.0 / (double)number_of_points;
      double s = 0.0;
      for (int i = 0; i < number_of_points; i++)
      {
        double theta = i * dtheta;
        float x = (float)(s*s*Math.Cos(theta*ratioX));
        float y = (float)(s*s*Math.Sin(theta*ratioY));
        float z = (float)(s*s);



        Vector3 A = new Vector3(x,y,z);
        int v = scene.AddVertex(Vector3.TransformPosition(A, m));

        scene.SetTxtCoord(v, new Vector2((float)s, (float)s));
        System.Drawing.Color c = Raster.Draw.ColorRamp(0.5 *(s + 1.0));
        scene.SetColor(v, new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));

        points.Add(v);
        s += ds;
      }

      for (int i = 0; i < number_of_points - 1; i++)
      {
        scene.AddLine(points[i], points[i + 1]);
      }
      //scene.AddLine(points[0], points[points.Count - 1]);
      return number_of_points;
    }

    private int drawRose (SceneBrep scene, Matrix4 m, string param)
    {
      int number_of_points = segments;
      double period = Math.PI * d;
      if ((n % 2 == 0) || (d % 2 == 0))
        period *= 2;

      double s = 0.0;
      double ds = 1.0/number_of_points;

      double dtheta = period / number_of_points;
      List<int> points = new List<int>();
      double k = (double)n / d;
      
      for (int i = 0; i < number_of_points; i++)
      {
        double theta = i * dtheta;
        double r = scale * Math.Cos(k * theta);
        float x = (float)(r * Math.Cos(theta*ratioX));
        float y = (float)(r * Math.Sin(theta*ratioY));
        float z = (float)(r * Math.Cos(theta*i*ratioZ));
        if (noRatioZ)
          z = (float)(r * Math.Cos(theta));
        if (noRatioX)
          x = (float)(r * Math.Cos(theta));
        if (noRatioY)
          y = (float)(r * Math.Sin(theta));
        
        
        Vector3 A = new Vector3(x,y,z);
        int v = scene.AddVertex(Vector3.TransformPosition(A, m));
        points.Add(v);
        scene.SetTxtCoord(v, new Vector2((float)s, (float)s));
        System.Drawing.Color c = Raster.Draw.ColorRamp(0.5 *(s + 1.0));
        scene.SetColor(v, new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));
        s += ds;

      }
        
      for (int i = 0; i < number_of_points -1; i++)
      {
        scene.AddLine(points[i], points[i + 1]);
      }
      scene.AddLine(points[0], points[points.Count-1]);
      return number_of_points;

    }

    private int drawKoch (SceneBrep scene, Matrix4 m, string param)
    {
      Vector3 A,B,C,D;
      int one,two,three,four;
      //float a = 1.0f;
      A.X = 0;
      A.Y = 0;
      A.Z = 0;
      one = scene.AddVertex(Vector3.TransformPosition(A, m));
      B.X = 1.0f;
      B.Y = 1.0f;
      B.Z = 0;
      two = scene.AddVertex(Vector3.TransformPosition(B, m));
      C.X = 0;
      C.Y = 1.0f;
      C.Z = 1.0f;
      three = scene.AddVertex(Vector3.TransformPosition(C, m));
      D.X = 1.0f;
      D.Y = 0;
      D.Z = 1.0f;
      four = scene.AddVertex(Vector3.TransformPosition(D, m));

      scene.AddLine(one, two);
      scene.AddLine(two, three);
      scene.AddLine(three, one);
      scene.AddLine(four, one);
      scene.AddLine(four, two);
      scene.AddLine(four, three);

      drawKochRecursive(depthParam, scene, m, param, A, B, C, D);

      return segments + (int)Math.Pow(4, depthParam) + 1;

    }

    private void drawKochRecursive (int depth, SceneBrep scene, Matrix4 m, string param, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
      if (depth == 0)
        return;

      Vector3 stand;
      stand.X = 0;
      stand.Y = 0;
      stand.Z = 0;
      int ab,ac,ad,bc,bd,cd,headabc,headadb,headacd,headbdc;
      Vector3 AB = stand,AC = stand,AD = stand;
      Vector3 BC = stand,BD = stand;
      Vector3 CD = stand;

      calculateMiddle(a, b, c, d, ref AB, ref AC, ref AD, ref BC, ref BD, ref CD);
      float lengthOfSide = (float)Math.Sqrt((AB.X-BC.X)*(AB.X-BC.X)+(AB.Y-BC.Y)*(AB.Y-BC.Y)+(AB.Z-BC.Z)*(AB.Z-BC.Z));
      float height = lengthOfSide*(float)Math.Sqrt(6) / 3.0f;

      Vector3 headABC = calculateHeightVertex(a,b,c,height);
      Vector3 headADB = calculateHeightVertex(a,d,b,height);
      Vector3 headACD = calculateHeightVertex(a,c,d,height);
      Vector3 headBDC = calculateHeightVertex(b,d,c,height);


      ab = scene.AddVertex(Vector3.TransformPosition(AB, m));
      ac = scene.AddVertex(Vector3.TransformPosition(AC, m));
      ad = scene.AddVertex(Vector3.TransformPosition(AD, m));
      bc = scene.AddVertex(Vector3.TransformPosition(BC, m));
      bd = scene.AddVertex(Vector3.TransformPosition(BD, m));
      cd = scene.AddVertex(Vector3.TransformPosition(CD, m));
      headabc = scene.AddVertex(Vector3.TransformPosition(headABC, m));
      headadb = scene.AddVertex(Vector3.TransformPosition(headADB, m));
      headacd = scene.AddVertex(Vector3.TransformPosition(headACD, m));
      headbdc = scene.AddVertex(Vector3.TransformPosition(headBDC, m));


      scene.AddLine(ab, ac);
      scene.AddLine(ac, bc);
      scene.AddLine(bc, ab);
      scene.AddLine(ab, headabc);
      scene.AddLine(ac, headabc);
      scene.AddLine(bc, headabc);


      scene.AddLine(ab, bd);
      scene.AddLine(bd, ad);
      scene.AddLine(ad, ab);
      scene.AddLine(ab, headadb);
      scene.AddLine(bd, headadb);
      scene.AddLine(ad, headadb);


      scene.AddLine(ac, ad);
      scene.AddLine(ad, cd);
      scene.AddLine(cd, ac);
      scene.AddLine(ac, headacd);
      scene.AddLine(ad, headacd);
      scene.AddLine(cd, headacd);

      
      scene.AddLine(bc, cd);
      scene.AddLine(cd, bd);
      scene.AddLine(bd, bc);
      scene.AddLine(bc, headbdc);
      scene.AddLine(bd, headbdc);
      scene.AddLine(cd, headbdc);
      
      drawKochRecursive(depth - 1, scene, m, param, AB, AC, BC, headABC);
      drawKochRecursive(depth - 1, scene, m, param, AB, BD, AD, headADB);
      drawKochRecursive(depth - 1, scene, m, param, AD, CD, AC, headACD);
      drawKochRecursive(depth - 1, scene, m, param, BD, BC, CD, headBDC);

    }

    private Vector3 calculateHeightVertex(Vector3 a, Vector3 b, Vector3 c, float height)
    {
      Vector3 stand;
      stand.X = 0;
      stand.Y = 0;
      stand.Z = 0;

      Vector3 norm = stand;
      norm = calculateNormalVetor(a, c, b);
      float lengthOfNorm = (float)Math.Sqrt(norm.X * norm.X + norm.Y * norm.Y + norm.Z * norm.Z);
      norm.X *= (1.0f / lengthOfNorm);
      norm.Y *= (1.0f / lengthOfNorm);
      norm.Z *= (1.0f / lengthOfNorm);
      Vector3 center = calculateCenter(a,b,c);
      Vector3 headABC;

      headABC.X = center.X + norm.X * height;
      headABC.Y = center.Y + norm.Y * height;
      headABC.Z = center.Z + norm.Z * height;

      return headABC;
    }

    private Vector3 calculateCenter (Vector3 a, Vector3 b, Vector3 c)
    {

      Vector3 result;
      result.X = a.X + b.X + c.X;
      result.X /= 3.0f;
      result.Y = a.Y + b.Y + c.Y;
      result.Y /= 3.0f;
      result.Z = a.Z + b.Z + c.Z;
      result.Z /= 3.0f;


      return result;
    }

    private Vector3 calculateNormalVetor (Vector3 a, Vector3 b, Vector3 c)
    {
      Vector3 result;

      float a1 = b.X - a.X;
      float b1 = b.Y - a.Y;
      float c1 = b.Z - a.Z;
      float a2 = c.X - a.X;
      float b2 = c.Y - a.Y;
      float c2 = c.Z - a.Z;

      float A = b1 * c2 - b2 * c1;
      float B = a2 * c1 - a1 * c2;
      float C = a1 * b2 - b1 * a2;

      result.X = A;
      result.Y = B;
      result.Z = C;
      return result;
    }

    private int drawDefault (SceneBrep scene, Matrix4 m, string param)
    {
      // If there will be large number of new vertices, reserve space for them to save time.
      scene.Reserve(segments + 1);

      double t = 0.0;
      double dt = maxT / segments;
      double s = 0.0;       // for both texture coordinate & color ramp
      double ds = 1.0 / segments;

      int vPrev = 0;
      Vector3 A;
      for (int i = 0; i <= segments; i++)
      {
        // New vertex's coordinates.
        A.X = (float)(radius * Math.Cos(kx * t + dx));
        A.Y = (float)(radius * Math.Cos(ky * t + dy));
        A.Z = (float)(radius * Math.Cos(kz * t + dz));

        // New vertex.
        int v = scene.AddVertex(Vector3.TransformPosition(A, m));

        // Vertex attributes.
        scene.SetTxtCoord(v, new Vector2((float)s, (float)s));
        System.Drawing.Color c = Raster.Draw.ColorRamp(0.5 *(s + 1.0));
        scene.SetColor(v, new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f));

        // New line?
        if (i > 0)
          scene.AddLine(vPrev, v);

        // Next vertex.
        t += dt;
        s += ds;
        vPrev = v;
      }

      // Thick line (for rendering).
      scene.LineWidth = 3.0f;

      return segments;
    }

    private int drawTetrahedron (SceneBrep scene, Matrix4 m, string param)
    {
      segments = 4;
      scene.Reserve(segments+(int)Math.Pow(4,depthParam)+1);
      /*c.Line(0, 0, 0, c.Height);
      c.Line(0, c.Height, c.Width, c.Height / 2);
      c.Line(c.Width, c.Height / 2, 0, 0);
      */
      Vector3 A,B,C,D;
      int one,two,three,four;
      //float a = 1.0f;
      A.X = 0;
      A.Y = 0;
      A.Z = 0;
      one = scene.AddVertex(Vector3.TransformPosition(A, m));
      B.X = 1.0f;
      B.Y = 1.0f;
      B.Z = 0;
      two = scene.AddVertex(Vector3.TransformPosition(B, m));
      C.X = 0;
      C.Y = 1.0f;
      C.Z = 1.0f;
      three = scene.AddVertex(Vector3.TransformPosition(C, m));
      D.X = 1.0f;
      D.Y = 0;
      D.Z = 1.0f;
      four = scene.AddVertex(Vector3.TransformPosition(D, m));

      scene.AddLine(one, two);
      scene.AddLine(two, three);
      scene.AddLine(three, one);
      scene.AddLine(four, one);
      scene.AddLine(four, two);
      scene.AddLine(four, three);

      drawTetrahedronRecursive(depthParam, scene, m, param, A, B, C, D);

      return segments + (int)Math.Pow(4, depthParam) + 1;
    }



    private void drawTetrahedronRecursive (int depth,SceneBrep scene, Matrix4 m, string param,Vector3 a, Vector3 b,Vector3 c, Vector3 d)
    {
      if (depth == 0)
      {
        return;
      }
      Vector3 stand;
      stand.X = 0;
      stand.Y = 0;
      stand.Z = 0;
      int ab,ac,ad,bc,bd,cd;
      Vector3 AB = stand,AC = stand,AD = stand;
      Vector3 BC = stand,BD = stand;
      Vector3 CD = stand;

      calculateMiddle(a, b, c, d, ref AB, ref AC, ref AD, ref BC, ref BD, ref CD);

      ab = scene.AddVertex(Vector3.TransformPosition(AB, m));
      ac = scene.AddVertex(Vector3.TransformPosition(AC, m));
      ad = scene.AddVertex(Vector3.TransformPosition(AD, m));
      bc = scene.AddVertex(Vector3.TransformPosition(BC, m));
      bd = scene.AddVertex(Vector3.TransformPosition(BD, m));
      cd = scene.AddVertex(Vector3.TransformPosition(CD, m));

      scene.AddLine(ab, ac);
      scene.AddLine(ac, bc);
      scene.AddLine(bc, ab);

      scene.AddLine(ab, bd);
      scene.AddLine(bd, ad);
      scene.AddLine(ad, ab);

      scene.AddLine(ac, ad);
      scene.AddLine(ad, cd);
      scene.AddLine(cd, ac);

      scene.AddLine(bc, cd);
      scene.AddLine(cd, bd);
      scene.AddLine(bd, bc);

      drawTetrahedronRecursive(depth - 1, scene, m, param, AB, b, BC, BD);
      drawTetrahedronRecursive(depth - 1, scene, m, param, a, AB, AC, AD);
      drawTetrahedronRecursive(depth - 1, scene, m, param, AC, BC, c, CD);
      drawTetrahedronRecursive(depth - 1, scene, m, param, AD, BD, CD, d);

    }

    private void calculateMiddle (Vector3 a, Vector3 b, Vector3 c, Vector3 d, ref Vector3 aB, ref Vector3 aC, ref Vector3 aD, ref Vector3 bC, ref Vector3 bD, ref Vector3 cD)
    {

      aB.X = (a.X + b.X) / 2.0f;
      aB.Y = (a.Y + b.Y) / 2.0f;
      aB.Z = (a.Z + b.Z) / 2.0f;

      aC.X = (a.X + c.X) / 2.0f;
      aC.Y = (a.Y + c.Y) / 2.0f;
      aC.Z = (a.Z + c.Z) / 2.0f;

      aD.X = (a.X + d.X) / 2.0f;
      aD.Y = (a.Y + d.Y) / 2.0f;
      aD.Z = (a.Z + d.Z) / 2.0f;

      bC.X = (b.X + c.X) / 2.0f;
      bC.Y = (b.Y + c.Y) / 2.0f;
      bC.Z = (b.Z + c.Z) / 2.0f;

      bD.X = (b.X + d.X) / 2.0f;
      bD.Y = (b.Y + d.Y) / 2.0f;
      bD.Z = (b.Z + d.Z) / 2.0f;

      cD.X = (c.X + d.X) / 2.0f;
      cD.Y = (c.Y + d.Y) / 2.0f;
      cD.Z = (c.Z + d.Z) / 2.0f;

    }
    #endregion
  }
}
