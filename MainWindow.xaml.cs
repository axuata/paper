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
    int PaperTextboxLengthInt = 0;

    public MainWindow() {
      InitializeComponent();

      UpdateStatusTextLength();
    }

    private void FluentWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      if (!ConfirmSaveIfNeeded()) {
        e.Cancel = true;
        return;
      }
    }

    private void PaperTextbox_TextChanged(object sender, TextChangedEventArgs e) {
      isSaved = false;
      if (PaperTextbox.IsFocused) {
        undoStack.Push(new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text);
        redoStack.Clear();
      }

      UpdateStatusTextLength();
    }

    private void UpdateStatusTextLength() {
      string text = new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text;
      string textWithoutNewLines = text.Replace("\r\n", "").Replace("\n", "");
      int textLength = textWithoutNewLines.Length;

      if (textLength < 0) {
        textLength = 0;
      }

      StatusTextLength.Text = "文字数：" + textLength.ToString();
    }

    #region Menu

    #region Application
    private void MenuApplication_AboutPaper_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show("Paper / 0.7.1 | © 2024 Axuata, CC BY 4.0\r\n", "Paperについて", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuApplication_AboutIcon_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show("https://www.flaticon.com/free-icons/send   Send icons created by Freepik - Flaticon", "アイコンについて", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuApplication_Exit_Click(object sender, RoutedEventArgs e) {
      if (!ConfirmSaveIfNeeded()) {
        return;
      }

      Application.Current.Shutdown();
    }
    #endregion

    #region File
    private void MenuFile_NewFile_Click(object sender, RoutedEventArgs e) {
      if (!ConfirmSaveIfNeeded()) {
        return;
      }

      PaperTextbox.Document.Blocks.Clear();
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
        new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text = File.ReadAllText(currentFilePath);
        isSaved = true;
      }
    }

    private void MenuFile_Save_Click(object sender, RoutedEventArgs e) {
      if (string.IsNullOrEmpty(currentFilePath)) {
        MenuFile_SaveAs_Click(sender, e);
      } else {
        File.WriteAllText(currentFilePath, new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text);
        isSaved = true;
      }
    }

    private void MenuFile_SaveAs_Click(object sender, RoutedEventArgs e) {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
      if (saveFileDialog.ShowDialog() == true) {
        currentFilePath = saveFileDialog.FileName;
        File.WriteAllText(currentFilePath, new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text);
        isSaved = true;
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
        redoStack.Push(new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text);
        PaperTextbox.TextChanged -= PaperTextbox_TextChanged;
        new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text = undoStack.Pop();
        PaperTextbox.TextChanged += PaperTextbox_TextChanged;
      }
    }

    private void MenuEdit_Redo_Click(object sender, RoutedEventArgs e) {
      if (redoStack.Count > 0) {
        undoStack.Push(new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text);
        PaperTextbox.TextChanged -= PaperTextbox_TextChanged;
        new TextRange(PaperTextbox.Document.ContentStart, PaperTextbox.Document.ContentEnd).Text = redoStack.Pop();
        PaperTextbox.TextChanged += PaperTextbox_TextChanged;
      }
    }

    private void MenuEdit_Cut_Click(object sender, RoutedEventArgs e) {
      if (PaperTextbox.Selection.Text.Length > 0) {
        PaperTextbox.Cut();
      }
    }

    private void MenuEdit_Copy_Click(object sender, RoutedEventArgs e) {
      if (PaperTextbox.Selection.Text.Length > 0) {
        PaperTextbox.Copy();
      }
    }

    private void MenuEdit_Paste_Click(object sender, RoutedEventArgs e) {
      PaperTextbox.Paste();
    }

    private void MenuEdit_Font_Click(object sender, RoutedEventArgs e) {
      var fontSelectionDialog = new FontSelectionDialog(PaperTextbox);

      bool? result = fontSelectionDialog.ShowDialog();

      if (result == true) {
        var selectedFontFamily = fontSelectionDialog.SelectedFontFamily;
        var selectedFontSize = fontSelectionDialog.SelectedFontSize;
        var selectedFontWeight = fontSelectionDialog.SelectedFontWeight;

        PaperTextbox.FontFamily = selectedFontFamily;
        PaperTextbox.FontSize = selectedFontSize;
        PaperTextbox.FontWeight = selectedFontWeight;
      }
    }

    #endregion

    #region Input
    private void MenuInputDay_Click(object sender, RoutedEventArgs e) {
      string currentDate = DateTime.Now.ToString("yyyy/MM/dd");

      PaperTextbox.CaretPosition.InsertTextInRun(currentDate);
    }

    private void MenuInputTime_Click(object sender, RoutedEventArgs e) {
      string currentTime = DateTime.Now.ToString("HH:mm:ss");

      PaperTextbox.CaretPosition.InsertTextInRun(currentTime);
    }
    #endregion

    #endregion
  }
}