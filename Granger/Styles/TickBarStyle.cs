using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Granger
{
    public class TickBarStyle : TickBar
    {
		protected override void OnRender(DrawingContext DrawingContext)
        {
			if (Ticks == null || Ticks.Count == 0) return;

			double Offset = ReservedSpace / 2;

			double SizeSpacing = ActualWidth - ReservedSpace;
			double ValueSize = SizeSpacing / (Maximum - Minimum);

			int TickCount = (int)(Maximum - Minimum / TickFrequency) + 1;

			for (int o = 0; o < Ticks.Count; o++)
			{
				for (int i = 0; i <= TickCount; i++)
                {
					if (Ticks[o] == i)
                    {
						double X = Offset + (Ticks[o] - Minimum) * ValueSize;
						double Y = -4;

						switch (Ticks.Count)
						{
							case 2:
								switch (o)
								{
									case 0:
										Y = -22;

										break;

									case 1:
										Y = 2;

										break;
								}

								break;
						}

						string Text = Ticks[o].ToString();

						if (Text.Length == 2)
						{
							X -= 7;
						}
						else if (Text.Length == 3)
						{
							X -= 14;
						}
						else if (Text.Length == 4)
						{
							X -= 21;
						}

                        var FormattedText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Consolas"), 12, Brushes.Gray, VisualTreeHelper.GetDpi(this).PixelsPerDip);

                        DrawingContext.DrawText(FormattedText, new Point(X, Y));
                    }
				}
			}
		}
    }
}
