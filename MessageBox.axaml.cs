using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MsgBox
{
    public partial class MessageBox : Window
    {
        public enum MessageBoxButtons
        {
            Ok,
            OkCancel,
            YesNo,
            YesNoCancel
        }

        public enum MessageBoxResult
        {
            Ok,
            Cancel,
            Yes,
            No
        }

        public MessageBox()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static Task<MessageBoxResult> Show(Window parent, string text, string title, MessageBoxButtons buttons)
        {
            var msgbox = new MessageBox
            {
                Title = title
            };
            msgbox.FindControl<TextBlock>("Text").Text = text;
            var buttonPanel = msgbox.FindControl<StackPanel>("Buttons");

            var result = MessageBoxResult.Ok;

            void AddButton(string caption, MessageBoxResult resultType, bool isDefault = false)
            {
                var button = new Button { Content = caption };
                button.Click += (_, __) =>
                {
                    result = resultType;
                    msgbox.Close();
                };
                buttonPanel.Children.Add(button);

                if (isDefault)
                {
                    result = resultType;
                }
            }

            // Add buttons based on the selected MessageBoxButtons
            switch (buttons)
            {
                case MessageBoxButtons.Ok:
                case MessageBoxButtons.OkCancel:
                    AddButton("Ok", MessageBoxResult.Ok, true);
                    break;
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.YesNoCancel:
                    AddButton("Yes", MessageBoxResult.Yes);
                    AddButton("No", MessageBoxResult.No, true);
                    break;
            }

            if (buttons == MessageBoxButtons.OkCancel || buttons == MessageBoxButtons.YesNoCancel)
            {
                AddButton("Cancel", MessageBoxResult.Cancel);
            }

            var tcs = new TaskCompletionSource<MessageBoxResult>();
            msgbox.Closed += delegate { tcs.TrySetResult(result); };

            if (parent != null)
            {
                msgbox.ShowDialog(parent);
            }
            else
            {
                msgbox.Show();
            }

            return tcs.Task;
        }
    }
}
