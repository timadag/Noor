using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Reflection;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.ComponentModel;
using Application = System.Windows.Forms.Application;
using System.Diagnostics;

namespace Noor
{
    /// <summary>
    /// путь к месту добавления контекстного меню 
    ///  HKEY_CLASSES_ROOT\Directory\Background\shell
    /// </summary>
    public partial class MainWindow : Window
    {
        //Переменные для ContextMenu
        private static readonly string targetPath = @"Directory\Background\shell";
        private static readonly string targetProgram = $"{Assembly.GetExecutingAssembly().Location}";
        private static readonly string programName = "Nail";
        private static readonly string programText = "Open Nail";
        private static int sa = 0;
        
       
        public MainWindow()
        {
            InitializeComponent();
            Relaunching_the_form();
            AddTry.IsChecked = NailOptimizator.Properties.Settings.Default.StartTheTrey;
            TheTreys();
            AddContextMenu.IsChecked = NailOptimizator.Properties.Settings.Default.AddContextMen;

        }
        //Работа с реекстром для добавления в контекстное меню
        private void AddContextMenus()
        {
            try
            {
                Registry.ClassesRoot.OpenSubKey(targetPath, true).CreateSubKey(programName).Close();
                Registry.SetValue(Path.Combine("HKEY_CLASSES_ROOT", targetPath, programName), "Icon", targetProgram);
                Registry.SetValue(Path.Combine("HKEY_CLASSES_ROOT", targetPath, programName), "", programText);

                Registry.ClassesRoot.OpenSubKey(Path.Combine(targetPath, programName), true).CreateSubKey("command").Close();
                Registry.SetValue(Path.Combine("HKEY_CLASSES_ROOT", targetPath, programName, "command"), "", targetProgram + ""); // если не будет работать добавить это \"%V\"
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Ошибка при выполнении " + ex.Message.ToString());
            }
        }
        private void DeleteContextMenus()
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\Background\shell\Noor");
            }

            catch (Exception ex)
            {
                _ = MessageBox.Show("Ошибка при выполнении " + ex.Message.ToString());
            }    
        }

        //Проверка повторного запуска программы
        private void Relaunching_the_form()
        {
            int prC = 0;
            foreach (Process pr in Process.GetProcesses())
                if (pr.ProcessName == "Noor") prC++;
            if (prC > 1) Process.GetCurrentProcess().Kill();
        }
        [STAThread]
        private static void Stain()
        {
            if (Process.GetProcessesByName("NailOptimizator").Length > 1)
            {
                MessageBox.Show("Программа уже запущена");
            }
            else
            {
                return;
            }

        }
        private void AddContextMenu_Click(object sender, RoutedEventArgs e)
        {
            bool switch_on = AddContextMenu.IsChecked.Value;

            switch (switch_on)
            {
                case true:
                    MessageBoxResult result = (MessageBoxResult)MessageBox.Show("Вы действительно хотите добавить в контекстное меню Windows?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        AddContextMenus();
                        NailOptimizator.Properties.Settings.Default.AddContextMen = true;
                        NailOptimizator.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        AddContextMenu.IsChecked = false;
                    }
                    break;
                
                case false:
                    MessageBoxResult result2 = (MessageBoxResult)MessageBox.Show("Вы действительно хотите удалить из контекстного меню Windows?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result2 == MessageBoxResult.Yes)
                    {
                        DeleteContextMenus();
                        NailOptimizator.Properties.Settings.Default.AddContextMen = false;
                        NailOptimizator.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        AddContextMenu.IsChecked = true;
                    }
                    break;
            }
        }

        //Работа с трей
        private void TheTreys()
        {
           if(AddTry.IsChecked == true)
            {
                TheTry.Visibility = Visibility.Visible;
                Hide();
            }
            else
            {
                TheTry.Visibility = Visibility.Hidden;
            }
        }
        private void AddTry_Click(object sender, RoutedEventArgs e)
        {
            bool switch_on = AddTry.IsChecked.Value;
            switch (switch_on)
            {
                case true:
                    MessageBoxResult result = (MessageBoxResult)MessageBox.Show("Вы действительно хотите запускать программу в трее?", "Внимание", 
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        TheTry.Visibility = Visibility.Visible;
                        NailOptimizator.Properties.Settings.Default.StartTheTrey = true;
                        NailOptimizator.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        TheTry.Visibility = Visibility.Visible;
                        AddTry.IsChecked = false;
                    }
                    break;

                case false:
                    MessageBoxResult result2 = (MessageBoxResult)MessageBox.Show("Вы действительно хотите убрать запуск в трее?", "Внимание",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    
                    if (result2 == MessageBoxResult.Yes)
                    {
                        TheTry.Visibility = Visibility.Hidden;
                        NailOptimizator.Properties.Settings.Default.StartTheTrey = false;
                        NailOptimizator.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        TheTry.Visibility = Visibility.Visible;
                        AddTry.IsChecked = true;
                    }
                    break;
            }
        }
        private void TheTry_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            sa++; Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (sa == 0)
            {
                sa -= 2;
                TheTry.Visibility = Visibility.Hidden;
                Show();
                MessageBoxResult result2 = (MessageBoxResult)MessageBox.Show("Вы действительно хотите выйти", "Внимание",
                                                     MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result2 == MessageBoxResult.Yes)
                {
                    Environment.Exit(0);
                }
                else
                {
                    TheTry.Visibility = Visibility.Visible;
                }
                return;

            }
            TheTry.Visibility = Visibility.Visible;
            Show();
            MessageBoxResult result = (MessageBoxResult)MessageBox.Show("Вы действительно хотите выйти", "Внимание",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {

            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            TheTry.Visibility = Visibility.Hidden;
            MessageBoxResult result2 = (MessageBoxResult)MessageBox.Show("Вы действительно хотите выйти", "Внимание",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result2 == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
                TheTry.Visibility = Visibility.Visible;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            sa++; Show();
        }
    }
    }


