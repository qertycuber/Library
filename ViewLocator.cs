using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Library.ViewModels;
using System;

namespace Library
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data == null)
            {
                return new TextBlock { Text = "Data is null" };
            }

            var viewName = GetViewName(data);
            var viewType = GetViewType(viewName);

            if (viewType != null)
            {
                return CreateControl(viewType, data);
            }

            return new TextBlock { Text = "View Not Found: " + viewName };
        }

        private string GetViewName(object data)
        {
            return data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        }

        private Type? GetViewType(string viewName)
        {
            return Type.GetType(viewName);
        }

        private Control CreateControl(Type viewType, object data)
        {
            var control = (Control)Activator.CreateInstance(viewType)!;
            control.DataContext = data;
            return control;
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}