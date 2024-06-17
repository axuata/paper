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
using System.Windows.Shapes;

namespace Paper.dialogs {
  /// <summary>
  /// FontSelectionDialog.xaml の相互作用ロジック
  /// </summary>
  public partial class FontSelectionDialog {
    public List<FontFamily> FontFamilies {
      get; private set;
    }
    public FontFamily? SelectedFontFamily {
      get; set;
    }
    public double SelectedFontSize {
      get; set;
    }

    public FontSelectionDialog() {
      InitializeComponent();

      FontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
      DataContext = this;
      ComboBox_FontFamily.SelectedItem = FontFamilies.FirstOrDefault(ff => ff.Source.Contains("Yu Gothic UI")) ?? FontFamilies.FirstOrDefault();
      SelectedFontFamily = FontFamilies.FirstOrDefault(ff => ff.Source.Contains("Yu Gothic UI")) ?? FontFamilies.FirstOrDefault();
      ComboBox_FontSize.Value = 16;
      SelectedFontSize = 16;
    }

    private void Button_OK_Click(object sender, RoutedEventArgs e) {
      if (ComboBox_FontFamily.SelectedItem is FontFamily selectedFontFamily) {
        SelectedFontFamily = selectedFontFamily;
      } else {
        SelectedFontFamily = FontFamilies.FirstOrDefault(ff => ff.Source.Contains("Yu Gothic UI")) ?? FontFamilies.FirstOrDefault();
      }

      if (ComboBox_FontSize.Value.HasValue) {
        SelectedFontSize = ComboBox_FontSize.Value.Value;
      } else {
        SelectedFontSize = 16;
      }

      DialogResult = true;
    }


    private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
      DialogResult = false;
    }
  }
}
