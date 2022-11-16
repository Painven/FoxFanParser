﻿using AutoMapper;
using FoxFanDownloader.AutoMapper;
using FoxFanDownloader.Models;
using FoxFanDownloader.ViewModels;
using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

namespace FoxFanDownloader;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        //toasts
        Notifier toasts = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.AddMaps(typeof(App).Assembly);
        });
        IMapper mapper = new Mapper(config); 

        ISettingsStorage settingsStorage = new JsonSettingsStorage(mapper);
        FoxFanParser parser = new FoxFanParser();
        CartoonUpdatesChecker updatesChecker = new CartoonUpdatesChecker(parser, settingsStorage);

        MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(parser, settingsStorage, updatesChecker, toasts);

        Window mainWindow = new MainWindow();
        mainWindow.DataContext = mainWindowViewModel;
        mainWindow.Show();
    }


}
