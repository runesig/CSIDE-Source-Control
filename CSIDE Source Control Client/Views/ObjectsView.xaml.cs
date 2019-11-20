using CSIDESourceControl.Client.Service;
using CSIDESourceControl.Client.ViewModels;
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
using CSIDESourceControl.Client.Helpers;

namespace CSIDESourceControl.Client
{
    /// <summary>
    /// Interaction logic for ObjectsView.xaml
    /// </summary>
    public partial class ObjectsView : Window
    {
        private readonly ObjectsViewModel _viewModel;
        public ObjectsView()
        {
            InitializeComponent();

            _viewModel = new ObjectsViewModel(new ObjectsViewDialogService());

            DataContext = _viewModel;

            AddFilterFieldsComboBoxItems(comparedDataGrid, ref fieldFilterComboBox);
        }

        private void comparedDataGrid_Drop(object sender, DragEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                // If the DataObject contains string data, extract it.
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                    var viewModel = (ObjectsViewModel)DataContext;
                    if (viewModel.ImportFilesCommand.CanExecute(null))
                        viewModel.ImportFilesCommand.Execute(fileList);
                }
            }
        }

        public static void AddFilterFieldsComboBoxItems(DataGrid comparedDataGrid, ref ComboBox fieldFilterComboBox)
        {
            foreach (DataGridColumn column in comparedDataGrid.Columns)
            {
                if (column is DataGridTextColumn)
                {
                    DataGridTextColumn textColumn = column as DataGridTextColumn;
                    Binding binding = (Binding)textColumn.Binding;
                    string path = binding.Path.Path;

                    fieldFilterComboBox.Items.Add(new ComboboxItem { Text = column.Header.ToString(), Value = path });
                }
            }

            fieldFilterComboBox.SelectedIndex = 0;
        }
    }
}
