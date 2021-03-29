using MathSupport;
using Raster;
using System;
using System.Diagnostics;
using System.Drawing;


namespace Utilities
{
  public class ImageHistogram
  {
    /// <summary>
    /// Cached histogram data.
    /// </summary>
    protected static int[] histArray = null;
    protected static int[] histArrayGray = null;
    protected static int[] histArrayRed = null;
    protected static int[] histArrayGreen = null;
    protected static int[] histArrayBlue = null;
    protected static int[] histArrayMagenta = null;
    protected static int[] histArrayCyan = null;
    protected static int[] histArrayYellow = null;
    protected static int[] histArrayWhite = null;
    protected static int[] histArraySelected = null;
    protected static int[] histArrayHue = null;
    protected static int[] histArraySaturation = null;
    protected static int[] histArrayValue = null;

    // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray)
    protected static int mode = 3;

    // Graph appearance (just an example of second visualization option
    // read from param string).
    protected static bool alt = false;

    protected static bool separateAll = false;
    protected static bool separateHSV = false;

    const int startPost = 300;

    /// <summary>
    /// Draws the current histogram to the given raster image.
    /// </summary>
    /// <param name="graph">Result image (already scaled to the desired size).</param>
    public static void DrawHistogramAll (
      Bitmap graph)
    {
      if (histArrayRed == null)
        return;
      if (histArrayBlue == null)
        return;
      if (histArrayGreen == null)
        return;

      float max = 0.0f;
      foreach (int f in histArrayGreen)
        if (f > max)
          max = f;
      foreach (int f in histArrayRed)
        if (f > max)
          max = f;
      foreach (int f in histArrayBlue)
        if (f > max)
          max = f;
      if(histArrayGray != null)
      foreach (int f in histArrayGray)
        if (f > max)
          max = f;

      using (Graphics gfx = Graphics.FromImage(graph))
      {
        gfx.Clear(Color.Black);

        // Graph scaling:
        float x0 = graph.Width * 0.05f;
        float y0 = graph.Height * 0.95f;
        float kx = graph.Width * 0.9f / histArrayRed.Length;
        float ky = -graph.Height * 0.9f / max;

        // Pens:
        Brush graphBrushGray= new SolidBrush(Color.Gray);
        Brush graphBrushRed = new SolidBrush(Color.Red);
        Brush graphBrushGreen = new SolidBrush(Color.Green);
        Brush graphBrushBlue = new SolidBrush(Color.Blue);
        Brush graphBrushYellow = new SolidBrush(Color.FromArgb(255,255,0));
        Brush graphBrushMagenta = new SolidBrush(Color.FromArgb(255,0,255));
        Brush graphBrushCyan = new SolidBrush(Color.FromArgb(0,255,255));
        Brush graphBrushWhite = new SolidBrush(Color.White);

        Pen axisPen = new Pen(Color.White);

        // Histogram all on one chart:
        if (separateAll)
        {
          // to make the charts three(ish) times smaller to equally fit the histogram
          kx /= 3.3f;

          for (int x = 0; x < histArray.Length; x++)
          {
            float yHeightRed = -histArrayRed[x] * ky;
            float yHeightGreen = -histArrayGreen[x] * ky;
            float yHeightBlue = -histArrayBlue[x] * ky;
            if (alt)
            {
              yHeightRed = Math.Min(3.0f, yHeightRed);
              yHeightGreen = Math.Min(3.0f, yHeightGreen);
              yHeightBlue = Math.Min(3.0f, yHeightBlue);

            }
            gfx.FillRectangle(graphBrushRed, x0 + x * kx, y0 + histArrayRed[x] * ky, kx, yHeightRed);
            gfx.FillRectangle(graphBrushGreen, x0 + (x + startPost) * kx, y0 + histArrayGreen[x] * ky, kx, yHeightGreen);
            gfx.FillRectangle(graphBrushBlue, x0 + (x + startPost * 2) * kx, y0 + histArrayBlue[x] * ky, kx, yHeightBlue);
          }
          //draw scale in Y axis and texts
          float ratio = (float)graph.Height/175.0f;

          //center alignment format
          StringFormat drawFormat = new StringFormat();
          drawFormat.Alignment = StringAlignment.Far;

          // Axes:
          gfx.DrawLine(axisPen, x0, y0, x0 + histArrayRed.Length * kx, y0);
          gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

          gfx.DrawLine(axisPen, x0 + (histArrayGreen.Length + 44) * kx, y0, x0 + (histArrayGreen.Length + 300) * kx, y0);
          gfx.DrawLine(axisPen, x0 + (histArrayGreen.Length + 44) * kx, y0, x0 + (histArrayGreen.Length + 44) * kx, y0 + max * ky);


          gfx.DrawLine(axisPen, x0 + (histArrayBlue.Length + 344) * kx, y0, x0 + (histArrayBlue.Length + 600) * kx, y0);
          gfx.DrawLine(axisPen, x0 + (histArrayBlue.Length + 344) * kx, y0, x0 + (histArrayBlue.Length + 344) * kx, y0 + max * ky);

          // Labels:

          gfx.DrawString("Red", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayRed.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);
          gfx.DrawString("Green", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * kx) + histArrayGreen.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);
          gfx.DrawString("Blue", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), ((x0 + startPost * 2 * kx) + histArrayBlue.Length * kx / 2), y0 + (max + 500) * ky, drawFormat);


          //Scale:

          drawFormat.LineAlignment = StringAlignment.Center;
          //these a re for the initial 0 values
          gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
          gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * kx), y0, drawFormat);
          gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * 2 * kx), y0, drawFormat);
          drawFormat.LineAlignment = StringAlignment.Near;

          //draw scale on Y axis
          for (int k = 1; k <= 8; ++k)
          {
            String numberToDisplay = (((int)max) * k/8).ToString();

            gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);
            gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * kx), y0 + max * ky * k / 8, drawFormat);
            gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * 2 * kx), y0 + max * ky * k / 8, drawFormat);

          }

          //draw scale on X axis
          drawFormat.Alignment = StringAlignment.Center;

          for (int k = 0; k <= 8; ++k)
          {

            gfx.DrawString(255 * k / 8 + "", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + 255 * kx * k / 8, y0, drawFormat);
            gfx.DrawString(255 * k / 8 + "", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * kx) + 255 * kx * k / 8, y0, drawFormat);
            gfx.DrawString(255 * k / 8 + "", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), (x0 + startPost * 2 * kx) + 255 * kx * k / 8, y0, drawFormat);

          }


        }
        else
        {
          //Print all colors to one histogram
          for (int x = 0; x < histArray.Length; x++)
          {
            float yHeightRed = -histArrayRed[x] * ky;
            float yHeightGreen = -histArrayGreen[x] * ky;
            float yHeightBlue = -histArrayBlue[x] * ky;
            float yHeightCyan = -histArrayCyan[x] * ky;
            float yHeightYellow = -histArrayYellow[x] * ky;
            float yHeightMagenta = -histArrayMagenta[x] * ky;
            float yHeightWhite = -histArrayWhite[x] * ky;
            float yHeightGray = 3.0f;

            if (alt)
            {
              //alternative representation as points
              yHeightRed = 3.0f;
              yHeightGreen = 3.0f;
              yHeightBlue = 3.0f;
              yHeightCyan = 3.0f;
              yHeightYellow = 3.0f;
              yHeightMagenta = 3.0f;
              yHeightWhite = 3.0f;
            }
            //draw all the columns obtaind during the computation
            gfx.FillRectangle(graphBrushRed, x0 + x * kx, y0 + histArrayRed[x] * ky, kx, yHeightRed);
            gfx.FillRectangle(graphBrushGreen, x0 + x * kx, y0 + histArrayGreen[x] * ky, kx, yHeightGreen);
            gfx.FillRectangle(graphBrushBlue, x0 + x * kx, y0 + histArrayBlue[x] * ky, kx, yHeightBlue);
            gfx.FillRectangle(graphBrushCyan, x0 + x * kx, y0 + histArrayCyan[x] * ky, kx, yHeightCyan);
            gfx.FillRectangle(graphBrushYellow, x0 + x * kx, y0 + histArrayYellow[x] * ky, kx, yHeightYellow);
            gfx.FillRectangle(graphBrushMagenta, x0 + x * kx, y0 + histArrayMagenta[x] * ky, kx, yHeightMagenta);
            gfx.FillRectangle(graphBrushWhite, x0 + x * kx, y0 + histArrayWhite[x] * ky, kx, yHeightWhite);
            gfx.FillRectangle(graphBrushGray, x0 + x * kx, y0 + histArrayGray[x] * ky, kx, yHeightGray);

          }

          // Axes:
          gfx.DrawLine(axisPen, x0, y0, x0 + histArrayRed.Length * kx, y0);
          gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

          //Scale:
          StringFormat drawFormat = new StringFormat();
          drawFormat.Alignment = StringAlignment.Far;

          //draw scale in Y axis
          float ratio = (float)graph.Height/175.0f;

          //draw label
          gfx.DrawString("All RGB values", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayRed.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);

          //draw Y axis
          drawFormat.LineAlignment = StringAlignment.Center;
          gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
          drawFormat.LineAlignment = StringAlignment.Near;

          for (int k = 1; k <= 8; ++k)
          {
            String numberToDisplay = (((int)max) * k/8).ToString();

            gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

          }

          //draw scale on X axis
          drawFormat.Alignment = StringAlignment.Center;
          for (int k = 0; k <= 8; ++k)
          {

            gfx.DrawString(255 * k / 8 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + 255 * kx * k / 8, y0, drawFormat);

          }
        }
      }
    }

    internal static void DrawHSVHistogram (Bitmap graph)
    {
      if (mode == 0 && histArrayHue == null)
        return;
      if (mode == 1 && histArraySaturation == null)
        return;
      if (mode == 2 && histArrayValue == null)
        return;
      if (mode == 3 && histArrayValue == null && histArraySaturation == null && histArrayValue == null)
        return;
      //calculationg the maximum value to draw everything in scale to the screen
      float max = 0.0f;
      float maxHue = 0.0f;
      if (histArrayHue != null)
        foreach (int f in histArrayHue)
        {
          if (f > maxHue)
            maxHue = f;
          if (f > max)
            max = f;
        }

      float maxSaturation = 0.0f;

      if (histArraySaturation != null)
        foreach (int f in histArraySaturation)
        {
          if (f > maxSaturation)
            maxSaturation = f;
          if (f > max)
            max = f;
        }
      float maxValue = 0.0f;

      if (histArrayValue != null)
        foreach (int f in histArrayValue)
        {
          if (f > maxValue)
            maxValue = f;
          if (f > max)
            max = f;
        }

      using (Graphics gfx = Graphics.FromImage(graph))
      {
        gfx.Clear(Color.Black);

        // Graph scaling:
        float x0 = graph.Width * 0.05f;
        float y0 = graph.Height * 0.95f;
        float kx = graph.Width * 0.9f / histArrayHue.Length;
        float ky = -graph.Height * 0.9f / max;

        // Pens:
        Brush graphBrushHue = new SolidBrush(Color.Red);
        Brush graphBrushSaturation = new SolidBrush(Color.Green);
        Brush graphBrushValue = new SolidBrush(Color.Blue);

        Pen axisPen = new Pen(Color.White);

        // Histogram:

        switch (mode)
        {
          case 3:
          {
            //alt = false;
            kx /= 3f;

            for (int x = 0; x < histArrayHue.Length; x++)
            {
              float yHeightHue = -histArrayHue[x] * ky;
              if (alt && yHeightHue > 3.0)
                yHeightHue = 3.0f;
              gfx.FillRectangle(graphBrushHue, x0 + x * kx, y0 + histArrayHue[x] * ky, kx, yHeightHue);
            }
            // Axes:
            gfx.DrawLine(axisPen, x0, y0, x0 + histArrayHue.Length * kx, y0);
            gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);


            //Scale:
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            //draw scale in Y axis
            float ratio = (float)graph.Height/175.0f;

            //draw label
            gfx.DrawString("Hue", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayHue.Length * kx / 2, y0 + (max) * ky, drawFormat);

            //draw Y axis
            drawFormat.Alignment = StringAlignment.Far;
            drawFormat.LineAlignment = StringAlignment.Center;
            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
            drawFormat.LineAlignment = StringAlignment.Near;

            for (int k = 1; k <= 8; ++k)
            {
              String numberToDisplay = (((int)max) * k/8).ToString();

              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

            }

            //draw scale on X axis
            drawFormat.Alignment = StringAlignment.Center;
            for (int k = 0; k <= 8; ++k)
            {

              gfx.DrawString(histArrayHue.Length * k / 8 + "", new Font("Arial", 3 * ratio), new SolidBrush(Color.White), x0 + histArrayHue.Length * kx * k / 8, y0, drawFormat);

            }
            //gfx.FillRectangle(graphBrushSaturation, x0 + x * kx, y0 + histArraySaturation[x] * ky, kx, yHeight);

            //gfx.DrawString("Hue", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayHue.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);

            //IMPORTANT the scaling changes because Saturation and Value are stored in different sized arrays
            kx = kx * 360 / 100;

            //drawing the labels with the new scale
            gfx.DrawString("Saturation", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + (histArraySaturation.Length/2 + (startPost + 20) / 3.0f) * kx, y0 + (max) * ky, drawFormat);
            gfx.DrawString("Value", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + (histArrayValue.Length/2 + 2 * (startPost + 20) / 3.0f) * kx, y0 + (max) * ky, drawFormat);

            //draw the Saturation histogram

            for (int x = 0; x < histArraySaturation.Length; x++)
            {
              float yHeightSaturation = -histArraySaturation[x] * ky;
              if (alt && yHeightSaturation > 3.0)
                yHeightSaturation = 3.0f;
              gfx.FillRectangle(graphBrushSaturation, x0 + (x + (startPost + 20) / 3.0f) * kx, y0 + histArraySaturation[x] * ky, kx, yHeightSaturation);
            }

            //gfx.FillRectangle(graphBrushValue, x0 + x * kx, y0 + histArrayValue[x] * ky, kx, yHeight);
            for (int x = 0; x < histArrayValue.Length; x++)
            {
              float yHeightValue = -histArrayValue[x] * ky;
              if (alt && yHeightValue > 3.0)
                yHeightValue = 3.0f;
              gfx.FillRectangle(graphBrushValue, x0 + (x + 2 * (startPost + 20) / 3.0f) * kx, y0 + histArrayValue[x] * ky, kx, yHeightValue);
            }
            //Axes for the saturation and value because the column ratio changed being that we have saturation and value as a percentage and hue as 360 degree values
            // or pi radian for other people
            gfx.DrawLine(axisPen, x0 + ((startPost + 20) / 3.0f) * kx, y0, x0 + (histArraySaturation.Length + (startPost + 20) / 3.0f) * kx, y0);
            gfx.DrawLine(axisPen, x0 + ((startPost + 20) / 3.0f) * kx, y0, x0 + ((startPost + 20) / 3.0f) * kx, y0 + max * ky);

            gfx.DrawLine(axisPen, x0 + (2*(startPost + 20) / 3.0f) * kx, y0, x0 + (histArrayValue.Length + 2 * (startPost + 20) / 3.0f) * kx, y0);
            gfx.DrawLine(axisPen, x0 + (2 * (startPost + 20) / 3.0f) * kx, y0, x0 + (2 * (startPost + 20) / 3.0f) * kx, y0 + max * ky);


            //draw the Y axis for the Saturation and the Value
            drawFormat.Alignment = StringAlignment.Far;
            drawFormat.LineAlignment = StringAlignment.Center;
            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + ((startPost + 20) / 3.0f) * kx, y0, drawFormat);
            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + (2*(startPost + 20) / 3.0f) * kx, y0, drawFormat);
            drawFormat.LineAlignment = StringAlignment.Near;

            for (int k = 1; k <= 8; ++k)
            {
              String numberToDisplay = (((int)max) * k/8).ToString();

              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + ((startPost + 20) / 3.0f) * kx, y0 + max * ky * k / 8, drawFormat);
              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + (2 * (startPost + 20) / 3.0f) * kx, y0 + max * ky * k / 8, drawFormat);

            }

            //Draw the X scale for saturation and value
            for (int k = 0; k <= 10; ++k)
            {

              gfx.DrawString((histArraySaturation.Length-1) * k / 10 + "", new Font("Arial", 3 * ratio), new SolidBrush(Color.White), x0 + (histArraySaturation.Length * k / 10 + (startPost + 20) / 3.0f) * kx , y0, drawFormat);
              gfx.DrawString((histArrayValue.Length-1) * k / 10 + "", new Font("Arial", 3 * ratio), new SolidBrush(Color.White), x0 + (histArrayValue.Length * k / 10 + 2 * (startPost + 20) / 3.0f) * kx , y0, drawFormat);

            }

          }
          break;
          case 2://Value
          {
            kx = kx * 360 / 100;
            for (int x = 0; x < histArrayValue.Length; x++)
            {
              float yHeightValue = -histArrayValue[x] * ky;
              if (alt && yHeightValue > 3.0)
                yHeightValue = 3.0f;
              gfx.FillRectangle(graphBrushValue, x0 + x * kx, y0 + histArrayValue[x] * ky, kx, yHeightValue);
            }
            // Axes:
            gfx.DrawLine(axisPen, x0, y0, x0 + histArrayValue.Length * kx, y0);
            gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

            //Scale:
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Far;

            //draw scale in Y axis
            float ratio = (float)graph.Height/175.0f;

            drawFormat.LineAlignment = StringAlignment.Center;
            //draw label
            gfx.DrawString("All RGB values", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayValue.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);
            drawFormat.LineAlignment = StringAlignment.Near;

            //draw Y axis

            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
            for (int k = 1; k <= 8; ++k)
            {
              String numberToDisplay = (((int)max) * k/8).ToString();

              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

            }

            //draw scale on X axis
            drawFormat.Alignment = StringAlignment.Center;
            for (int k = 0; k <= 10; ++k)
            {

              gfx.DrawString(histArrayValue.Length * k / 10 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + histArrayValue.Length * kx * k / 10, y0, drawFormat);

            }

          }
          break;
          case 1://Saturation
          {
            kx = kx * 360 / 100;
            for (int x = 0; x < histArraySaturation.Length; x++)
            {
              float yHeightSaturation = -histArraySaturation[x] * ky;
              if (alt && yHeightSaturation > 3.0)
                yHeightSaturation = 3.0f;

              gfx.FillRectangle(graphBrushSaturation, x0 + x * kx, y0 + histArraySaturation[x] * ky, kx, yHeightSaturation);
            }
            // Axes:
            gfx.DrawLine(axisPen, x0, y0, x0 + histArraySaturation.Length * kx, y0);
            gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);


            //Scale:
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Far;

            //draw scale in Y axis
            float ratio = (float)graph.Height/175.0f;

            //draw label
            gfx.DrawString("All RGB values", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArraySaturation.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);

            drawFormat.LineAlignment = StringAlignment.Center;

            //draw Y axis
            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
            drawFormat.LineAlignment = StringAlignment.Near;

            for (int k = 1; k <= 10; ++k)
            {
              String numberToDisplay = (((int)max) * k/8).ToString();

              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 10, drawFormat);

            }

            //draw scale on X axis
            drawFormat.Alignment = StringAlignment.Center;
            for (int k = 0; k <= 10; ++k)
            {

              gfx.DrawString(histArraySaturation.Length * k / 10 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + histArraySaturation.Length * kx * k / 10, y0, drawFormat);

            }
          }
          break;
          case 0://Hue
          {
            for (int x = 0; x < histArrayHue.Length; x++)
            {
              float yHeightHue = -histArrayHue[x] * ky;
              if (alt && yHeightHue > 3.0)
                yHeightHue = 3.0f;
              gfx.FillRectangle(graphBrushHue, x0 + x * kx, y0 + histArrayHue[x] * ky, kx, yHeightHue);
            }
            // Axes:
            gfx.DrawLine(axisPen, x0, y0, x0 + histArrayHue.Length * kx, y0);
            gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

            //Scale:
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Far;

            //draw scale in Y axis
            float ratio = (float)graph.Height/175.0f;

            //draw label
            gfx.DrawString("All RGB values", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0 + histArrayHue.Length * kx / 2, y0 + (max + 500) * ky, drawFormat);

            //draw Y axis
            drawFormat.LineAlignment = StringAlignment.Center;
            gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
            drawFormat.LineAlignment = StringAlignment.Near;
            for (int k = 1; k <= 8; ++k)
            {
              String numberToDisplay = (((int)max) * k/8).ToString();

              gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

            }

            //draw scale on X axis
            drawFormat.Alignment = StringAlignment.Center;
            for (int k = 0; k <= 8; ++k)
            {

              gfx.DrawString(histArrayHue.Length * k / 8 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + histArrayHue.Length * kx * k / 8, y0, drawFormat);

            }
          }
          break;

        }


      }
    }

    public static void DrawHistogramSelected (Bitmap graph)
    {
      if (histArraySelected == null)
        return;

      float max = 0.0f;
      foreach (int f in histArraySelected)
        if (f > max)
          max = f;

      using (Graphics gfx = Graphics.FromImage(graph))
      {
        gfx.Clear(Color.Black);

        // Graph scaling:
        float x0 = graph.Width * 0.05f;
        float y0 = graph.Height * 0.95f;
        float kx = graph.Width * 0.9f / histArraySelected.Length;
        float ky = -graph.Height * 0.9f / max;

        // Pens:
        Color c = mode == 0 ? Color.Red : (mode == 1 ? Color.DarkGreen : (mode == 2 ? Color.Blue : Color.Gray));
        Pen graphPen = new Pen(c);
        Brush graphBrush = new SolidBrush(c);
        Pen axisPen = new Pen(Color.White);

        // Histogram:
        for (int x = 0; x < histArraySelected.Length; x++)
        {
          float yHeight = -histArraySelected[x] * ky;
          if (alt && yHeight > 3.0)
            yHeight = 3.0f;
          gfx.FillRectangle(graphBrush, x0 + x * kx, y0 + histArraySelected[x] * ky, kx, yHeight);
        }

        // Axes:
        gfx.DrawLine(axisPen, x0, y0, x0 + histArraySelected.Length * kx, y0);
        gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

        //Scale:
        StringFormat drawFormat = new StringFormat();
        drawFormat.Alignment = StringAlignment.Far;
        drawFormat.LineAlignment = StringAlignment.Center;
        //draw scale in Y axis
        float ratio = (float)graph.Height/175.0f;

        //draw Y axis
        gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 , drawFormat);
        drawFormat.LineAlignment = StringAlignment.Near;
        for (int k = 1; k <= 8; ++k)
        {
          String numberToDisplay = (((int)max) * k/8).ToString();

          gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

        }

        //draw scale on X axis
        drawFormat.Alignment = StringAlignment.Center;
        for (int k = 0; k <= 15; ++k)
        {

          gfx.DrawString(255 * k / 15 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + 255 * kx * k / 15, y0, drawFormat);

        }
      }
    }

    public static void DrawHistogram (
      Bitmap graph)
    {
      if (histArray == null)
        return;

      float max = 0.0f;
      foreach (int f in histArray)
        if (f > max)
          max = f;

      using (Graphics gfx = Graphics.FromImage(graph))
      {
        gfx.Clear(Color.Black);

        // Graph scaling:
        float x0 = graph.Width * 0.05f;
        float y0 = graph.Height * 0.95f;
        float kx = graph.Width * 0.9f / histArray.Length;
        float ky = -graph.Height * 0.9f / max;

        // Pens:
        Color c = mode == 0 ? Color.Red : (mode == 1 ? Color.DarkGreen : (mode == 2 ? Color.Blue : Color.Gray));
        Pen graphPen = new Pen(c);
        Brush graphBrush = new SolidBrush(c);
        Pen axisPen = new Pen(Color.White);

        // Histogram:
        for (int x = 0; x < histArray.Length; x++)
        {
          float yHeight = -histArray[x] * ky;
          if (alt && yHeight > 3.0)
            yHeight = 3.0f;
          gfx.FillRectangle(graphBrush, x0 + x * kx, y0 + histArray[x] * ky, kx, yHeight);
        }

        // Axes:
        gfx.DrawLine(axisPen, x0, y0, x0 + histArray.Length * kx, y0);
        gfx.DrawLine(axisPen, x0, y0, x0, y0 + max * ky);

        //Scale:
        StringFormat drawFormat = new StringFormat();
        drawFormat.Alignment = StringAlignment.Far;

        //draw scale in Y axis
        float ratio = (float)graph.Height/175.0f;


        //draw Y axis
        drawFormat.LineAlignment = StringAlignment.Center;
        gfx.DrawString("0 - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0, drawFormat);
        drawFormat.LineAlignment = StringAlignment.Near;

        for (int k = 1; k <= 8; ++k)
        {
          String numberToDisplay = (((int)max) * k/8).ToString();

          gfx.DrawString(numberToDisplay + " - ", new Font("Arial", 5 * ratio / 1.5f), new SolidBrush(Color.White), x0, y0 + max * ky * k / 8, drawFormat);

        }

        //draw scale on X axis
        drawFormat.Alignment = StringAlignment.Center;
        for (int k = 0; k <= 8; ++k)
        {

          gfx.DrawString(255 * k / 8 + "", new Font("Arial", 5 * ratio), new SolidBrush(Color.White), x0 + 255 * kx * k / 8, y0, drawFormat);

        }
      }
    }

    /// <summary>
    /// Recomputes image histogram and draws the result in the given raster image.
    /// </summary>
    /// <param name="input">Input image.</param>
    /// <param name="param">Textual parameter.</param>
    ///

    public static void ComputeHSVHistogram (Bitmap input, string param,int StartX,int StartY, int EndX, int Endy)
    {
      alt = param.IndexOf("alt") >= 0;
      separateHSV = param.IndexOf("separate") >= 0;

      // Text parameters:
      param = param.ToLower().Trim();

      // Histogram mode (0 .. hue, 1 .. saturation, 2 .. value, 3 .. all)
      mode = 3;
      if (param.IndexOf("hue") >= 0)
        mode = 0;
      if (param.IndexOf("saturation") >= 0)
        mode = 1;
      if (param.IndexOf("value") >= 0)
        mode = 2;


      //double H, S, V;
      //Arith.ColorToHSV( col, out H, out S, out V );

      histArrayHue = new int[361];
      histArraySaturation = new int[101];
      histArrayValue = new int[101];
      int x =0 , y = 0;
      int width = input.Width;
      int height = input.Height;
      if (StartX != -1)
      {
        width = EndX;
        height = Endy;
        x = StartX;
        y = StartY;
      }
      
      for (; y < height; y++)
      {
        for (; x < width; x++)
        {
          Color col = input.GetPixel( x, y );

          double H, S, V;
          Arith.ColorToHSV(col, out H, out S, out V);
          switch (mode)
          {
            case 3:
              histArrayHue[(int)(Math.Round(H))]++;
              histArraySaturation[(int)(Math.Round(S * 100.0f))]++;
              histArrayValue[(int)(Math.Round(V * 100.0f))]++;
              break;
            case 2:
              histArrayValue[(int)(Math.Round(V * 100.0f))]++;
              break;
            case 1:
              histArraySaturation[(int)(Math.Round(S * 100.0f))]++;
              break;
            case 0:
              histArrayHue[(int)(Math.Round(H))]++;
              break;

          }

        }
      }
      if (mode == 0 || mode == 3)
      {
        histArrayHue[0] += histArrayHue[360];
        Array.Resize(ref histArrayHue, histArrayHue.Length - 1);
      }
    }

    public static void ComputeHistogramAll (
      Bitmap input, string param)
    {
      // Text parameters:
      param = param.ToLower().Trim();


      // Sorted histogram:
      //bool sort = param.IndexOf("sort") >= 0;

      // Graph appearance:
      alt = param.IndexOf("alt") >= 0;

      // Separete Red, Green, Blue histogram or all together
      separateAll = param.IndexOf("separate") >= 0;

      int x, y;

      // 1. Histogram recomputation.
      histArray = new int[256];
      histArrayBlue = new int[256];
      histArrayGreen = new int[256];
      histArrayRed = new int[256];
      histArrayGray = new int[256];

      int width = input.Width;
      int height = input.Height;
      for (y = 0; y < height; y++)
      {
        for (x = 0; x < width; x++)
        {
          Color col = input.GetPixel( x, y );

          histArrayBlue[col.B]++;
          histArrayRed[col.R]++;
          histArrayGreen[col.G]++;
          histArrayGray[Draw.RgbToGray(col.R, col.G, col.B)]++;
        }
      }
      if (!separateAll)
      {
        histArrayCyan = new int[256];
        histArrayYellow = new int[256];
        histArrayMagenta = new int[256];
        histArrayWhite = new int[256];

        for (x = 0; x < histArray.Length; ++x)
        {
          histArrayYellow[x] = Math.Min(histArrayRed[x], histArrayGreen[x]);
          histArrayCyan[x] = Math.Min(histArrayGreen[x], histArrayBlue[x]);
          histArrayMagenta[x] = Math.Min(histArrayRed[x], histArrayBlue[x]);
          histArrayWhite[x] = Math.Min(Math.Min(histArrayBlue[x], histArrayRed[x]), histArrayGreen[x]);
        }
      }
      // 2. optional sorting.
      // if (sort)
      //Array.Sort(histArray, new ReverseComparer<int>().Compare);
    }

    public static void ComputeHistogramSelected (Bitmap input, int StartX, int StartY, int EndX, int Endy, string param)
    {
      param = param.ToLower().Trim();

      // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray, 4 .. all)
      mode = 4;
      if (param.IndexOf("red") >= 0)
        mode = 0;
      if (param.IndexOf("green") >= 0)
        mode = 1;
      if (param.IndexOf("blue") >= 0)
        mode = 2;
      if (param.IndexOf("gray") >= 0)
        mode = 3;

      // Sorted histogram:
      bool sort = param.IndexOf("sort") >= 0;

      // Graph appearance:
      alt = param.IndexOf("alt") >= 0;

      int x, y;

      histArraySelected = new int[256];
      for (y = StartY; y <= Endy; ++y)
      {
        for (x = StartX; x <= EndX; ++x)
        {
          Color col = input.GetPixel(x,y);
          if (mode == 0)
          {
            histArraySelected[col.R]++;
          }
          else if (mode == 1)
          {
            histArraySelected[col.G]++;

          }
          else if (mode == 2)
          {
            histArraySelected[col.B]++;

          }
          else if (mode == 3)
          {
            histArraySelected[Draw.RgbToGray(col.R, col.G, col.B)]++;

          }
          else if (mode == 4)
          {
            histArraySelected[Draw.RgbToGray(col.R, col.G, col.B)]++;

          }
        }
      }
    }


    public static void ComputeHistogram (
      Bitmap input,
      string param)
    {
      // Text parameters:
      param = param.ToLower().Trim();

      // Histogram mode (0 .. red, 1 .. green, 2 .. blue, 3 .. gray)
      mode = 3;
      if (param.IndexOf("red") >= 0)
        mode = 0;
      if (param.IndexOf("green") >= 0)
        mode = 1;
      if (param.IndexOf("blue") >= 0)
        mode = 2;

      // Sorted histogram:
      bool sort = param.IndexOf("sort") >= 0;

      // Graph appearance:
      alt = param.IndexOf("alt") >= 0;

      int x, y;

      // 1. Histogram recomputation.
      histArray = new int[256];

      int width = input.Width;
      int height = input.Height;
      for (y = 0; y < height; y++)
        for (x = 0; x < width; x++)
        {
          Color col = input.GetPixel( x, y );
          int Y = Draw.RgbToGray( col.R, col.G, col.B );

          //double H, S, V;
          //Arith.ColorToHSV( col, out H, out S, out V );

          histArray[mode == 0 ? col.R : (mode == 1 ? col.G : (mode == 2 ? col.B : Y))]++;
        }

      // 2. optional sorting.
      if (sort)
        Array.Sort(histArray, new ReverseComparer<int>().Compare);
    }
  }
}
