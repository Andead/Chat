using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Andead.Chat.Client.WinForms.Utilities;

namespace Andead.Chat.Client.WinForms
{
    internal abstract class View : Form
    {
        protected View(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            ViewModel = viewModel;
            ViewModel.Error += ViewModelOnError;
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        protected ViewModel ViewModel { get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ViewModel.Load();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            ViewModel.Unload();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            this.InvokeIfRequired(Set, args.PropertyName);
        }

        private void ViewModelOnError(object sender, ErrorEventArgs args)
        {
            this.InvokeIfRequired(OnException, args.GetException());
        }

        protected virtual void Set(string propertyName)
        {
        }

        protected virtual void OnException(Exception exception)
        {
            ShowError(exception.Message);
        }

        protected static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}