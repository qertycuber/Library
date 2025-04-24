﻿using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace Library
{
    internal sealed class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                AvaloniaAppBuilder.Build().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                throw;
            }
        }
    }

    internal sealed class AvaloniaAppBuilder
    {
        
        public static AppBuilder Build()
        {
            return AppBuilder.Configure<App>()
                             .UsePlatformDetect()
                             .WithInterFont()
                             .LogToTrace()
                             .UseReactiveUI();
        }
    }
}
