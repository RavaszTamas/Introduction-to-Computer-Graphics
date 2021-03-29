using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Utilities;

namespace Modules
{
  public class SelectPixelModuleHistogram : DefaultRasterModule
  {

    private int StartX=-1;
    private int StartY =-1;
    private int EndX=-1;
    private int EndY =-1;
    private int heightOfBox = -1;
    private int widthOfBox = -1;
    private bool selected = false;
    /// <summary>
    /// Mandatory plain constructor.
    /// </summary>
    public SelectPixelModuleHistogram ()
    {}

    /// <summary>
    /// Author's full name.
    /// </summary>
    public override string Author => "Ravasz Tamás";

    /// <summary>
    /// Name of the module (short enough to fit inside a list-boxes, etc.).
    /// </summary>
    public override string Name => "SelectPixel Histogram";

    /// <summary>
    /// Tooltip for Param (text parameters).
    /// </summary>
    public override string Tooltip => "\"nxm\" <=>{(n,m), where n and m are natural numbers  + color, e.g. \"10x10, red\"\nA CTRL+LeftClick action must performed on the picture to make any calculations";

    /// <summary>
    /// Default mode - gray.
    /// </summary>
    protected string param = "10x20, gray";

    /// <summary>
    /// Current 'Param' string is stored in the module.
    /// Set reasonable initial value.
    /// </summary>
    public override string Param
    {
      get => param;
      set
      {
        if (value != param)
        {
          param = value;
          dirty = true;     // to recompute histogram table

          recompute();
        }
      }
    }

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of inputs).
    /// </summary>
    public override int InputSlots => 1;

    /// <summary>
    /// Usually read-only, optionally writable (client is defining number of outputs).
    /// </summary>
    public override int OutputSlots => 0;

    /// <summary>
    /// Input raster image.
    /// </summary>
    protected Bitmap inImage = null;

    /// <summary>
    /// True if the histogram needs recomputation.
    /// </summary>
    protected bool dirty = true;

    /// <summary>
    /// Active histogram view window.
    /// </summary>
    protected HistogramForm hForm = null;

    /// <summary>
    /// Interior (visualization) of the hForm.
    /// </summary>
    protected Bitmap hImage = null;

    protected void recompute ()
    {
      if (hForm   != null &&
          inImage != null)
      {
        hImage = new Bitmap(hForm.ClientSize.Width, hForm.ClientSize.Height, PixelFormat.Format24bppRgb);
        if (dirty && selected)
        {
          ImageHistogram.ComputeHistogramSelected(inImage,StartX,StartY,EndX,EndY, Param);
          dirty = false;
        }
        ImageHistogram.DrawHistogramSelected(hImage);
        hForm.SetResult(hImage);
      }
    }

    /// <summary>
    /// Returns true if there is an active GUI window associted with this module.
    /// Open/close GUI window using the setter.
    /// </summary>
    public override bool GuiWindow
    {
      get => hForm != null;
      set
      {
        if (value)
        {
          // Show GUI window.
          if (hForm == null)
          {
            hForm = new HistogramForm(this);
            hForm.Show();
          }

          recompute();
        }
        else
        {
          if(hForm != null)
            hForm.Hide();
          hForm = null;
        }
      }
    }

    /// <summary>
    /// Assigns an input raster image to the given slot.
    /// Doesn't start computation (see #Update for this).
    /// </summary>
    /// <param name="inputImage">Input raster image (can be null).</param>
    /// <param name="slot">Slot number from 0 to InputSlots-1.</param>
    public override void SetInput (
      Bitmap inputImage,
      int slot = 0)
    {
      dirty   = inImage != inputImage;     // to recompute histogram table
      if (dirty)
        selected = false;
      inImage = inputImage;

      recompute();
    }

    /// <summary>
    /// Recompute the output image[s] according to input image[s].
    /// Blocking (synchronous) function.
    /// #GetOutput() functions can be called after that.
    /// </summary>
    public override void Update ()
    {
      recompute();
    }

    /// <summary>
    /// PixelUpdate() is called after every user interaction.
    /// </summary>
    public override bool HasPixelUpdate => true;

    /// <summary>
    /// Optional action performed at the given pixel.
    /// Blocking (synchronous) function.
    /// Logically equivalent to Update() but with potential local effect.
    /// </summary>
    public override void PixelUpdate (
      int x,
      int y)
    {
      if (inImage != null)
      {
        this.selected = false;
        String[] spearator = { "x", ", " };
        String[] paramForSize = param.Split(spearator,3,StringSplitOptions.RemoveEmptyEntries);

        if (paramForSize.Length != 3)
          return;

        if (int.TryParse(paramForSize[1], out heightOfBox) && int.TryParse(paramForSize[0], out widthOfBox))
        {
          this.StartX = x - widthOfBox/2;
          if (StartX < 0)
            StartX = 0;

          this.StartY = y - heightOfBox/2;
          if (StartY < 0)
            StartY = 0;

          this.EndX = x + widthOfBox/2;

          if (EndX > inImage.Width)
            EndX = inImage.Width;

          this.EndY = y + heightOfBox/2;

          if (EndY > inImage.Height)
            EndY = inImage.Height;

          this.selected = true;
          this.dirty = true;
        }

      }
      recompute();
    }

    /// <summary>
    /// Notification: GUI window has been closed.
    /// </summary>
    public override void OnGuiWindowClose ()
    {
      this.selected = false;
      hForm = null;
    }
  }
}
