using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Paper.dialogs {
  /// <summary>
  /// FontSelectionDialog.xaml の相互作用ロジック
  /// </summary>
  public partial class FontSelectionDialog {
    private RichTextBox _targetTextBox;

    public List<FontFamily> FontFamilies {
      get; private set;
    }
    public List<FontWeight>? FontWeights {
      get; private set;
    }
    public FontFamily? SelectedFontFamily {
      get; set;
    }
    public double SelectedFontSize {
      get; set;
    }
    public FontWeight SelectedFontWeight {
      get; set;
    }

    public FontSelectionDialog(RichTextBox targetTextBox) {
      InitializeComponent();
      _targetTextBox = targetTextBox;

      FontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();
      DataContext = this;

      SelectedFontFamily = _targetTextBox.FontFamily;
      SelectedFontSize = _targetTextBox.FontSize;
      SelectedFontWeight = _targetTextBox.FontWeight;

      InitializeDialog();
    }

    private void InitializeDialog() {
      ComboBox_FontFamily.SelectedItem = SelectedFontFamily;
      NumberBox_FontSize.Value = SelectedFontSize;
      UpdateFontWeights(SelectedFontFamily);
    }

    private void ComboBox_FontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (ComboBox_FontFamily.SelectedItem is FontFamily selectedFontFamily) {
        UpdateFontWeights(selectedFontFamily);
      }
    }

    private void UpdateFontWeights(FontFamily? fontFamily) {
      if (fontFamily == null)
        return;

      ComboBox_FontWeight.ItemsSource = null;
      ComboBox_FontWeight.Items.Clear();

      FontWeights = fontFamily.FamilyTypefaces
          .Select(ft => ft.Weight)
          .Distinct()
          .ToList();

      ComboBox_FontWeight.ItemsSource = FontWeights;
      ComboBox_FontWeight.SelectedItem = FontWeights.FirstOrDefault();
    }

    private void Button_OK_Click(object sender, RoutedEventArgs e) {
      if (ComboBox_FontFamily.SelectedItem is FontFamily selectedFontFamily) {
        SelectedFontFamily = selectedFontFamily;
      } else {
        SelectedFontFamily = FontFamilies.FirstOrDefault(ff => ff.Source.Contains("Yu Gothic UI")) ?? FontFamilies.FirstOrDefault();
      }

      if (NumberBox_FontSize.Value.HasValue) {
        SelectedFontSize = NumberBox_FontSize.Value.Value;
      } else {
        SelectedFontSize = 16;
      }

      if (ComboBox_FontWeight.SelectedItem is FontWeight selectedFontWeight) {
        SelectedFontWeight = selectedFontWeight;
      } else {
        SelectedFontWeight = FontWeights?.FirstOrDefault() ?? FontWeight.FromOpenTypeWeight(400); // 400 は通常の標準ウェイト
      }

      _targetTextBox.FontFamily = SelectedFontFamily;
      _targetTextBox.FontSize = SelectedFontSize;
      _targetTextBox.FontWeight = SelectedFontWeight;

      DialogResult = true;
    }

    private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
      DialogResult = false;
    }
  }
}
