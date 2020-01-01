using System.Windows.Controls;
using ICOCrawler.Medium.ViewModel;

namespace ICOCrawler.Medium.View
{
    /// <summary>
    /// Interaction logic for MediumView.xaml
    /// </summary>
    public partial class MediumView : UserControl
    {
        public MediumView()
        {
            InitializeComponent();
            var mediumViewModel = new MediumViewModel();
            DataContext = mediumViewModel;
        }
    }
}
