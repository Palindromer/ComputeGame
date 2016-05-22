using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ComputeGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //Для текущего домена приложения вешаем свой обработчик в котором и будем вручную подсовывать нужные сборки
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("WPFToolkit"))
            {
                    return GetAssemblyFromDeflatedResource(
                                       ComputeGame.Properties.Resources.WPFToolkit_dll);
            }
            if (args.Name.Contains("DataVisualization"))
            {
                // Загрузка запакованной сборки из ресурсов, ее распаковка и подстановка
                return GetAssemblyFromDeflatedResource(
                    ComputeGame.Properties.Resources.system_windows_controls_datavisualization_toolkit_dll);
            }

            if (args.Name.Contains("NAudio"))
            {
                // Загрузка запакованной сборки из ресурсов, ее распаковка и подстановка
                return GetAssemblyFromDeflatedResource(
                    ComputeGame.Properties.Resources.NAudio_dll);
            }

            return null;
        }

        private static Assembly GetAssemblyFromDeflatedResource(byte[] resource)
        {
            using (var resourceStream = new MemoryStream(resource))
            using (var deflated = new DeflateStream(resourceStream, CompressionMode.Decompress))
            using (var reader = new BinaryReader(deflated))
            {
                var one_megabyte = 1024 * 1024;
                var buffer = reader.ReadBytes(one_megabyte);
                return Assembly.Load(buffer);
            }
        }

    }
}
