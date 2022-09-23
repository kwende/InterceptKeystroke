using System.Windows;
using System.Windows.Input;

namespace InterceptKeystroke
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _counter = 0;
        private HookKeyboard _hook;

        public MainWindow()
        {
            InitializeComponent();

            _hook = new HookKeyboard(Key.OemMinus, (code, wParam, lParam) =>
            {
                _counter++;
                Dispatcher.Invoke(() =>
                {
                    TimesPressed.Content = $"Pressed {_counter} times.";
                });
            });

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _hook?.Dispose();
        }
    }
}
