using The49.Maui.BottomSheet;

namespace SmartExpenseApp.Views;

public partial class AccountBottomSheetView : BottomSheet
{
	public AccountBottomSheetView()
	{
        InitializeComponent();
	}

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = sender as Picker;

        //if (picker.selectedindex == 0)
        //{
        //    bank_button.isvisible = true;
        //}
        //else
        //{
        //    bank_button.isvisible = false;
        //}

    }
}