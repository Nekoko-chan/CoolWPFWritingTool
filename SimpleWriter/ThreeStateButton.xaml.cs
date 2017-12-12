using System.Windows;
using System.Windows.Media;

namespace ComplexWriter
{
    /// <summary>
    /// Interaction logic for ThreeStateButton.xaml
    /// </summary>
    public partial class ThreeStateButton
    {
        public ThreeStateButton()
        {
            InitializeComponent();
        }

        public static readonly RoutedEvent ButtonStateChangedEvent = EventManager.RegisterRoutedEvent(
          "ButtonStateChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ThreeStateButton));

        public event RoutedEventHandler ButtonStateChanged
        {
            add { AddHandler(ButtonStateChangedEvent, value); }
            remove { RemoveHandler(ButtonStateChangedEvent, value); }
        }

        private void RaiseButtonStateChangedEvent()
        {
            var newEventArgs = new RoutedEventArgs(ButtonStateChangedEvent);
            RaiseEvent(newEventArgs);
        }

        public static DependencyProperty ButtonStateProperty = DependencyProperty.Register("ButtonState", typeof(ButtonState), typeof(ThreeStateButton),new PropertyMetadata(ButtonState.State1,RaiseChangeEvent));

        private static void RaiseChangeEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
                ((ThreeStateButton)d).RaiseButtonStateChangedEvent();
        }

        [System.ComponentModel.Category("ButtonSpecific")]
        public ButtonState ButtonState
        {
            get { return (ButtonState)GetValue(ButtonStateProperty); }
            set { SetValue(ButtonStateProperty, value); }
        }

        public static DependencyProperty ButtonMaskState1Property = DependencyProperty.Register("ButtonMaskState1", typeof(ImageSource), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public ImageSource ButtonMaskState1
        {
            get { return (ImageSource)GetValue(ButtonMaskState1Property); }
            set { SetValue(ButtonMaskState1Property, value); }
        }
        public static DependencyProperty ButtonMaskState2Property = DependencyProperty.Register("ButtonMaskState2", typeof(ImageSource), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public ImageSource ButtonMaskState2
        {
            get { return (ImageSource)GetValue(ButtonMaskState2Property); }
            set { SetValue(ButtonMaskState2Property, value); }
        }

        public static DependencyProperty ButtonMaskState3Property = DependencyProperty.Register("ButtonMaskState3", typeof(ImageSource), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public ImageSource ButtonMaskState3
        {
            get { return (ImageSource)GetValue(ButtonMaskState3Property); }
            set { SetValue(ButtonMaskState3Property, value); }
        }
        public static DependencyProperty ButtonMaskState4Property = DependencyProperty.Register("ButtonMaskState4", typeof(ImageSource), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public ImageSource ButtonMaskState4
        {
            get { return (ImageSource)GetValue(ButtonMaskState4Property); }
            set { SetValue(ButtonMaskState4Property, value); }
        }

        public static DependencyProperty ButtonMaskState5Property = DependencyProperty.Register("ButtonMaskState5", typeof(ImageSource), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public ImageSource ButtonMaskState5
        {
            get { return (ImageSource)GetValue(ButtonMaskState5Property); }
            set { SetValue(ButtonMaskState5Property, value); }
        }

        public static DependencyProperty ButtonColorProperty = DependencyProperty.Register("ButtonColor", typeof(Brush), typeof(ThreeStateButton));

        [System.ComponentModel.Category("ButtonSpecific")]
        public Brush ButtonColor
        {
            get { return (Brush)GetValue(ButtonColorProperty); }
            set { SetValue(ButtonColorProperty, value); }
        }

        public static readonly DependencyProperty DefaultOpacityProperty =
          DependencyProperty.Register("DefaultOpacity", typeof(double), typeof(ThreeStateButton),
              new PropertyMetadata(0.9d));

        public double DefaultOpacity
        {
            get { return (double)GetValue(DefaultOpacityProperty); }
            set { SetValue(DefaultOpacityProperty, value); }
        }

        public static readonly DependencyProperty DisabledOpacityProperty =
         DependencyProperty.Register("DisabledOpacity", typeof(double), typeof(ThreeStateButton),
             new PropertyMetadata(0.5d));

        public double DisabledOpacity
        {
            get { return (double)GetValue(DisabledOpacityProperty); }
            set { SetValue(DisabledOpacityProperty, value); }
        }

        protected override void OnClick()
        {
            switch (ButtonState)
            {
                case ButtonState.State1:
                    ButtonState = ButtonState.State2;
                    break;
                case ButtonState.State2:
                    ButtonState = ButtonState.State3;
                    break;
                case ButtonState.State3:
                    ButtonState = ButtonState.State4;
                    break;
                case ButtonState.State4:
                    ButtonState = ButtonState.State5;
                    break;
                default:
                    ButtonState = ButtonState.State1;
                    break;
            }
            base.OnClick();
        }
    }


    // ComplexWriter.ButtonState
    public enum ButtonState
    {
        State1, State2, State3, State4, State5
    }
}
