using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RubikDemo;

namespace Rubik.TestGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<WeakReference<RubikViewModel>> viewModels = new List<WeakReference<RubikViewModel>>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new RubikViewModel();
            viewModels.Add(new WeakReference<RubikViewModel>((RubikViewModel) DataContext));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new RubikViewModel();
            viewModels.Add(new WeakReference<RubikViewModel>((RubikViewModel)DataContext));
        }
    }
}