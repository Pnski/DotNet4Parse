using System.IO;

using System.Windows;

using CUE4Parse.UE4.Versions;

using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.Core.Misc;


namespace Dotnet4Parse.Parse
{
    public class Mount
    {
        public DefaultFileProvider LoadAndMount(String? usmapFile, String? AesKey, String GamePakDir)
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);

            ZlibHelper.DownloadDll();
            ZlibHelper.Initialize(ZlibHelper.DLL_NAME);

            var version = new VersionContainer(EGame.GAME_UE4_27);
            var provider = new DefaultFileProvider(GamePakDir, SearchOption.TopDirectoryOnly, version);

            if (usmapFile != null)
            {
                provider.MappingsContainer = new FileUsmapTypeMappingsProvider(usmapFile);
            };
            provider.Initialize();
            try
            {
                provider.SubmitKey(new FGuid(), new FAesKey(AesKey));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No or invalid AesKey provided.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            provider.Mount();

            provider.PostMount();
            try{
                provider.LoadLocalization(ELanguage.English);
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadLoca Error.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return provider;
        }
    }
}