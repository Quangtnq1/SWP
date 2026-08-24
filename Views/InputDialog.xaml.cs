using System.Windows;

namespace SWP.Views;

// Hộp thoại nhập 1 dòng text, dùng cho lý do từ chối/hủy phiếu.
// Tự viết thay vì dùng Microsoft.VisualBasic.Interaction.InputBox để khỏi kéo theo Windows Forms.
public partial class InputDialog : Window
{
    public string KetQua { get; private set; } = "";

    public InputDialog(string tieuDe, string goiY)
    {
        InitializeComponent();
        TxtTieuDe.Text = tieuDe;
        TxtGoiY.Text = goiY;
        Loaded += (_, _) => TxtNoiDung.Focus();
    }

    private void BtnXacNhan_Click(object sender, RoutedEventArgs e)
    {
        KetQua = TxtNoiDung.Text.Trim();
        DialogResult = true;
    }

    private void BtnHuy_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    // Trả về null nếu người dùng bấm Hủy hoặc để trống.
    public static string? Show(string tieuDe, string goiY, Window? owner = null)
    {
        var dialog = new InputDialog(tieuDe, goiY);
        dialog.Owner = owner;
        var ok = dialog.ShowDialog();
        if (ok != true || string.IsNullOrWhiteSpace(dialog.KetQua))
        {
            return null;
        }
        return dialog.KetQua;
    }
}
