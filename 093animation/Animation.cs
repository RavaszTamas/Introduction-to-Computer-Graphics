using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using LineCanvas;
using MathSupport;
using Utilities;

namespace _093animation
{
  public class Animation
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="wid">Image width in pixels.</param>
    /// <param name="hei">Image height in pixels.</param>
    /// <param name="from">Animation start in seconds.</param>
    /// <param name="to">Animation end in seconds.</param>
    /// <param name="fps">Frames-per-seconds.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out int wid, out int hei, out double from, out double to, out double fps, out string param, out string tooltip)
    {
      // {{

      // Put your name here.
      name = "Ravasz Tamás";

      // Image size in pixels.
      wid = 1920;
      hei = 1080;

      // Animation.
      from = 0.0;
      to   = 60.0;
      fps  = 30.0;

      // Specific animation params.
      param = "width=1.0,anti=true,objects=10,hatches=12,prob=0.95,tree2,angle=0,branches=2,periods=1";

      // Tooltip = help.
      tooltip = "width=<double>, anti[=<bool>], objects=<int>, hatches=<int>, prob=<double>";
      // }}
    }
    static int[] branchesAtMoment;

    /// <summary>
    /// Global initialization. Called before each animation batch
    /// or single-frame computation.
    /// </summary>
    /// <param name="width">Width of the future canvas in pixels.</param>
    /// <param name="height">Height of the future canvas in pixels.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="fps">Required fps.</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void InitAnimation (int width, int height, double start, double end, double fps, string param)
    {
      // {{ TODO: put your init code here

      // }}
    }

    /// <summary>
    /// Draw single animation frame.
    /// Has to be re-entrant!
    /// </summary>
    /// <param name="c">Canvas to draw to.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <param name="start">Start time (t0)</param>
    /// <param name="end">End time (for animation length normalization).</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void DrawFrame (Canvas c, double time, double start, double end, string param)
    {
      // {{ TODO: put your drawing code here

      double timeNorm = Arith.Clamp((time - start) / (end - start), 0.0, 1.0);


      // {{ TODO: put your drawing code here

      // Input params.
      float penWidth = 1.0f;   // pen width
      bool antialias = false;  // use anti-aliasing?
      int objects    = 100;    // number of randomly generated objects (squares, stars, Brownian particles)
      int hatches    = 12;     // number of hatch-lines for the squares
      double prob    = 0.95;   // continue-probability for the Brownian motion simulator
      int seed       = 12;     // random generator seed
      bool sierpTri = false;
      bool mandelbrot = false;
      bool sierpinskiTree = false;
      bool sierpinskiTreeIterative = false;
      bool kochSnowflake = false;
      bool kochSnowflakePattern = false;
      bool decreaseBranch = false;
      bool increaseBranch = false;

      double paramAngle = Math.PI/2;
      int branches = 2;
      int periods = 1;
      float scale = 1;
      int currentPeriod = 1;
      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // with=<line-width>
        if (Util.TryParse(p, "width", ref penWidth))
        {
          if (penWidth < 0.0f)
            penWidth = 0.0f;
        }

        // anti[=<bool>]
        Util.TryParse(p, "anti", ref antialias);

        // squares=<number>
        if (Util.TryParse(p, "objects", ref objects) &&
            objects < 0)
          objects = 0;

        // hatches=<number>
        if (Util.TryParse(p, "hatches", ref hatches) &&
            hatches < 1)
          hatches = 1;

        // prob=<probability>
        if (Util.TryParse(p, "prob", ref prob) &&
            prob > 0.999)
          prob = 0.999;

        // seed=<int>
        Util.TryParse(p, "seed", ref seed);

        Util.TryParse(p, "sierpinski", ref sierpTri);

        Util.TryParse(p, "mandelbrot", ref mandelbrot);

        Util.TryParse(p, "tree", ref sierpinskiTree);

        Util.TryParse(p, "tree2", ref sierpinskiTreeIterative);

        Util.TryParse(p, "increaseB", ref increaseBranch);
        Util.TryParse(p, "decreaseB", ref decreaseBranch);

        if (Util.TryParse(p, "angle", ref paramAngle))
        {
          paramAngle = paramAngle * Math.PI / 180.0;
        }

        if (Util.TryParse(p, "periods", ref periods))
        {
          if (periods < 1)
            periods = 1;
        }

        if (Util.TryParse(p, "branches", ref branches))
        {
          if (branches < 1)
            branches = 1;
        }
        Util.TryParse(p, "koch", ref kochSnowflake);
        Util.TryParse(p, "kochpattern", ref kochSnowflakePattern);
        if (Util.TryParse(p, "scale", ref scale))
        {

          if (scale <= 0)
            scale = 1;
        }



      }
      double tangle = (time - start) * 2.0 * Math.PI / (end - start) * periods;

      int wq = c.Width / 4;
      int hq = c.Height / 4;
      int wh = wq + wq;
      int hh = hq + hq;
      int minh = Math.Min(wh, hh);
      double t;
      int i, j;
      double cx, cy, angle, x, y;

      c.Clear(Color.Black);

      // 1st quadrant - star.
      c.SetPenWidth(penWidth);
      c.SetAntiAlias(antialias);
      int MAX_LINES = objects;

      if (sierpTri)
      {
        //drawSierpinskiTri(c, MAX_LINES);
        return;
      }

      if (mandelbrot)
      {
        drawMandelbrot(c, (int)(time*100));
        return;
      }

      if (sierpinskiTree)
      {
        if (branches >= 4 && MAX_LINES > 11)
          MAX_LINES = 11;
        if (branches == 3 && MAX_LINES > 14)
          MAX_LINES = 14;
        if (branches == 2 && MAX_LINES > 20)
          MAX_LINES = 20;
        if (increaseBranch && decreaseBranch)
        {
          currentPeriod = (int)(tangle / (2.0 * Math.PI));
          int discr = Math.Max(0,(currentPeriod+1) - periods/2 );
          if (discr != 0)
          {
            branches = (periods / 2) + branches - discr;
          }
          else
          {
            branches += currentPeriod;
          }
          if (branches > 5)
          {
            MAX_LINES = 5;
          }
        }
        else
        {
          if (increaseBranch)
          {
            currentPeriod = (int)(tangle / (2.0 * Math.PI));
            branches += currentPeriod;
            branches = Math.Min(10, branches);

          }
          else if (decreaseBranch)
          {
            currentPeriod = (int)(tangle / (2.0 * Math.PI));
            branches -= currentPeriod - 1;
            branches = Math.Max(1, branches);

          }

        }
        if (branches >= 4 && MAX_LINES > 11)
          MAX_LINES = 5;
        if (branches == 3 && MAX_LINES > 14)
          MAX_LINES = 8;
        if (branches == 2 && MAX_LINES > 20)
          MAX_LINES = 10;

        drawSierpinskiTree(c, MAX_LINES, MAX_LINES, new PointF(c.Width / 2, 0), new PointF(c.Width / 2, c.Height / 3), paramAngle + tangle, branches);
        return;
      }

      if (kochSnowflakePattern)
      {
        if (objects > 10)
          objects = 10;

        //c.Line(c.Width / 2, 0, c.Width/2, c.Height/2);
        float otherKWh = c.Width/10.0f * scale;
        float height = otherKWh*(float)Math.Sqrt(3)/2.0f;
        float smallheightConst = (otherKWh/3)*(float)Math.Sqrt(3)/2.0f;
        float smallheight;



        smallheight = 0.0f;
        for (int kj = -1; kj * otherKWh <= c.Height + otherKWh; kj++)
        {
          for (int k = -1; k * otherKWh <= c.Width + otherKWh; ++k)
          {
            if (((k % 2) + 2) % 2 == 0)
            {
              smallheight = (otherKWh / 3) * (float)Math.Sqrt(3) / 2.0f;
              smallheight *= 2.0f;
            }
            else
            {
              smallheight = 0;
            }

            //drawKochSnowFlake(c, new PointF(0 + k * otherKWh, smallheight + kj * (4 * smallheightConst)), new PointF(0 + (k + 1) * otherKWh, smallheight + kj * (4 * smallheightConst)), objects);
            //drawKochSnowFlake(c, new PointF((k + 1) * otherKWh, smallheight + kj * (4 * smallheightConst)), new PointF(otherKWh / 2 + k * otherKWh, height + smallheight + kj * (4 * smallheightConst)), objects);
            //drawKochSnowFlake(c, new PointF(otherKWh / 2 + k * otherKWh, height + smallheight + kj * (4 * smallheightConst)), new PointF(0 + k * otherKWh, smallheight + kj * (4 * smallheightConst)), objects);
          }
        }



        return;
      }

      if (kochSnowflake)
      {
        if (objects > 10)
          objects = 10;
        int kw = c.Width/8;
        int kh = c.Height/3;
        //drawKochSnowFlake(c, new PointF(0 + kw, 0 + kh), new PointF(c.Width - kw, 0 + kh), objects);
        //drawKochSnowFlake(c, new PointF(c.Width - kw, 0 + kh), new PointF(c.Width / 2, c.Height - kh / 3), objects);
        //drawKochSnowFlake(c, new PointF(c.Width / 2, c.Height - kh / 3), new PointF(0 + kw, 0 + kh), objects);

        return;

      }

      if (sierpinskiTreeIterative)
      {
        if (MAX_LINES > 20)
          MAX_LINES = 20;
        //tangle *= 3.0;
        if (increaseBranch && decreaseBranch)
        {
          currentPeriod = (int)(tangle / (2.0 * Math.PI));
          int discr = Math.Max(0,(currentPeriod+1) - periods/2 );
          if (discr != 0)
          {
            branches = (periods / 2) + branches - discr;
          }
          else
          {
            branches += currentPeriod;
          }
          if(branches > 5)
          {
            MAX_LINES = 5;
          }
          //Debug.WriteLine(discr + " " + currentPeriod + " " + branches);
        }
        else
        {
          if (increaseBranch)
          {
            currentPeriod = (int)(tangle / (2.0 * Math.PI));
            branches += currentPeriod;
            if (MAX_LINES > 5)
              branches = Math.Min(4, branches);

          }
          else if (decreaseBranch)
          {
            currentPeriod = (int)(tangle / (2.0 * Math.PI));
            branches -= currentPeriod;
            branches = Math.Max(1, branches);

          }
        }
        drawTreeIterativeIterative(c, MAX_LINES, MAX_LINES, new PointF(c.Width / 2, 0), new PointF(c.Width / 2, c.Height / 3), paramAngle + tangle*branches, branches);
        return;
      }

      c.Clear(Color.Black);

      // 1st quadrant - star.
      c.SetPenWidth(penWidth);
      c.SetAntiAlias(antialias);

      MAX_LINES = 30;
      for (i = 0, t = 0.0; i < MAX_LINES; i++, t += 1.0 / MAX_LINES)
      {
        c.SetColor(Color.FromArgb(i * 255 / MAX_LINES, 255, 255 - i * 255 / MAX_LINES)); // [0,255,255] -> [255,255,0]
        c.Line(t * wh, 0, wh - t * wh, hh);
      }
      for (i = 0, t = 0.0; i < MAX_LINES; i++, t += 1.0 / MAX_LINES)
      {
        c.SetColor(Color.FromArgb(255, 255 - i * 255 / MAX_LINES, i * 255 / MAX_LINES)); // [255,255,0] -> [255,0,255]
        c.Line(0, hh - t * hh, wh, t * hh);
      }

      // 2nd quadrant - random hatched squares.
      double size = minh / 10.0;
      double padding = size * Math.Sqrt(0.5);
      c.SetColor(Color.LemonChiffon);
      c.SetPenWidth(1.0f);
      Random r = new Random(12);

      for (i = 0; i < objects; i++)
      {
        do
          cx = r.NextDouble() * wh;
        while (cx < padding ||
               cx > wh - padding);

        c.SetAntiAlias(cx > wq);
        cx += wh;

        do
          cy = r.NextDouble() * hh;
        while (cy < padding ||
               cy > hh - padding);

        angle = (r.NextDouble() + timeNorm) * Math.PI;

        double dirx = Math.Sin(angle) * size * 0.5;
        double diry = Math.Cos(angle) * size * 0.5;
        cx -= dirx - diry;
        cy -= diry + dirx;
        double dx = -diry * 2.0 / hatches;
        double dy = dirx * 2.0 / hatches;
        double linx = dirx + dirx;
        double liny = diry + diry;

        for (j = 0; j++ < hatches; cx += dx, cy += dy)
          c.Line(cx, cy, cx + linx, cy + liny);
      }

      // 3rd quadrant - random stars.
      c.SetColor(Color.LightCoral);
      c.SetPenWidth(penWidth);
      size = minh / 16.0;
      padding = size;
      const int MAX_SIDES = 30;
      List<PointF> v = new List<PointF>(MAX_SIDES + 1);

      for (i = 0; i < objects; i++)
      {
        do
          cx = r.NextDouble() * wh;
        while (cx < padding ||
               cx > wh - padding);

        c.SetAntiAlias(cx > wq);

        do
          cy = r.NextDouble() * hh;
        while (cy < padding ||
               cy > hh - padding);

        cy += hh;

        int sides = r.Next(3, MAX_SIDES);
        double dAngle = Math.PI * 2.0 / sides;

        v.Clear();
        angle = 0.0;

        for (j = 0; j++ < sides; angle += dAngle)
        {
          double rad = size * (0.1 + 0.9 * r.NextDouble());
          x = cx + rad * Math.Sin(angle);
          y = cy + rad * Math.Cos(angle);
          v.Add(new PointF((float)x, (float)y));
        }
        v.Add(v[0]);
        c.PolyLine(v);
      }

      // 4th quadrant - Brownian motion.
      c.SetPenWidth(penWidth);
      c.SetAntiAlias(true);
      size = minh / 10.0;
      padding = size;

      for (i = 0; i < objects; i++)
      {
        do
          x = r.NextDouble() * wh;
        while (x < padding ||
               x > wh - padding);

        do
          y = r.NextDouble() * hh;
        while (y < padding ||
               y > hh - padding);

        c.SetColor(Color.FromArgb(127 + r.Next(0, 128),
                                  127 + r.Next(0, 128),
                                  127 + r.Next(0, 128)));

        for (j = 0; j++ < 1000;)
        {
          angle = r.NextDouble() * Math.PI * 2.0;
          double rad = size * r.NextDouble();
          cx = x + rad * Math.Sin(angle);
          cy = y + rad * Math.Cos(angle);
          if (cx < 0.0 || cx > wh ||
              cy < 0.0 || cy > hh)
            break;

          if (j < 70.0 * timeNorm)
            c.Line(x + wh, y + hh, cx + wh, cy + hh);

          x = cx;
          y = cy;
          if (r.NextDouble() > prob)
            break;
        }
      }



      // }}
    }
    private static void drawSierpinskiTree (Canvas canvas, int MAX_LEVEL, int level, PointF start, PointF finish, double angle, int samples)
    {
      if (level <= 0)
        return;
      canvas.Line(start.X, start.Y, finish.X, finish.Y);
      canvas.SetColor(Color.FromArgb(level * 255 / MAX_LEVEL, 255, 255 - level * 255 / MAX_LEVEL)); // [0,255,255] -> [255,255,0]


      float vectorX = (start.X-finish.X)*(5.0f/8.0f);
      float VectorY = (start.Y-finish.Y)*(5.0f/8.0f);

      PointF p = new PointF(finish.X+vectorX,finish.Y+VectorY);

      float s = (float)Math.Sin(angle);
      float c = (float)Math.Cos(angle);
      // translate point back to origin:
      p.X -= finish.X;
      p.Y -= finish.Y;
      // rotate point
      float Xnew = p.X * c - p.Y * s;
      float Ynew = p.X * s + p.Y * c;
      // translate point back:
      p.X = Xnew + (finish.X);
      p.Y = Ynew + (finish.Y);

      drawSierpinskiTree(canvas, MAX_LEVEL, level - 1, finish, p, angle, samples);
      if (samples < 2)
        return;


      p = new PointF(finish.X + vectorX, finish.Y + VectorY);
      s = (float)Math.Sin(-angle);
      c = (float)Math.Cos(-angle);
      // translate point back to origin:
      p.X -= finish.X;
      p.Y -= finish.Y;
      // rotate point
      Xnew = p.X * c - p.Y * s;
      Ynew = p.X * s + p.Y * c;
      // translate point back:
      p.X = Xnew + (finish.X);
      p.Y = Ynew + (finish.Y);

      drawSierpinskiTree(canvas, MAX_LEVEL, level - 1, finish, p, angle, samples);
      if (samples < 3)
        return;


      p = new PointF(finish.X + vectorX / 2, finish.Y + VectorY / 2);
      s = (float)Math.Sin(Math.PI / 2);
      c = (float)Math.Cos(Math.PI / 2);
      // translate point back to origin:
      p.X -= finish.X;
      p.Y -= finish.Y;
      // rotate point
      Xnew = p.X * c - p.Y * s;
      Ynew = p.X * s + p.Y * c;
      // translate point back:
      p.X = Xnew + (finish.X);
      p.Y = Ynew + (finish.Y);

      drawSierpinskiTree(canvas, MAX_LEVEL, level - 1, finish, p, angle, samples);

      if (samples < 4)
        return;

      p = new PointF(finish.X + vectorX / 2, finish.Y + VectorY / 2);
      s = (float)Math.Sin(-Math.PI / 2);
      c = (float)Math.Cos(-Math.PI / 2);
      // translate point back to origin:
      p.X -= finish.X;
      p.Y -= finish.Y;
      // rotate point
      Xnew = p.X * c - p.Y * s;
      Ynew = p.X * s + p.Y * c;
      // translate point back:
      p.X = Xnew + (finish.X);
      p.Y = Ynew + (finish.Y);


      drawSierpinskiTree(canvas, MAX_LEVEL, level - 1, finish, p, angle, samples);


    }

    private static void drawTreeIterativeIterative (Canvas canvas, int MAX_LEVEL, int level, PointF start, PointF finish, double angle, int samples)
    {
      if (level <= 0)
        return;
      canvas.Line(start.X, start.Y, finish.X, finish.Y);
      canvas.SetColor(Color.FromArgb(level * 255 / MAX_LEVEL, 255, 255 - level * 255 / MAX_LEVEL)); // [0,255,255] -> [255,255,0]

      float vectorX = (start.X-finish.X)*(5.0f/8.0f);
      float VectorY = (start.Y-finish.Y)*(5.0f/8.0f);


      for (int i = 0; i < samples; i++)
      {
        PointF p = new PointF(finish.X+vectorX,finish.Y+VectorY);

        float s = (float)Math.Sin(i*angle/samples + angle);
        float c = (float)Math.Cos(i*angle/samples + angle);
        // translate point back to origin:
        p.X -= finish.X;
        p.Y -= finish.Y;
        // rotate point
        float Xnew = p.X * c - p.Y * s;
        float Ynew = p.X * s + p.Y * c;
        // translate point back:
        p.X = Xnew + (finish.X);
        p.Y = Ynew + (finish.Y);

        drawTreeIterativeIterative(canvas, MAX_LEVEL, level - 1, finish, p, angle, samples);

      }


    }

    private static int mandelbrotCalc (Complex c, int MAX_LINES)
    {
      Complex z = 0;
      int n = 0;
      while (Math.Sqrt(z.Real * z.Real + z.Imaginary * z.Imaginary) <= 2 && n < MAX_LINES)
      {
        z = z * z + c;
        n++;
      }
      return n;
    }
    private static void drawMandelbrot (Canvas canvas, int MAX_LINES)
    {
      int RE_START = -2;
      int RE_END = 1;
      int IM_START = -1;
      int IM_END = 1;
      List<PointF> pointsFront = new List<PointF>();
      List<PointF> pointsBack = new List<PointF>();

      bool prevWasMax= false;
      bool prevWasNotMax= false;
      bool once1 = true;

      for (int i = 0; i < canvas.Width; i++)
      {
        once1 = true;
        for (int j = 0; j < canvas.Height / 2; j++)
        {
          Complex complexNum = new Complex(RE_START + ((double)i / (double)canvas.Width) * (RE_END - RE_START),IM_START + ((double)j / (double)canvas.Height) * (IM_END - IM_START));
          int m = mandelbrotCalc(complexNum,MAX_LINES);

          //int col = 255 - (m * 255 / MAX_LINES);
          if (m == MAX_LINES)
          {
            //canvas.SetColor(Color.FromArgb(0, 0, 0));
            prevWasMax = true;
          }
          else
          {
            prevWasNotMax = true;
            //canvas.SetColor(Color.FromArgb(255, 255, 255));
          }
          //canvas.Line(i, j, i, j + 1);
          if (prevWasMax && once1 && m != MAX_LINES)
          {
            prevWasMax = false;

            pointsFront.Add(new PointF(i, j));
            once1 = false;
          }
          else if (prevWasNotMax && m == MAX_LINES)
          {
            prevWasNotMax = false;
            pointsBack.Add(new PointF(i, j));

          }

        }
        prevWasMax = false;
        prevWasNotMax = false;

      }

      for (int i = 0; i < canvas.Width; i++)
      {
        once1 = true;
        for (int j = canvas.Height / 2 + 1; j < canvas.Height; j++)
        {
          Complex complexNum = new Complex(RE_START + ((double)i / (double)canvas.Width) * (RE_END - RE_START),IM_START + ((double)j / (double)canvas.Height) * (IM_END - IM_START));
          int m = mandelbrotCalc(complexNum,MAX_LINES);

          //int col = 255 - (m * 255 / MAX_LINES);
          if (m == MAX_LINES)
          {
            //canvas.SetColor(Color.FromArgb(0, 0, 0));
            prevWasMax = true;
          }
          else
          {
            prevWasNotMax = true;
            //canvas.SetColor(Color.FromArgb(255, 255, 255));
          }
          //canvas.Line(i, j, i, j + 1);
          if (prevWasMax && once1 && m != MAX_LINES)
          {
            prevWasMax = false;

            pointsFront.Add(new PointF(i, j));
            once1 = false;
          }
          else if (prevWasNotMax && m == MAX_LINES)
          {
            prevWasNotMax = false;
            pointsBack.Add(new PointF(i, j));

          }

        }
        prevWasMax = false;
        prevWasNotMax = false;

      }

      if (pointsFront.Count > 1)
        canvas.PolyLine(pointsFront.ToArray());
      if (pointsBack.Count > 1)
        canvas.PolyLine(pointsBack.ToArray());

    }

  }
}
