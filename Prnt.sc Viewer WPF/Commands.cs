using System.Windows.Input;

namespace TehGM.PrntScViewer.WPF
{
    public class Commands
    {
        public static readonly RoutedUICommand Copy = new RoutedUICommand();
        public static readonly RoutedUICommand Save = new RoutedUICommand();
        public static readonly RoutedUICommand Reset = new RoutedUICommand();
        public static readonly RoutedUICommand Next = new RoutedUICommand();
        public static readonly RoutedUICommand Previous = new RoutedUICommand();
    }
}
