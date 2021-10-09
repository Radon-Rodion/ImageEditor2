using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace SPP_3
{
    class CanvasController
    {
        private Canvas canvas;
        public CanvasController(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void CheckAndAdd(Shape shape)
        {
            if (!canvas.Children.Contains(shape)) canvas.Children.Add(shape);
        }

        public void StartPenDrawing(Point startPoint)
        {
            new PenDrawer(startPoint);
        }

        public void LineToPoint(Point point, Brush brush, double lineWidth)
        {
            PenDrawer drawer = PenDrawer.GetPenDrawer();
            Line line = drawer.LineTo(point);
            line.Stroke = brush;
            line.Fill = brush;
            line.StrokeThickness = lineWidth;
            canvas.Children.Add(line);
        }

        public void StartSelection(Point firstPoint)
        {
            SelectionRect selRect = new SelectionRect(firstPoint);
            canvas.Children.Add(selRect.GetRectFigure());
        }

        public void ContinueSelection(Point tempSecondPoint)
        {
            SelectionRect selRect = SelectionRect.GetSelectionRect();
            selRect.SetSecondPoint(tempSecondPoint);
        }

        public Int32Rect FinishSelection()
        {
            SelectionRect selRect = SelectionRect.GetSelectionRect();
            canvas.Children.Remove(selRect.GetRectFigure());
            return selRect.GetSelectionArea();
        }

        private class SelectionRect
        {
            private static SelectionRect selectionRect;
            private Rectangle rect;
            double xFirst, yFirst;

            public SelectionRect(Point firstPoint)
            {
                this.xFirst = firstPoint.X;
                this.yFirst = firstPoint.Y;

                rect = new Rectangle();
                ChangeRectangle(xFirst, yFirst, 0, 0);
                rect.Stroke = Brushes.Black;
                rect.StrokeThickness = 3;

                selectionRect = this;
            }
            public static SelectionRect GetSelectionRect()
            {
                return selectionRect;
            }

            public void SetSecondPoint(Point secondPoint)
            {
                double xLeft = secondPoint.X > xFirst ? xFirst : secondPoint.X;
                double yTop = secondPoint.Y > yFirst ? yFirst : secondPoint.Y;

                double width = Math.Abs(secondPoint.X - xFirst);
                double height = Math.Abs(secondPoint.Y - yFirst);
                ChangeRectangle(xLeft, yTop, width, height);
            }

            private void ChangeRectangle(double xLeft, double yTop, double width, double height)
            {
                Canvas.SetLeft(rect, xLeft);
                Canvas.SetTop(rect, yTop);
                rect.Width = width;
                rect.Height = height;
            }

            public Rectangle GetRectFigure()
            {
                return rect;
            }

            public Int32Rect GetSelectionArea()
            {
                int x = System.Convert.ToInt32(Canvas.GetLeft(rect));
                int y = System.Convert.ToInt32(Canvas.GetTop(rect));
                int width = System.Convert.ToInt32(rect.Width);
                int height = System.Convert.ToInt32(rect.Height);

                Int32Rect area = new Int32Rect(x, y, width, height);
                return area;
            }
        }

        private class PenDrawer
        {
            static PenDrawer drawer;
            Point prevPoint;

            public PenDrawer(Point point)
            {
                prevPoint = point;
                drawer = this;
            }

            public static PenDrawer GetPenDrawer()
            {
                return drawer;
            }

            public Line LineTo(Point nextPoint)
            {
                Line line = new Line();
                line.X1 = prevPoint.X;
                line.Y1 = prevPoint.Y;
                line.X2 = nextPoint.X;
                line.Y2 = nextPoint.Y;

                prevPoint = nextPoint;
                return line;
            }
        }
    }
}
