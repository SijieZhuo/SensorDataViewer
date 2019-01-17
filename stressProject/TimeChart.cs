using MindFusion.RealTimeCharting.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace stressProject
{
    public class TimeChart
    {
        int clusterSize = 1;
        int currCount = 0;

        string title, yTitle;
        int yOrigin, yLength, yInterval, width, height;

        RealTimeChart chart;

        public TimeChart(int width, int height, string title, string yTitle, int yOrigin, int yLength, int yInterval) {
            this.title = title;
            this.yTitle = yTitle;
            this.yOrigin = yOrigin;
            this.yLength = yLength;
            this.yInterval = yInterval;
            this.width = width;
            this.height = height;

            TimeChartSetUp();
        }


        public void TimeChartSetUp() {
            chart = new RealTimeChart();
            chart.Width = width;
            chart.Height = height;
            chart.FastScrollMode = true;
            chart.TitleText = title;

            chart.TooltipVisibility = Visibility.Visible;
            chart.ShowFallbackTooltip = true;

            chart.XAxis.Title = "Time";
            chart.XAxis.PinLabels = false;
            chart.XAxis.Length = 10;
            chart.XAxis.Interval = 1;


            Axis yAxis = new Axis();
            yAxis.Origin = yOrigin;
            yAxis.Length = yLength;
            yAxis.Interval = yInterval;
            yAxis.Title = yTitle;
            yAxis.TitleRotationAngle = -90.0;
            yAxis.TitleFontFamily = new FontFamily("Verdana");
            yAxis.LabelFontFamily = new FontFamily("Verdana");
            yAxis.TickLength = 5;
            yAxis.TitleOffset = 10;
            chart.YAxisCollection.Add(yAxis);


            Series series1 = new Series(chart.YAxisCollection[0])
            {
                Stroke = new SolidColorBrush(Color.FromRgb(166, 46, 68)),
                Title = "GSR",
                ScatterType = ScatterType.None,
                TitleFontFamily = new FontFamily("Verdana"),
                TitleFontSize = 12

            };

            chart.SeriesCollection.Add(series1);

            chart.TooltipAxis = chart.YAxisCollection[0];

            
        }



        public void updateShimmerChart(double x, double y)
        {
            chart.FastScrollMode = true;
            Point[] points1 = new Point[clusterSize];

            double minNewX = currCount- 2000;
            
            points1[0] = new Point(x, y);
            currCount++;
            Debug.WriteLine(currCount + "  " + x);
            Series series = chart.SeriesCollection.ElementAt(0);
            series.Data.AddRange(points1);

            chart.Commit();

            DisposeOldData(chart);



        }

        private void DisposeOldData(RealTimeChart chart)
        {
            Series series = chart.SeriesCollection.ElementAt(0);
            if (series.Data.Count > 500 &&
                series.Data[499].X < chart.XAxis.Origin)
                series.Data.RemoveRange(0, 500);
        }



        public RealTimeChart GetTimeChart() {
            return chart;
        }

    }
}
