using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MathSupport;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Utilities;

namespace _086shader
{
  public class AnimatedCamera : DefaultDynamicCamera
  {
    /// <summary>
    /// Optional form-data initialization.
    /// </summary>
    /// <param name="name">Return your full name.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out string param, out string tooltip)
    {
      // {{

      name    = "Ravasz Tamás";
      param   = "period=10.0, rad=2.0";
      tooltip = "period=<cam period in seconds>, rad=<cam radius in scene diameters>";
      // }}
    }

    List<Vector3> positions = new List<Vector3>();

    public void defaultCamera()
    {
      positions.Clear();
      positions.Add(new Vector3(-6, 0, 0));
      positions.Add(new Vector3(0.5f, -1, -10));
      positions.Add(new Vector3(6, 0, 0));

      
      positions.Add(new Vector3(6, 0, 0));
      positions.Add(new Vector3(0.5f, -1, -10));
      positions.Add(new Vector3(0, 10, 0));


      positions.Add(new Vector3(0, 10, 0));
      positions.Add(new Vector3(0.5f, -1, -10));
      positions.Add(new Vector3(-6, 0, 0));

      positions.Add(new Vector3(-6, 0, 0));
      positions.Add(new Vector3(-2, -1, 1));
      positions.Add(new Vector3(0.5f, 0, 4));

      positions.Add(new Vector3(0.5f, 0, 4));
      positions.Add(new Vector3(-2, -1, 1));
      positions.Add(new Vector3(6, 0, 0));

      positions.Add(new Vector3(6, 0, 0));
      positions.Add(new Vector3(0, 15, 5));
      positions.Add(new Vector3(0, 10, 0));


      /*positions.Add(new Vector3(0, 10, 0));
      positions.Add(new Vector3(0.5f, -1, -5));
      positions.Add(new Vector3(0.5f, 0, 4));*/

      positions.Add(new Vector3(0, 10, 0));
      positions.Add(new Vector3(0.5f, 40, 20));
      positions.Add(new Vector3(0.5f, 0, 4));

      positions.Add(new Vector3(0.5f, 0, 4));
      positions.Add(new Vector3(0.5f, 40, 20));
      positions.Add(new Vector3(0, 10, 0));


      positions.Add(new Vector3(0, 10, 0));
      positions.Add(new Vector3(0, 15, 5));
      positions.Add(new Vector3(-6, 0, 0));


    }
    public void processFile(String cameraFile)
    {

      try
      {
        positions.Clear();
        using(StreamReader input = new StreamReader(cameraFile))
        {
          string line;
          while((line = input.ReadLine()) != null)
          {
            if (line != "")
            {
              Dictionary<string, string> p = Util.ParseKeyValueList(line);
              String P0,P1,P2;
              p.TryGetValue("P0", out P0);
              p.TryGetValue("P1", out P1);
              p.TryGetValue("P2", out P2);
              Double X,Y,Z;
              String[] values;


              values = P0.Split(';');
              X = double.Parse(values[0]);
              Y = double.Parse(values[1]);
              Z = double.Parse(values[2]);
              positions.Add(new Vector3((float)X, (float)Y, (float)Z));

              values = P1.Split(';');
              X = double.Parse(values[0]);
              Y = double.Parse(values[1]);
              Z = double.Parse(values[2]);
              positions.Add(new Vector3((float)X, (float)Y, (float)Z));

              values = P2.Split(';');
              X = double.Parse(values[0]);
              Y = double.Parse(values[1]);
              Z = double.Parse(values[2]);
              positions.Add(new Vector3((float)X, (float)Y, (float)Z));
            }
          }
        }

        if(positions.Count%3 != 0 || positions.Count == 0)
        {
          defaultCamera();
        }
      }
      catch(Exception ex)
      {
        defaultCamera();
      }

    }
    /// <summary>
    /// Called after form's Param field is changed.
    /// </summary>
    /// <param name="param">String parameters from the form.</param>
    /// <param name="cameraFile">Optional file-name of your custom camera definition (camera script?).</param>
    public override void Update (string param, string cameraFile)
    {
      // {{ Put your parameter-parsing code here

      if (cameraFile == "" || cameraFile == null)
        defaultCamera();
      else
        processFile(cameraFile);

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // period=<double>
        double v = 1.0;
        if (Util.TryParse(p, "period", ref v))
          MaxTime = Math.Max(v, 0.1);

        // rad=<double>
        if (Util.TryParse(p, "rad", ref v))
          radius = (float)Math.Max(v, 0.01);

        // ... you can add more parameters here ...
      }

      Time = Util.Clamp(Time, MinTime, MaxTime);

      // Put your camera-definition-file parsing here.

      // }}
    }

    /// <summary>
    /// Radius of camera trajectory.
    /// </summary>
    float radius = 1.0f;

    /// <param name="param">String parameters from the form.</param>
    /// <param name="cameraFile">Optional file-name of your custom camera definition (camera script?).</param>
    public AnimatedCamera (string param, string cameraFile = "")
    {
      // {{ Put your camera initialization code here

      Update(param, cameraFile);

      // }}
    }

    Matrix4 perspectiveProjection;

    /// <summary>
    /// Returns Projection matrix. Must be implemented.
    /// </summary>
    public override Matrix4 Projection => perspectiveProjection;

    /// <summary>
    /// Called every time a viewport is changed.
    /// It is possible to ignore some arguments in case of scripted camera.
    /// </summary>
    public override void GLsetupViewport (int width, int height, float near = 0.01f, float far = 1000.0f)
    {
      // 1. set ViewPort transform:
      GL.Viewport(0, 0, width, height);

      // 2. set projection matrix
      perspectiveProjection = Matrix4.CreatePerspectiveFieldOfView(Fov, width / (float)height, near, far);
      GLsetProjection();
    }

    /// <summary>
    /// I'm using internal ModelView matrix computation.
    /// </summary>
    ///

    Vector3 computePoistionOnCurveAtTime(int idx, double start, double finish, double time)
    {
      float t = (float)((time-start)/(finish-start));
      Vector3 P0,P1,P2;
      P0 = positions[idx * 3];
      P1 = positions[idx * 3 + 1];
      P2 = positions[idx * 3 + 2];
      Vector3 P = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2);
      return P;
    }

    Vector3 computePositionAtTime(double start,double finish, double current)
    {

      float t = (float)((current-start)/(finish-start));
      int n = positions.Count/3;
      double unit = 1.0/(double)n;
      double val = t/unit;
      //Debug.WriteLine(val + " " + (int)val + " " + unit + " " + t);
      Vector3 P = computePoistionOnCurveAtTime((int)val, unit * (double)((int)val), unit * (double)(((int)val)+1),  t);
      return P;
    }

    Matrix4 computeModelView ()
    {
      Vector3 P0,P1,P2;

      P0.X = 3;
      P0.Y = 0;
      P0.Z = 0;

      P1.X = 0.5f;
      P1.Y = -1;
      P1.Z = -1;

      P2.X = 0;
      P2.Y = 5;
      P2.Z = 0;


      //float t = (float)((Time - MinTime) * 2 * Math.PI / (MaxTime - MinTime));
      float t = (float)((Time-MinTime)/(MaxTime-MinTime));
      double r = radius * 0.5 * Diameter;
      /*Vector3 eye = Center + new Vector3((float)(Math.Sin(t) * r),
                                         (float)(Math.Sin(t + 1.0) * r * 0.2),
                                         (float)(Math.Cos(t) * r) );*/
      Vector3 P  = computePositionAtTime(MinTime, MaxTime, Time);
      //Debug.WriteLine(t);
      //Vector3 P = (1 - t) * ((1 - t) * P0 + t * P1) + t * ((1 - t) * P1 + t * P2);
      Vector3 eye = Center + P;
      return Matrix4.LookAt(eye, Center, Vector3.UnitY);
    }

    /// <summary>
    /// Crucial property = is called in every frame.
    /// </summary>
    public override Matrix4 ModelView => computeModelView();

    /// <summary>
    /// Crucial property = is called in every frame.
    /// </summary>
    public override Matrix4 ModelViewInv => computeModelView().Inverted();
  }
}
