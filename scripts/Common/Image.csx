#r "System.Drawing"

using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

        string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        Color getDominantColor(Bitmap bitmap)
        {

            Color pixelColor;
            Color c = Color.Black;
            int r, g, b;

            for (int row = 0; row < bitmap.Size.Width; row++)
            {
                for (int col = 0; col < bitmap.Size.Height; col++)
                {
                    pixelColor = bitmap.GetPixel(row, col);
                    if (row == 0 && col == 0)
                        c = pixelColor;
                    else
                    {

                        r = (int)interpolate(c.R, pixelColor.R, 1, 2);
                        g = (int)interpolate(c.G, pixelColor.G, 1, 2);
                        b = (int)interpolate(c.B, pixelColor.B, 1, 2);

                        c = Color.FromArgb(r, g, b);


                    }
                }
            } 
            return c;
        }




        double interpolate(double startValue, double endValue, double stepNumber, double lastStepNumber)
        {
            //Console.WriteLine("startValue :" + startValue + " endValue: " + endValue + " stepNumber: " + stepNumber + " lastStepNumber: " + lastStepNumber);
            return (endValue - startValue) * stepNumber / lastStepNumber + startValue;
        }