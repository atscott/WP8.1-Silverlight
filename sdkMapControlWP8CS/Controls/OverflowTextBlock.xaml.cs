using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace sdkMapControlWP8CS.Controls
{
    public partial class OverflowTextBlock : UserControl
    {
        public static readonly DependencyProperty AnimatablOffsetProperty = DependencyProperty.Register("AnimatableOffset",
            typeof(double), typeof(OverflowTextBlock), new PropertyMetadata(AnimatableOffsetPropertyChanged));

        public double AnimatableOffset
        {
            get { return (double)GetValue(AnimatablOffsetProperty); }
            set { SetValue(AnimatablOffsetProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(OverflowTextBlock),
            new PropertyMetadata(string.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        new public static readonly DependencyProperty StyleProperty =
            DependencyProperty.Register("Style", typeof(Style), typeof(OverflowTextBlock), null);

        new public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        private DispatcherTimer delayTimer = new DispatcherTimer();

        public OverflowTextBlock()
        {
            InitializeComponent();
            OuterScrollViewer.MouseLeftButtonUp += OuterScrollViewer_MouseLeftButtonUp;
            OuterScrollViewer.MouseLeftButtonDown += OuterScrollViewer_MouseLeftButtonDown;
            OuterScrollViewer.ManipulationMode = ManipulationMode.Control;
            delayTimer.Interval = new TimeSpan(0, 0, 1);
            delayTimer.Tick += (s, ev) =>
            {
                var sb = new Storyboard();

                var animation = new DoubleAnimation
                {
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 400)),
                    From = OuterScrollViewer.HorizontalOffset,
                    To = 0,
                    EasingFunction = new SineEase()
                };
                Storyboard.SetTarget(animation, this);
                Storyboard.SetTargetProperty(animation, new PropertyPath("(AnimatableHorizontalScrollViewer.AnimatableOffset)"));

                sb.Children.Add(animation);

                sb.Begin();
                delayTimer.Stop();
            };
            AnimatableOffset = OuterScrollViewer.HorizontalOffset;
        }

        private void OuterScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            delayTimer.Start();
        }

        private void OuterScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            delayTimer.Stop();
        }

        private static void AnimatableOffsetPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            OverflowTextBlock cThis = sender as OverflowTextBlock;
            cThis.OuterScrollViewer.ScrollToHorizontalOffset((double)args.NewValue);
        }

    }
}
