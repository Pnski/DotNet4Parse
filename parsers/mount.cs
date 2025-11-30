using System.IO;

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
        public DefaultFileProvider LoadAndMount(String UsmapFile, String AesKey, String GamePakDir)
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);

            ZlibHelper.DownloadDll();
            ZlibHelper.Initialize(ZlibHelper.DLL_NAME);

            var version = new VersionContainer(EGame.GAME_UE4_27);
            var provider = new DefaultFileProvider(GamePakDir, SearchOption.TopDirectoryOnly, version)
            {
                MappingsContainer = new FileUsmapTypeMappingsProvider(UsmapFile)
            };
            provider.Initialize();
            provider.SubmitKey(new FGuid(), new FAesKey(AesKey));
            provider.Mount();

            provider.PostMount();
            provider.LoadLocalization(ELanguage.English);

            return provider;
        }
    }
}