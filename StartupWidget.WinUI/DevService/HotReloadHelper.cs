using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
[assembly: MetadataUpdateHandler(typeof(StartupWidget.WinUI.DevService.HotReloadHelper))]
namespace StartupWidget.WinUI.DevService
{
    public static class HotReloadHelper
    {
        public static event Action? OnHotReload;

        // ホットリロード適用直後に .NET ランタイムから自動で呼ばれるメソッド
        public static void UpdateApplication(Type[]? updatedTypes)
        {
            // UI スレッドで画面再描画を実行
            OnHotReload?.Invoke();
        }
    }
}
#endif