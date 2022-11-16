using FoxFanDownloader.Models;
using FoxFanDownloader.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Messages;

namespace FoxFanDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ISettingsStorage settingsStorage;
    private readonly CartoonUpdatesChecker updatesChecker;
    private readonly Notifier toasts;
    private readonly FoxFanParser parser;
    public ObservableCollection<Cartoon> Cartoons { get; set; }

    bool isOneLineSubtitles;
    public bool IsOneLineSubtitles
    {
        get => isOneLineSubtitles;
        set => Set(ref isOneLineSubtitles, value);
    }

    Cartoon selectedMultfilm;
    public Cartoon SelectedMultfilm
    {
        get => selectedMultfilm;
        set => Set(ref selectedMultfilm, value);
    }
    public ICommand AddNewCartoonCommand { get; }
    public ICommand LoadedCommand { get; }
    public ICommand CheckUpdatesForAllCatoonsCommand { get; }

    bool inProgress;
    
    public bool InProgress
    {
        get => inProgress;
        set => Set(ref inProgress, value);
    }
    
    public MainWindowViewModel(FoxFanParser parser, 
        ISettingsStorage settingsStorage, 
        CartoonUpdatesChecker updatesChecker,
        Notifier toasts) : this()
    {      
        this.settingsStorage = settingsStorage;
        this.updatesChecker = updatesChecker;
        this.toasts = toasts;
        this.parser = parser;
    }
    
    public MainWindowViewModel()
    {
        AddNewCartoonCommand = new LambdaCommand(AddNewCartoon, e => !InProgress);
        LoadedCommand = new LambdaCommand(async e => await Loaded());
        CheckUpdatesForAllCatoonsCommand = new LambdaCommand(async e => await CheckUpdatesForAllCatoons(), e => !InProgress);
        Cartoons = new ObservableCollection<Cartoon>();
    }

    private async Task Loaded(string activeCartoonName = null)
    {
        Cartoons.Clear();
        var cartoons = await Task.Run(() => settingsStorage.LoadCartoons());
        cartoons.ToList().ForEach(c => Cartoons.Add(c));

        var allSeries = cartoons.SelectMany(c => c.SeasonsInfo.Seasons).SelectMany(s => s.Series);
        foreach (var series in allSeries)
        {
            series.OpenVideoClicked += async (e) => VLC_Helper.OpenVideo(await parser.GetSourceVideoFromUri(e.Uri), true);
        }     
        
        foreach(var cartoon in cartoons)
        {
            cartoon.SeasonsInfo.OnRefresSeasonsClick += async () => await SeasonsInfo_OnRefresSeasonsClick(cartoon);
        }

        if(activeCartoonName != null)
        {
            SelectedMultfilm = Cartoons.FirstOrDefault(c => c.Name.Equals(activeCartoonName));
            SelectedMultfilm.SeasonsInfo.SelectLastSeason();
        }
    }

    private async Task SeasonsInfo_OnRefresSeasonsClick(Cartoon cartoon)
    {
        InProgress = true;
        cartoon.SeasonsInfo.IsRefreshInProgress = true;
        try
        {
            var hasUpdates = await updatesChecker.CheckUpdatesAndSave(cartoon);
            if (hasUpdates)
            {
                await Loaded(cartoon.Name);
                toasts.ShowSuccess($"Найдены новые серии '{cartoon.Name}'");
            }
            else
            {
                toasts.ShowInformation("Новых серий нет");
            }
        }
        finally
        {
            InProgress = false;
            cartoon.SeasonsInfo.IsRefreshInProgress = false;
        }
    }

    private async Task CheckUpdatesForAllCatoons()
    {
        InProgress = true;

        var tasks = Cartoons.Select(c => updatesChecker.CheckUpdatesAndSave(c)).ToArray();
        var updateResults = await Task.WhenAll(tasks);
        var cartoonsWithUpdates = Cartoons.Where((c, index) => updateResults[index]).Select(c => c.Name).ToArray();

        if (cartoonsWithUpdates.Length > 1)
        {
            toasts.ShowSuccess($"Найдены новые серии для сериалов {string.Join(", ", cartoonsWithUpdates)}");           
        } 
        else if(cartoonsWithUpdates.Length == 1)
        {
            toasts.ShowSuccess($"Найдены новые серии у сериала '{cartoonsWithUpdates[0]}'");
        }
        else
        {
            toasts.ShowInformation("Новых серий нет");
        }

        InProgress = false;

    }

    private async void AddNewCartoon(object obj)
    {
        var dialogVM = new AddNewCartoonSourceWindowViewModel(this, parser, settingsStorage);
        var dialog = new AddNewCartoonSourceWindow();
        dialog.DataContext = dialogVM;
        if (dialog.ShowDialog() == true)
        {
            await Loaded(dialogVM.SelectedCartoon.Name);
        }
    }
}
