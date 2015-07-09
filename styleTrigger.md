http://www.wpf-tutorial.com/misc/dispatchertimer/
  <TextBlock Text="Hello, styled world!" FontSize="28" HorizontalAlignment="Center" VerticalAlignment="Center">
            <TextBlock.Style>
                <Style TargetType="TextBlock">
                    <Setter Property="Foreground" Value="Blue"></Setter>
                    <Style.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="Foreground" Value="Red" />
                            <Setter Property="TextDecorations" Value="Underline" />
                        </Trigger>
                    </Style.Triggers>
                </Style>
            </TextBlock.Style>
        </TextBlock>
		   Title="StyleDataTriggerSample" Height="200" Width="200">
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <CheckBox Name="cbSample" Content="Hello, world?" />
        <TextBlock HorizontalAlignment="Center" Margin="0,20,0,0" FontSize="48">
            <TextBlock.Style>
                <Style TargetType="TextBlock">
                    <Setter Property="Text" Value="No" />
                    <Setter Property="Foreground" Value="Red" />
                    <Style.Triggers>
                        <DataTrigger Binding="{Binding ElementName=cbSample, Path=IsChecked}" Value="True">
                            <Setter Property="Text" Value="Yes!" />
                            <Setter Property="Foreground" Value="Green" />
                        </DataTrigger>
                    </Style.Triggers>
                </Style>
            </TextBlock.Style>
        </TextBlock>
    </StackPanel>
	
	<Grid>
        <Label Name="lblTime" FontSize="48" HorizontalAlignment="Center" VerticalAlignment="Center" />
    </Grid>
</Window>
Download sample
using System;
using System.Windows;
using System.Windows.Threading;

namespace WpfTutorialSamples.Misc
{
        public partial class DispatcherTimerSample : Window
        {
                public DispatcherTimerSample()
                {
                        InitializeComponent();
                        DispatcherTimer timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += timer_Tick;
                        timer.Start();
                }

                void timer_Tick(object sender, EventArgs e)
                {
                        lblTime.Content = DateTime.Now.ToLongTimeString();
                }
        }
		
		
		
		<Window x:Class="WpfTutorialSamples.Audio_and_Video.AudioVideoPlayerCompleteSample"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="WPF Media Player" Height="300" Width="300"
        MinWidth="300" SizeToContent="WidthAndHeight">
    <Window.CommandBindings>
        <CommandBinding Command="ApplicationCommands.Open" CanExecute="Open_CanExecute" Executed="Open_Executed" />
        <CommandBinding Command="MediaCommands.Play" CanExecute="Play_CanExecute" Executed="Play_Executed" />
        <CommandBinding Command="MediaCommands.Pause" CanExecute="Pause_CanExecute" Executed="Pause_Executed" />
        <CommandBinding Command="MediaCommands.Stop" CanExecute="Stop_CanExecute" Executed="Stop_Executed" />
    </Window.CommandBindings>
    <Grid MouseWheel="Grid_MouseWheel">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        <ToolBar>
            <Button Command="ApplicationCommands.Open">
                <Image Source="/WpfTutorialSamples;component/Images/folder.png" />
            </Button>
            <Separator />
            <Button Command="MediaCommands.Play">
                <Image Source="/WpfTutorialSamples;component/Images/control_play_blue.png" />
            </Button>
            <Button Command="MediaCommands.Pause">
                <Image Source="/WpfTutorialSamples;component/Images/control_pause_blue.png" />
            </Button>
            <Button Command="MediaCommands.Stop">
                <Image Source="/WpfTutorialSamples;component/Images/control_stop_blue.png" />
            </Button>
        </ToolBar>

        <MediaElement Name="mePlayer" Grid.Row="1" LoadedBehavior="Manual" Stretch="None" />

        <StatusBar Grid.Row="2">
            <StatusBar.ItemsPanel>
                <ItemsPanelTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                    </Grid>
                </ItemsPanelTemplate>
            </StatusBar.ItemsPanel>
            <StatusBarItem>
                <TextBlock Name="lblProgressStatus">00:00:00</TextBlock>
            </StatusBarItem>
            <StatusBarItem Grid.Column="1" HorizontalContentAlignment="Stretch">
                <Slider Name="sliProgress" Thumb.DragStarted="sliProgress_DragStarted"  Thumb.DragCompleted="sliProgress_DragCompleted" ValueChanged="sliProgress_ValueChanged" />
            </StatusBarItem>
            <StatusBarItem Grid.Column="2">
                <ProgressBar Name="pbVolume" Width="50" Height="12" Maximum="1" Value="{Binding ElementName=mePlayer, Path=Volume}" />
            </StatusBarItem>
        </StatusBar>
    </Grid>
</Window>
Download sample
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace WpfTutorialSamples.Audio_and_Video
{
        public partial class AudioVideoPlayerCompleteSample : Window
        {
                private bool mediaPlayerIsPlaying = false;
                private bool userIsDraggingSlider = false;

                public AudioVideoPlayerCompleteSample()
                {
                        InitializeComponent();

                        DispatcherTimer timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += timer_Tick;
                        timer.Start();
                }

                private void timer_Tick(object sender, EventArgs e)
                {
                        if((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
                        {
                                sliProgress.Minimum = 0;
                                sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                                sliProgress.Value = mePlayer.Position.TotalSeconds;
                        }
                }

                private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                        e.CanExecute = true;
                }

                private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
                {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
                        if(openFileDialog.ShowDialog() == true)
                                mePlayer.Source = new Uri(openFileDialog.FileName);
                }

                private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                        e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
                }

                private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
                {
                        mePlayer.Play();
                        mediaPlayerIsPlaying = true;
                }

                private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                        e.CanExecute = mediaPlayerIsPlaying;
                }

                private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
                {
                        mePlayer.Pause();
                }

                private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                        e.CanExecute = mediaPlayerIsPlaying;
                }

                private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
                {
                        mePlayer.Stop();
                        mediaPlayerIsPlaying = false;
                }

                private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
                {
                        userIsDraggingSlider = true;
                }

                private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
                {
                        userIsDraggingSlider = false;
                        mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
                }

                private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
                {
                        lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
                }

                private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
                {
                        mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
                }