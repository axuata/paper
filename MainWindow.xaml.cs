using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Paper.dialogs;

namespace Paper {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow {
    private string currentFilePath = string.Empty;
    private bool isSaved = true;
    private Stack<string> undoStack = new Stack<string>();
    private Stack<string> redoStack = new Stack<string>();

    public MainWindow() {
      InitializeComponent();
    }

    private void PaperTextbox_TextChanged(object sender, TextChangedEventArgs e) {
      isSaved = false;
      if (PaperTextbox.IsFocused) {
        undoStack.Push(PaperTextbox.Text);
        redoStack.Clear();
      }
    }

    #region Menu

    #region Application
    private void MenuApplication_AboutPaper_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show("Paper / 1.0.0 | © 2024 Axuata, CC BY 4.0\r\n", "Paperについて", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuApplication_AboutIcon_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show("https://www.flaticon.com/free-icons/send   Send icons created by Freepik - Flaticon", "アイコンについて", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuApplication_Exit_Click(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    #endregion

    #region File
    private void MenuFile_NewFile_Click(object sender, RoutedEventArgs e) {
      if (!ConfirmSaveIfNeeded()) {
        return;
      }

      PaperTextbox.Clear();
      currentFilePath = string.Empty;
      isSaved = true;
    }

    private void MenuFile_Open_Click(object sender, RoutedEventArgs e) {
      if (!ConfirmSaveIfNeeded()) {
        return;
      }

      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      if (openFileDialog.ShowDialog() == true) {
        currentFilePath = openFileDialog.FileName;
        PaperTextbox.Text = File.ReadAllText(currentFilePath);
        isSaved = true;
      }
    }

    private void MenuFile_Save_Click(object sender, RoutedEventArgs e) {
      if (string.IsNullOrEmpty(currentFilePath)) {
        MenuFile_SaveAs_Click(sender, e);
      } else {
        File.WriteAllText(currentFilePath, PaperTextbox.Text);
        isSaved = true; // 保存後にフラグを設定
      }
    }

    private void MenuFile_SaveAs_Click(object sender, RoutedEventArgs e) {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      if (saveFileDialog.ShowDialog() == true) {
        currentFilePath = saveFileDialog.FileName;
        File.WriteAllText(currentFilePath, PaperTextbox.Text);
        isSaved = true; // 保存後にフラグを設定
      }
    }

    private bool ConfirmSaveIfNeeded() {
      if (!isSaved) {
        MessageBoxResult result = MessageBox.Show("現在の内容が保存されていません。保存しますか？", "警告", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes) {
          MenuFile_Save_Click(this, new RoutedEventArgs());
        } else if (result == MessageBoxResult.Cancel) {
          return false;
        }
      }
      return true;
    }
    #endregion

    #region Edit
    private void MenuEdit_Undo_Click(object sender, RoutedEventArgs e) {
      if (undoStack.Count > 0) {
        redoStack.Push(PaperTextbox.Text);
        PaperTextbox.TextChanged -= PaperTextbox_TextChanged;
        PaperTextbox.Text = undoStack.Pop();
        PaperTextbox.TextChanged += PaperTextbox_TextChanged;
      }
    }

    private void MenuEdit_Redo_Click(object sender, RoutedEventArgs e) {
      if (redoStack.Count > 0) {
        undoStack.Push(PaperTextbox.Text);
        PaperTextbox.TextChanged -= PaperTextbox_TextChanged;
        PaperTextbox.Text = redoStack.Pop();
        PaperTextbox.TextChanged += PaperTextbox_TextChanged;
      }
    }

    private void MenuEdit_Cut_Click(object sender, RoutedEventArgs e) {
      if (PaperTextbox.SelectedText.Length > 0) {
        PaperTextbox.Cut();
      }
    }

    private void MenuEdit_Copy_Click(object sender, RoutedEventArgs e) {
      if (PaperTextbox.SelectedText.Length > 0) {
        PaperTextbox.Copy();
      }
    }

    private void MenuEdit_Paste_Click(object sender, RoutedEventArgs e) {
      PaperTextbox.Paste();
    }

    private void MenuEdit_Font_Click(object sender, RoutedEventArgs e) {
      var FontSelectionDialog = new FontSelectionDialog();

      if (FontSelectionDialog.ShowDialog() == true) {
        var selectedFontFamily = FontSelectionDialog.SelectedFontFamily;
        var selectedFontSize = FontSelectionDialog.SelectedFontSize;
        PaperTextbox.FontFamily = selectedFontFamily;
        PaperTextbox.FontSize = selectedFontSize;
      }
    }
    #endregion

    #endregion
  }
}